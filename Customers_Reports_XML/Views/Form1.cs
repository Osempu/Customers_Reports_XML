using Customers_Reports_XML.Controllers;
using Customers_Reports_XML.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using transactions_Reports_XML.Controllers;

namespace Customers_Reports_XML
{
    public partial class MainView : Form
    {
        public MainView()
        {
            InitializeComponent();
        }

        static string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        XmlDocument xDoc = new XmlDocument();
        static string transactionsDirectory = appDataPath + @"\BankingApp\customersTransactions\";
        XmlDocument transactionsXmlDoc = new XmlDocument();

        static DataTable customersTable = new DataTable();
        CustomerController _customer = new CustomerController(customersTable);

        static DataTable transactionsTable = new DataTable();
        TransactionController _transaction = new TransactionController(transactionsTable);

        #region Customer Region

            #region Controls Functions
        private void MainView_Load(object sender, EventArgs e)
        {
            CheckForExistingAppDirectory();
            CheckForExistingFolderForTransactions();

            xDoc.Load(appDataPath + @"\BankingApp\customers.xml");
            transactionsXmlDoc.Load(transactionsDirectory + "transactions.xml");

            InitializeCustomersTale();
            InitializeTransactionsTable();

            _customer.FillCustomerTableFromXml(xDoc);
            _transaction.FillTransactionsTableFromXml(transactionsXmlDoc);

            FillCustomerNameComboBox(cbCustomerName);
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddCustomer();
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (SelectedRowIsNotNull())
            {
                UpdateRow();
            }
        }
        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (SelectedRowIsNotNull())
            {
                DeleteRow();
            }
        }
        private void dgvCustomers_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int selectedRow = dgvCustomers.Rows[e.RowIndex].Index;

                FillTextFieldsFromClickedRow(e);

                txtName.Focus();
            }
        }
        private void txtInitialBalance_KeyPress(object sender, KeyPressEventArgs e)
        {
            AllowOnlyNumbers(sender, e);
        }
        #endregion

            #region Functions
        private void AddCustomer()
        {
            if (DataFieldsFilled())
            {
                var age = Convert.ToInt32(DateTime.Today.Year - dateBirthday.Value.Year);

                _customer.Create(txtName.Text,
                    txtLastName.Text,
                    txtEmail.Text,
                    age,
                    txtAddress.Text,
                    dateBirthday.Value.Date,
                    Convert.ToDecimal(txtInitialBalance.Text),
                    xDoc);

                ClearFields();
                MessageBox.Show("Customer added!", "Customer added", MessageBoxButtons.OK, MessageBoxIcon.Information);

                FillCustomerNameComboBox(cbCustomerName);
            }
        }
        private void ClearFields()
        {
            txtName.Text = "";
            txtLastName.Text = "";
            txtAddress.Text = "";
            dateBirthday.Value = DateTime.Now;
            txtInitialBalance.Text = "";
            txtEmail.Text = "";
            txtTransactionAmount.Text = "";
        }
        private void CheckForExistingAppDirectory()
        {
            if (!Directory.Exists(appDataPath + @"\BankingApp\"))
                Directory.CreateDirectory(appDataPath + @"\BankingApp\");
            if (!File.Exists(appDataPath + @"\BankingApp\customers.xml"))
            {
                XmlTextWriter xWriter = new XmlTextWriter(appDataPath + @"\BankingApp\customers.xml",
                    Encoding.UTF8);
                xWriter.WriteStartElement("Customers");
                xWriter.WriteEndElement();
                xWriter.Close();
            }
        }
        private void InitializeCustomersTale()
        {
            customersTable.Columns.Add("Name", typeof(string));
            customersTable.Columns.Add("LastName", typeof(string));
            customersTable.Columns.Add("Email", typeof(string));
            customersTable.Columns.Add("Age", typeof(int));
            customersTable.Columns.Add("Address", typeof(string));
            customersTable.Columns.Add("Birthday", typeof(DateTime));
            customersTable.Columns.Add("Balance", typeof(Decimal));

            dgvCustomers.DataSource = customersTable;
        }
        private bool DataFieldsFilled()
        {
            var age = Convert.ToInt32(DateTime.Today.Year - dateBirthday.Value.Year);

            if (age < 18)
            {
                MessageBox.Show("Customer must be 18 years old", "Age restriction", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (txtName.Text != "" && txtLastName.Text != "" && txtAddress.Text != "" && txtInitialBalance.Text != "" && txtEmail.Text != "")
            {
                return true;
            }

            MessageBox.Show("Information missing. Please fill all the fields!", "Fill all the required fields", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
        private void DeleteRow()
        {
            var selectedRow = dgvCustomers.SelectedRows[0].Index;

            if (selectedRow >= 0)
            {
                dgvCustomers.Rows.RemoveAt(selectedRow);
                CheckForExistingAppDirectory();
                xDoc.Load(appDataPath + @"\BankingApp\customers.xml");
                _customer.WriteCustomerToXML(xDoc);
                ClearFields();
                MessageBox.Show("Customer removed succesfuly", "Customer removed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void UpdateRow()
        {
            if (DataFieldsFilled())
            {
                var selectedRow = dgvCustomers.SelectedRows[0].Index;

                customersTable.Rows[selectedRow]["Name"] = txtName.Text;
                customersTable.Rows[selectedRow]["LastName"] = txtLastName.Text;
                customersTable.Rows[selectedRow]["Address"] = txtAddress.Text;
                customersTable.Rows[selectedRow]["Birthday"] = dateBirthday.Value;
                customersTable.Rows[selectedRow]["Balance"] = txtInitialBalance.Text;
                customersTable.Rows[selectedRow]["Email"] = txtEmail.Text;

                CheckForExistingAppDirectory();
                xDoc.Load(appDataPath + @"\BankingApp\customers.xml");
                _customer.WriteCustomerToXML(xDoc);
                ClearFields();

                MessageBox.Show("Customer information updated!", "Information updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void FillTextFieldsFromClickedRow(DataGridViewCellMouseEventArgs e)
        {
            txtName.Text = dgvCustomers.Rows[e.RowIndex].Cells["Name"].Value.ToString();
            txtLastName.Text = dgvCustomers.Rows[e.RowIndex].Cells["LastName"].Value.ToString();
            txtEmail.Text = dgvCustomers.Rows[e.RowIndex].Cells["Email"].Value.ToString();
            txtAddress.Text = dgvCustomers.Rows[e.RowIndex].Cells["Address"].Value.ToString();
            dateBirthday.Value = DateTime.Parse(dgvCustomers.Rows[e.RowIndex].Cells["Birthday"].Value.ToString());
            txtInitialBalance.Text = dgvCustomers.Rows[e.RowIndex].Cells["Balance"].Value.ToString();
        }
        private bool SelectedRowIsNotNull()
        {
            if (dgvCustomers.CurrentRow != null)
            {
                var selectedRow = dgvCustomers.SelectedRows[0].Index;

                if (selectedRow >= 0)
                {
                    return true;
                }
            }

            MessageBox.Show("Select a row to continue", "No row selected", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
        #endregion

        #endregion

        #region Transaction Region
            #region Control Functions
        private void btnCompleteTransaction_Click(object sender, EventArgs e)
        {
            CompleteTransaction();
        }
        private void cbCustomerName_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplaySelectedCustomerTransactions();
        }
        private void cbTransactionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplaySelectedCustomerTransactions();
        }
        private void txtTransactionAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            AllowOnlyNumbers(sender, e);
        } 
        #endregion

            #region Functions
        private void CheckForExistingFolderForTransactions()
        {
            string transactionsDirectory = appDataPath + @"\BankingApp\customersTransactions\";

            if (!Directory.Exists(transactionsDirectory))
                Directory.CreateDirectory(transactionsDirectory);
            if (!File.Exists(transactionsDirectory + "transactions.xml"))
            {
                XmlTextWriter xmlWriter = new XmlTextWriter(transactionsDirectory + @"transactions.xml",
                    Encoding.UTF8);
                xmlWriter.WriteStartElement("Transactions");
                xmlWriter.WriteEndElement();
                xmlWriter.Close();
            }
        }
        private void InitializeTransactionsTable()
        {
            transactionsTable.Columns.Add("Issuing", typeof(string));
            transactionsTable.Columns.Add("Amount", typeof(Decimal));
            transactionsTable.Columns.Add("TransactionType", typeof(TransactionType));
            transactionsTable.Columns.Add("Date", typeof(DateTime));

            dgvCustomerTransactions.DataSource = transactionsTable;
        }
        private bool TransactionDataFieldsFilled()
        {
            if (cbCustomerName.Text == "" || cbTransactionType.Text == "" || txtTransactionAmount.Text == "")
            {
                MessageBox.Show("Please field all the fields to continue.", "Missing information required",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        private void CompleteTransaction()
        {
            if (TransactionDataFieldsFilled())
            {
                Enum.TryParse(cbTransactionType.Text, out TransactionType tType);

                int selectedCustomer = ((KeyValuePair<int, string>)cbCustomerName.SelectedItem).Key;
                Decimal currentBalance = Convert.ToDecimal(customersTable.Rows[selectedCustomer]["Balance"]);
                Decimal transactionAmount = Convert.ToDecimal(txtTransactionAmount.Text);

                if (tType == TransactionType.Withdraw)
                {
                    if ((currentBalance - transactionAmount) > 0)
                    {
                        customersTable.Rows[selectedCustomer]["Balance"] = (currentBalance - transactionAmount).ToString();

                        CheckForExistingAppDirectory();
                        xDoc.Load(appDataPath + @"\BankingApp\customers.xml");
                        _customer.WriteCustomerToXML(xDoc);

                        MakeTransaction(tType);
                    }
                    else
                    {
                        MessageBox.Show("Not enough funds to complete operation.", "Operation not possible", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    customersTable.Rows[selectedCustomer]["Balance"] = (currentBalance + transactionAmount).ToString();

                    CheckForExistingAppDirectory();
                    xDoc.Load(appDataPath + @"\BankingApp\customers.xml");
                    _customer.WriteCustomerToXML(xDoc);

                    MakeTransaction(tType);
                }
            }
        }
        private void MakeTransaction(TransactionType tType)
        {
            _transaction.Create(cbCustomerName.Text, Convert.ToDecimal(txtTransactionAmount.Text),
                                tType, DateTime.Today, transactionsXmlDoc);
            ClearFields();
            MessageBox.Show("Transaction completed.", "Transaction success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void FillCustomerNameComboBox(ComboBox cbCustomerNames)
        {
            cbTransactionType.SelectedIndex = 0;
            Dictionary<int, string> comboSource = new Dictionary<int, string>();

            var customers = customersTable.AsEnumerable().ToList();

            if (customers.Count > 0)
            {
                for (int i = 0; i < customers.Count; i++)
                {
                    comboSource.Add(i, customers[i][0].ToString());
                }

                cbCustomerName.DataSource = new BindingSource(comboSource, null);
                cbCustomerName.DisplayMember = "Value";
                cbCustomerName.ValueMember = "Key"; 
            }
            lblAlert.Visible = false;
        }
        private void DisplaySelectedCustomerTransactions()
        {
            string Issuing = cbCustomerName.Text.Split(' ')[0];
            int transactionType = cbTransactionType.SelectedIndex;

            var selectCustomerTransactions = transactionsTable.AsEnumerable().Where(x => x.Field<string>("Issuing") == Issuing && x.Field<int>("TransactionType") == transactionType);

            if (selectCustomerTransactions.Any())
            {
                dgvCustomerTransactions.DataSource = selectCustomerTransactions.CopyToDataTable();
                lblAlert.Visible = false;
            }
            else
            {
                dgvCustomerTransactions.DataSource = transactionsTable;
                lblAlert.Visible = true;
            }
        }
        private static void AllowOnlyNumbers(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                            !char.IsDigit(e.KeyChar) &&
                            (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        } 
        #endregion
        #endregion
    }
}
