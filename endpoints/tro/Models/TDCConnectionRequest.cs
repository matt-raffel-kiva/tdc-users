using Flurl;
using Flurl.Http;

namespace tro.Models
{
    /**
     * please note the field names match
     * https://github.com/kiva/protocol-aries/tree/master/implementations/fsp/src/transactions/dtos
     */
    public class TDCConnectionRequest
    {
        public string alias { get; set; }
        public string identityProfileId { get; set; } = "citizen.identity";
        public CitizenConnectionData invitation { get; set; }
    }
}