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

        //public DistanceDataController() : this (new AWSIoTAdapter("distanceData"))
        //{
        //    //adapter = new AWSIoTAdapter("distanceData");
        //    //adapter = new MqttAdapter("distanceData");
        //}

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    adapter.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DistanceDataController()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
