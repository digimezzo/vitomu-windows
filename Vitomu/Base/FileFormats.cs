namespace Vitomu.Base
{
    public static class FileFormats
    {
        public static string MP4 = ".mp4";
        public static string MKV = ".mkv";
        public static string MP3 = ".mp3";
        public static string FLAC = ".flac";
        public static string OGG = ".ogg";
        public static string WMA = ".wma";
        public static string M4A = ".m4a";

        public static string[] SupportedVideoExtensions = { FileFormats.MP4, FileFormats.MKV };
        public static string[] SupportedOutputAudioExtensions = { FileFormats.MP3, FileFormats.FLAC, FileFormats.OGG, FileFormats.WMA, FileFormats.M4A };
    }
}
