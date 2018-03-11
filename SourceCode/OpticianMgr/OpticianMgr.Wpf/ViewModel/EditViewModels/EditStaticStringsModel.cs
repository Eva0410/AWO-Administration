using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using OpticianMgr.Wpf.WindowServices;
using System;
using System.Windows;
using System.Windows.Input;

namespace OpticianMgr.Wpf.ViewModel
{
    public class EditStaticStringsModel : ViewModelBase, IRequestClose
    {

        public event EventHandler<EventArgs> CloseRequested;
        public static string SingleEmailSubjectGlassesOrder { get { return Properties.Settings.Default.SingleEmailSubjectGlassesOrder; } set { Properties.Settings.Default.SingleEmailSubjectGlassesOrder = value; } }
        public static string SingleEmailTextGlassesOrder { get { return Properties.Settings.Default.SingleEmailTextGlassesOrder; } set { Properties.Settings.Default.SingleEmailTextGlassesOrder = value; } }
        public static string SingleEMailSubjectContactLensOrder { get { return Properties.Settings.Default.SingleEMailSubjectContactLensOrder; } set { Properties.Settings.Default.SingleEMailSubjectContactLensOrder = value; } }
        public static string SingleEmailTextContactLensOrder { get { return Properties.Settings.Default.SingleEmailTextContactLensOrder; } set { Properties.Settings.Default.SingleEmailTextContactLensOrder = value; } }
        public static string SingleSMSTextGlassesOrder { get { return Properties.Settings.Default.SingleSMSTextGlassesOrder; } set { Properties.Settings.Default.SingleSMSTextGlassesOrder = value; } }
        public static string SingleSMSTextContactLensOrder { get { return Properties.Settings.Default.SingleSMSTextContactLensOrder; } set { Properties.Settings.Default.SingleSMSTextContactLensOrder = value; } }
        public static string SmsSenderText { get { return Properties.Settings.Default.SmsSenderText; } set { Properties.Settings.Default.SmsSenderText = value; } }
        public ICommand CloseCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public EditStaticStringsModel()
        {
            CloseCommand = new RelayCommand(Cancel);
            SaveCommand = new RelayCommand(Save);
        }
        private void Save()
        {
            Properties.Settings.Default.Save();
            this.CloseRequested?.Invoke(this, null);
        }
        private void Cancel()
        {
            Properties.Settings.Default.Reload();
            this.CloseRequested?.Invoke(this, null);
        }

    }
}
