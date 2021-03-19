﻿/* Name:    Jovany Romo
 * Date:    1/14/2021
 * Summray: Connects to a database that also has error handling.
 */

using System;
using System.IO;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chapter5_2_AuthorsTableInputForm
{
    public partial class frmPublishers : Form
    {
        SqlConnection booksConnection;
        SqlCommand publishersCommand;
        SqlDataAdapter publishersAdapter;
        DataTable publishersTable;
        CurrencyManager publishersManager;

        OpenFileDialog dlgOpen = new OpenFileDialog();

        string myState;
        int myBookmark;
        public frmPublishers()
        {
            InitializeComponent();
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmAuthors_Load(object sender, EventArgs e)
        {
            
            try
            {
                hlpPublishers.HelpNamespace = Application.StartupPath + "\\Publishers.chm";

                if (dlgOpen.ShowDialog() == DialogResult.OK) 
                {
                    try
                    {
                        booksConnection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;" +
                            "AttachDbFilename=" + dlgOpen.FileName + "; " +
                            "Integrated Security=True;" +
                            "Connect Timeout=30;" +
                            "User Instance=False");


                        booksConnection.Open();

                        publishersCommand = new SqlCommand(
                            "SELECT * " +
                            "FROM Publishers " +
                            "ORDER BY Name", booksConnection);

                        publishersAdapter = new SqlDataAdapter();
                        publishersAdapter.SelectCommand = publishersCommand;
                        publishersTable = new DataTable();
                        publishersAdapter.Fill(publishersTable);

                        txtPubID.DataBindings.Add("Text", publishersTable, "PubID");
                        txtPubName.DataBindings.Add("Text", publishersTable, "Name");
                        txtCompanyName.DataBindings.Add("Text", publishersTable, "Company_Name");
                        txtPubAddress.DataBindings.Add("Text", publishersTable, "Address");
                        txtPubCity.DataBindings.Add("Text", publishersTable, "City");
                        txtPubState.DataBindings.Add("Text", publishersTable, "State");
                        txtPubZip.DataBindings.Add("Text", publishersTable, "Zip");
                        txtPubTelephone.DataBindings.Add("Text", publishersTable, "Telephone");
                        txtPubFAX.DataBindings.Add("Text", publishersTable, "FAX");
                        txtPubComments.DataBindings.Add("Text", publishersTable, "Comments");

                        publishersManager = (CurrencyManager)
                            this.BindingContext[publishersTable];
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(
                            ex.Message, 
                            "Error establishing Titles table.", 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Error); 
                        return;
                    }
                }

                else
                {
                    MessageBox.Show(
                        "No file selected", 
                        "Program stopping", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Information); 
                    this.Close();
                } 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Error establishing Publishers table.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }


            this.Show();
            SetState("View");
            SetText();
        }


        private void frmAuthors_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (myState.Equals("Edit") || myState.Equals("Add"))
            {
                MessageBox.Show("You must finish the current edit before stopping the application,",
                    "",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                e.Cancel = true;
            }
            else
            {
                try
                {
                    SqlCommandBuilder publishersAdapterCommands = new SqlCommandBuilder(publishersAdapter);
                    publishersAdapter.Update(publishersTable);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving database to file: \r\n"
                        + ex.Message,
                        "Save Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                booksConnection.Close();

                booksConnection.Dispose();
                publishersCommand.Dispose();
                publishersAdapter.Dispose();
                publishersTable.Dispose();
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (publishersManager.Position == 0)
            {
                Console.Beep();
            }
            publishersManager.Position--;
            SetText();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (publishersManager.Position == publishersManager.Count - 1)
            {
                Console.Beep();
            }
            publishersManager.Position++;
            SetText();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            
            if (!ValidateData())
            {
                return;
            }
            string savedName = txtPubName.Text;
            int savedRow;
            try
            {
                publishersManager.EndCurrentEdit();
                publishersTable.DefaultView.Sort = "Name";
                savedRow = publishersTable.DefaultView.Find(savedName);
                publishersManager.Position = savedRow;
                string Message = "Record saved.",
                Title = "Save";

            MessageBox.Show(Message,
                Title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
                SetState("View");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            SetText();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            
            string Message = "Are you sure you want to delete this record?",
                Title = "Delete";

            DialogResult response;
            response = MessageBox.Show(Message,
                Title,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (response == DialogResult.No)
            {
                return;
            }
            try
            {
                publishersManager.RemoveAt(publishersManager.Position);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            SetText();
        }
        private void SetState(string appState)
        {
            myState = appState;
            switch (appState)
            {
                case "View":
                    txtPubID.BackColor = Color.White;
                    txtPubID.ForeColor = Color.Black;
                    txtPubName.ReadOnly = true;
                    txtCompanyName.ReadOnly = true;
                    txtPubAddress.ReadOnly = true;
                    txtPubCity.ReadOnly = true;
                    txtPubState.ReadOnly = true;
                    txtPubZip.ReadOnly = true;
                    txtPubTelephone.ReadOnly = true;
                    txtPubFAX.ReadOnly = true;
                    txtPubComments.ReadOnly = true;
                    btnPrevious.Enabled = true;
                    btnNext.Enabled = true;
                    btnAddNew.Enabled = true;
                    btnSave.Enabled = false;
                    btnCancel.Enabled = false;
                    btnEdit.Enabled = true;
                    btnDelete.Enabled = true;
                    btnDone.Enabled = true;
                    txtPubName.Focus();
                    break;
                //Add or Edit State
                default:
                    txtPubID.BackColor = Color.Red;
                    txtPubID.ForeColor = Color.White;
                    txtPubName.ReadOnly = false;
                    txtCompanyName.ReadOnly = false;
                    txtPubAddress.ReadOnly = false;
                    txtPubCity.ReadOnly = false;
                    txtPubState.ReadOnly = false;
                    txtPubZip.ReadOnly = false;
                    txtPubTelephone.ReadOnly = false;
                    txtPubFAX.ReadOnly = false;
                    txtPubComments.ReadOnly = false;
                    btnPrevious.Enabled = false;
                    btnNext.Enabled = false;
                    btnAddNew.Enabled = false;
                    btnSave.Enabled = true;
                    btnCancel.Enabled = true;
                    btnEdit.Enabled = false;
                    btnDelete.Enabled = false;
                    btnDone.Enabled = false;
                    txtPubName.Focus();
                    break;
            }
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            
            try
            {
                myBookmark = publishersManager.Position;
                publishersManager.AddNew();
                SetState("Add");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            SetText();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            SetState("Edit");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            
            publishersManager.CancelCurrentEdit();
            if (myState.Equals("Add"))
                publishersManager.Position = myBookmark;
            SetState("View");
            SetText();
        }

        private void txtYearBorn_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0' && e.KeyChar <= '9' || (int)e.KeyChar == 8))
            {
                e.Handled = false;
            }
            else if((int)e.KeyChar == 13)
            {
                txtPubName.Focus();
            }
            else
            {
                e.Handled = true;
                Console.Beep();
            }
        }
        private bool ValidateData()
        {
            string message = "";
            bool allOK = true;

            if (txtPubName.Text.Trim().Equals(""))
            {
                message = "You must enter a Publisher Name." + "\r\n";
                txtPubName.Focus();
                allOK = false;
            }
            if (!allOK)
            {
                MessageBox.Show(
                    message, 
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            return (allOK);
        }

        private void txtAuthorName_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void frmAuthors_HelpButtonClicked(object sender, CancelEventArgs e)
        {

        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, hlpPublishers.HelpNamespace);
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            publishersManager.Position = 0;
            SetText();
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            publishersManager.Position = publishersManager.Count - 1;
            SetText();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if (txtFind.Text.Equals(""))
            {
                return;
            }
            int savedRow = publishersManager.Position;
            DataRow[] foundRows;
            publishersTable.DefaultView.Sort = "Name";
            foundRows = publishersTable.Select("Name LIKE '" +
            txtFind.Text + "*'");
            if (foundRows.Length == 0)
            {
                publishersManager.Position = savedRow;
            }
            else
            {
                publishersManager.Position =
                publishersTable.DefaultView.Find(foundRows[0]["Name"]);
            }
        }
        private void SetText()
        {
            this.Text = "Publishers - Record " +
            (publishersManager.Position + 1).ToString() + " of " +
            publishersManager.Count.ToString() + " Records";
        }
    }
}
