using EZ_Bill.BLL;
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
    public partial class frmDeaCust : Form
    {
        public frmDeaCust()
        {
            InitializeComponent();
        }

        private void lblTop_Click(object sender, EventArgs e)
        {

        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        DeaCustBLL dc = new DeaCustBLL();
        DeaCustDAL dcDal = new DeaCustDAL();
        userDAL uDal = new userDAL();

        private void btnAdd_Click(object sender, EventArgs e)
        {
            dc.type = cmbDeaCustType.Text;
            dc.name = txtName.Text;
            dc.email = txtEmail.Text;
            dc.contact = txtContact.Text;
            dc.address = txtAddress.Text;
            dc.added_date = DateTime.Now;

            string loggedUsr = frmLogin.loggedIn;
            userBLL usr = uDal.GetIDFromUsername(loggedUsr);
            dc.added_by = usr.id;

            bool success = dcDal.Insert(dc);
            if(success == true)
            {
                if (cmbDeaCustType.Text == "Customer")
                {
                    MessageBox.Show("Customer Added successfully");
                    Clear();

                    //refresh data grid view
                    DataTable dt = dcDal.Select();
                    dgvDeaCust.DataSource = dt;
                }

                if (cmbDeaCustType.Text == "Dealer")
                {
                    MessageBox.Show("Dealer Added successfully");
                    Clear();

                    //refresh data grid view
                    DataTable dt = dcDal.Select();
                    dgvDeaCust.DataSource = dt;
                }
            }
            else
            {
                MessageBox.Show("Failed to add Dealer or Customer");
            }
        }

        public void Clear()
        {
            txtDeaCustID.Text = "";
            cmbDeaCustType.Text = "";
            txtName.Text = "";
            txtEmail.Text = "";
            txtContact.Text = "";
            txtAddress.Text = "";

        }

        private void frmDeaCust_Load(object sender, EventArgs e)
        {
            DataTable dt = dcDal.Select();
            dgvDeaCust.DataSource = dt;


        }

        private void dgvDeaCust_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int rowIndex = e.RowIndex;

            txtDeaCustID.Text = dgvDeaCust.Rows[rowIndex].Cells[0].Value.ToString();
            cmbDeaCustType.Text = dgvDeaCust.Rows[rowIndex].Cells[1].Value.ToString();
            txtName.Text = dgvDeaCust.Rows[rowIndex].Cells[2].Value.ToString();
            txtEmail.Text = dgvDeaCust.Rows[rowIndex].Cells[3].Value.ToString();
            txtContact.Text = dgvDeaCust.Rows[rowIndex].Cells[4].Value.ToString();
            txtAddress.Text = dgvDeaCust.Rows[rowIndex].Cells[5].Value.ToString();


        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            dc.id = int.Parse(txtDeaCustID.Text);
            dc.type = cmbDeaCustType.Text;
            dc.name = txtName.Text;
            dc.email = txtEmail.Text;
            dc.contact = txtContact.Text;
            dc.address = txtAddress.Text;
            dc.added_date = DateTime.Now;

            string loggedUsr = frmLogin.loggedIn;
            userBLL usr = uDal.GetIDFromUsername(loggedUsr);
            dc.added_by = usr.id;

            bool success = dcDal.Update(dc);
            if(success == true)
            {
                if(cmbDeaCustType.Text == "Customer") 
                { 
                    MessageBox.Show("Customer Updated Successfully");
                    Clear();

                    //refresh data grid view
                    DataTable dt = dcDal.Select();
                    dgvDeaCust.DataSource = dt;
                }

                if (cmbDeaCustType.Text == "Dealer")
                {
                    MessageBox.Show("Dealer Updated Successfully");
                    Clear();

                    //refresh data grid view
                    DataTable dt = dcDal.Select();
                    dgvDeaCust.DataSource = dt;
                }


            }
            else
            {
                MessageBox.Show("Failed to Update Dealer or Customer");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            dc.id = int.Parse(txtDeaCustID.Text);

            bool success = dcDal.Delete(dc);
            if(success == true)
            {
                if (cmbDeaCustType.Text == "Customer")
                {
                    MessageBox.Show("Customer Deleted Successfully");
                    Clear();

                    //refresh data grid view
                    DataTable dt = dcDal.Select();
                    dgvDeaCust.DataSource = dt;
                }
                if (cmbDeaCustType.Text == "Dealer")
                {
                    MessageBox.Show("Dealer Deleted Successfully");
                    Clear();

                    //refresh data grid view
                    DataTable dt = dcDal.Select();
                    dgvDeaCust.DataSource = dt;
                }


            }
            else
            {
                MessageBox.Show("Failed to Delete Dealer or Customer");
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keywords = txtSearch.Text;

            if(keywords != null)
            {
                DataTable dt = dcDal.Search(keywords);
                dgvDeaCust.DataSource = dt;
            }
            else
            {
                DataTable dt = dcDal.Select();
                dgvDeaCust.DataSource = dt;
            }
        }
    }
}
