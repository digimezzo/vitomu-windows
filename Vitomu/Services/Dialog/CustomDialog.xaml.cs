using Digimezzo.Utilities.Utils;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Vitomu.Controls;

namespace Vitomu.Services.Dialog
{
    public partial class CustomDialog : VitomuWindow
    {
        #region Variables
        private Func<Task<bool>> callback;
        #endregion

        #region Construction
        public CustomDialog(string title, UserControl content, int width, int height, bool canResize, bool autoSize, bool showCancelButton, string okText, string cancelText, Func<Task<bool>> callback) : base()
        {
            InitializeComponent();

            this.Title = title;
            this.Width = width;
            this.MinWidth = width;
            this.CustomContent.Content = content;

            if (canResize)
            {
                this.ResizeMode = ResizeMode.CanResize;
                this.Height = height;
                this.MinHeight = height;
                this.SizeToContent = SizeToContent.Manual;
            }
            else
            {
                this.ResizeMode = ResizeMode.NoResize;

                if (autoSize)
                {
                    this.SizeToContent = SizeToContent.Height;
                }
                else
                {
                    this.Height = height;
                    this.MinHeight = height;
                    this.SizeToContent = SizeToContent.Manual;
                }
            }

            if (showCancelButton)
            {
                this.ButtonCancel.Visibility = Visibility.Visible;
            }
            else
            {
                this.ButtonCancel.Visibility = Visibility.Collapsed;
            }

            this.ButtonOK.Content = okText.ToUpper();
            this.ButtonCancel.Content = cancelText.ToUpper();

            this.callback = callback;

            WindowUtils.CenterWindow(this);
        }
        #endregion

        private async void ButtonOK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.callback != null)
            {
                // Prevents clicking the buttons when the callback is already executing, and this prevents this Exception:
                // System.InvalidOperationException: DialogResult can be set only after Window is created and shown as dialog.
                this.ButtonOK.IsEnabled = false;
                this.ButtonOK.IsDefault = false;
                this.ButtonCancel.IsEnabled = false;
                this.ButtonCancel.IsCancel = false;

                // Execute some function in the caller of this dialog.
                // If the result is False, DialogResult is not set.
                // That keeps the dialog open.
                if (await this.callback.Invoke())
                {
                    DialogResult = true;
                }
                else
                {
                    this.ButtonOK.IsEnabled = true;
                    this.ButtonOK.IsDefault = true;
                    this.ButtonCancel.IsEnabled = true;
                    this.ButtonCancel.IsCancel = true;
                }
            }
            else
            {
                DialogResult = true;
            }
        }

        private void ButtonCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
