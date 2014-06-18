namespace MottoBeneApps.GoHome
{
    using System.ComponentModel.Composition;
    using System.Reflection;
    using System.Windows;

    using Caliburn.Micro;

    using Gemini.Framework;
    using Gemini.Framework.Services;


    [Export(typeof(IModule))]
    public class MainModule : ModuleBase
    {
        public override void Initialize()
        {
            base.Initialize();

            MainWindow.WindowState = WindowState.Maximized;
            MainWindow.Title = "Go Home";
            MainWindow.Icon = IoC.Get<IResourceManager>().GetBitmap("Resources/GoHome.png", Assembly.GetExecutingAssembly().GetAssemblyName());
        }
    }
}