using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Automotive_System_Management.Controls
{
    public partial class salesreport : UserControl
    {
        private string connectionString = @"Data Source=DESKTOP-OIEBO4N\SQL2019X;Initial Catalog=automotiveSys;Integrated Security=True;Encrypt=False; TrustServerCertificate=True";

        public salesreport()
        {
            InitializeComponent();
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            LoadData();

            // Set properties for DataGridView
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Columns fill the available width
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right; // Adjust according to your layout
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM dbo.salesreport"; // Replace with your actual query

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        dataGridView1.DataSource = dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public void LoadDataFromDatabase()
        {
            // SQL query to select all columns from the "dbo.autoitems" table
            string selectQuery = "SELECT * FROM dbo.salesreport";

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




        private void dataGridView1_SelectionChanged(Object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
        }
    }
}
