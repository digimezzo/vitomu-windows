using Digimezzo.Utilities.Log;
using Digimezzo.Utilities.Settings;
using Digimezzo.Utilities.Utils;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Vitomu.Base;

namespace Vitomu.Services.Convert
{
    public class ConvertService : IConvertService
    {
        #region Variables
        private string workingFolder;
        private string musicFolder;
        private string lastConvertedFile;
        private Process youtubedlProcess;
        private Process ffmpegProcess;
        private ConvertState convertState;
        private System.Timers.Timer convertStateResetTimer = new System.Timers.Timer(3000);
        #endregion

        #region Properties
        public string LastConvertedFile => this.lastConvertedFile;

        public string MusicFolder => this.musicFolder;
        #endregion

        #region Construction
        public ConvertService()
        {
            this.musicFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                ProductInformation.ApplicationDisplayName);

            this.workingFolder = Path.Combine(SettingsClient.ApplicationFolder(), "Working");

            // Create the music folder. If this fails, we cannot continue (let it crash).
            this.CreateMusicFolder();

            // Try to delete the working folder. If it fails: no problem.
            try
            {
                if (Directory.Exists(this.workingFolder))
                {
                    Directory.Delete(this.workingFolder, true);
                }
            }
            catch (Exception ex)
            {
                LogClient.Error("An error occurred while deleting the working folder {0}. Exception: {1}", this.workingFolder, ex.Message);
            }

            // Try to create the working folder. If this fails, we cannot continue (let it crash).
            try
            {
                if (!Directory.Exists(this.workingFolder))
                {
                    Directory.CreateDirectory(this.workingFolder);
                }
            }
            catch (Exception ex)
            {
                LogClient.Error("An error occurred while creating the music folder {0}. Exception: {1}", this.workingFolder, ex.Message);
                throw;
            }

            this.convertState = ConvertState.Idle;

            convertStateResetTimer.Elapsed += ConvertStateResetTimer_Elapsed;
        }
        #endregion

        #region IConvertService
        public event ConvertStateChangedHander ConvertStateChanged = delegate { };

        public void KillProcesses()
        {
            if (this.youtubedlProcess != null && !this.youtubedlProcess.HasExited)
            {
                this.youtubedlProcess.Kill();
            }
        }

        public bool IsValidVideo(string uri)
        {
            if (this.convertState != ConvertState.Idle || string.IsNullOrWhiteSpace(uri))
            {
                return false;
            }

            return this.IsVideoUrl(uri) || this.IsVideoFile(uri);
        }
        #endregion

        #region Event handlers
        private void ConvertStateResetTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.convertStateResetTimer.Stop();
            this.SetConvertState(ConvertState.Idle);
        }
        #endregion

        #region Private
        private void CreateMusicFolder()
        {
            // Create the music folder. If this fails, we cannot continue (let it crash).
            try
            {
                if (!Directory.Exists(this.musicFolder))
                {
                    Directory.CreateDirectory(this.musicFolder);
                }
            }
            catch (Exception ex)
            {
                LogClient.Error("An error occurred while creating the music folder {0}. Exception: {1}", this.musicFolder, ex.Message);
                throw;
            }
        }

        private bool IsVideoUrl(string uri)
        {
            try
            {
                // TODO: this is a quick and dirty way of checking for a Youtube URL. Improve this.
                return ValidationUtils.IsUrl(uri) && uri.ToLower().Contains("youtube") && uri.ToLower().Contains("/watch");
            }
            catch (Exception ex)
            {
                LogClient.Error("Could not verify if video URI is a URL. Exception: {0}", ex.Message);
            }

            return false;
        }

        private bool IsVideoFile(string uri)
        {
            if (File.Exists(uri) && FileFormats.SupportedVideoExtensions.Contains(Path.GetExtension(uri.ToLower())))
            {
                return true;
            }

            return false;
        }

        private void SetConvertState(ConvertState convertState, double convertPercent = -1)
        {
            convertStateResetTimer.Stop();
            this.convertState = convertState;
            this.ConvertStateChanged(convertState, convertPercent);

            if (convertState == ConvertState.Success || convertState == ConvertState.Failed)
            {
                convertStateResetTimer.Start();
            }
        }

        public void ProcessVideo(string uri)
        {
            try
            {
                // Get the audio format and bit rate from the settings
                AudioFormat format = FileFormats.GetValidAudioFormat(SettingsClient.Get<string>("Output", "Format"));
                int bitRate = FileFormats.GetValidBitRate(SettingsClient.Get<int>("Output", "BitRate"));

                // Create a temporary directory
                string tempFolder = Path.Combine(this.workingFolder, Guid.NewGuid().ToString());

                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }

                if (this.IsVideoUrl(uri))
                {
                    // Process the video URL
                    this.ConvertOnlineVideo(tempFolder, uri, format, bitRate);
                }
                else if (this.IsVideoFile(uri))
                {
                    // Process the video file
                    this.ConvertLocalVideo(tempFolder, uri, format, bitRate);
                }
            }
            catch (Exception ex)
            {
                LogClient.Error("An error occurred while processing the video. Exception: {0}", ex.Message);
                this.SetConvertState(ConvertState.Failed);
            }
        }
        private void ConvertOnlineVideo(string tempFolder, string uri, AudioFormat format, int bitRate)
        {
            this.SetConvertState(ConvertState.Processing);

            try
            {
                ProcessStartInfo youtubeDlStartInfo = new ProcessStartInfo()
                {
                    FileName = "youtube-dl.exe",
                    Arguments = $"{uri} --no-check-certificate --no-playlist --output \"{tempFolder}\\%(title)s.%(ext)s\" -f bestaudio --extract-audio --audio-format {format.YoutubedlCodec} --audio-quality {bitRate}k",
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };

                this.youtubedlProcess = new Process();
                this.youtubedlProcess.StartInfo = youtubeDlStartInfo;
                this.youtubedlProcess.EnableRaisingEvents = true;

                this.youtubedlProcess.OutputDataReceived += (sender, e) =>
                {
                    LogClient.Info("youtube-dl: {0}", e.Data);

                    var regex = new Regex(@"^\[download\].*?(?<percentage>(.*?))% of"); //[download]   2.7% of 4.62MiB at 200.00KiB/s ETA 00:23

                    if (e.Data != null)
                    {
                        var match = regex.Match(e.Data);

                        if (match.Success)
                        {
                            double progressPercent = double.Parse(match.Groups["percentage"].Value, CultureInfo.InvariantCulture);
                            this.SetConvertState(ConvertState.Downloading, progressPercent);
                        }
                        else
                        {
                            if (e.Data.Contains("[ffmpeg] Destination"))
                            {
                                this.SetConvertState(ConvertState.Converting);
                            }
                        }
                    }
                };

                this.youtubedlProcess.ErrorDataReceived += (sender, e) =>
                {
                    LogClient.Error("youtube-dl: {0}", e.Data);
                };

                this.youtubedlProcess.Exited += (_, __) =>
                {
                    // Move the audio file to the music folder
                    this.ProcessAudioFile(tempFolder, format);
                };

                this.youtubedlProcess.Start();
                this.youtubedlProcess.BeginErrorReadLine();
                this.youtubedlProcess.BeginOutputReadLine();
            }
            catch (Exception ex)
            {
                LogClient.Error("An error occurred while converting the online video. Exception: {0}", ex.Message);
                this.SetConvertState(ConvertState.Failed);
            }
        }

        private void ConvertLocalVideo(string tempFolder, string uri, AudioFormat format, int bitRate)
        {
            this.SetConvertState(ConvertState.Processing);

            try
            {
                ProcessStartInfo ffmpegStartInfo = new ProcessStartInfo()
                {
                    FileName = "ffmpeg.exe",
                    Arguments = $"-i \"{uri}\" -b:a {bitRate}K -vn \"{Path.Combine(tempFolder, Path.GetFileNameWithoutExtension(uri) + format.Extension)}\"",
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };

                this.ffmpegProcess = new Process();
                this.ffmpegProcess.StartInfo = ffmpegStartInfo;
                this.ffmpegProcess.EnableRaisingEvents = true;

                this.ffmpegProcess.OutputDataReceived += (sender, e) =>
                {
                    LogClient.Info("ffmpeg: {0}", e.Data);
                    this.SetConvertState(ConvertState.Converting);
                };

                this.ffmpegProcess.ErrorDataReceived += (sender, e) =>
                {
                    LogClient.Error("ffmpeg: {0}", e.Data);
                };

                this.ffmpegProcess.Exited += (_, __) =>
                {
                    // Move the audio file to the music folder
                    this.ProcessAudioFile(tempFolder, format);
                };

                this.ffmpegProcess.Start();
                this.ffmpegProcess.BeginErrorReadLine();
                this.ffmpegProcess.BeginOutputReadLine();
            }
            catch (Exception ex)
            {
                LogClient.Error("An error occurred while converting the local video. Exception: {0}", ex.Message);
                this.SetConvertState(ConvertState.Failed);
            }
        }

        private void ProcessAudioFile(string tempFolder, AudioFormat format)
        {
            // Create the music folder. If this fails, we cannot continue (let it crash).
            this.CreateMusicFolder();

            try
            {
                // Move the audio file to the music folder
                string[] files = Directory.GetFiles(tempFolder, "*" + format.Extension);

                if (files.Count() > 0)
                {
                    string firstFoundFile = files.First();
                    string movedAudioFile = Path.Combine(this.musicFolder, Path.GetFileName(firstFoundFile));

                    LogClient.Info("File of type {0} was found in the temporary folder: {1}\\{2}", format.Extension, tempFolder, firstFoundFile);

                    // If a file was found: move it to the music folder.
                    int counter = 0;

                    while (File.Exists(movedAudioFile))
                    {
                        counter++;
                        movedAudioFile = Path.Combine(this.musicFolder, $"{Path.GetFileNameWithoutExtension(firstFoundFile)} ({counter}){Path.GetExtension(firstFoundFile)}");
                    }

                    File.Move(firstFoundFile, movedAudioFile);
                    LogClient.Info("Moved file {0} to {1}", firstFoundFile, movedAudioFile);
                    this.lastConvertedFile = movedAudioFile;
                    this.SetConvertState(ConvertState.Success);
                }
                else
                {
                    LogClient.Info("File of type {0} was not found", format.Extension);
                    this.SetConvertState(ConvertState.Failed);
                }
            }
            catch (Exception ex)
            {
                LogClient.Info("An error occurred while processing the audio file. Exception: {0}", ex.Message);
                this.SetConvertState(ConvertState.Failed);
            }

            try
            {
                // Delete the temporary folder
                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, true);
                }
            }
            catch (Exception ex)
            {
                LogClient.Error("An error occurred while deleting the temporary folder {0}. Exception: {1}", tempFolder, ex.Message);
            }
        }
        #endregion
    }
}