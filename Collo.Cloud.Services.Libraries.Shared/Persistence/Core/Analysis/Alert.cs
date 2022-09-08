using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Analysis
{
    public class Alert : ContainerEntity
    {
        public string Schema { get; set; } = "http://json-schema.org/draft-04/schema#";
        public string AlertType { get; set; }
        public string InstruemntId { get; set; }
        protected override string GetPartitionKeyValue() => InstruemntId;
        public string Feature { get; set; }
        public DateTime MeasuredAtUtc { get; set; }
        public string Rule { get; set; }
        public float Expected { get; set; }
        public float Threshold { get; set; }
        public float Reported { get; set; }
    }
}
