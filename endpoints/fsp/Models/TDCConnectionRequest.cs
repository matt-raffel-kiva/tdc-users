using Flurl;
using Flurl.Http;

namespace fsp.Models
{
    public class TDCConnectionRequest
    {
        public string TdcPrefix { get; set; }
        public string TdcUrl { get; set; }
    }
}