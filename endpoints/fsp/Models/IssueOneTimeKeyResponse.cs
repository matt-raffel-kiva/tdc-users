namespace fsp.Models
{
    public class IssueOneTimeKeyResponse
    {
        public string tdcTroId { get; set; }
        public string tdcFspId { get; set; }
        // TODO: we could make this enum
        public string state { get; set; }
    }
}