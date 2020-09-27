using System;
using System.Text;

namespace HealthChecks.Extensions.AzureSearch.Models
{
    public class AzureSearchStatistics
    {
        public long DocumentsNumber { get; set; }
        public long UsageBytes { get; set; }
        public long? QuotaBytes { get; set; }

        public decimal? UsagePercentage
        {
            get
            {
                if (QuotaBytes == null || QuotaBytes <= 0)
                {
                    return null;
                }

                var percentage = (decimal) (UsageBytes * 100) / QuotaBytes.Value;
                return Math.Round(percentage, 2);
            }
        }

        public decimal? AverageDocumentBytes
        {
            get
            {
                if (DocumentsNumber <= 0)
                {
                    return null;
                }

                var averageBytes = (decimal) UsageBytes / DocumentsNumber;
                return Math.Round(averageBytes, 2);
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"{nameof(UsagePercentage)} = {UsagePercentage} % ");
            builder.Append($"{nameof(DocumentsNumber)} = {DocumentsNumber} document(s) ");
            builder.Append($"{nameof(AverageDocumentBytes)} = {AverageDocumentBytes} bytes per document ");
            builder.Append($"{nameof(UsageBytes)} = {UsageBytes} bytes ");
            builder.Append($"{nameof(QuotaBytes)} = {QuotaBytes} bytes");
            return builder.ToString();
        }
    }
}