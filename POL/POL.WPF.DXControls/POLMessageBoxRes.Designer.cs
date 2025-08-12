
namespace POL.WPF.DXControls {
    using System;
    
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class POLMessageBoxRes {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal POLMessageBoxRes() {
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("POL.WPF.DXControls.POLMessageBoxRes", typeof(POLMessageBoxRes).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        public static string POLMessageBoxStringId_Cancel {
            get {
                return ResourceManager.GetString("POLMessageBoxStringId.Cancel", resourceCulture);
            }
        }
        
        public static string POLMessageBoxStringId_No {
            get {
                return ResourceManager.GetString("POLMessageBoxStringId.No", resourceCulture);
            }
        }
        
        public static string POLMessageBoxStringId_Ok {
            get {
                return ResourceManager.GetString("POLMessageBoxStringId.Ok", resourceCulture);
            }
        }
        
        public static string POLMessageBoxStringId_Yes {
            get {
                return ResourceManager.GetString("POLMessageBoxStringId.Yes", resourceCulture);
            }
        }
    }
}
