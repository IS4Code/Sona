using System;
using System.Globalization;
using System.Runtime.Loader;
using System.Text;
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
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
                if(SonaCompiler.ResolveEmbeddedAssembly(args.Name) is not { } stream)
                {
                    return null;
                }
#if NETCOREAPP
                return AssemblyLoadContext.Default.LoadFromStream(stream);
#else
                return AppDomainAssemblyLoader.Instance.AssemblyLoadStream(stream);
#endif
            };

#if NETCOREAPP
            ApplicationConfiguration.Initialize();
#else
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
#endif
            Application.Run(new MainForm());
        }
    }
}