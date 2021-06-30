using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistanceMonitoring.Configuration
{
    public class AppConfig
    {
        public AWSIoTConfiguration AWSIoT { get; set; }
        public MqttConfiguration MQTT { get; set; }
    }
}
