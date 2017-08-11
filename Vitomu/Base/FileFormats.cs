using Digimezzo.Utilities.Log;
using System.Linq;

namespace Vitomu.Base
{
    public static class FileFormats
    {
        public static string MP4 = ".mp4";
        public static string MKV = ".mkv";
        public static int minimumBitRate = 32;
        public static int maximumBitRate = 320;

        public static string[] SupportedVideoExtensions = { FileFormats.MP4, FileFormats.MKV };

        public static AudioFormat[] SupportedAudioFormats = {
            new AudioFormat("mp3","MP3","mp3",".mp3"),
            new AudioFormat("ogg","Ogg Vorbis","vorbis",".ogg"),
            new AudioFormat("flac","FLAC","flac",".flac")
        };

        public static AudioFormat GetValidAudioFormat(string audioFormatId)
        {
            AudioFormat validAudioFormat = SupportedAudioFormats.Select(f => f).Where(f => f.Id.Equals(audioFormatId)).FirstOrDefault();

            if (validAudioFormat == null)
            {
                LogClient.Warning("Audio format '{0}' was not found. Defaulting to MP3", audioFormatId);
                validAudioFormat = SupportedAudioFormats.First();
            }

            return validAudioFormat;
        }

        public static int GetValidBitRate(int bitRate)
        {
            if (bitRate < minimumBitRate)
            {
                LogClient.Warning("bitRate = {0}. bitRate < minimumBitRate. Using minimumBitRate.", bitRate);
                return minimumBitRate;
            }

            if (bitRate > maximumBitRate)
            {
                LogClient.Warning("bitRate = {0}. bitRate > maximumBitRate. Using maximumBitRate.", bitRate);
                return maximumBitRate;
            }

            return bitRate;
        }
    }
}
