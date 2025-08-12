using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;

namespace POL.WPF.DXControls.POLDateEdit
{
    public class MyDateEdit : DateEdit
    {
        protected override void OnLoadedInternal()
        {
            base.OnLoadedInternal();
            GeneratePopupContentTemplate();
        }

        private void GeneratePopupContentTemplate()
        {
            PopupContentTemplate = CreatePopupContentTemplate();
        }

        private static ControlTemplate CreatePopupContentTemplate()
        {
            var popupContentTemplate = new ControlTemplate();
            var element = new FrameworkElementFactory(typeof (MyDateEditCalendar));
            element.SetValue(SnapsToDevicePixelsProperty, true);
            element.SetValue(FocusHelper2.FocusableProperty, false);
            popupContentTemplate.VisualTree = element;
            return popupContentTemplate;
        }

        protected override EditStrategyBase CreateEditStrategy()
        {
            return new MyDateEditStrategy(this);
        }

        public static void RegisterEditor()
        {
            EditorSettingsProvider.Default.RegisterUserEditor(typeof (MyDateEdit), typeof (DateEditSettings),
                () => new MyDateEdit(),
                () => new DateEditSettings {PopupContentTemplate = CreatePopupContentTemplate()});
        }









    }
}
