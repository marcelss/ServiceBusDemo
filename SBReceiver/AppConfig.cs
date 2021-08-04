namespace SBReceiver
{
    public class AppConfig
    {
        public ConnectionStrings ConnectionStrings { get; set; }
    }

    public class ConnectionStrings
    {
        public string AzureServiceBus { get; set; }
    }
}
