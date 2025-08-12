using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CSharp;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using System;
using POL.Lib.Utils.Westwind;
using POL.WPF.DXControls;

namespace POC.Shell.Adapters
{
    internal class AdapterPOCEventUnit : IPOCEventUnit
    {
        #region CTOR
        public AdapterPOCEventUnit()
        {
            Holder = new Dictionary<string, POCEventItem>();
        }
        #endregion

        private Dictionary<string, POCEventItem> Holder { get; set; }

        #region IPOCEventUnit
        public List<POCEventItem> GetList()
        {
            var aMembership = ServiceLocator.Current.GetInstance<IMembership>();
            var q = from n in Holder orderby n.Value.Order select n.Value;
            return q.Any() ? q.ToList() : new List<POCEventItem>();
        }

        public void Register(POCEventItem item)
        {
            if (item == null) return;
            if (Holder.ContainsKey(item.Key)) return;
            Holder.Add(item.Key, item);
        }
        #endregion


        public void InvokeByKey(string key, params object[] args)
        {
            var aDatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            var xpc = DBCTEvent.GetByKey(aDatabase.Dxs, key);

            foreach (var dbe in xpc)
            {

                RunCode(dbe,args);
            }
        }

        private void RunCode(DBCTEvent dbe, params object[] args)
        {
            var aLogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();

            string lcCode = dbe.Script;
            var MyNamespace = "NS" + Guid.NewGuid().ToString().Replace("-", "");

            var loSetup = new AppDomainSetup();
            loSetup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            var loAppDomain = AppDomain.CreateDomain("MyAppDomain", null, loSetup);


            lcCode = @"
using System;
using System.IO;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.Lib.Utils.Westwind;

using System.Reflection;

namespace " + MyNamespace + @"{
public class MyClass : MarshalByRefObject,IRemoteInterface  {

public object Invoke(string lcMethod,object[] Parameters) {
	return this.GetType().InvokeMember(lcMethod,
            BindingFlags.InvokeMethod,null,this,Parameters);
}

public object DynamicCode(params object[] Parameters) {
" + lcCode +
                "}   }    }";

            var loCompiler = new CSharpCodeProvider().CreateCompiler();
            var loParameters = new CompilerParameters();

            loParameters.ReferencedAssemblies.Add("System.dll");
            loParameters.ReferencedAssemblies.Add("POL.Lib.Interfaces.dll");
            loParameters.ReferencedAssemblies.Add("POL.Lib.Utils.dll");
            

            loParameters.GenerateInMemory = true;
            loParameters.OutputAssembly = MyNamespace + ".dll";

            CompilerResults loCompiled = loCompiler.CompileAssemblyFromSource(loParameters, lcCode);

            if (loCompiled.Errors.HasErrors)
            {
                string lcErrorMsg = "";

                lcErrorMsg = loCompiled.Errors.Count.ToString() + " Errors:";
                for (int x = 0; x < loCompiled.Errors.Count; x++)
                    lcErrorMsg = lcErrorMsg + "\r\nLine: " + loCompiled.Errors[x].Line.ToString() + " - " +
                        loCompiled.Errors[x].ErrorText;

                aLogger.Log(lcErrorMsg + "\r\n\r\n" + lcCode,Category.Warn, Priority.High);
                dbe.Errors = 1;
                dbe.Save();
                return;
            }


            var factory =(RemoteLoaderFactory)loAppDomain.CreateInstance("POL.Lib.Utils","POL.Lib.Utils.Westwind.RemoteLoaderFactory").Unwrap();

            object loObject = factory.Create(MyNamespace + ".dll", MyNamespace + ".MyClass", null);

            var loRemote = (IRemoteInterface)loObject;

            if (loObject == null)
            {
                aLogger.Log("Couldn't load class.", Category.Warn, Priority.High);
                dbe.Errors = 2;
                dbe.Save();
                return;
            }

            object[] loCodeParms = args;

            try
            {
                object loResult = loRemote.Invoke("DynamicCode", loCodeParms);


                dbe.RunCount += 1;
                dbe.Errors = 0;
                dbe.Save();
            }
            catch (Exception loError)
            {
                aLogger.Log(loError.Message, Category.Warn, Priority.High);
                dbe.Errors = 2;
                dbe.Save();
            }

            loRemote = null;
            AppDomain.Unload(loAppDomain);
            loAppDomain = null;
            File.Delete(MyNamespace + ".dll");
            GC.Collect();
            GC.WaitForFullGCComplete();
            GC.Collect();

        }
    }
}
