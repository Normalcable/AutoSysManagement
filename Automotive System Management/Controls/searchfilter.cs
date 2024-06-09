using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Automotive_System_Management.Controls
{
    public partial class searchfilter : UserControl
    {
        private string connectionString = @"Data Source=DESKTOP-OIEBO4N\SQL2019X;Initial Catalog=automotiveSys;Integrated Security=True;Encrypt=False; TrustServerCertificate=True";

        public searchfilter()
        {
            InitializeComponent();

            // Set DataGridView to fill the entire width
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Call the search method instead of adding a new item
            SearchDataInDatabase();
        }


        // New method for searching data in the database
        private void SearchDataInDatabase()
        {
            // Check if all textboxes are empty
            if (AllTextBoxesEmpty())
            {
                MessageBox.Show("Please enter at least one search criteria.", "Search Criteria Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get values from textboxes for search criteria
            string itemId = textBox3.Text;
            string name = textBox1.Text;
            string category = textBox2.Text;
            string priceText = textBox4.Text;
            string stockText = textBox5.Text;
            string manufacturer = textBox6.Text;

            // SQL query to search for data based on user input
            string searchQuery = "SELECT * FROM autoitems WHERE " +
                                 "(@ItemId IS NULL OR ItemID = @ItemId) " +
                                 "AND Name LIKE @Name AND Category LIKE @Category " +
                                 "AND Price LIKE @Price AND Stock LIKE @Stock " +
                                 "AND Manufacturer LIKE @Manufacturer";
            // Add more conditions as needed for searching

            // Create and open a connection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Create a command with parameters
                using (SqlCommand searchCommand = new SqlCommand(searchQuery, connection))
                {
                    searchCommand.Parameters.AddWithValue("@ItemId", string.IsNullOrEmpty(itemId) ? (object)DBNull.Value : int.Parse(itemId));
                    searchCommand.Parameters.AddWithValue("@Name", "%" + name + "%");
                    searchCommand.Parameters.AddWithValue("@Category", "%" + category + "%");
                    searchCommand.Parameters.AddWithValue("@Price", "%" + priceText + "%");
                    searchCommand.Parameters.AddWithValue("@Stock", "%" + stockText + "%");
                    searchCommand.Parameters.AddWithValue("@Manufacturer", "%" + manufacturer + "%");

                    // Create a SqlDataAdapter to fill a DataTable
                    using (SqlDataAdapter adapter = new SqlDataAdapter(searchCommand))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Clear existing data in DataGridView
                        dataGridView1.DataSource = null;
                        dataGridView1.Rows.Clear();

                        // Display the search results in DataGridView
                        if (dataTable.Rows.Count > 0)
                        {
                            dataGridView1.DataSource = dataTable;
                        }
                        else
                        {
                            MessageBox.Show("No results found.", "Search Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            ResetTextBoxes();
        }

        // Helper method to check if all textboxes are empty
        private bool AllTextBoxesEmpty()
        {
            foreach (Control control in Controls)
            {
                if (control is System.Windows.Forms.TextBox textBox && !string.IsNullOrEmpty(textBox.Text))
                {
                    return false;
                }
            }
            return true;
        }

        private void ResetTextBoxes()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
        }

        private void dataGridView1_SelectionChanged(Object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
        }
    }
}