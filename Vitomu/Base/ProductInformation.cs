using Digimezzo.Utilities.Packaging;
using System;

namespace Vitomu.Base
{
    public sealed class ProductInformation
    {
        #region Variables
        public static string ApplicationDisplayName = "Vitomu";
        public static string Copyright = "Copyright Digimezzo © " + DateTime.Now.Year;
        #endregion

        #region Components
        public static ExternalComponent[] Components = {
             new ExternalComponent {
                Name = "FFmpeg",
                Description = "A complete, cross-platform solution to record, convert and stream audio and video.",
                Url = "https://ffmpeg.org/",
                LicenseUrl = "https://ffmpeg.org/legal.html"
            },
            new ExternalComponent {
                Name = "MVVM Light Toolkit",
                Description = "A toolkit is to accelerate the creation and development of MVVM applications in WPF, Silverlight, Windows Store, Windows Phone and Xamarin.",
                Url = "https://mvvmlight.codeplex.com/",
                LicenseUrl = "https://mvvmlight.codeplex.com/license"
            },
            new ExternalComponent {
                Name = "Youtube-dl",
                Description = "A command-line program to download videos from YouTube.com and a few more sites.",
                Url = "https://github.com/rg3/youtube-dl/",
                LicenseUrl = "https://github.com/rg3/youtube-dl/blob/master/LICENSE"
            }
        };
        #endregion
    }
}
