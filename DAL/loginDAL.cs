﻿using EZ_Bill.BLL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EZ_Bill.DAL
{
    internal class loginDAL
    {
        static String str = ConfigurationManager.ConnectionStrings["c1"].ConnectionString;

        public bool loginCheck(loginBLL l)
        {
            bool isSuccess = false;

            SqlConnection con = new SqlConnection(str);
            try
            {
                String sql = "select * from tbl_users where username = @username and password = @password and user_type = @user_type";
                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@username", l.username);
                cmd.Parameters.AddWithValue("@password",l.password);
                cmd.Parameters.AddWithValue("@user_type",l.user_type);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                adapter.Fill(dt);

                if(dt.Rows.Count > 0)
                {
                    isSuccess = true;
                }
                else
                {
                    isSuccess = false;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }

            return isSuccess;
        }
    }
}
