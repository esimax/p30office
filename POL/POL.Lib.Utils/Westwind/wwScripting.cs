using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace POL.Lib.Utils.Westwind
{
    public delegate void DelegateCompleted(object sender, EventArgs e);

    public class wwScripting
    {
        protected ICodeCompiler oCompiler;

        protected CompilerParameters oParameters;

        protected Assembly oAssembly;

        protected CompilerResults oCompiled;
        protected string cOutputAssembly;
        protected string cNamespaces = "";
        protected bool lFirstLoad = true;



        public object oObjRef;

        public bool lSaveSourceCode = false;

        public string cSourceCode = "";

        protected int nStartCodeLine = 0;

        public string cAssemblyNamespace = "WestWindScripting";

        public string cClassName = "WestWindScript";

        public bool lDefaultAssemblies = true;

        protected AppDomain oAppDomain;

        public string cErrorMsg = "";
        public bool bError;

        public string cSupportAssemblyPath = "";

        public string cScriptingLanguage = "CSharp";

        public wwScripting(string lcLanguage)
        {
            SetLanguage(lcLanguage);
        }
        public wwScripting()
        {
            SetLanguage("CSharp");
        }



        public void SetLanguage(string lcLanguage)
        {
            cScriptingLanguage = lcLanguage;

            if (cScriptingLanguage == "CSharp" || cScriptingLanguage == "C#")
            {
                oCompiler = new CSharpCodeProvider().CreateCompiler();
                cScriptingLanguage = "CSharp";
            }
            else if (cScriptingLanguage == "VB")
            {
                oCompiler = new VBCodeProvider().CreateCompiler();
            }

            oParameters = new CompilerParameters();
        }


        public void AddAssembly(string lcAssemblyDll, string lcNamespace)
        {
            if (lcAssemblyDll == null && lcNamespace == null)
            {
                oParameters.ReferencedAssemblies.Clear();
                cNamespaces = "";
                return;
            }

            if (lcAssemblyDll != null)
                oParameters.ReferencedAssemblies.Add(lcAssemblyDll);

            if (lcNamespace != null)
                if (cScriptingLanguage == "CSharp")
                    cNamespaces = string.Format("{0}using {1};\r\n", cNamespaces, lcNamespace);
                else
                    cNamespaces = string.Format("{0}imports {1}\r\n", cNamespaces, lcNamespace);
        }

        public void AddAssembly(string lcAssemblyDll)
        {
            AddAssembly(lcAssemblyDll, null);
        }
        public void AddNamespace(string lcNamespace)
        {
            AddAssembly(null, lcNamespace);
        }
        public void AddDefaultAssemblies()
        {
            AddAssembly("System.dll", "System");
            AddNamespace("System.Reflection");
            AddNamespace("System.IO");
        }


        public object ExecuteMethod(string lcCode, string lcMethodName, params object[] loParameters)
        {

            if (oObjRef == null)
            {
                if (lFirstLoad)
                {
                    if (lDefaultAssemblies)
                    {
                        AddDefaultAssemblies();
                    }
                    AddAssembly(cSupportAssemblyPath + "POL.Lib.Utils.dll", "POL.Lib.Utils.Westwind");
                    lFirstLoad = false;
                }

                StringBuilder sb = new StringBuilder("");

                sb.Append(cNamespaces);
                sb.Append("\r\n");

                if (cScriptingLanguage == "CSharp")
                {
                    sb.AppendFormat("namespace {0}{{\r\npublic class {1}:MarshalByRefObject,IRemoteInterface {{\r\n", cAssemblyNamespace, cClassName);

                    sb.Append(
                        "public object Invoke(string lcMethod,object[] parms) {\r\n" +
                        "return this.GetType().InvokeMember(lcMethod,BindingFlags.InvokeMethod,null,this,parms );\r\n" +
                        "}\r\n\r\n");

                    sb.Append(lcCode);

                    sb.Append("\r\n} }");  // Class and namespace closed
                }
                else if (cScriptingLanguage == "VB")
                {
                    sb.AppendFormat("Namespace {0}\r\npublic class {1}\r\n", cAssemblyNamespace, cClassName);
                    sb.Append("Inherits MarshalByRefObject\r\nImplements IRemoteInterface\r\n\r\n");

                    sb.Append(
                        "Public Overridable Overloads Function Invoke(ByVal lcMethod As String, ByVal Parameters() As Object) As Object _\r\n" +
                        "Implements IRemoteInterface.Invoke\r\n" +
                        "return me.GetType().InvokeMember(lcMethod,BindingFlags.InvokeMethod,nothing,me,Parameters)\r\n" +
                        "End Function\r\n\r\n");

                    sb.Append(lcCode);

                    sb.Append("\r\n\r\nEnd Class\r\nEnd Namespace\r\n");  // Class and namespace closed
                }

                if (lSaveSourceCode)
                {
                    cSourceCode = sb.ToString();
                }

                if (!CompileAssembly(sb.ToString()))
                    return null;

                object loTemp = CreateInstance();
                if (loTemp == null)
                    return null;
            }

            return CallMethod(oObjRef, lcMethodName, loParameters);
        }

        public object ExecuteCode(string lcCode, params object[] loParameters)
        {
            if (cScriptingLanguage == "CSharp")
                return ExecuteMethod("public object ExecuteCode(params object[] Parameters) {" +
                        lcCode +
                        "}",
                        "ExecuteCode", loParameters);
            else if (cScriptingLanguage == "VB")
                return ExecuteMethod("public function ExecuteCode(ParamArray Parameters() As Object) as object\r\n" +
                    lcCode +
                    "\r\nend function\r\n",
                    "ExecuteCode", loParameters);

            return null;
        }

        public bool CompileAssembly(string lcSource)
        {

            if (oAppDomain == null && cOutputAssembly == null)
                oParameters.GenerateInMemory = true;
            else if (oAppDomain != null && cOutputAssembly == null)
            {
                cOutputAssembly = "wws_" + Guid.NewGuid().ToString() + ".dll";
                oParameters.OutputAssembly = cOutputAssembly;
            }
            else
            {
                oParameters.OutputAssembly = cOutputAssembly;
            }

            oCompiled = oCompiler.CompileAssemblyFromSource(oParameters, lcSource);

            if (oCompiled.Errors.HasErrors)
            {
                bError = true;

                cErrorMsg = oCompiled.Errors.Count.ToString() + " Errors:";
                for (int x = 0; x < oCompiled.Errors.Count; x++)
                    cErrorMsg = string.Format("{0}\r\nLine: {1} - {2}", cErrorMsg, oCompiled.Errors[x].Line, oCompiled.Errors[x].ErrorText);
                return false;
            }

            if (oAppDomain == null)
                oAssembly = oCompiled.CompiledAssembly;

            return true;
        }

        public object CreateInstance()
        {
            if (oObjRef != null)
            {
                return oObjRef;
            }

            try
            {
                if (oAppDomain == null)
                    try
                    {
                        oObjRef = oAssembly.CreateInstance(string.Format("{0}.{1}", cAssemblyNamespace, cClassName));
                        return oObjRef;
                    }
                    catch (Exception ex)
                    {
                        bError = true;
                        cErrorMsg = ex.Message;
                        return null;
                    }
                else
                {
                    RemoteLoaderFactory factory = (RemoteLoaderFactory)oAppDomain.CreateInstance("RemoteLoader", "POL.Lib.Utils.Westwind.RemoteLoader.RemoteLoaderFactory").Unwrap();

                    oObjRef = factory.Create(cOutputAssembly, string.Format("{0}.{1}", cAssemblyNamespace, cClassName), null);

                    return oObjRef;
                }
            }
            catch (Exception ex)
            {
                bError = true;
                cErrorMsg = ex.Message;
                return null;
            }

        }

        public object CallMethod(object loObject, string lcMethod, params object[] loParameters)
        {
            try
            {
                if (oAppDomain == null)
                    return loObject.GetType().InvokeMember(lcMethod, BindingFlags.InvokeMethod, null, loObject, loParameters);
                else
                {
                    object loResult;
                    try
                    {
                        IRemoteInterface loRemote = (IRemoteInterface)loObject;

                        loResult = loRemote.Invoke(lcMethod, loParameters);
                    }
                    catch (Exception ex)
                    {
                        bError = true;
                        cErrorMsg = ex.Message;
                        return null;
                    }
                    return loResult;
                }
            }
            catch (Exception ex)
            {
                bError = true;
                cErrorMsg = ex.Message;
            }
            return null;
        }

        public bool CreateAppDomain(string lcAppDomain)
        {
            if (lcAppDomain == null)
                lcAppDomain = "wwscript";

            AppDomainSetup loSetup = new AppDomainSetup {ApplicationBase = AppDomain.CurrentDomain.BaseDirectory};

            oAppDomain = AppDomain.CreateDomain(lcAppDomain, null, loSetup);
            return true;
        }

        public bool UnloadAppDomain()
        {
            if (oAppDomain != null)
                AppDomain.Unload(oAppDomain);

            oAppDomain = null;

            if (cOutputAssembly != null)
            {
                try
                {
                    File.Delete(cOutputAssembly);
                }
                catch (Exception) {
                }
            }

            return true;
        }
        public void Release()
        {
            oObjRef = null;
        }

        public void Dispose()
        {
            Release();
            UnloadAppDomain();
        }

        ~wwScripting()
        {
            Dispose();
        }
    }
}
