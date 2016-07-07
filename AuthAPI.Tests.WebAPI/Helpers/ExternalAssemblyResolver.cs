using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dispatcher;

namespace AuthAPI.Tests.WebAPI.Helpers
{
    public class ExternalAssemblyResolver : DefaultAssembliesResolver
    {
        public virtual ICollection<Assembly> GetAssemblies()
        {
            ICollection<Assembly> baseAssemblies = base.GetAssemblies();
            List<Assembly> assemblies = new List<Assembly>(baseAssemblies);
            var controllersAssembly = Assembly.LoadFrom(@"AuthAPI.Samples.WebAPI.dll");
            baseAssemblies.Add(controllersAssembly);

            return baseAssemblies;
        }
    }
}
