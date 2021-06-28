using System;

namespace DistanceMonitoring.Controller.Adapters
{
    public interface IMqttAdapter : IDisposable
    {
        public delegate void MessageReceivedEventHandler(object sender, MqttMessageEventArgs e);
        event MessageReceivedEventHandler MessageReceived;
    }
}
