using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace POL.WPF.Licensor
{
    public class LogicNPLicensor : IHookController
    {
        public static void License()
        {
        }

        bool IHookController.InternalPostFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam)
        {
            return false;
        }

        Control _ParentControl = null;
        public Control ParentControl
        {
            get { return _ParentControl ?? (_ParentControl = new Control()); }
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
        bool IHookController.InternalPreFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam)
        {
            if (wnd is Form)
            {
                var nameSpace = wnd.GetType().ToString();
                var title = wnd.Text.Trim();
                if (nameSpace.StartsWith("A.ce4dd73f441f604fe2bc5087b17d35d5") || nameSpace.StartsWith("A.c3fdad19125856b461f6d1d5188f8e87b")
                    || title == "About ShComboBox.WPF" /*A.c86dcaea489c0e73eb3142c652dce6156*/)
                {
                    Kill(wnd as Form);
                }
            }
            return false;
        }

        IntPtr IHookController.OwnerHandle
        {
            get { return IntPtr.Zero; }
        }

        static LogicNPLicensor()
        {
            Instance = new LogicNPLicensor();
        }
        static readonly LogicNPLicensor Instance = null;
        private LogicNPLicensor()
        {
            var fi = typeof(LicenseManager).GetField("providers", BindingFlags.Static | BindingFlags.NonPublic);
            Hashtable providers = new CustomHashtable();
            if (fi != null) fi.SetValue(typeof(LicenseManager), providers);
            HookManager.DefaultManager.AddController(this);
        }
        ~LogicNPLicensor()
        {
            HookManager.DefaultManager.RemoveController(this);
        }
        class CustomLogicNPLicense : License
        {

            public override string LicenseKey
            {
                get
                {
                    return "DJ-Esi";
                }
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
                        var fullName = (type).FullName;
                        if (fullName != null && fullName.ToLower().StartsWith("logicnp."))
                        {
                            return new CustomLogicNPLicenseProvider();
                        }
                    }
                    return base[key];
                }
                set
                {
                    base[key] = value;

                }
            }
        }
    }
}
