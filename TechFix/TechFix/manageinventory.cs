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
using static TechFix.viewquotation;

namespace TechFix
{
    public partial class manageinventory : Form
    {
        private static readonly HttpClient client = new HttpClient();
        public manageinventory()
        {
            InitializeComponent();
           
           LoadInventory();
            LoadData();

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
        private async void LoadInventory()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost:50980/api/Inventory");
                if (response.IsSuccessStatusCode)
                {
                    string jsonData = await response.Content.ReadAsStringAsync();
                    List<Inventory> itemList = JsonConvert.DeserializeObject<List<Inventory>>(jsonData);

                    dataGridView1.DataSource = itemList; // Bind data to DataGridView

                    // Check for low stock items
                    var lowStockItems = itemList.Where(item => item.Quantity < 5).ToList();
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

        public class Inventory
        {
            public int Id { get; set; }

            public string ItemName { get; set; }
            public int Quantity { get; set; }

            public decimal Price { get; set; }

            public decimal Discount { get; set; }

            public string Supplier { get; set; }
            public DateTime Date { get; set; }

        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void manageinventory_Load(object sender, EventArgs e)
        {

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

                    dataGridView2.DataSource = itemList; // Bind data to DataGridView

                    // Check for low stock items
                   
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
        private  void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button9_Click(object sender, EventArgs e)
        {
            SupplierMenu supplierMenu = new SupplierMenu("YourUsernameHere");
            supplierMenu.Show();
            this.Hide();
        }
       

        
        private void button10_Click(object sender, EventArgs e)
        {
           
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            AdminMenu am = new AdminMenu();
            am.Show();

            this.Hide();
        }

        private async void button10_Click_1(object sender, EventArgs e)
        {
            try
            {
                var Inventory = new
                {
                    ItemName = itemtxt.Text,
                    Quantity = int.Parse(qtytxt.Text),
                    Price = decimal.Parse(txtprice.Text),
                    Discount = decimal.Parse(discounttxt.Text),
                    Supplier = supcom.Text
                };

                string json = JsonConvert.SerializeObject(Inventory);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("http://localhost:50980/api/Inventory", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Item inserted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadInventory();
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

        private async void button11_Click(object sender, EventArgs e)
        {

            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);

                    HttpResponseMessage response = await client.DeleteAsync($"http://localhost:50980/api/Inventory/{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Item deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadInventory(); 
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

        private async void button12_Click(object sender, EventArgs e)
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


                    var inventory = new
                    {
                        Id = id,
                        ItemName = itemtxt.Text.Trim(),
                        Quantity = quantity,
                        Price = txtprice.Text.Trim(),
                        Discount = discounttxt.Text.Trim(),
                        Supplier = supcom.Text.Trim(),
                       
                    };

                    string json = JsonConvert.SerializeObject(inventory);

                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync($"http://localhost:50980/api/Inventory/{id}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Inventory updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadInventory();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update quotation!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a quotation to edit!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void button13_Click(object sender, EventArgs e)
        {
            try
            {
                string itemName = searchtxt.Text.Trim(); // Get search input from textbox

                if (string.IsNullOrEmpty(itemName))
                {
                    MessageBox.Show("Please enter an item name to search.", "Warning", MessageBoxButtons.OK);
                    LoadInventory();
                    return;
                }


                // Make GET request to search API
                HttpResponseMessage response = await client.GetAsync($"http://localhost:50980/api/Inventory/search?itemName={Uri.EscapeDataString(itemName)}");

                if (response.IsSuccessStatusCode)
                {
                    string jsonData = await response.Content.ReadAsStringAsync();
                    List<Inventory> searchResults = JsonConvert.DeserializeObject<List<Inventory>>(jsonData);

                    // Bind results to DataGridView
                    dataGridView1.DataSource = searchResults;
                }
                else
                {
                    MessageBox.Show("No items found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
