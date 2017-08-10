using Vitomu.Base;

namespace Vitomu.Services.Convert
{
    public delegate void ConvertStateChangedHander(ConvertState convertState, double convertPercent);
    public interface IConvertService
    {
        event ConvertStateChangedHander ConvertStateChanged;
        string LastConvertedFile { get; }
        string MusicFolder { get; }
        bool IsValidVideo(string uri);
        void ProcessVideo(string uri);
        void KillProcesses();
    }
}