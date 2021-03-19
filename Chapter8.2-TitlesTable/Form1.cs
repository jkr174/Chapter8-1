using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chapter8._2_TitlesTable
{
    public partial class frmTitles : Form
    {
        SqlConnection booksConnection; 
        SqlCommand titlesCommand; 
        SqlDataAdapter titlesAdapter; 
        DataTable titlesTable;

        bool selectedFile = true;
        public frmTitles()
        {
            InitializeComponent();
        }

        private void frmTitles_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (selectedFile != false)
            {
                booksConnection.Dispose();
                titlesCommand.Dispose();
                titlesAdapter.Dispose();
                titlesTable.Dispose();
            }
        }

        private void frmTitles_Load(object sender, EventArgs e)
        {
            if (dlgOpen.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // connect to books database 
                    booksConnection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;" +
                            "AttachDbFilename=" + dlgOpen.FileName + "; " +
                            "Integrated Security=True;" +
                            "Connect Timeout=30;" +
                            "User Instance=False");

                    booksConnection.Open(); 
                    // establish command object 
                    titlesCommand = new SqlCommand(
                        "Select * " +
                        "from Titles " +
                        "ORDER BY Title", booksConnection); 
                    // establish data adapter/data table 
                    titlesAdapter = new SqlDataAdapter(); 
                    titlesAdapter.SelectCommand = titlesCommand; 
                    titlesTable = new DataTable(); 
                    titlesAdapter.Fill(titlesTable); 
                    // bind grid to data table 
                    grdTitles.DataSource = titlesTable; 
                } 
                catch (Exception ex) 
                {
                    MessageBox.Show(
                        ex.Message, 
                        "Error establishing database file connection.", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Error); 
                    return; 
                } 
            } 
            else 
            {
                selectedFile = false;
                MessageBox.Show(
                    "No file selected", 
                    "Program stopping",
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information); 
                this.Close(); 
            }
        }
    }
}
