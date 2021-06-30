using System;

namespace DistanceMonitoring.View
{
    public class DistanceMonitor
    {
        private readonly IDistanceDataController controller;
        public DistanceMonitor(IDistanceDataController distanceDataController)
        {
            controller = distanceDataController;
        }

        public void StartMonitoring()
        {
            controller.DistanceDataReceived += Controller_DistanceDataReceived;
        }
        public void StopMonitoring()
        {
            controller.DistanceDataReceived -= Controller_DistanceDataReceived;
        }

        private void Controller_DistanceDataReceived(object sender, DistanceDataEventArgs e)
        {
            Console.WriteLine("Distance data received:");
            Console.WriteLine(e.DistanceData.Value);
            double distance = e.DistanceData.Value;
            switch (distance)
            {
                case double d when (d < 10):
                    Console.WriteLine("Dangerously close!");
                    break;
                case double d when (d > 100):
                    Console.WriteLine("Out of sight!");
                    break;
                default:
                    Console.WriteLine("In safe zone.");
                    break;
            }
        }
    }
}
