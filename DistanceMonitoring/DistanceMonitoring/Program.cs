using DistanceMonitoring.Configuration;
using DistanceMonitoring.Controller;
using DistanceMonitoring.Controller.Adapters;
using DistanceMonitoring.View;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;

namespace DistanceMonitoring
{
    class Program
    {
        private static ManualResetEvent manualResetEvent;

        static void Main(string[] args)
        {
            var application = ConfigureServices().Services.GetService<DistanceMonitor>();
            application.StartMonitoring();
            KeepConsoleAppRunning(() =>
            {
                Console.WriteLine("Shutting down..");
                application.StopMonitoring();
            });
        }

        private static IHost ConfigureServices()
        {
            var configRoot = new ConfigurationBuilder()
                .AddJsonFile($"appSettings.json", true, true)
                .Build();
            var appConfig = configRoot.GetSection("AppConfig").Get<AppConfig>();

            return new HostBuilder()
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddOptions();
                        services.AddSingleton<IConfigProvider>(c => new ConfigProvider(appConfig));
                        services.AddSingleton<IMqttAdapter, MqttAdapter>();
                        services.AddSingleton<IDistanceDataController, DistanceDataController>();
                        services.AddTransient<DistanceMonitor>();
                    })
              .Build();
        }
        private static void KeepConsoleAppRunning(Action onShutdown)
        {
            manualResetEvent = new ManualResetEvent(false);
            Console.WriteLine("Press CTRL + C or CTRL + Break to exit...");

            Console.CancelKeyPress += (sender, e) =>
            {
                onShutdown();
                e.Cancel = true;
                manualResetEvent.Set();
            };

            manualResetEvent.WaitOne();
        }
    }
}
