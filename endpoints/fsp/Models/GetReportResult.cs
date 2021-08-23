using System;

namespace fsp.Models
{
    public class GetReportResult
    {
        public int id { get; set; }
        public string tdcFspId { get; set; }
        public string reportId { get; set; }
        public DateTime requestDate { get; set; }
        public string content { get; set; }
        public string state { get; set; }
    }
}