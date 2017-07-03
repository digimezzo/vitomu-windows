using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Vitomu.Controls;

namespace Vitomu.Services.Dialog
{
    public class DialogService : IDialogService
    {
        #region Private
        private void ShowDialog(VitomuWindow win)
        {
            win.ShowDialog();
        }
        #endregion

        #region IDialogService
        public bool ShowCustomDialog(string title, UserControl content, int width, int height, bool canResize, bool autoSize, bool showCancelButton, string okText, string cancelText, Func<Task<bool>> callback)
        {
            bool returnValue = false;

            Application.Current.Dispatcher.Invoke(() =>
            {
                var dialog = new CustomDialog(title: title, content: content, width: width, height: height, canResize: canResize, autoSize: autoSize, showCancelButton: showCancelButton, okText: okText, cancelText: cancelText, callback: callback);
                this.ShowDialog(dialog);

                if (dialog.DialogResult.HasValue & dialog.DialogResult.Value)
                {
                    returnValue = true;
                }
                else
                {
                    returnValue = false;
                }
            });

            return returnValue;
        }
        #endregion
    }
}
