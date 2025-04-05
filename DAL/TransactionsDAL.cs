using EZ_Bill.BLL;
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
    internal class TransactionsDAL
    {
        static string str = ConfigurationManager.ConnectionStrings["c1"].ConnectionString;

        #region Insert Method

        public bool Insert(transactionsBLL t ,out int transactionID)
        {
            bool isSuccess = false;

            transactionID = -1;
            SqlConnection con = new SqlConnection(str);

            try
            {
                string sql = "insert into tbl_transcations (type,dea_cust_id,grandTotal,transaction_date,tax,discount,added_by) values (@type,@dea_cust_id,@grandTotal,@transaction_date,@tax,@discount,@added_by); select @@IDENTITY";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@type",t.type);
                cmd.Parameters.AddWithValue("@dea_cust_id",t.dea_cust_id);
                cmd.Parameters.AddWithValue("@grandTotal",t.grandTotal);
                cmd.Parameters.AddWithValue("@transaction_date",t.transaction_date);
                cmd.Parameters.AddWithValue("@tax",t.tax);
                cmd.Parameters.AddWithValue("@discount",t.discount);
                cmd.Parameters.AddWithValue("@added_by",t.added_by);

                con.Open();

                object o = cmd.ExecuteScalar();
                if(o != null)
                {
                    transactionID = int.Parse(o.ToString());
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

        #region Method to display all the transaction 

        public DataTable DisplayAllTransactions()
        {
            SqlConnection con = new SqlConnection(str);
            DataTable dt = new DataTable();

            try
            {
                string sql = "select * from tbl_transcations";
                SqlCommand cmd = new SqlCommand(sql,con);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                con.Open();
                adapter.Fill(dt);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
            return dt;
        }

        #endregion

        #region Method to display transaction based on transaction type

        public DataTable DisplayTranscationByType(string type)
        {
            SqlConnection con = new SqlConnection(str);
            DataTable dt = new DataTable();

            try
            {
                string sql = "select * from tbl_transcations where type = '"+type+"'";
                SqlCommand cmd = new SqlCommand(sql,con);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                con.Open();
                adapter.Fill(dt);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }

            return dt;
        }

        #endregion



    }
}
