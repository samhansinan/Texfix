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

namespace TechFix
{
    public partial class Salesreport : Form
    {
        private string supplierUsername;
        private static readonly HttpClient client = new HttpClient();
        public Salesreport(string username)
        {
            InitializeComponent();
            supplierUsername = username;
            supp.Text = $"Welcome, {supplierUsername}";
            LoadOrder();
        }

        private void Salesreport_Load(object sender, EventArgs e)
        {

        }

        private async void LoadOrder()
        {
            try
            {
                HttpResponseMessage orderResponse = await client.GetAsync("http://localhost:50980/api/Order");
                HttpResponseMessage itemResponse = await client.GetAsync("http://localhost:50980/api/Item");

                if (orderResponse.IsSuccessStatusCode && itemResponse.IsSuccessStatusCode)
                {
                    string orderJson = await orderResponse.Content.ReadAsStringAsync();
                    string itemJson = await itemResponse.Content.ReadAsStringAsync();

                    List<Ordes> orderList = JsonConvert.DeserializeObject<List<Ordes>>(orderJson);
                    List<Item> itemList = JsonConvert.DeserializeObject<List<Item>>(itemJson);

                    // Join Orders with Items based on ItemId and filter by supplier username
                    var enrichedOrders = (from order in orderList
                                          join item in itemList on order.ItemId equals item.Id
                                          where order.Status.Equals("Delivered", StringComparison.OrdinalIgnoreCase) // Filter only delivered orders
                                          && item.Supplier == supplierUsername // Filter orders by logged-in supplier
                                          select new
                                          {
                                              order.Id,
                                              order.ItemId,
                                              order.ItemName,
                                              order.Quantity,
                                              order.Status,
                                              order.Date,
                                              item.Price, // Add Price from Item table
                                              Total = order.Quantity * item.Price // Calculate total per order
                                          }).ToList();

                    dataGridView1.DataSource = enrichedOrders; // Bind filtered data to DataGridView

                    // Calculate and display total sum
                    decimal totalSum = enrichedOrders.Sum(order => order.Total);
                    total.Text = $"LKR {totalSum:N2}";
                }
                else
                {
                    MessageBox.Show("Failed to load orders or items.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


        public class Ordes
        {
            public string Id { get; set; }
            public int ItemId { get; set; }

            public string ItemName { get; set; }
            public int Quantity { get; set; }

            

            public string Status { get; set; }
            public DateTime Date { get; set; }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            SupplierMenu supplierMenu = new SupplierMenu(supplierUsername);
            supplierMenu.Show();
            this.Close();
        }
    }
}
