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
    internal class userDAL
    {
        static string str = ConfigurationManager.ConnectionStrings["c1"].ConnectionString;

        #region Select Data from Database
        public DataTable Select()
        {
            SqlConnection con = new SqlConnection(str);

            DataTable dt = new DataTable();
            try
            {
                String sql = "select * from tbl_users";
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                con.Open();
                adapter.Fill(dt);
;            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
            return dt;

;
        }
        #endregion

        #region Insert Data in Database
        public bool Insert(userBLL u)
        {
            bool isSuccess = false;

            // Validate fields
            if (string.IsNullOrWhiteSpace(u.first_name) ||
                string.IsNullOrWhiteSpace(u.last_name) ||
                string.IsNullOrWhiteSpace(u.email) ||
                string.IsNullOrWhiteSpace(u.username) ||
                string.IsNullOrWhiteSpace(u.password) ||
                string.IsNullOrWhiteSpace(u.contact) ||
                string.IsNullOrWhiteSpace(u.address) ||
                string.IsNullOrWhiteSpace(u.gender) ||
                string.IsNullOrWhiteSpace(u.user_type) ||
                u.added_date == DateTime.MinValue || // Assuming added_date can't be empty
                u.added_by <= 0) // Assuming added_by is an integer representing user ID
            {
                MessageBox.Show("All fields are compulsory. Please fill in all the details correctly.");
                return false;
            }
            // validate contact
            if (!Regex.IsMatch(u.contact, @"^\d{10}$"))
            {
                MessageBox.Show("Contact number should contain exactly 10 digits.");
                return false;
            }

            // validate email
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(u.email, emailPattern))
            {
                MessageBox.Show("Please enter a valid email address.");
                return false;
            }

            SqlConnection con = new SqlConnection(str);
            try
            {
                string sql = "INSERT INTO tbl_users(first_name, last_name, email, username, password, contact, address, gender, user_type, added_date, added_by) " +
                             "VALUES (@first_name, @last_name, @email, @username, @password, @contact, @address, @gender, @user_type, @added_date, @added_by)";

                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@first_name", u.first_name);
                cmd.Parameters.AddWithValue("@last_name", u.last_name);
                cmd.Parameters.AddWithValue("@email", u.email);
                cmd.Parameters.AddWithValue("@username", u.username);
                cmd.Parameters.AddWithValue("@password", u.password);
                cmd.Parameters.AddWithValue("@contact", u.contact);
                cmd.Parameters.AddWithValue("@address", u.address);
                cmd.Parameters.AddWithValue("@gender", u.gender);
                cmd.Parameters.AddWithValue("@user_type", u.user_type);
                cmd.Parameters.AddWithValue("@added_date", u.added_date);
                cmd.Parameters.AddWithValue("@added_by", u.added_by);

                con.Open();

                int rows = cmd.ExecuteNonQuery();

                isSuccess = rows > 0;
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

        #region Update Data in Database
        

public bool Update(userBLL u)
    {
        bool isSuccess = false;
        SqlConnection con = new SqlConnection(str);

        try
        {
            // Regular expressions for validation
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            string contactPattern = @"^\d{10}$"; // 10-digit contact number

            // Validate all mandatory fields
            if (string.IsNullOrWhiteSpace(u.first_name) ||
                string.IsNullOrWhiteSpace(u.last_name) ||
                string.IsNullOrWhiteSpace(u.email) ||
                string.IsNullOrWhiteSpace(u.username) ||
                string.IsNullOrWhiteSpace(u.password) ||
                string.IsNullOrWhiteSpace(u.contact) ||
                string.IsNullOrWhiteSpace(u.address) ||
                string.IsNullOrWhiteSpace(u.gender) ||
                string.IsNullOrWhiteSpace(u.user_type) ||
                u.added_date == DateTime.MinValue ||
                u.added_by <= 0 ||
                u.id <= 0)
            {
                MessageBox.Show("All fields are compulsory. Please fill in all the details correctly.");
                return false;
            }

            // Email validation
            if (!Regex.IsMatch(u.email, emailPattern))
            {
                MessageBox.Show("Please enter a valid email address.");
                return false;
            }

            // Contact validation
            if (!Regex.IsMatch(u.contact, contactPattern))
            {
                MessageBox.Show("Contact number should contain exactly 10 digits.");
                return false;
            }

            // Update Query
            string sql = "UPDATE tbl_users SET first_name = @first_name, last_name = @last_name, email = @email, " +
                         "username = @username, password = @password, contact = @contact, address = @address, " +
                         "gender = @gender, user_type = @user_type, added_date = @added_date, added_by = @added_by " +
                         "WHERE id = @id";

            SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@first_name", u.first_name);
            cmd.Parameters.AddWithValue("@last_name", u.last_name);
            cmd.Parameters.AddWithValue("@email", u.email);
            cmd.Parameters.AddWithValue("@username", u.username);
            cmd.Parameters.AddWithValue("@password", u.password);
            cmd.Parameters.AddWithValue("@contact", u.contact);
            cmd.Parameters.AddWithValue("@address", u.address); // Fixed missing '@'
            cmd.Parameters.AddWithValue("@gender", u.gender);
            cmd.Parameters.AddWithValue("@user_type", u.user_type);
            cmd.Parameters.AddWithValue("@added_date", u.added_date);
            cmd.Parameters.AddWithValue("@added_by", u.added_by);
            cmd.Parameters.AddWithValue("@id", u.id);

            con.Open();
            int rows = cmd.ExecuteNonQuery();
            isSuccess = rows > 0;

            if (isSuccess)
            {
                MessageBox.Show("User updated successfully.");
                   
            }
            else
            {
                MessageBox.Show("Update failed. No record was modified.");
            }
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

    #region Delete Data from Database
    public bool Delete(userBLL u)
        {
            bool isSuccess = false;

            SqlConnection con = new SqlConnection(str);

            try
            {
                String sql = "delete from tbl_users where id = @id";
                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@id", u.id);
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

        #region Search User

        public DataTable Search(string keywords)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(str))
            {
                try
                {
                    string sql = "SELECT * FROM tbl_users WHERE id LIKE @keywords OR first_name LIKE @keywords OR last_name LIKE @keywords OR username LIKE @keywords";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@keywords", "%" + keywords + "%");
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            con.Open();
                            adapter.Fill(dt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            return dt;
        }

        #endregion

        #region getting user id from username

        public userBLL GetIDFromUsername(string username)
        {
            userBLL u = new userBLL();
            SqlConnection con = new SqlConnection(str);
            DataTable dt = new DataTable();

            try
            {
                string sql = "select id from tbl_users where username = '"+username+"'";
                SqlDataAdapter adapter = new SqlDataAdapter(sql, con);
                con.Open();
                adapter.Fill(dt);
                if(dt.Rows.Count > 0)
                {
                    u.id = int.Parse(dt.Rows[0]["id"].ToString());
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
            return u;
        }
        #endregion
    }
}
