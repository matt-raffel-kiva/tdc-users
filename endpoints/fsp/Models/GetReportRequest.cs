namespace fsp.Models
{

    // TODO: if this app becomes production, use Json attributes to make field names style compatible with
    // the rest of the application
    public class GetReportRequest
    {
        public string tdcTroId { get; set; }
        public string tdcFspId { get; set; }
        public string tdcEndpoint { get; set; }
    }
}