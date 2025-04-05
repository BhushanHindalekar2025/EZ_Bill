using EZ_Bill.BLL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EZ_Bill.DAL
{
    internal class TransactionsDetails
    {
        static string str = ConfigurationManager.ConnectionStrings["c1"].ConnectionString;

        #region Insert Method

        public bool Insert_TransactionDetails(transactionsDetailsBLL td)
        {
            bool isSuccess = false;

            SqlConnection con = new SqlConnection(str);
            try
            {
                string sql = "insert into tbl_transcation_detail(product_id,rate,qty,total,dea_cust_id,added_date,added_by) values(@product_id,@rate,@qty,@total,@dea_cust_id,@added_date,@added_by)";
                SqlCommand cmd = new SqlCommand(sql,con);
                cmd.Parameters.AddWithValue("@product_id",td.product_id);
                cmd.Parameters.AddWithValue("@rate",td.rate);
                cmd.Parameters.AddWithValue("@qty",td.qty);
                cmd.Parameters.AddWithValue("@total",td.total);
                cmd.Parameters.AddWithValue("@dea_cust_id",td.dea_cust_id);
                cmd.Parameters.AddWithValue("@added_date",td.added_date);
                cmd.Parameters.AddWithValue("@added_by",td.added_by);
                con.Open();

                int rows = cmd.ExecuteNonQuery();
                if(rows > 0)
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

        #endregion


    }
}
