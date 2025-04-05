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
    internal class ProductDAL
    {
        static string str = ConfigurationManager.ConnectionStrings["c1"].ConnectionString;

        #region Select Method

        public DataTable Select()
        {
            SqlConnection con = new SqlConnection(str);

            DataTable dt = new DataTable();

            try
            {
                string sql = "select * from tbl_products";
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

        #region Insert Method

        public bool Insert(ProductBLL p)
        {
            bool isSuccess = false;
            SqlConnection con = new SqlConnection(str);

            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(p.name) ||
                    string.IsNullOrWhiteSpace(p.category) ||
                    string.IsNullOrWhiteSpace(p.description) ||
                    p.rate <= 0 ||
                    p.qty < 0 ||
                    p.added_date == DateTime.MinValue ||
                    p.added_by <= 0)
                {
                    MessageBox.Show("All fields are compulsory and must be valid. Please fill in all the details.");
                    return false;
                }


                // SQL Insert Query
                string sql = "INSERT INTO tbl_products (name, category, description, rate, qty, added_date, added_by) " +
                             "VALUES (@name, @category, @description, @rate, @qty, @added_date, @added_by)";
                SqlCommand cmd = new SqlCommand(sql, con);

                // Add parameters
                cmd.Parameters.AddWithValue("@name", p.name);
                cmd.Parameters.AddWithValue("@category", p.category);
                cmd.Parameters.AddWithValue("@description", p.description);
                cmd.Parameters.AddWithValue("@rate", p.rate);
                cmd.Parameters.AddWithValue("@qty", p.qty);
                cmd.Parameters.AddWithValue("@added_date", p.added_date);
                cmd.Parameters.AddWithValue("@added_by", p.added_by);

                // Open connection and execute query
                con.Open();
                int rows = cmd.ExecuteNonQuery();

                isSuccess = rows > 0;

                if (isSuccess)
                {
                    MessageBox.Show("Product inserted successfully.");
                }
                else
                {
                    MessageBox.Show("Failed to insert product.");
                }
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

        public bool Update(ProductBLL p)
        {
            bool isSuccess = false;
            SqlConnection con = new SqlConnection(str);

            try
            {
                // Validate required fields
                if (p.id <= 0 ||
                    string.IsNullOrWhiteSpace(p.name) ||
                    string.IsNullOrWhiteSpace(p.category) ||
                    string.IsNullOrWhiteSpace(p.description) ||
                    p.rate <= 0 ||
                    p.qty < 0 ||
                    p.added_date == DateTime.MinValue ||
                    p.added_by <= 0)
                {
                    MessageBox.Show("All fields are compulsory and must be valid. Please fill in all the details.");
                    return false;
                }

                // SQL Update Query
                string sql = "UPDATE tbl_products SET name = @name, category = @category, description = @description, rate = @rate, qty = @qty, added_date = @added_date, added_by = @added_by WHERE id = @id";
                SqlCommand cmd = new SqlCommand(sql, con);

                // Add parameters
                cmd.Parameters.AddWithValue("@name", p.name);
                cmd.Parameters.AddWithValue("@category", p.category);
                cmd.Parameters.AddWithValue("@description", p.description);
                cmd.Parameters.AddWithValue("@rate", p.rate);
                cmd.Parameters.AddWithValue("@qty", p.qty);
                cmd.Parameters.AddWithValue("@added_date", p.added_date);
                cmd.Parameters.AddWithValue("@added_by", p.added_by);
                cmd.Parameters.AddWithValue("@id", p.id);

                // Open connection and execute query
                con.Open();
                int rows = cmd.ExecuteNonQuery();

                isSuccess = rows > 0;

               /* if (isSuccess)
                {
                    MessageBox.Show("Product updated successfully.");
                }
                else
                {
                    MessageBox.Show("Failed to update product.");
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

        #region Delete Method

        public bool Delete(ProductBLL p)
        {
            bool isSuccess = false;

            SqlConnection con = new SqlConnection(str);

            try
            {
                string sql = "delete from tbl_products where id = @id";
                SqlCommand cmd = new SqlCommand(sql,con);
                cmd.Parameters.AddWithValue("@id",p.id);
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

        #region Search Method

        public DataTable Search(string keywords)
        {
            DataTable dt = new DataTable();

            SqlConnection con = new SqlConnection(str);

            try
            {
                string sql = "select * from tbl_products where id like '%"+keywords+"%' or name like '%"+keywords+"%' or category like '%"+keywords+"%'";
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

        #region Method to search product in transaction module

        public ProductBLL GetProductForTransaction(string keyword)
        {
            ProductBLL p = new ProductBLL();

            SqlConnection con = new SqlConnection(str);
            DataTable dt = new DataTable();
            try
            {
                string sql = "select name, rate, qty from tbl_products where id like '%"+keyword+"%' or name like '%"+keyword+"%'";
                SqlDataAdapter adapter = new SqlDataAdapter(sql, con);
                con.Open();

                adapter.Fill(dt);

                if(dt.Rows.Count > 0)
                {
                    p.name = dt.Rows[0]["name"].ToString();
                    p.rate = decimal.Parse(dt.Rows[0]["rate"].ToString());
                    p.qty = decimal.Parse(dt.Rows[0]["qty"].ToString());


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

            return p;
        }

        #endregion

        #region Method tp get product id based on product name

        public ProductBLL GetProductIdFromName(string ProductName)
        {
            ProductBLL p = new ProductBLL();

            SqlConnection con = new SqlConnection(str);
            DataTable dt = new DataTable();

            try
            {
                string sql = "select id from tbl_products where name = '" + ProductName + "'";
                SqlDataAdapter adapter = new SqlDataAdapter(sql, con);
                con.Open();

                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    p.id = int.Parse(dt.Rows[0]["id"].ToString());

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }

            return p;
        }

        #endregion

        #region Method to get current quantity from database based on product id

        public decimal GetProductQty(int ProductID)
        {
            SqlConnection con = new SqlConnection(str);
            decimal qty = 0;

            DataTable dt = new DataTable();
            try
            {
                string sql = "select qty from tbl_products where id = "+ProductID;
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                con.Open();

                adapter.Fill(dt);
                if(dt.Rows.Count > 0)
                {
                    qty = decimal.Parse(dt.Rows[0]["qty"].ToString());

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

            return qty;
        }

        #endregion

        #region Method to update quantity 

        public bool UpdateQuantity(int ProductID,decimal Qty)
        {
            bool success = false;

            SqlConnection con = new SqlConnection(str);

            try
            {
                string sql = "update tbl_products set qty = @qty where id = @id";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@qty",Qty);
                cmd.Parameters.AddWithValue("@id",ProductID);
                con.Open();

                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    success = true;
                }
                else 
                {
                    success = false;
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

            return success;
        }

        #endregion

        #region Method to increase product

        public bool IncreaseProduct(int ProductID,decimal IncreaseQty)
        {
            bool success = false;

            SqlConnection con = new SqlConnection(str);

            try
            {
                decimal currentQty = GetProductQty(ProductID);
                //increase quantity when product purchase from dealer
                decimal NewQty = currentQty + IncreaseQty;
                //update the product quantity
                success = UpdateQuantity(ProductID,NewQty);

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }

            return success;
        }

        #endregion

        #region Method to decrease product

        public bool DecreaseProduct(int ProductID,decimal Qty)
        {
            bool success = false;

            SqlConnection con = new SqlConnection(str); ;

            try
            {
                //grt current product quantity
                decimal currentQty = GetProductQty(ProductID);

                //decrease the product quantity based on purchase sale
                decimal NewQty = currentQty - Qty;

                //update the product in database
                success = UpdateQuantity(ProductID, NewQty);


            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }

            return success;
        }

        #endregion

        #region Display Products based on category

        public DataTable DisplayProductByCategory(string category)
        {
            SqlConnection con = new SqlConnection(str);
            DataTable dt = new DataTable();

            try
            {
                string sql = "select * from tbl_products where category = '"+category+"'";
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
