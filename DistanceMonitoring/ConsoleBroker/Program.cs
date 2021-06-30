using System;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace ConsoleBroker
{
    class Program
    {
        static void Main(string[] args)
        {
            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithConnectionValidator(c =>
                {
                    Console.WriteLine($"{c.ClientId} connection validator for endpoint: {c.Endpoint}");
                    c.ReasonCode = MqttConnectReasonCode.Success;
                })
                .WithConnectionBacklog(100)
                .WithDefaultEndpointPort(1884);

            var mqttServer = new MqttFactory().CreateMqttServer();
            mqttServer.StartAsync(optionsBuilder.Build()).Wait();

            Console.WriteLine($"Broker is Running: Host: {mqttServer.Options.DefaultEndpointOptions.BoundInterNetworkAddress} Port: {mqttServer.Options.DefaultEndpointOptions.Port}");
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();

            mqttServer.StopAsync().Wait();
        }
    }
}
