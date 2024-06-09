using System.Data.SqlClient;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Automotive_System_Management
{
    public partial class Signup : Form
    {
        private string connectionString = @"Data Source=DESKTOP-OIEBO4N\SQL2019X;Initial Catalog=automotiveSys;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";

        public Signup()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.FormClosing += Signup_FormClosing;

            // Attach the Click event handler for the "Create" button
            button1.Click += button1_Click;
        }

        private void Signup_FormClosing(object sender, FormClosingEventArgs e)
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
            // Get user input from textboxes
            string username = textBox1.Text;
            string password = textBox2.Text;
            string secuquest = textBox3.Text;

            // Validate input (you may want to add more robust validation)
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(secuquest))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Insert data into the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Use parameterized query to prevent SQL injection
                string query = "INSERT INTO dbo.users (username, password, secuquest, IsAdmin) VALUES (@Username, @Password, @Secuquest, @IsAdmin)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Secuquest", secuquest);

                    // Set IsAdmin to False
                    command.Parameters.AddWithValue("@IsAdmin", false);

                    command.ExecuteNonQuery();

                    MessageBox.Show("User created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            // Clear textboxes after successful registration
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
        }





        private void button2_Click(object sender, System.EventArgs e)
        {
            // Temporarily remove the FormClosing event handler
            this.FormClosing -= Signup_FormClosing;

            // Close the current Signup form
            this.Close();

            // Reattach the FormClosing event handler
            this.FormClosing += Signup_FormClosing;

            // Create an instance of the Login form if it doesn't exist
            Login loginForm = Application.OpenForms.OfType<Login>().FirstOrDefault() ?? new Login();

            // Show the Login form
            loginForm.Show();
        }
    }
}
