using System.Windows;
using POL.Lib.Interfaces.SmsSettings;
using POL.WPF.Controles.MVVM;

namespace POL.Lib.Common
{
    public class SmsPoxModel
    {
        public EnumSmsStatus Status { get; set; }
        public string Number { get; set; }
        public string Message { get; set; }
        public string Credit { get; set; }
        public string Usage { get; set; }
        public SmsModuleSettings Settings { get; set; }

        public RelayCommand<string> CommandDelete { get; set; }
        public RelayCommand<string> CommandEdit { get; set; }

        public Visibility VisibilitySerialPort
        {
            get
            {
                try
                {
                    return Settings.SmsDeviceType == EnumSmsDeviceType.Hardware
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
                catch
                {
                    return Visibility.Collapsed;
                }
            }
        }

        public Visibility VisibilityCloud
        {
            get
            {
                try
                {
                    return Settings.SmsDeviceType == EnumSmsDeviceType.Software
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
                catch
                {
                    return Visibility.Collapsed;
                }
            }
        }
    }
}
