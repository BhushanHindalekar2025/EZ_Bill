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
    public partial class frmInventory : Form
    {
        public frmInventory()
        {
            InitializeComponent();
        }
        CategoriesDAL cdal = new CategoriesDAL();
        ProductDAL pdal = new ProductDAL();

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void frmInventory_Load(object sender, EventArgs e)
        {
            DataTable cdt = cdal.Select();
            cmbCategories.DataSource = cdt;
            cmbCategories.DisplayMember = "Title";
            cmbCategories.ValueMember = "title";

            //display the products in data grid view when the form is load
            DataTable pdt = pdal.Select();
            dgvProducts.DataSource = pdt;

        }

        private void cmbCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            //select products based on categories
            string category = cmbCategories.Text;
            DataTable dt = pdal.DisplayProductByCategory(category);
            dgvProducts.DataSource = dt;

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DataTable dt = pdal.Select();
            dgvProducts.DataSource = dt;

        }
    }
}
