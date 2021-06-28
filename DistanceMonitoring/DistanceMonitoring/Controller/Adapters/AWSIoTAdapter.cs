using System;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using uPLibrary.Networking.M2Mqtt;
using System.IO;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace DistanceMonitoring.Controller.Adapters
{
    public class AWSIoTAdapter : IMqttAdapter
    {
        public event IMqttAdapter.MessageReceivedEventHandler MessageReceived;
        private readonly MqttClient client;
        public AWSIoTAdapter(string topic)
        {
            string iotEndpoint = "<ENDPOINT>";
            Console.WriteLine("AWS IoT Dotnet message publisher starting..");

            int brokerPort = 8883;

            var caCert = X509Certificate.CreateFromCertFile(Path.Combine(@"thing_certs", "root-CA.crt"));
            var clientCert = new X509Certificate2(Path.Combine(@"thing_certs", "distance-thing.pfx"), "<PASSWORD>");

            client = new MqttClient(iotEndpoint, brokerPort, true, caCert, clientCert, MqttSslProtocols.TLSv1_2);
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            client.MqttMsgSubscribed += Client_MqttMsgSubscribed;

            string clientId = Guid.NewGuid().ToString();
            client.Connect(clientId);
            Console.WriteLine($"Connected to AWS IoT with client id: {clientId}.");

            client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
        }

        private void Client_MqttMsgSubscribed(object sender, EventArgs e)
        {
            Console.WriteLine($"Successfully subscribed to the AWS IoT topic.");
        }

        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Console.WriteLine("Message received: " + Encoding.UTF8.GetString(e.Message));
            if (MessageReceived != null)
            {
                MessageReceived.Invoke(this, new MqttMessageEventArgs { Payload = e.Message });
            }
        }

        public void Dispose()
        {
            client.Disconnect();
        }
    }
}
