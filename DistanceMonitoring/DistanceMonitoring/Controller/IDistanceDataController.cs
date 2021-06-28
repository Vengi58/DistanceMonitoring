using DistanceMonitoring.Model;

namespace DistanceMonitoring
{
    public interface IDistanceDataController
    {
        void PostDistanceData(DistanceData distanceData);
        public delegate void DistanceDataEventHandler(object sender, DistanceDataEventArgs e);
        event DistanceDataEventHandler DistanceDataReceived;
    }
}
