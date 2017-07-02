using Digimezzo.Utilities.Packaging;
using System.Reflection;

namespace Vitomu.Packager
{
    class Packager
    {
        static void Main(string[] args)
        {
            Assembly asm = Assembly.GetEntryAssembly();
            AssemblyName an = asm.GetName();

            Configuration config;

#if DEBUG
            config = Configuration.Debug;
#else
		   config = Configuration.Release;
#endif

            var worker = new PackageCreator("Vitomu", an.Version, config);
            worker.Execute();
        }
    }
}
