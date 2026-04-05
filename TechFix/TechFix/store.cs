using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static TechFix.manageinventory;
using static TechFix.Quotation;

namespace TechFix
{
    public partial class store : Form
    {
        private string supplierUsername;
        private static readonly HttpClient client = new HttpClient();
      
        public store(string username)
        {
            InitializeComponent();
            supplierUsername = username;
            supp.Text = $"Welcome, {supplierUsername}";
            LoadData();


        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SupplierMenu form2 = new SupplierMenu("your_username");
            // Show Form2
            form2.Show();
            // Optionally, hide Form1
            this.Hide();
        }

        private void logout_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();


            this.Close();


            clear loginWindow = new clear();
            loginWindow.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            
            

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnGetAll_Click(object sender, EventArgs e)
        {

        }

        private void store_Load(object sender, EventArgs e)
        {

        }

        private async void insertbtn_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                var item = new
                {
                    ItemName = itemtxt.Text,
                    Quantity = int.Parse(qtytxt.Text),
                    Price = decimal.Parse(pricetxt.Text),
                    Discount = decimal.Parse(discounttxt.Text),
                    Supplier = comsup.Text
                };

                string json = JsonConvert.SerializeObject(item);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("http://localhost:50980/api/Item", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Item inserted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Failed to insert item!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void LoadData()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost:50980/api/Item");
                if (response.IsSuccessStatusCode)
                {
                    string jsonData = await response.Content.ReadAsStringAsync();
                    List<Item> itemList = JsonConvert.DeserializeObject<List<Item>>(jsonData);

                    // 🔥 Filter the items based on the logged-in supplier's username
                    var filteredItems = itemList.Where(item => item.Supplier == supplierUsername).ToList();

                    dataGridView1.DataSource = filteredItems; // Bind filtered data to DataGridView

                    // Check for low stock items for this supplier only
                    var lowStockItems = filteredItems.Where(item => item.Quantity < 5).ToList();
                    if (lowStockItems.Any())
                    {
                        string lowStockMessage = "Warning! The following items are low in stock:\n\n";
                        foreach (var item in lowStockItems)
                        {
                            lowStockMessage += $"{item.ItemName} - {item.Quantity} left\n";
                        }

                        MessageBox.Show(lowStockMessage, "Low Stock Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Failed to load items.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        public class Item
        {
            public int Id { get; set; }
            public string ItemName { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            public decimal Discount { get; set; }
            public string Supplier { get; set; }
        }
        private  void button4_Click_1(object sender, EventArgs e)
        {
           
        }

        private async void deletebtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);

                    HttpResponseMessage response = await client.DeleteAsync($"http://localhost:50980/api/Item/{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Item deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData(); // Refresh data after deletion
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete item!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please select an item to delete!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void updatebtn_Click(object sender, EventArgs e)
        {

            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);

                    // Validate Quantity
                    if (!int.TryParse(qtytxt.Text.Trim(), out int quantity))
                    {
                        MessageBox.Show("Please enter a valid number for Quantity.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var quotation = new
                    {
                        Id = id,
                        ItemName = itemtxt.Text.Trim(),
                        Quantity = quantity,
                        Price = pricetxt.Text.Trim(),
                        Supplier = comsup.Text.Trim(),
                        Discount = discounttxt.Text.Trim()
                    };

                    string json = JsonConvert.SerializeObject(quotation);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync($"http://localhost:50980/api/Item/{id}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Item updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData(); // Refresh DataGridView
                    }
                    else
                    {
                        MessageBox.Show("Failed to update Item!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a Item to edit!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

        }

        private void savebtn_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
            SupplierMenu am = new SupplierMenu(supplierUsername);
            am.Show();

            this.Hide();
        }

        private  void button1_Click_1(object sender, EventArgs e)
        {
            
        }
    }
    
}
