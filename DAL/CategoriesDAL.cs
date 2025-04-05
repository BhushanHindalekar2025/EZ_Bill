using EZ_Bill.BLL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EZ_Bill.DAL
{
    internal class CategoriesDAL
    {
        static string str = ConfigurationManager.ConnectionStrings["c1"].ConnectionString;

        #region Select Method

        public DataTable Select()
        {
            SqlConnection con = new SqlConnection(str);

            DataTable dt = new DataTable();

            try
            {
                string sql = "select * from tbl_categories";
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

        #region Insert Data

        public bool Insert(CategoriesBLL c)
        {
            bool isSuccess = false;
            SqlConnection con = new SqlConnection(str);

            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(c.title) ||
                    string.IsNullOrWhiteSpace(c.description) ||
                    c.added_date == DateTime.MinValue ||
                    c.added_by <= 0)
                {
                    MessageBox.Show("All fields are compulsory. Please fill in all the details.");
                    return false;
                }

                // SQL Insert Query
                string sql = "INSERT INTO tbl_categories (title, description, added_date, added_by) " +
                             "VALUES (@title, @description, @added_date, @added_by)";
                SqlCommand cmd = new SqlCommand(sql, con);

                // Add parameters
                cmd.Parameters.AddWithValue("@title", c.title);
                cmd.Parameters.AddWithValue("@description", c.description);
                cmd.Parameters.AddWithValue("@added_date", c.added_date);
                cmd.Parameters.AddWithValue("@added_by", c.added_by);

                // Open connection and execute query
                con.Open();
                int rows = cmd.ExecuteNonQuery();

                isSuccess = rows > 0;

              /*  if (isSuccess)
                {
                    MessageBox.Show("Category inserted successfully.");
                }
                else
                {
                    MessageBox.Show("Failed to insert category.");
                }  */
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            return isSuccess;
        }

        #endregion

        #region Update Method

        public bool Update(CategoriesBLL c)
        {
            bool isSuccess = false;
            SqlConnection con = new SqlConnection(str);

            try
            {
                // Validate required fields
                if (c.id <= 0 ||
                    string.IsNullOrWhiteSpace(c.title) ||
                    string.IsNullOrWhiteSpace(c.description) ||
                    c.added_date == DateTime.MinValue ||
                    c.added_by <= 0)
                {
                    MessageBox.Show("All fields are compulsory. Please fill in all the details.");
                    return false;
                }

                // SQL Update Query
                string sql = "UPDATE tbl_categories SET title = @title, description = @description, added_date = @added_date, added_by = @added_by WHERE id = @id";
                SqlCommand cmd = new SqlCommand(sql, con);

                // Add parameters
                cmd.Parameters.AddWithValue("@title", c.title);
                cmd.Parameters.AddWithValue("@description", c.description);
                cmd.Parameters.AddWithValue("@added_date", c.added_date);
                cmd.Parameters.AddWithValue("@added_by", c.added_by);
                cmd.Parameters.AddWithValue("@id", c.id);

                // Open connection and execute query
                con.Open();
                int rows = cmd.ExecuteNonQuery();

                isSuccess = rows > 0;

            /*    if (isSuccess)
                {
                    MessageBox.Show("Category updated successfully.");
                }
                else
                {
                    MessageBox.Show("Failed to update category.");
                } */
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            return isSuccess;
        }


        #endregion

        #region Delete Category 
        public bool Delete(CategoriesBLL c)
        {
            bool isSuccess = false;

            SqlConnection con = new SqlConnection(str);
            try
            {
                String sql = "delete from tbl_categories where id = @id";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id",c.id);
                con.Open();
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
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

        #region Search

        public DataTable Search(string keywords)
        {
            SqlConnection con = new SqlConnection(str);
            DataTable dt = new DataTable();

            try
            {
                string sql = "select * from tbl_categories where id like '%"+keywords+"%' or title like '%"+keywords+"%' or description like '%"+keywords+"%'";
                SqlCommand cmd = new SqlCommand(sql,con);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                con.Open();
                adapter.Fill(dt);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to search",ex.ToString());
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
