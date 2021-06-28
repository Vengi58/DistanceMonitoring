using DistanceMonitoring;
using DistanceMonitoring.Controller;
using DistanceMonitoring.Controller.Adapters;
using DistanceMonitoring.Model;
using DistanceMonitoring.View;
using Moq;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace DistanceMonitorTests
{
    public class Tests
    {
        [Test]
        [TestCase(5.0, "Dangerously close!")]
        [TestCase(12.2, "In safe zone.")]
        [TestCase(111.3, "Out of sight!")]
        public void Test1(double distance, string output)
        {
            using StringWriter sw = new();
            Console.SetOut(sw);
            var dc = new Mock<IDistanceDataController>();
            DistanceMonitor dm = new(dc.Object);
            dm.StartMonitoring();
            dc.Raise(x => x.DistanceDataReceived += null, EventArgs.Empty,
                new DistanceDataEventArgs(new DistanceData { Value = distance }));

            Assert.True(sw.ToString().Contains(output));
        }

        [Test]
        [TestCase(5.0, "Dangerously close!")]
        [TestCase(12.2, "In safe zone.")]
        [TestCase(111.3, "Out of sight!")]
        public void Test2(double distance, string output)
        {
            using StringWriter sw = new();
            Console.SetOut(sw);
            var mqttAdapter = new Mock<IMqttAdapter>();
            var dc = new DistanceDataController(mqttAdapter.Object);
            var dm = new DistanceMonitor(dc);
            dm.StartMonitoring();
            mqttAdapter.Raise(x => x.MessageReceived += null, EventArgs.Empty,
                new MqttMessageEventArgs { Payload = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(new DistanceData { Value = distance }))});

            Assert.True(sw.ToString().Contains(output));
        }
    }
}