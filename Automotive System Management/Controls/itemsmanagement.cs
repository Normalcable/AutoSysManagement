using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Automotive_System_Management.Controls
{
    public partial class itemsmanagement : UserControl
    {
        private string connectionString = @"Data Source=DESKTOP-OIEBO4N\SQL2019X;Initial Catalog=automotiveSys;Integrated Security=True;Encrypt=False; TrustServerCertificate=True";
   
        public itemsmanagement()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Get the itemID from textBox3
            int itemIdToUpdate;

            if (int.TryParse(textBox3.Text, out itemIdToUpdate))
            {
                // Update the row with the specified itemID
                UpdateItemById(itemIdToUpdate);

                // Reload data after update
                LoadDataFromDatabase();
                ResetTextBoxes();
            }
            else
            {
                MessageBox.Show("Invalid item ID. Please enter a valid numeric ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateItemById(int itemId)
        {
            // Get values from textboxes
            string name = textBox1.Text;
            string category = textBox2.Text;
            string priceText = textBox4.Text;
            string stockText = textBox5.Text;
            string manufacturer = textBox6.Text;

            // Parse numeric values if not empty
            decimal? price = !string.IsNullOrWhiteSpace(priceText) ? (decimal?)decimal.Parse(priceText) : null;
            int? stock = !string.IsNullOrWhiteSpace(stockText) ? (int?)int.Parse(stockText) : null;

            // SQL query to update data based on itemID
            string updateQuery = "UPDATE dbo.autoitems SET " +
                                 "Name = ISNULL(@Name, Name), " +
                                 "Category = ISNULL(@Category, Category), " +
                                 "Price = ISNULL(@Price, Price), " +
                                 "Stock = ISNULL(@Stock, Stock), " +
                                 "Manufacturer = ISNULL(@Manufacturer, Manufacturer) " +
                                 "WHERE itemID = @ItemID";

            // Create and open a connection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Create a command with parameters
                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", string.IsNullOrWhiteSpace(name) ? (object)DBNull.Value : name);
                    command.Parameters.AddWithValue("@Category", string.IsNullOrWhiteSpace(category) ? (object)DBNull.Value : category);
                    command.Parameters.AddWithValue("@Price", price.HasValue ? (object)price.Value : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Stock", stock.HasValue ? (object)stock.Value : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Manufacturer", string.IsNullOrWhiteSpace(manufacturer) ? (object)DBNull.Value : manufacturer);
                    command.Parameters.AddWithValue("@ItemID", itemId);

                    // Execute the update query
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Item updated successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Item with specified ID not found. No rows updated.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            LoadDataFromDatabase();
            ResetTextBoxes();
        }






        private void button2_Click(object sender, EventArgs e)
        {
            // Check if Item ID is manually entered
            if (!string.IsNullOrEmpty(textBox3.Text))
            {
                MessageBox.Show("Item ID is automatically assigned and should not be manually entered. It is only needed for edit and delete operations.");
                return;
            }

            // Get values from textboxes
            string name = textBox1.Text;
            string category = textBox2.Text;
            string priceText = textBox4.Text;
            string stockText = textBox5.Text;
            string manufacturer = textBox6.Text;

            // Check if required textboxes are empty
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(category) ||
                string.IsNullOrEmpty(priceText) || string.IsNullOrEmpty(stockText) || string.IsNullOrEmpty(manufacturer))
            {
                MessageBox.Show("Please fill up all fields except Item ID.");
                return;
            }

            // Parse numeric values
            decimal price = decimal.Parse(priceText);
            int stock = int.Parse(stockText);

            // SQL query to retrieve the last itemID
            string getLastItemIdQuery = "SELECT MAX(itemID) FROM autoitems";

            // Create and open a connection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Create a command
                using (SqlCommand command = new SqlCommand(getLastItemIdQuery, connection))
                {
                    // Retrieve the last itemID from the database
                    object result = command.ExecuteScalar();

                    // Calculate the next itemID
                    int nextItemId = (result == DBNull.Value) ? 1 : ((int)result) + 1;

                    // SQL query to insert data into the table excluding itemID
                    string insertQuery = "SET IDENTITY_INSERT autoitems ON; " +
                                         "INSERT INTO autoitems (itemID, Name, Category, Price, Stock, Manufacturer) " +
                                         "VALUES (@ItemID, @Name, @Category, @Price, @Stock, @Manufacturer); " +
                                         "SET IDENTITY_INSERT autoitems OFF;";

                    // Create a command with parameters
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@ItemID", nextItemId);
                        insertCommand.Parameters.AddWithValue("@Name", name);
                        insertCommand.Parameters.AddWithValue("@Category", category);
                        insertCommand.Parameters.AddWithValue("@Price", price);
                        insertCommand.Parameters.AddWithValue("@Stock", stock);
                        insertCommand.Parameters.AddWithValue("@Manufacturer", manufacturer);

                        // Execute the query
                        int rowsAffected = insertCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Item added successfully!");
                        }
                        else
                        {
                            MessageBox.Show("Failed to add item. Please check your input and try again.");
                        }
                    }
                }
            }
            LoadDataFromDatabase();
            ResetTextBoxes();
        }








        private void button3_Click(object sender, EventArgs e)
        {
            // Get the itemID from textBox3
            int itemIdToDelete;

            // Check if Item ID is provided
            if (!int.TryParse(textBox3.Text, out itemIdToDelete))
            {
                MessageBox.Show("Invalid item ID. Please enter a valid numeric ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if other textboxes have values
            if (!string.IsNullOrEmpty(textBox1.Text) || !string.IsNullOrEmpty(textBox2.Text) ||
                !string.IsNullOrEmpty(textBox4.Text) || !string.IsNullOrEmpty(textBox5.Text) || !string.IsNullOrEmpty(textBox6.Text))
            {
                MessageBox.Show("Only Item ID is needed for deletion. Please leave the other fields empty.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Confirm with the user before deletion
            DialogResult result = MessageBox.Show("Are you sure you want to delete this item?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Delete the row with the specified itemID
                DeleteItemById(itemIdToDelete);

                // Reload data after deletion
                LoadDataFromDatabase();

                ResetTextBoxes();
            }
        }



        private void DeleteItemById(int itemId)
        {
            // SQL query to delete a row based on itemID
            string deleteQuery = "DELETE FROM dbo.autoitems WHERE itemID = @ItemID";

            // Create and open a connection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Create a command with parameters
                using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@ItemID", itemId);

                    // Execute the delete query
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Item deleted successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Item with specified ID not found. No rows deleted.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }



        public void LoadDataFromDatabase()
        {
            // SQL query to select all columns from the "dbo.autoitems" table
            string selectQuery = "SELECT * FROM dbo.autoitems";

            // Create and open a connection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Create a command
                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    // Create a SqlDataAdapter to fill a DataTable
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        // Create a DataTable to store the data
                        DataTable dataTable = new DataTable();

                        // Fill the DataTable with data from the database
                        adapter.Fill(dataTable);

                        // Clear the existing data source
                        dataGridView1.DataSource = null;

                        // Set the AutoSizeColumnsMode property to Fill
                        dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                        // Make all columns editable
                        foreach (DataGridViewColumn column in dataGridView1.Columns)
                        {
                            column.ReadOnly = false;  // Allow editing
                            column.SortMode = DataGridViewColumnSortMode.Automatic; // Enable sorting

                            // Adjust the width of the columns based on their names
                            AdjustColumnWidth(column);
                        }

                        // Remove these lines that make the DataGridView uneditable
                        // dataGridView1.Enabled = false;
                        // dataGridView1.ColumnHeadersVisible = false;

                        // Bind the DataTable to the DataGridView
                        dataGridView1.DataSource = dataTable;

                        // Set the ScrollBars property to enable both horizontal and vertical scrollbars
                        dataGridView1.ScrollBars = ScrollBars.Both;
                    }
                }
            }
        }



        private void AdjustColumnWidth(DataGridViewColumn column)
        {
            // Dictionary to map column names to desired widths
            Dictionary<string, int> columnWidths = new Dictionary<string, int>
    {
        { "itemID", 50 }, // Adjust the width based on your preference
        { "Name", 150 },  // Adjust the width based on your preference
        { "Category", 100 }, // Adjust the width based on your preference
        { "Price", 80 },   // Adjust the width based on your preference
        { "Stock", 80 },   // Adjust the width based on your preference
        { "Manufacturer", 150 } // Adjust the width based on your preference
        // Add more columns as needed
    };

            // Set the width based on the dictionary, or a default width if not found
            column.Width = columnWidths.ContainsKey(column.Name) ? columnWidths[column.Name] : 100;
        }




        private void dataGridView1_SelectionChanged(Object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
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
    }
}
