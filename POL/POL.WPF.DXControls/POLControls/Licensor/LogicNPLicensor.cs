using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using POL.WPF.Licensor;

namespace POL.WPF.DXControls.POLControls.Licensor
{
    public class LogicNPLicensor : IHookController
    {
        private static readonly LogicNPLicensor Instance;

        private Control _ParentControl;

        static LogicNPLicensor()
        {
            Instance = new LogicNPLicensor();
        }

        private LogicNPLicensor()
        {
            var fi = typeof (LicenseManager).GetField("providers", BindingFlags.Static | BindingFlags.NonPublic);
            Hashtable providers = new CustomHashtable();
            if (fi != null) fi.SetValue(typeof (LicenseManager), providers);
            HookManager.DefaultManager.AddController(this);
        }

        private Control ParentControl
        {
            get { return _ParentControl ?? (_ParentControl = new Control()); }
        }

        bool IHookController.InternalPostFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam)
        {
            return false;
        }

        bool IHookController.InternalPreFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam)
        {
            var form = wnd as Form;
            if (form != null)
            {
                var nameSpace = form.GetType().ToString();
                var title = wnd.Text.Trim();
                if (nameSpace.StartsWith("A.ce4dd73f441f604fe2bc5087b17d35d5") ||
                    nameSpace.StartsWith("A.c3fdad19125856b461f6d1d5188f8e87b")
                    || title == "About ShComboBox.WPF" )
                {
                    Kill(form);
                }
            }
            return false;
        }

        IntPtr IHookController.OwnerHandle
        {
            get { return IntPtr.Zero; }
        }

        public static void License()
        {
        }

        private void Kill(Form about)
        {
            about.TopLevel = false;
            about.Parent = ParentControl;
            about.DialogResult = DialogResult.Cancel;
            try
            {
                about.Close();
            }
            catch
            {
            }
        }

        ~LogicNPLicensor()
        {
            HookManager.DefaultManager.RemoveController(this);
        }

        private class CustomLogicNPLicense : License
        {
            public override string LicenseKey
            {
                get { return "DJ-Esi"; }
            }

            public override void Dispose()
            {
            }
        }

        private class CustomLogicNPLicenseProvider : LicenseProvider
        {
            public override License GetLicense(LicenseContext context, Type type, object instance, bool allowExceptions)
            {
                return new CustomLogicNPLicense();
            }
        }

        private class CustomHashtable : Hashtable
        {
            public override object this[object key]
            {
                get
                {
                    var type = key as Type;
                    if (type != null)
                    {
                        var fullName = type.FullName;
                        if (fullName != null && fullName.ToLower().StartsWith("logicnp."))
                        {
                            return new CustomLogicNPLicenseProvider();
                        }
                    }
                    return base[key];
                }
                set { base[key] = value; }
            }
        }
    }
}
