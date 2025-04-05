using EZ_Bill.BLL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EZ_Bill.DAL
{
    internal class DeaCustDAL
    {
        static string str = ConfigurationManager.ConnectionStrings["c1"].ConnectionString;

        #region Select Method

        public DataTable Select()
        {
            SqlConnection con = new SqlConnection(str);
            DataTable dt = new DataTable();

            try
            {
                string sql = "select * from tbl_dea_cust";
                SqlCommand cmd = new SqlCommand(sql, con);
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

        #region Insert Method

        public bool Insert(DeaCustBLL dc)
        {
            SqlConnection con = new SqlConnection(str);
            bool isSuccess = false;

            try
            {
                // Validate all fields
                if (string.IsNullOrWhiteSpace(dc.type) ||
                    string.IsNullOrWhiteSpace(dc.name) ||
                    string.IsNullOrWhiteSpace(dc.email) ||
                    string.IsNullOrWhiteSpace(dc.contact) ||
                    string.IsNullOrWhiteSpace(dc.address) ||
                    dc.added_date == DateTime.MinValue ||  // Ensures a valid date
                    dc.added_by <= 0)  // Assuming added_by is an int representing user ID
                {
                    MessageBox.Show("All fields are compulsory. Please fill in all the details correctly.");
                    return false;
                }

                // contact validation
                if (!Regex.IsMatch(dc.contact, @"^\d{10}$"))
                {
                    MessageBox.Show("Contact number should contain exactly 10 digits.");
                    return false;
                }

                // meail validation
                string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(dc.email, emailPattern))
                {
                    MessageBox.Show("Please enter a valid email address.");
                    return false;
                }

                string sql = "INSERT INTO tbl_dea_cust (type, name, email, contact, address, added_date, added_by) " +
                             "VALUES (@type, @name, @email, @contact, @address, @added_date, @added_by)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@type", dc.type);
                cmd.Parameters.AddWithValue("@name", dc.name);
                cmd.Parameters.AddWithValue("@email", dc.email);
                cmd.Parameters.AddWithValue("@contact", dc.contact);
                cmd.Parameters.AddWithValue("@address", dc.address);
                cmd.Parameters.AddWithValue("@added_date", dc.added_date);
                cmd.Parameters.AddWithValue("@added_by", dc.added_by);

                con.Open();
                int rows = cmd.ExecuteNonQuery();
                isSuccess = rows > 0;
            }
            catch (Exception ex)
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

        #region Update Method

        

public bool Update(DeaCustBLL dc)
    {
        SqlConnection con = new SqlConnection(str);
        bool isSuccess = false;

        try
        {
            // Regular expressions for validation
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            string contactPattern = @"^\d{10}$"; // 10-digit contact number

            // Validate all fields
            if (string.IsNullOrWhiteSpace(dc.type) ||
                string.IsNullOrWhiteSpace(dc.name) ||
                string.IsNullOrWhiteSpace(dc.email) ||
                string.IsNullOrWhiteSpace(dc.contact) ||
                string.IsNullOrWhiteSpace(dc.address) ||
                dc.added_date == DateTime.MinValue ||
                dc.added_by <= 0 ||
                dc.id <= 0)
            {
                MessageBox.Show("All fields are compulsory. Please fill in all the details correctly.");
                return false;
            }

            // Email validation
            if (!Regex.IsMatch(dc.email, emailPattern))
            {
                MessageBox.Show("Please enter a valid email address.");
                return false;
            }

            // Contact validation
            if (!Regex.IsMatch(dc.contact, contactPattern))
            {
                MessageBox.Show("Contact number should contain exactly 10 digits.");
                return false;
            }

            // Update Query
            string sql = "UPDATE tbl_dea_cust SET type = @type, name = @name, email = @email, contact = @contact, " +
                         "address = @address, added_date = @added_date, added_by = @added_by WHERE id = @id";

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@type", dc.type);
            cmd.Parameters.AddWithValue("@name", dc.name);
            cmd.Parameters.AddWithValue("@email", dc.email);
            cmd.Parameters.AddWithValue("@contact", dc.contact);
            cmd.Parameters.AddWithValue("@address", dc.address);
            cmd.Parameters.AddWithValue("@added_date", dc.added_date);
            cmd.Parameters.AddWithValue("@added_by", dc.added_by);
            cmd.Parameters.AddWithValue("@id", dc.id);

            con.Open();
            int rows = cmd.ExecuteNonQuery();
            isSuccess = rows > 0;

          /*  if (isSuccess)
            {
                MessageBox.Show("Record updated successfully.");
            }
            else
            {
                MessageBox.Show("Update failed. No record was modified.");
            } */
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}");
        }
        finally
        {
            con.Close();
        }

        return isSuccess;
    }


    #endregion

         #region Delete Method

    public bool Delete(DeaCustBLL dc)
        {
            SqlConnection con = new SqlConnection(str);
            bool isSuccess = false;

            try
            {
                string sql = "delete from tbl_dea_cust where id = @id";
                SqlCommand cmd = new SqlCommand(sql,con);
                cmd.Parameters.AddWithValue("@id",dc.id);

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

        #region Search Method

        public DataTable Search(string keywords)
        {
            SqlConnection con = new SqlConnection(str);
            DataTable dt = new DataTable();

            try
            {
                string sql = "select * from tbl_dea_cust where id like '%"+keywords+"%' or type like '%"+keywords+"%' or name like '%"+keywords+"%'";
                SqlCommand cmd = new SqlCommand(sql, con);
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

        #region Method to search dealer or customer for transaction module

        public DeaCustBLL SearchDealerCustomerForTransaction(string keyword)
        {
            DeaCustBLL dc = new DeaCustBLL();

            SqlConnection con = new SqlConnection(str);

            DataTable dt = new DataTable();

            try
            {
                string sql = "select name, email, contact, address from tbl_dea_cust where id like '%"+keyword+"%' or name like '%"+keyword+"%'";
                SqlDataAdapter adapter = new SqlDataAdapter(sql, con);
                con.Open();
                adapter.Fill(dt);

                if(dt.Rows.Count  > 0)
                {
                    dc.name = dt.Rows[0]["name"].ToString();
                    dc.email = dt.Rows[0]["email"].ToString();
                    dc.contact = dt.Rows[0]["contact"].ToString();
                    dc.address = dt.Rows[0]["address"].ToString();
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

            return dc;
        }

        #endregion

        #region Method to get id of dealer or customer based on name

        public DeaCustBLL GetDeaCustIdFromName(string Name)
        {
            DeaCustBLL dc = new DeaCustBLL();

            SqlConnection con = new SqlConnection(str);
            DataTable dt = new DataTable();

            try
            {
                string sql = "select id from tbl_dea_cust where name = '"+Name+"'";
                SqlDataAdapter adapter = new SqlDataAdapter(sql,con);
                con.Open();

                adapter.Fill(dt);
                if(dt.Rows.Count > 0)
                {
                    dc.id = int.Parse(dt.Rows[0]["id"].ToString());

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

            return dc;
        }

        #endregion
    }
}
