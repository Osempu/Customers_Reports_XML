using Customers_Reports_XML.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Customers_Reports_XML.Controllers
{
    class CustomerController
    {
        private readonly DataTable customersTable;

        public CustomerController(DataTable customersTable)
        {
            this.customersTable = customersTable;
        }

        public void Create(string name, string lastName, string email, int age, string address, 
            DateTime birthday, Decimal balance, XmlDocument xDoc)
        {
            customersTable.Rows.Add(name, lastName, email, age, address, birthday, balance);

            WriteCustomerToXML(xDoc);
        }

        public void WriteCustomerToXML(XmlDocument xDoc)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            xDoc.Load(appDataPath + @"\BankingApp\customers.xml");
            XmlNode xNode = xDoc.SelectSingleNode("Customers");
            xNode.RemoveAll();

            foreach (DataRow customer in customersTable.Rows)
            {
                XmlNode xTop = xDoc.CreateElement("Customer");
                XmlNode xName = xDoc.CreateElement("Name");
                XmlNode xLastName = xDoc.CreateElement("LastName");
                XmlNode xEmail = xDoc.CreateElement("Email");
                XmlNode xAge = xDoc.CreateElement("Age");
                XmlNode xAddress = xDoc.CreateElement("Address");
                XmlNode xBirthday = xDoc.CreateElement("Birthday");
                XmlNode xBalance = xDoc.CreateElement("Balance");

                xName.InnerText = customer["Name"].ToString();
                xLastName.InnerText = customer["LastName"].ToString();
                xEmail.InnerText = customer["Email"].ToString();
                xAge.InnerText = customer["Age"].ToString();
                xAddress.InnerText = customer["Address"].ToString();
                xBirthday.InnerText = DateTime.Parse(customer["Birthday"].ToString()).ToFileTime().ToString();
                xBalance.InnerText = customer["Balance"].ToString();

                xTop.AppendChild(xName);
                xTop.AppendChild(xLastName);
                xTop.AppendChild(xEmail);
                xTop.AppendChild(xAge);
                xTop.AppendChild(xAddress);
                xTop.AppendChild(xBirthday);
                xTop.AppendChild(xBalance);

                xDoc.DocumentElement.AppendChild(xTop);
            }

            xDoc.Save(appDataPath + @"\BankingApp\customers.xml");
        }

        public void FillCustomerTableFromXml(XmlDocument xDoc)
        {
            foreach (XmlNode xNode in xDoc.SelectNodes("Customers/Customer"))
            {
                customersTable.Rows.Add(xNode.SelectSingleNode("Name").InnerText,
                    xNode.SelectSingleNode("LastName").InnerText,
                    xNode.SelectSingleNode("Email").InnerText,
                    Convert.ToInt32(xNode.SelectSingleNode("Age").InnerText),
                    xNode.SelectSingleNode("Address").InnerText,
                    DateTime.FromFileTime(Convert.ToInt64(xNode.SelectSingleNode("Birthday").InnerText)),
                    Convert.ToDecimal(xNode.SelectSingleNode("Balance").InnerText));
            }
        }

    }
}
