using System;
using System.Text;
using DistanceMonitoring.Configuration;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace DistanceMonitoring.Controller.Adapters
{
    public class MqttAdapter: IMqttAdapter, IDisposable
    {
        public event IMqttAdapter.MessageReceivedEventHandler MessageReceived;
        private IMqttClient _client;
        private IMqttClientOptions _options;
        private bool disposedValue1;
        private readonly MqttConfiguration config;
        public MqttAdapter(IConfigProvider configProvider)
        {
            config = configProvider.GetMqttConfiguration();
            StartSubscriber();
        }
        private void StartSubscriber()
        {
            try
            {
                Console.WriteLine("Starting Subscriber....");
                //create subscriber client
                var factory = new MqttFactory();
                _client = factory.CreateMqttClient();

                //configure options
                _options = new MqttClientOptionsBuilder()
                    .WithClientId(config.ClientId)
                    .WithTcpServer(config.Host, config.Port)
                    .WithCleanSession()
                    .Build();

                //Handlers
                _client.UseConnectedHandler(e =>
                {
                    Console.WriteLine("Connected successfully with MQTT Brokers.");
                    //Subscribe to topic
                    _client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(config.TopicName).Build()).Wait();
                });
                _client.UseDisconnectedHandler(e =>
                {
                    Console.WriteLine("Disconnected from MQTT Brokers.");
                });
                _client.UseApplicationMessageReceivedHandler(e =>
                {
                    Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                    Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                    Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                    Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                    Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                    Console.WriteLine();

                    if (MessageReceived != null)
                    {
                        MessageReceived.Invoke(this,
                            new MqttMessageEventArgs { Payload = e.ApplicationMessage.Payload });
                    }
                });

                //actually connect
                _client.ConnectAsync(_options).Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue1)
            {
                if (disposing)
                {
                    _client.DisconnectAsync().Wait();
                }
                disposedValue1 = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
