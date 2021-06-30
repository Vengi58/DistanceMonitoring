using DistanceMonitoring.Model;

namespace DistanceMonitoring
{
    public interface IDistanceDataController
    {
        public delegate void DistanceDataEventHandler(object sender, DistanceDataEventArgs e);
        event DistanceDataEventHandler DistanceDataReceived;
    }
}
