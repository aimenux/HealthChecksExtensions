using System;

namespace HealthChecks.Extensions.AzureWebjob.Models
{
    public class AzureWebjobSettings
    {
        public string Name { get; }
        public string Type { get; }
        public string Url { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string RelativeWebjobUrl => $"/api/{Type}jobs/{Name}".ToLower();

        public AzureWebjobSettings(string name, string type)
        {
            Name = name;
            Type = type;

            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new ArgumentException($"Invalid {nameof(Name)}");
            }

            if (!Enum.TryParse<AzureWebjobType>(Type, true, out _))
            {
                throw new ArgumentException($"Invalid {nameof(Type)}");
            }
        }
    }
}