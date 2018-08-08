using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayExCheckout2.PayExClient.Models
{
    public class TransactionResponseModel
    {
        public string payment { get; set; }
        
        public Transaction_Operation capture { get; set; }

        public Transaction_Operation cancellation { get; set; }

        public Transaction_Operation authorization { get; set; }

        public Transaction_Operation reversal { get; set; }

        public class Transaction_Operation
        {
            public string Id { get; set; }

            public TransactionInfoModel transaction { get; set; }
        }

        public class TransactionInfoModel
        {
            public string id { get; set; }
            public DateTime created { get; set; }
            public DateTime updated { get; set; }
            public string type { get; set; }
            public string state { get; set; }
            public long number { get; set; }
            public int amount { get; set; }
            public int vatAmount { get; set; }
            public string description { get; set; }
            public string payeeReference { get; set; }
            public bool isOperational { get; set; }
            public int reconciliationNumber { get; set; }
        }
    }
}
