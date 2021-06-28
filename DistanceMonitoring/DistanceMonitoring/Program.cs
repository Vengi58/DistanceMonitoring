using DistanceMonitoring.Controller;
using DistanceMonitoring.View;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DistanceMonitoring
{
    class Program
    {
        private static ManualResetEvent manualResetEvent;
        static void Main(string[] args)
        {
            //allow time for the broker to start when starting together in VS
            Thread.Sleep(TimeSpan.FromSeconds(5));
            using var controller = new DistanceDataController();
            var monitor = new DistanceMonitor(controller);
            monitor.StartMonitoring();

            // Keep the main thread alive for the event receivers to get invoked
            KeepConsoleAppRunning(() =>
            {
                Console.WriteLine("Shutting down..");
            });
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
