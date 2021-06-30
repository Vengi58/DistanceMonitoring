using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistanceMonitoring.Configuration
{
    public class MqttConfiguration
    {
        public string TopicName { get; set; }
        public string ClientId { get; set; }
        public string Host { get; set; }
        public int Port { get; set; } 
    }
}
