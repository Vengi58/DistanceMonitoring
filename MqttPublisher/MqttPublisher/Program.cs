using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace MqttPublisher
{
    class Program
    {
        private static IMqttClient _client;
        private static IMqttClientOptions _options;
        private static string topicName = "distanceData";

        static void Main(string[] args)
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

                SimulatePublish();

                Console.WriteLine("Simulation ended! press any key to exit.");
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        static void SimulatePublish()
        {
            Random rnd = new Random();
            while (true)
            {
                var distanceData = new DistanceData { Value = rnd.NextDouble() * 200.0 };
                var payload = JsonSerializer.Serialize(distanceData);
                var testMessage = new MqttApplicationMessageBuilder()
                           .WithTopic(topicName)
                           .WithPayload(payload)
                           .WithExactlyOnceQoS()
                           .WithRetainFlag()
                           .Build();
                if (_client.IsConnected)
                {
                    Console.WriteLine($"publishing at {DateTime.UtcNow}");
                    _client.PublishAsync(testMessage);
                }
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
        }
        public class DistanceData
        {
            public double Value { get; set; }
        }
    }
}
