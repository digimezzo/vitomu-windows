using Digimezzo.Utilities.IO;
using Digimezzo.Utilities.Log;
using Digimezzo.Utilities.Utils;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using Vitomu.Services.Convert;

namespace Vitomu.ViewModels
{
    public class ConvertViewModel : ViewModelBase, IDropTarget
    {
        #region Variables
        private bool isDraggingValidVideo;
        private string progressInformation;
        private IConvertService convertService;
        private System.Timers.Timer dragTimer = new System.Timers.Timer(100);
        private ConvertState convertState = ConvertState.Idle;
        private string convertedFile;
        #endregion

        #region Commands
        public RelayCommand ViewInFolderCommand { get; set; }
        public RelayCommand PlayCommand { get; set; }
        public RelayCommand PasteCommand { get; set; }
        #endregion

        #region Properties
        public string ConvertedFileName
        {
            get
            {
                if (!string.IsNullOrEmpty(convertedFile))
                {
                    return Path.GetFileName(convertedFile);
                }

                return string.Empty;
            }
        }

        public bool IsDraggingValidVideo
        {
            get { return this.isDraggingValidVideo; }
            set
            {
                this.isDraggingValidVideo = value;
                RaisePropertyChanged(() => this.IsDraggingValidVideo);
            }
        }

        public string ProgressInformation
        {
            get { return this.progressInformation; }
            set
            {
                this.progressInformation = value;
                RaisePropertyChanged(() => this.ProgressInformation);
            }
        }

        public ConvertState ConvertState
        {
            get { return this.convertState; }
            set
            {
                this.convertState = value;
                RaisePropertyChanged(() => this.ConvertState);
            }
        }

        public bool CanViewConvertedFile
        {
            get { return !string.IsNullOrEmpty(this.convertedFile); }
        }

        #endregion

        #region Construction
        public ConvertViewModel(IConvertService convertService)
        {
            this.convertService = convertService;
            this.dragTimer.Elapsed += DragTimer_Elapsed;

            this.convertService.ConvertStateChanged += (convertState, progressPercent) =>
            {
                this.ConvertState = convertState;

                switch (convertState)
                {
                    case ConvertState.Idle:
                        this.ProgressInformation = ResourceUtils.GetStringResource("Language_DropVideo");
                        break;
                    case ConvertState.Processing:
                        this.ProgressInformation = string.Concat(
                            ResourceUtils.GetStringResource("Language_Processing"),
                            progressPercent == -1 ? "..." : " " + Convert.ToInt32(progressPercent).ToString() + "%");
                        break;
                    case ConvertState.Downloading:

                        this.ProgressInformation = string.Concat(
                            ResourceUtils.GetStringResource("Language_Downloading"),
                            progressPercent == -1 ? "..." : " " + Convert.ToInt32(progressPercent).ToString() + "%");
                        break;
                    case ConvertState.Converting:
                        this.ProgressInformation = string.Concat(
                            ResourceUtils.GetStringResource("Language_Converting"),
                            progressPercent == -1 ? "..." : " " + Convert.ToInt32(progressPercent).ToString() + "%");
                        break;
                    case ConvertState.Success:
                        this.ProgressInformation = ResourceUtils.GetStringResource("Language_ConversionSuccessful");
                        break;
                    case ConvertState.Failed:
                        this.ProgressInformation = ResourceUtils.GetStringResource("Language_ConversionFailed");
                        break;
                    default:
                        break;
                }

                this.convertedFile = this.convertService.LastConvertedFile;

                RaisePropertyChanged(() => this.ConvertedFileName);
                RaisePropertyChanged(() => this.CanViewConvertedFile);
            };

            this.PasteCommand = new RelayCommand(() => HandlePaste());

            this.ViewInFolderCommand = new RelayCommand(() =>
            {
                if (!string.IsNullOrEmpty(this.convertedFile))
                {
                    try
                    {
                        if (System.IO.File.Exists(this.convertedFile))
                        {
                            Actions.TryViewInExplorer(this.convertedFile);
                        }
                        else
                        {
                            Process.Start(this.convertService.MusicFolder);
                        }

                    }
                    catch (Exception ex)
                    {
                        LogClient.Error("Could not view file '{0}' in explorer. Exception: {1}", this.convertedFile, ex.Message);
                    }

                }
            });

            this.PlayCommand = new RelayCommand(() =>
            {
                if (!string.IsNullOrEmpty(this.convertedFile))
                {
                    try
                    {
                        Process.Start(this.convertedFile);
                    }
                    catch (Exception ex)
                    {
                        LogClient.Error("Could not play file '{0}' in default audio player. Exception: {1}", this.convertedFile, ex.Message);
                    }
                }
            });

            this.ProgressInformation = ResourceUtils.GetStringResource("Language_DropVideo");
        }
        #endregion

        #region Event handlers
        private void DragTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.dragTimer.Stop();
            this.IsDraggingValidVideo = false;
        }
        #endregion

        #region Private
        private string GetDroppedUri(IDropInfo dropInfo)
        {
            var dataObject = dropInfo.Data as DataObject;
            string droppedUri = string.Empty;

            try
            {
                // First check if we're dragging text
                droppedUri = dataObject.GetText();

                if (string.IsNullOrWhiteSpace(droppedUri))
                {
                    // If we're not dragging text, we're probably dragging a file.
                    List<string> droppedFiles = dataObject.GetFileDropList().Cast<string>().ToList();
                    droppedUri = droppedFiles.FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                LogClient.Error("Could not get the dropped items. Exception: {0}", ex.Message);
            }

            return droppedUri;
        }

        private void HandlePaste()
        {
            // Get clipboard content
            string clipboardContent = string.Empty;

            try
            {
                clipboardContent = Clipboard.GetText();

                // Validate clipboard content
                if (!string.IsNullOrEmpty(clipboardContent))
                {
                    if (!this.convertService.IsValidVideo(clipboardContent))
                    {
                        return;
                    }

                    LogClient.Error("Processing clipboard content '{0}'", clipboardContent);

                    this.convertService.ProcessVideo(clipboardContent);
                }
            }
            catch (Exception ex)
            {
                LogClient.Error("Could not get the clipboard content. Exception: {0}", ex.Message);
            }
            string clipbaordContent = Clipboard.GetText();

        }
        #endregion

        #region IDropTarget
        public void DragOver(IDropInfo dropInfo)
        {
            try
            {
                this.dragTimer.Stop();
                string droppedUri = this.GetDroppedUri(dropInfo);

                // Validate dragged item
                if (!this.convertService.IsValidVideo(droppedUri))
                {
                    return;
                }

                // If a convertible video is selected, allow dragging.
                GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.DragOver(dropInfo);
                dropInfo.NotHandled = true;
                dropInfo.Effects = DragDropEffects.All; // Required to get the mouse cursor more or less right when dragging a URL
                this.IsDraggingValidVideo = true;
                this.dragTimer.Start();
            }
            catch (Exception ex)
            {
                dropInfo.NotHandled = false;
                this.IsDraggingValidVideo = false;
                LogClient.Error("Could not drop video. Exception: {0}", ex.Message);
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            this.dragTimer.Stop();
            this.IsDraggingValidVideo = false;
            string droppedUri = this.GetDroppedUri(dropInfo);

            // Validate dropped item
            if (!this.convertService.IsValidVideo(droppedUri))
            {
                return;
            }

            LogClient.Info("Processing dropped URI '{0}'", droppedUri);

            this.convertService.ProcessVideo(droppedUri);
        }
        #endregion
    }
}
