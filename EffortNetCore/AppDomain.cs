using Microsoft.Extensions.DependencyModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class AppDomain
{
    public static AppDomain CurrentDomain { get; private set; }

    static AppDomain()
    {
        CurrentDomain = new AppDomain();
    }

    public Assembly[] GetAssembliesFilteredBy(string assemblyName)
    {
        var assemblies = new List<Assembly>();
        var dependencies = DependencyContext.Default.RuntimeLibraries;
        foreach (var library in dependencies)
        {
            if (library.Name.ToLower().Contains(assemblyName.ToLower()))
            {
                var assembly = Assembly.Load(new AssemblyName(library.Name));
                assemblies.Add(assembly);
            }
        }
        return assemblies.ToArray();
    }
}