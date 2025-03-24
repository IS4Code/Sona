using System;
using System.Globalization;
using System.Runtime.Loader;
using System.Windows.Forms;

namespace Sona.Compiler.Gui
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
                if(SonaCompiler.ResolveEmbeddedAssembly(args.Name) is not { } stream)
                {
                    return null;
                }
                return AssemblyLoadContext.Default.LoadFromStream(stream);
            };

            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}