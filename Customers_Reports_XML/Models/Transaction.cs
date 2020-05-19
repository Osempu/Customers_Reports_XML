using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers_Reports_XML.Models
{
    public class Transaction
    {
        public string Issuing { get; set; }
        public Decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
