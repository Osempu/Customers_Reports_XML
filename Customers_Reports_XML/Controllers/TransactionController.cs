using System;
using System.Data;
using System.Xml;
using Customers_Reports_XML.Models;

namespace transactions_Reports_XML.Controllers
{
    public class TransactionController
    {
        private readonly DataTable transactionsTable;

        public TransactionController(DataTable transactionsTable)
        {
            this.transactionsTable = transactionsTable;
        }

        public void Create(string transactionName, Decimal amount, TransactionType transactionType, DateTime transactionDate, XmlDocument xDoc)
        {
            transactionsTable.Rows.Add(transactionName, amount, transactionType, transactionDate);
            WriteTransactionToXml(xDoc);
        }

        private void WriteTransactionToXml(XmlDocument xDoc)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string transactionsDirectory = appDataPath + @"\BankingApp\customersTransactions\";
            xDoc.Load(transactionsDirectory + @"transactions.xml");
            XmlNode xNode = xDoc.SelectSingleNode("Transactions");
            xNode.RemoveAll();

            foreach (DataRow transaction in transactionsTable.Rows)
            {
                XmlNode xTop = xDoc.CreateElement("Transaction");
                XmlNode xIssuing = xDoc.CreateElement("Issuing");
                XmlNode xAmount = xDoc.CreateElement("Amount");
                XmlNode xTransactionType = xDoc.CreateElement("TransactionType");
                XmlNode xDate = xDoc.CreateElement("Date");

                xIssuing.InnerText = transaction["Issuing"].ToString();
                xAmount.InnerText = transaction["Amount"].ToString();
                xTransactionType.InnerText = transaction["TransactionType"].ToString();
                xDate.InnerText = DateTime.Parse(transaction["Date"].ToString()).ToFileTime().ToString();

                xTop.AppendChild(xIssuing);
                xTop.AppendChild(xAmount);
                xTop.AppendChild(xTransactionType);
                xTop.AppendChild(xDate);

                xDoc.DocumentElement.AppendChild(xTop);
            }

            xDoc.Save(transactionsDirectory + @"transactions.xml");
        }

        public void FillTransactionsTableFromXml(XmlDocument xDoc)
        {
            foreach (XmlNode xNode in xDoc.SelectNodes("Transactions/Transaction"))
            {
                transactionsTable.Rows.Add(
                    xNode.SelectSingleNode("Issuing").InnerText,
                    Convert.ToDecimal(xNode.SelectSingleNode("Amount").InnerText),
                    xNode.SelectSingleNode("TransactionType").InnerText,
                    DateTime.FromFileTime(Convert.ToInt64(xNode.SelectSingleNode("Date").InnerText)));
            }
        }
    }
}
