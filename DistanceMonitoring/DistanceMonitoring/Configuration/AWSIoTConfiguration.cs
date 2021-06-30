using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistanceMonitoring.Configuration
{
    public class AWSIoTConfiguration
    {
        public string TopicName { get; set; }
        public string EndpointName { get; set; }
        public string CAcertPath { get; set; }
        public string ThingCertPath { get; set; }
        public string ThingCertPassword { get; set; }
        public int BrokerPort { get; set; }
        public string ClientId { get; set; }
    }
}
