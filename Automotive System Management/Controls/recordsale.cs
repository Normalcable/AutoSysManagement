using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace Automotive_System_Management.Controls
{
    public partial class recordsale : UserControl
    {
        private string connectionString = @"Data Source=DESKTOP-OIEBO4N\SQL2019X;Initial Catalog=automotiveSys;Integrated Security=True;Encrypt=False; TrustServerCertificate=True";

        public recordsale()
        {
            InitializeComponent();
            dataGridView2.SelectionChanged += dataGridView2_SelectionChanged;
        }

        // Method to load and display data from the "dbo.autoitems" table
        public void LoadDataFromDatabase()
        {
            // SQL query to select all columns from the "dbo.autoitems" table
            string autoItemsSelectQuery = "SELECT * FROM dbo.autoitems";

            // SQL query to select all columns from the "dbo.recordsales" table
            string recordSalesSelectQuery = "SELECT * FROM dbo.recordsales";

            // Create and open a connection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Create a SqlDataAdapter to fill a DataTable for autoitems
                using (SqlDataAdapter autoItemsAdapter = new SqlDataAdapter(autoItemsSelectQuery, connection))
                {
                    // Create a DataTable to store the data for autoitems
                    DataTable autoItemsDataTable = new DataTable();

                    // Fill the DataTable with data from the database
                    autoItemsAdapter.Fill(autoItemsDataTable);

                    // Bind the DataTable to the first DataGridView (dataGridView1)
                    dataGridView1.DataSource = autoItemsDataTable;

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
                }

                // Create a SqlDataAdapter to fill a DataTable for recordsales
                using (SqlDataAdapter recordSalesAdapter = new SqlDataAdapter(recordSalesSelectQuery, connection))
                {
                    // Create a DataTable to store the data for recordsales
                    DataTable recordSalesDataTable = new DataTable();

                    // Fill the DataTable with data from the database
                    recordSalesAdapter.Fill(recordSalesDataTable);

                    // Bind the DataTable to the second DataGridView (dataGridView2)
                    dataGridView2.DataSource = recordSalesDataTable;

                    // Set the AutoSizeColumnsMode property to Fill
                    dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    // Make all columns editable
                    foreach (DataGridViewColumn column in dataGridView2.Columns)
                    {
                        column.ReadOnly = false;  // Allow editing
                        column.SortMode = DataGridViewColumnSortMode.Automatic; // Enable sorting

                        // Adjust the width of the columns based on their names
                        AdjustColumnWidth(column);
                    }

                    // Remove these lines that make the DataGridView uneditable
                    // dataGridView2.Enabled = false;
                }
            }

            // Calculate and display the total amount
            CalculateAndDisplayTotal();
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





        private void button2_Click(object sender, EventArgs e)
        {
            // Retrieve values from textboxes and dateTimePicker
            string customerName = textBox4.Text.Trim();
            string itemIDText = textBox2.Text.Trim();
            string quantityText = textBox1.Text.Trim();

            // Input validation
            if (string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(itemIDText) || string.IsNullOrEmpty(quantityText))
            {
                MessageBox.Show("Please enter all required information.");
                return;
            }

            if (!int.TryParse(itemIDText, out int itemID) || !int.TryParse(quantityText, out int quantity))
            {
                MessageBox.Show("Invalid ItemID or Quantity. Please enter valid numeric values.");
                return;
            }

            DateTime date = dateTimePicker1.Value;

            // Retrieve the selected row from dataGridView1 based on the entered itemID
            DataGridViewRow selectedRow = null;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["itemID"].Value != null && (int)row.Cells["itemID"].Value == itemID)
                {
                    selectedRow = row;
                    break;
                }
            }

            if (selectedRow != null)
            {
                try
                {
                    // Retrieve the price from the selected row
                    decimal price = Convert.ToDecimal(selectedRow.Cells["Price"].Value);

                    // Calculate the total price based on quantity and price
                    decimal totalPrice = quantity * price;

                    // Update the total TextBox with the Philippine Peso sign
                    textBox3.Text = "₱" + totalPrice.ToString("N2"); // "N2" formats the number with two decimal places

                    // SQL query to insert data into "dbo.recordsales" table
                    string insertQuery = "INSERT INTO dbo.recordsales (CustomerName, itemID, Quantity, Price, dateColumn) " +
                                         "VALUES (@CustomerName, @ItemID, @Quantity, @Price, @Date)";

                    // Create and open a connection
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Create a command
                        using (SqlCommand command = new SqlCommand(insertQuery, connection))
                        {
                            // Add parameters to the command
                            command.Parameters.AddWithValue("@CustomerName", customerName);
                            command.Parameters.AddWithValue("@ItemID", itemID);
                            command.Parameters.AddWithValue("@Quantity", quantity);
                            command.Parameters.AddWithValue("@Price", totalPrice); // Use the calculated total price here
                            command.Parameters.AddWithValue("@Date", date);

                            // Execute the query
                            command.ExecuteNonQuery();

                            // Display a message or perform additional actions if needed
                            MessageBox.Show("Record added successfully!");

                            // Reload data in the DataGridView after insertion
                            LoadDataFromDatabase();

                            // Calculate and display the total amount
                            CalculateAndDisplayTotal();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
                finally
                {
                    // Clear TextBoxes after successful insertion or error
                    textBox4.Clear();
                    textBox1.Clear();
                    textBox2.Clear();
                }
            }
            else
            {
                MessageBox.Show("Invalid ItemID. Please enter a valid ItemID.");
            }
        }




        private void button1_Click(object sender, EventArgs e)
        {
            // SQL query to delete all rows from "dbo.recordsales" table
            string deleteQuery = "DELETE FROM dbo.recordsales";

            // Create and open a connection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Create a command
                using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                {
                    // Execute the DELETE query
                    command.ExecuteNonQuery();

                    // Clear the total TextBox
                    textBox3.Text = string.Empty;

                    // Display a message or perform additional actions if needed
                    MessageBox.Show("All records deleted successfully!");

                    // Reload data in the DataGridView after deletion
                    LoadDataFromDatabase();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Retrieve user input from textboxes
            string customerNameToDelete = textBox4.Text;
            int itemIDToDelete;

            // Check if itemID is a valid integer
            bool isItemIDValid = int.TryParse(textBox2.Text, out itemIDToDelete);

            // SQL query to delete rows from "dbo.recordsales" table based on user input
            string deleteQuery = "DELETE FROM dbo.recordsales WHERE ";

            // Use parameters to avoid SQL injection
            List<SqlParameter> parameters = new List<SqlParameter>();

            // Add conditions based on user input
            if (!string.IsNullOrEmpty(customerNameToDelete))
            {
                deleteQuery += "CustomerName = @CustomerName";
                parameters.Add(new SqlParameter("@CustomerName", customerNameToDelete));
            }

            if (isItemIDValid)
            {
                if (!string.IsNullOrEmpty(customerNameToDelete))
                {
                    deleteQuery += " AND ";
                }
                deleteQuery += "ItemID = @ItemID";
                parameters.Add(new SqlParameter("@ItemID", itemIDToDelete));
            }

            // If both CustomerName and ItemID are empty, do not execute the query
            if (parameters.Count == 0)
            {
                MessageBox.Show("Please enter CustomerName, ItemID, or both to delete records.");
                return;
            }

            // Create and open a connection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Create a command
                using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                {
                    // Add parameters to the command
                    command.Parameters.AddRange(parameters.ToArray());

                    // Execute the DELETE query
                    int rowsAffected = command.ExecuteNonQuery();

                    // Display a message or perform additional actions based on the result
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Records deleted successfully!");
                    }
                    else
                    {
                        MessageBox.Show("No records found for the specified criteria.");
                    }

                    // Reload data in the DataGridView after deletion
                    LoadDataFromDatabase();

                    // Check if DataGridView2 is empty, then clear the total amount TextBox
                    if (dataGridView2.Rows.Count == 0)
                    {
                        textBox3.Clear();
                    }

                    textBox4.Clear();
                    textBox1.Clear();
                    textBox2.Clear();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Start a transaction to ensure atomicity (either all operations succeed or fail)
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        // Check if the row is not null and has valid values
                        if (row.Cells["itemID"].Value != null && row.Cells["Quantity"].Value != null && row.Cells["Price"].Value != null &&
                            row.Cells["CustomerName"].Value != null && row.Cells["dateColumn"].Value != null)
                        {
                            int itemID = Convert.ToInt32(row.Cells["itemID"].Value);
                            int quantitySold = Convert.ToInt32(row.Cells["Quantity"].Value);
                            decimal price = Convert.ToDecimal(row.Cells["Price"].Value);
                            string customerName = row.Cells["CustomerName"].Value.ToString();
                            DateTime dateColumn = Convert.ToDateTime(row.Cells["dateColumn"].Value);

                            // Insert into dbo.salesreport
                            using (SqlCommand insertCmd = new SqlCommand("INSERT INTO dbo.salesreport (itemID, CustomerName, Quantity, Price, dateColumn) VALUES (@ItemID, @CustomerName, @Quantity, @Price, @DateColumn)", connection, transaction))
                            {
                                insertCmd.Parameters.AddWithValue("@ItemID", itemID);
                                insertCmd.Parameters.AddWithValue("@CustomerName", customerName);
                                insertCmd.Parameters.AddWithValue("@Quantity", quantitySold);
                                insertCmd.Parameters.AddWithValue("@Price", price);
                                insertCmd.Parameters.AddWithValue("@DateColumn", dateColumn);

                                insertCmd.ExecuteNonQuery();
                            }

                            // Update stock quantity in dbo.autoitems
                            UpdateStockQuantity(connection, transaction, itemID, quantitySold);
                        }
                    }

                    // Commit the transaction if all operations succeed
                    transaction.Commit();

                    MessageBox.Show("Transaction recorded successfully");

                    // Reload data in the DataGridView after the transaction
                    LoadDataFromDatabase();
                    CalculateAndDisplayTotal();
                }
                catch (Exception ex)
                {
                    // Rollback the transaction if any exception occurs
                    transaction.Rollback();
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }





        private void UpdateStockQuantity(SqlConnection connection, SqlTransaction transaction, int itemID, int quantitySold)
        {
            // SQL query to update stock quantity in dbo.autoitems
            string updateQuery = "UPDATE dbo.autoitems SET Stock = Stock - @Quantity WHERE itemID = @ItemID";

            using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection, transaction))
            {
                // Add parameters for the UPDATE query
                updateCommand.Parameters.AddWithValue("@ItemID", itemID);
                updateCommand.Parameters.AddWithValue("@Quantity", quantitySold);

                // Execute the UPDATE query
                updateCommand.ExecuteNonQuery();
            }
        }



        private void CalculateAndDisplayTotal()
        {
            decimal totalAmount = 0;

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.Cells["Price"].Value != null)
                {
                    totalAmount += Convert.ToDecimal(row.Cells["Price"].Value);
                }
            }

            textBox3.Text = "₱" + totalAmount.ToString("N2");
        }








        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView2.ClearSelection();
        }

    }
}
