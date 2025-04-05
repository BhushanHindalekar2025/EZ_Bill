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
    public partial class frmProduct : Form
    {
        public frmProduct()
        {
            InitializeComponent();
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        CategoriesDAL cdal = new CategoriesDAL();
        ProductBLL p = new ProductBLL();
        ProductDAL pdal = new ProductDAL();
        userDAL udal = new userDAL();

        private void frmProduct_Load(object sender, EventArgs e)
        {
            DataTable categoriesDT = cdal.Select();
            cmbPCategory.DataSource = categoriesDT;
            cmbPCategory.DisplayMember = "title";
            cmbPCategory.ValueMember = "title";

            //load product when form open
            DataTable dt = pdal.Select();
            dgvProducts.DataSource = dt;

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            p.name = txtPName.Text;
            p.category = cmbPCategory.Text;
            p.description = txtPDescription.Text;
            p.rate = decimal.Parse(txtPRate.Text);
            p.qty = 0;
            p.added_date = DateTime.Now;

            string loggedUsr = frmLogin.loggedIn;
            userBLL usr = udal.GetIDFromUsername(loggedUsr);
            p.added_by = usr.id;


            bool success = pdal.Insert(p);
            if (success == true)
            {
                MessageBox.Show("Product Inserted Successfully");
                Clear();
                DataTable dt = pdal.Select();
                dgvProducts.DataSource = dt;

            }
            else
            {
                MessageBox.Show("Failed to Add Product");

            }
        }
        public void Clear()
        {
            txtPID.Text = "";
            txtPName.Text = "";
            txtPDescription.Text = "";
            txtPRate.Text = "";
            txtSearch.Text = "";

        }

        private void dgvProducts_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int RowIndex = e.RowIndex;
            txtPID.Text = dgvProducts.Rows[RowIndex].Cells[0].Value.ToString();
            txtPName.Text = dgvProducts.Rows[RowIndex].Cells[1].Value.ToString();
            cmbPCategory.Text = dgvProducts.Rows[RowIndex].Cells[2].Value.ToString();
            txtPDescription.Text = dgvProducts.Rows[RowIndex].Cells[3].Value.ToString();
            txtPRate.Text = dgvProducts.Rows[RowIndex].Cells[4].Value.ToString();

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            p.id = int.Parse(txtPID.Text);
            p.name = txtPName.Text;
            p.category = cmbPCategory.Text;
            p.description = txtPDescription.Text;
            p.rate = decimal.Parse(txtPRate.Text);
            p.added_date = DateTime.Now;

            string loggedUsr = frmLogin.loggedIn;
            userBLL usr = udal.GetIDFromUsername(loggedUsr);
            p.added_by = usr.id;

            bool success = pdal.Update(p);
            if (success == true)
            {
                MessageBox.Show("Product Updated Successfully");
                Clear();
                DataTable dt = pdal.Select();
                dgvProducts.DataSource = dt;

            }
            else
            {
                MessageBox.Show("Failed to Update Product");

            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            p.id = int.Parse(txtPID.Text);

            bool success = pdal.Delete(p);
            if (success == true)
            {
                MessageBox.Show("Product Deleted Successfully");
                Clear();
                DataTable dt = pdal.Select();
                dgvProducts.DataSource = dt;

            }
            else
            {
                MessageBox.Show("Failed to Delete Product");

            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keywords = txtSearch.Text;

            if(keywords != null)
            {
                DataTable dt = pdal.Search(keywords);
                dgvProducts.DataSource = dt;
            }
            else
            {
                DataTable dt = pdal.Select();
                dgvProducts.DataSource = dt;
            }
        }
    }
}
