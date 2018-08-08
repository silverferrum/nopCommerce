using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.PayExCheckout2.PayExClient.Models
{
    public class TransactionRequestModel
    {
        public TransactionCommon_Transaction Transaction { get; set; }

        public class TransactionCommon_Transaction
        {
            public string Description { get; set; }
            public int Amount { get; set; }
            public int VatAmount { get; set; }
            public string PayeeReference { get; set; }
            public List<TransactionCommon_ItemDescription> ItemDescriptions { get; set; }
            public List<TransactionCommon_VatSummary> VatSummary { get; set; }

            public class TransactionCommon_ItemDescription
            {
                public int Amount { get; set; }
                public string Description { get; set; }
            }

            public class TransactionCommon_VatSummary
            {
                public int Amount { get; set; }
                public int VatAmount { get; set; }
                public int VatPercent { get; set; }
            }
        }

        public static TransactionRequestModel CreateCaptureModel(int amount, int vatAmount, int vatPercent, string description, string payeeReference = null)
        {
            return new TransactionRequestModel {
                 Transaction = new TransactionCommon_Transaction
                 {
                     Amount = amount,
                     VatAmount = vatAmount,
                     Description = description,
                     PayeeReference = payeeReference ?? $"CAPTURE_{DateTime.UtcNow:yyyyMMdd_HHmmss}",
                     ItemDescriptions = new List<TransactionCommon_Transaction.TransactionCommon_ItemDescription>
                     {
                          new TransactionCommon_Transaction.TransactionCommon_ItemDescription { Amount = amount, Description = "Order" }                           
                     },
                     VatSummary = new List<TransactionCommon_Transaction.TransactionCommon_VatSummary>
                     {
                         new TransactionCommon_Transaction.TransactionCommon_VatSummary { Amount = amount, VatAmount = vatAmount, VatPercent = vatPercent }
                     }
                 }
            };
        }

        public static TransactionRequestModel CreateCancelModel(string description, string payeeReference = null)
        {
            return new TransactionRequestModel
            {
                Transaction = new TransactionCommon_Transaction
                {
                    Description = description,
                    PayeeReference = payeeReference ?? $"CANCEL_{DateTime.UtcNow:yyyyMMdd_HHmmss}",
                }
            };
        }
    }
}
