namespace fsp.Models
{
    // TODO: if this app becomes production, we should replace eventType with enum, eventDate type as Date, and
    // eventJson with concrete types. AND use Json attributes to make field names style compatible with
    // the rest of the application
    public class CreateTransactionRequest
    {
        public string fspId { get; set; }
        public string tdcEndpoint { get; set; }
        public string eventType { get; set; }
        public string eventDate { get; set; }
        public string eventJson { get; set; }
        public string fspHash { get; set; }
    }
}