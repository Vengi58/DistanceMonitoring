using System;
using System.Text;
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
        private string topicName = "distanceData";
        private bool disposedValue1;

        public MqttAdapter(string topic)
        {
            topicName = topic;
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
                    .WithClientId("SubscriberId")
                    .WithTcpServer("localhost", 1884)
                    .WithCleanSession()
                    .Build();

                //Handlers
                _client.UseConnectedHandler(e =>
                {
                    Console.WriteLine("Connected successfully with MQTT Brokers.");

                    //Subscribe to topic
                    _client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topicName).Build()).Wait();
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

        private void StartPublisher()
        {
            Console.WriteLine("Starting Publisher....");
            try
            {
                // Create a new MQTT client.
                var factory = new MqttFactory();
                _client = factory.CreateMqttClient();

                //configure options
                _options = new MqttClientOptionsBuilder()
                    .WithClientId("PublisherId")
                    .WithTcpServer("localhost", 1884)
                    //.WithCredentials("bud", "%spencer%")
                    .WithCleanSession()
                    .Build();

                //handlers
                _client.UseConnectedHandler(e =>
                {
                    Console.WriteLine("Connected successfully with MQTT Brokers.");
                });
                _client.UseDisconnectedHandler(e =>
                {
                    Console.WriteLine("Disconnected from MQTT Brokers.");
                });
                _client.UseApplicationMessageReceivedHandler(e =>
                {
                    try
                    {
                        string topic = e.ApplicationMessage.Topic;
                        if (string.IsNullOrWhiteSpace(topic) == false)
                        {
                            string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                            Console.WriteLine($"Topic: {topic}. Message Received: {payload}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message, ex);
                    }
                });


                //connect
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

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue1 = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~MqttAdapter()
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
