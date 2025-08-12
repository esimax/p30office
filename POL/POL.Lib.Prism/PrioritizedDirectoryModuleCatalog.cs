using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using Microsoft.Practices.Prism.Modularity;
using POL.Lib.Interfaces;

namespace POL.Lib.Prism
{
    [SecurityPermission(SecurityAction.InheritanceDemand), SecurityPermission(SecurityAction.LinkDemand)]
    public class PrioritizedDirectoryModuleCatalog : DirectoryModuleCatalog
    {
        private Dictionary<string, int> GetModulePriorities(IEnumerable<ModuleInfo> modules)
        {
            var childDomain = BuildChildDomain(AppDomain.CurrentDomain);
            try
            {
                var loaderType = typeof (ModulePriorityLoader);
                var loader =
                    (ModulePriorityLoader)
                        childDomain.CreateInstanceFrom(loaderType.Assembly.Location, loaderType.FullName).Unwrap();

                return loader.GetPriorities(modules);
            }
            finally
            {
                AppDomain.Unload(childDomain);
            }
        }

        protected override IEnumerable<ModuleInfo> Sort(IEnumerable<ModuleInfo> modules)
        {
            var priorities = GetModulePriorities(modules);
            var result = new List<ModuleInfo>(base.Sort(modules));
            result.Sort((x, y) =>
            {
                var xModuleName = x.ModuleName;
                var yModuleName = y.ModuleName;
                if (x.DependsOn.Contains(yModuleName))
                    return 1; 
                if (y.DependsOn.Contains(xModuleName))
                    return -1;
                return priorities[xModuleName].CompareTo(priorities[yModuleName]);
            });

            return result;
        }

        private class ModulePriorityLoader : MarshalByRefObject
        {
            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"),
             SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods",
                 MessageId = "System.Reflection.Assembly.LoadFrom")]
            public Dictionary<string, int> GetPriorities(IEnumerable<ModuleInfo> modules)
            {
                var priorities = new Dictionary<string, int>();
                var assemblies = new Dictionary<string, Assembly>();
                var exev = Assembly.GetExecutingAssembly().GetName().Version;

                foreach (var module in modules)
                {
                    if (!assemblies.ContainsKey(module.Ref))
                    {
                        assemblies.Add(module.Ref, Assembly.LoadFrom(module.Ref));
                    }

                    var type =
                        assemblies[module.Ref].GetExportedTypes()
                            .First(
                                t =>
                                    t.AssemblyQualifiedName != null &&
                                    t.AssemblyQualifiedName.Equals(module.ModuleType, StringComparison.Ordinal));

                    var priorityAttribute =
                        CustomAttributeData.GetCustomAttributes(type).FirstOrDefault(
                            cad =>
                                cad.Constructor.DeclaringType != null &&
                                cad.Constructor.DeclaringType.FullName == typeof (PriorityAttribute).FullName);


                    var versionAttribute =
                        CustomAttributeData.GetCustomAttributes(type).FirstOrDefault(
                            cad =>
                                cad.Constructor.DeclaringType != null &&
                                cad.Constructor.DeclaringType.FullName == typeof (VersionAttribute).FullName);

                    if (versionAttribute != null)
                    {
                        var m = assemblies[module.Ref];
                        var v = m.GetName(false).Version;
                        if (v != exev)
                            break;
                    }
                    else
                        break;

                    int priority;
                    if (priorityAttribute != null)
                    {
                        priority = (int) priorityAttribute.ConstructorArguments[0].Value;
                    }
                    else
                    {
                        priority = 0;
                    }

                    priorities.Add(module.ModuleName, priority);
                }

                return priorities;
            }
        }
    }
}
