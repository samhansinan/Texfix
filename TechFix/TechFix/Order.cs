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

namespace TechFix
{
    public partial class Order : Form
    {
        private static readonly HttpClient client = new HttpClient();
        public Order()
        {
            InitializeComponent();

            LoadOrder();
            ProductData();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private async void LoadOrder()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost:50980/api/Order");
                if (response.IsSuccessStatusCode)
                {
                    string jsonData = await response.Content.ReadAsStringAsync();
                    List<Ordes> itemList = JsonConvert.DeserializeObject<List<Ordes>>(jsonData);

                    dataGridView1.DataSource = itemList; // Bind data to DataGridView
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


        private async void button6_Click(object sender, EventArgs e)
        {
            try
            {
                var order = new
                {
                    ItemId = int.Parse(idtxt.Text),
                    ItemName = nametxt.Text,
                    Quantity = int.Parse(qtytxt.Text),
                    Supplier = supcom.Text,
                    Status = statuscom.Text
                };

                string json = JsonConvert.SerializeObject(order);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("http://localhost:50980/api/Order", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Item Ordered successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadOrder();
                }
                else
                {
                    MessageBox.Show("Failed to insert Orders!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void button7_Click(object sender, EventArgs e)
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
                        ItemId = idtxt.Text.Trim(),
                        ItemName = nametxt.Text.Trim(),
                        Quantity = quantity,
                        Supplier = supcom.Text.Trim(),
                        Status = statuscom.Text.Trim()
                    };

                    string json = JsonConvert.SerializeObject(quotation);
                   
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync($"http://localhost:50980/api/Order/{id}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Order updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                         LoadOrder(); // Refresh DataGridView
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

        public class Ordes
        {
           public string Id { get; set; }
            public int ItemId { get; set; }

            public string ItemName { get; set; }
            public int Quantity { get; set; }
          
            public string Supplier { get; set; }

            public string Status { get; set; }
            public DateTime Date { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AdminMenu adminMenu = new AdminMenu();
            adminMenu.Show();

            this.Hide();
        }

        private async void button8_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);

                    HttpResponseMessage response = await client.DeleteAsync($"http://localhost:50980/api/Order/{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Order deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadOrder(); // Refresh data after deletion
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete Order!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please select an Order to delete!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void ProductData()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost:50980/api/Item");
                if (response.IsSuccessStatusCode)
                {
                    string jsonData = await response.Content.ReadAsStringAsync();
                    List<Item> itemList = JsonConvert.DeserializeObject<List<Item>>(jsonData);

                    dataGridView2.DataSource = itemList; // Bind data to DataGridView
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
        private void Order_Load(object sender, EventArgs e)
        {

        }

        private async void button9_Click(object sender, EventArgs e)
        {
            try
            {
                string itemName = searchtxt.Text.Trim(); // Get search input from textbox

                if (string.IsNullOrEmpty(itemName))
                {
                    MessageBox.Show("Please enter an item name to search.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Make GET request to search API
                HttpResponseMessage response = await client.GetAsync($"http://localhost:50980/api/Order/search?itemName={Uri.EscapeDataString(itemName)}");

                if (response.IsSuccessStatusCode)
                {
                    string jsonData = await response.Content.ReadAsStringAsync();
                    List<Ordes> searchResults = JsonConvert.DeserializeObject<List<Ordes>>(jsonData);

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

        private void button10_Click(object sender, EventArgs e)
        {
            LoadOrder();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            AdminMenu am = new AdminMenu();
            am.Show();

            this.Hide();
        }


        private async void button1_Click_1(object sender, EventArgs e)
        {
              try
            {
                string itemName = search.Text.Trim(); // Get search input from textbox

                if (string.IsNullOrEmpty(itemName))
                {
                    MessageBox.Show("Please enter an item name to search.", "Warning", MessageBoxButtons.OK);
                   // await LoadQuotations();
                    return;
                }

                // Make GET request to search API
                HttpResponseMessage response = await client.GetAsync($"http://localhost:50980/api/Item/search?itemName={Uri.EscapeDataString(itemName)}");

                if (response.IsSuccessStatusCode)
                {
                    string jsonData = await response.Content.ReadAsStringAsync();
                    List<Item> searchResults = JsonConvert.DeserializeObject<List<Item>>(jsonData);

                    // Bind results to DataGridView
                    dataGridView2.DataSource = searchResults;
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
