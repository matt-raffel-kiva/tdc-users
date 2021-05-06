using System;

namespace fsp.Models
{
    public class GetReportResult
    {
        public int id { get; set; }
        public string fsp_id { get; set; }
        public string report_id { get; set; }
        public DateTime request_date { get; set; }
        public string content { get; set; }
    }
}