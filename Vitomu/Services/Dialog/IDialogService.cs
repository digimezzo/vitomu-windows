using System;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Vitomu.Services.Dialog
{
    public interface IDialogService
    {
        bool ShowCustomDialog(string title, UserControl content, int width, int height, bool canResize, bool autoSize, bool showCancelButton, string okText, string cancelText, Func<Task<bool>> callback);
    }
}
