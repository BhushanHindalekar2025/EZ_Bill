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
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        loginBLL l = new loginBLL();
        loginDAL dal = new loginDAL();
        public static string loggedIn;

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            l.username = txtusername_log.Text.Trim();
            l.password = txtpassword_log.Text.Trim();
            l.user_type = cmbuser_type_log.Text.Trim();

            bool success = dal.loginCheck(l);
            if(success == true)
            {
                MessageBox.Show("Login Successful");
                loggedIn = l.username;

                switch (l.user_type)
                {
                    case "Admin":
                        {
                            frmAdminDashboard admin = new frmAdminDashboard();
                            admin.Show();
                            this.Hide();
                        }
                        break;
                    case "User":
                        {
                            frmUserDashboard user = new frmUserDashboard();
                            user.Show();
                            this.Hide();
                        }
                        break;
                    default:
                        {
                            MessageBox.Show("Invalid user type. ");
                        }
                        break;
                }
            }
            else
            {
                MessageBox.Show("Login Failed");
            }
        }
    }
}
