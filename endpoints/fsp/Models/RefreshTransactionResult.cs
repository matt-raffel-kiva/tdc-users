using System;

namespace fsp.Models
{
    public class RefreshTransactionResult
    {
        public int id { get; set; }
        public string fsp_id { get; set; }
        public string transaction_id { get; set; }
        public DateTime transaction_date { get; set; }
        public string hash { get; set; }
    }
}