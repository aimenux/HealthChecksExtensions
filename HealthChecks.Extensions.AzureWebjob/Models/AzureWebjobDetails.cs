using Newtonsoft.Json;

namespace HealthChecks.Extensions.AzureWebjob.Models
{
    public class AzureWebjobDetails
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("history_url")]
        public string HistoryUrl { get; set; }

        [JsonProperty("scheduler_logs_url")]
        public string SchedulerLogsUrl { get; set; }

        [JsonProperty("run_command")]
        public string RunCommand { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("extra_info_url")]
        public string ExtraInfoUrl { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("using_sdk")]
        public bool UsingSdk { get; set; }

        [JsonProperty("detailed_status")]
        public string DetailedStatus { get; set; }

        [JsonProperty("log_url")]
        public string LogUrl { get; set; }

        [JsonProperty("latest_run")]
        public AzureWebjobLatestRun LatestRun { get; set; }
    }
}
