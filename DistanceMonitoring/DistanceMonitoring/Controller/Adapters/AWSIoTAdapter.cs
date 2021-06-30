using System;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using DistanceMonitoring.Configuration;

namespace DistanceMonitoring.Controller.Adapters
{
    public class AWSIoTAdapter : IMqttAdapter
    {
        public event IMqttAdapter.MessageReceivedEventHandler MessageReceived;
        private MqttClient client;
        private readonly AWSIoTConfiguration config;
        public AWSIoTAdapter(IConfigProvider configProvider)
        {
            config = configProvider.GetAWSIoTConfiguration();
            StartSubscriber();
        }

        private void StartSubscriber()
        {
            Console.WriteLine("AWS IoT Dotnet message publisher starting..");
            var caCert = X509Certificate.CreateFromCertFile(config.CAcertPath);
            var clientCert = new X509Certificate2(config.ThingCertPath, config.ThingCertPassword);

            client = new MqttClient(config.EndpointName, config.BrokerPort, true, caCert, clientCert, MqttSslProtocols.TLSv1_2);
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            client.MqttMsgSubscribed += Client_MqttMsgSubscribed;
            client.Connect(config.ClientId);
            Console.WriteLine($"Connected to AWS IoT with client id: {config.ClientId}.");

            client.Subscribe(new string[] { config.TopicName }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
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
