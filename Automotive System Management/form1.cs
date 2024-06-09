using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automotive_System_Management
{
    public partial class form1 : Form
    {
        Controls.itemsmanagement items = new Controls.itemsmanagement();
        Controls.searchfilter category = new Controls.searchfilter();
        Controls.recordsale bill = new Controls.recordsale();
        Controls.salesreport sales = new Controls.salesreport();
        public form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.FormClosing += ItemsManagement_FormClosing;
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


        private void button11_Click(object sender, EventArgs e)
        {
            panelMain.Controls.Clear();
            panelMain.Controls.Add(items);
            items.Dock = DockStyle.Fill;

            // Call the method to load and display data in the DataGridView
            items.LoadDataFromDatabase();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            panelMain.Controls.Clear();
            panelMain.Controls.Add(category);
            category.Dock = DockStyle.Fill;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            panelMain.Controls.Clear();
            panelMain.Controls.Add(bill);
            bill.Dock = DockStyle.Fill;
            bill.LoadDataFromDatabase();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            panelMain.Controls.Clear();
            panelMain.Controls.Add(sales);
            sales.Dock = DockStyle.Fill;
            sales.LoadDataFromDatabase();
        }

        private void label9_Click(object sender, EventArgs e)
        {
            // Detach the FormClosing event to avoid triggering it again
            this.FormClosing -= ItemsManagement_FormClosing;

            // Display a confirmation dialog
            DialogResult result = MessageBox.Show("Are you sure you want to exit?\nAny unsaved data may be lost.", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            // Check the user's response
            if (result == DialogResult.Yes)
            {
                // User clicked "Yes," so exit the application
                Login log1 = new Login();
                log1.Show();

                // Close the form without triggering the FormClosing event
                this.Close();
            }
            // If the user clicked "No," do nothing (don't close the form)
        }
    }
}