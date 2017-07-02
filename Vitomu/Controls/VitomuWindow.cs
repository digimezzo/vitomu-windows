using Digimezzo.WPFControls;
using System.Windows;

namespace Vitomu.Controls
{
    public class VitomuWindow : BorderlessWindows8Window
    {
        #region Construction
        static VitomuWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VitomuWindow), new FrameworkPropertyMetadata(typeof(VitomuWindow)));
        }
        #endregion
    }
}
