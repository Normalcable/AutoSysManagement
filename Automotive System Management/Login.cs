using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Automotive_System_Management
{
    public partial class Login : Form
    {
        // Replace with your actual connection string
        readonly SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-OIEBO4N\SQL2019X;Initial Catalog=automotiveSys;Integrated Security=True;Encrypt=False; TrustServerCertificate=True");

        public Login()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.FormClosing += ItemsManagement_FormClosing;

            // Attach the KeyPress event handler to the username textbox
            textBox1.KeyPress += TextBox_KeyPress;
            // Attach the KeyPress event handler to the password textbox
            textBox2.KeyPress += TextBox_KeyPress;
            // Attach the KeyPress event handler specifically for the password textbox to trigger login button click
            textBox2.KeyPress += TextBox2_KeyPress;
        }

        private void ItemsManagement_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check if the form is closing due to a user action (e.g., clicking the X button)
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // Display a confirmation dialog
                DialogResult result = MessageBox.Show("Are you sure you want to exit?\nAny unsaved data may be lost.", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                // Check the user's response
                if (result == DialogResult.Yes)
                {
                    // User clicked "Yes," so exit the application
                    Application.Exit();
                }
                else
                {
                    // User clicked "No," cancel the form closing
                    e.Cancel = true;
                }
            }
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                // Move focus to the next control in the tab order
                this.SelectNextControl((Control)sender, true, true, true, true);
                e.Handled = true; // Suppress the Enter key
            }
        }

        private void TextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                // Trigger the login button click event
                button1.PerformClick();
                e.Handled = true; // Suppress the Enter key
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String username, user_password;

            username = textBox1.Text;
            user_password = textBox2.Text;

            try
            {
                string query = "SELECT * FROM users WHERE username = '" + username + "' AND password = '" + user_password + "'";
                SqlDataAdapter sda = new SqlDataAdapter(query, conn);

                DataTable dtable = new DataTable();
                sda.Fill(dtable);

                if (dtable.Rows.Count > 0)
                {
                    DataRow row = dtable.Rows[0];

                    username = row["username"].ToString();
                    user_password = row["password"].ToString();
                    bool isAdmin = Convert.ToBoolean(row["IsAdmin"]);

                    // Update the user's admin status in dbo.userstat table
                    UpdateUserStat(Convert.ToInt32(row["user_id"]), isAdmin);

                    // Page that needs to load next
                    form1 mainfrm = new form1();
                    mainfrm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid login details", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.Clear();
                    textBox2.Clear();

                    // Focus on username textbox
                    textBox1.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        private void UpdateUserStat(int userId, bool isAdmin)
        {
            try
            {
                conn.Open();
                string updateQuery = "UPDATE dbo.userstat SET IsAdmin = @IsAdmin WHERE UserID = @UserID";
                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@IsAdmin", isAdmin);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating user status: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }



        private void button2_Click(object sender, EventArgs e)
        {
            // Create an instance of the Signup form
            Signup signupForm = new Signup();

            // Show the Signup form
            signupForm.Show();

            // Optionally, you can hide the current Login form if needed
            this.Hide();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            // Create an instance of the Signup form
            Forgotpass forgotpass = new Forgotpass();

            // Show the Signup form
            forgotpass.Show();

            // Optionally, you can hide the current Login form if needed
            this.Hide();
        }


        private bool showPassword = false;

        private void button3_Click(object sender, EventArgs e)
        {
            // Toggle the showPassword variable
            showPassword = !showPassword;

            // Change the password character based on the showPassword variable
            textBox2.UseSystemPasswordChar = !showPassword;
        }
    }
}
