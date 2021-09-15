using System;

namespace fsp.Models
{
    public class GetReportResult
    {
        public int id { get; set; }
        public string tdcFspId { get; set; }
        public string reportId { get; set; }
        public string requestDate { get; set; } // there is a serialization error when this is DateTime.  Fix prob on the service however.
        public string content { get; set; }
        public string state { get; set; }
    }
}