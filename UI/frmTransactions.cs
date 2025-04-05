using EZ_Bill.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EZ_Bill.UI
{
    public partial class frmTransactions : Form
    {
        public frmTransactions()
        {
            InitializeComponent();
        }
        TransactionsDAL tdal = new TransactionsDAL();

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void frmTransactions_Load(object sender, EventArgs e)
        {
            DataTable dt = tdal.DisplayAllTransactions();
            dgvTransactions.DataSource = dt;

        }

        private void cmbTransactionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //get value from combobox
            string type = cmbTransactionType.Text;

            DataTable dt = tdal.DisplayTranscationByType(type);
            dgvTransactions.DataSource = dt;

        }

        private void btn_Click(object sender, EventArgs e)
        {
            DataTable dt = tdal.DisplayAllTransactions();
            dgvTransactions.DataSource = dt;
        }
    }
}
