using DistanceMonitoring.Model;
using System;
using System.Text;
using MQTTnet;
using System.Text.Json;
using DistanceMonitoring.Controller.Adapters;

namespace DistanceMonitoring.Controller
{
    public class DistanceDataController : IDistanceDataController, IDisposable
    {
        public event IDistanceDataController.DistanceDataEventHandler DistanceDataReceived;
        private bool disposedValue;
        private readonly IMqttAdapter adapter;

        public DistanceDataController(IMqttAdapter _adapter)
        {
            adapter = _adapter;
            adapter.MessageReceived += (s, e) =>
            {
                var distanceData = JsonSerializer.Deserialize<DistanceData>(Encoding.UTF8.GetString(e.Payload));
                DistanceDataReceived(s, new DistanceDataEventArgs(distanceData));
            };
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    adapter.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
