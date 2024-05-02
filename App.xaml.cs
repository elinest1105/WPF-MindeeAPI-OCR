using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Data;
using System.Windows;
using System.IO;

namespace MindeeAPI_OCR
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            base.OnStartup(e);
        }
    }

}
