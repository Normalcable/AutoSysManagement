using System.Data.SqlClient;
using System;
using System.Windows.Forms;
using System.Linq;

namespace Automotive_System_Management
{
    public partial class Forgotpass : Form
    {
        private string connectionString = @"Data Source=DESKTOP-OIEBO4N\SQL2019X;Initial Catalog=automotiveSys;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
        private SqlConnection sqlConnection;

        public Forgotpass()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.FormClosing += Forgotpass_FormClosing;

            // Initialize your SqlConnection
            sqlConnection = new SqlConnection(connectionString);
        }

        private void Forgotpass_FormClosing(object sender, FormClosingEventArgs e)
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

        private void button1_Click(object sender, EventArgs e)
        {
            // Get username and security question from textBox1 and textBox2
            string username = textBox1.Text;
            string secuquest = textBox2.Text;

            // SQL query to retrieve password based on provided username and security question
            string query = "SELECT password FROM dbo.users WHERE username = @Username AND secuquest = @Secuquest";

            using (SqlCommand command = new SqlCommand(query, sqlConnection))
            {
                // Add parameters to the SQL query
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Secuquest", secuquest);

                try
                {
                    // Open the database connection
                    sqlConnection.Open();

                    // Execute the SQL query
                    object result = command.ExecuteScalar();

                    // Check if result is not null (username and security question match)
                    if (result != null)
                    {
                        // Display the password in textBox3
                        textBox3.Text = result.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Username or security question does not match.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    // Close the database connection
                    sqlConnection.Close();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Temporarily remove the FormClosing event handler
            this.FormClosing -= Forgotpass_FormClosing;

            // Close the current Signup form
            this.Close();

            // Reattach the FormClosing event handler
            this.FormClosing += Forgotpass_FormClosing;

            // Create an instance of the Login form if it doesn't exist
            Login loginForm = Application.OpenForms.OfType<Login>().FirstOrDefault() ?? new Login();

            // Show the Login form
            loginForm.Show();
        }
    }
}
