using DistanceMonitoring.Model;

namespace DistanceMonitoring
{
    public class DistanceDataEventArgs
    {
        public DistanceDataEventArgs(DistanceData distanceData)
        {
            DistanceData = distanceData;
        }
        public DistanceData DistanceData { get; }
    }
}