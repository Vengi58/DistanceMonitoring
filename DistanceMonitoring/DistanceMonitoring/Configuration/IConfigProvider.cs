namespace DistanceMonitoring.Configuration
{
    public interface IConfigProvider
    {
        public AWSIoTConfiguration GetAWSIoTConfiguration();
        public MqttConfiguration GetMqttConfiguration();
    }
    public class ConfigProvider : IConfigProvider
    {
        private readonly AppConfig config;
        public ConfigProvider(AppConfig _config)
        {
            config = _config;
        }
        public AWSIoTConfiguration GetAWSIoTConfiguration()
        {
            return config.AWSIoT;
        }

        public MqttConfiguration GetMqttConfiguration()
        {
            return config.MQTT;
        }
    }
}
