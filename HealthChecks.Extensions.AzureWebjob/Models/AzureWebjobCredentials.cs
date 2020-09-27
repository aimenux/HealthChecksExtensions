namespace HealthChecks.Extensions.AzureWebjob.Models
{
    public class AzureWebjobCredentials
    {
        public string Url { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public AzureWebjobType? Type { get; set; }
    }
}