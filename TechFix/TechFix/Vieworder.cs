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
using static TechFix.Order;

namespace TechFix
{
    public partial class Vieworder : Form
    {
        private string supplierUsername;
        private static readonly HttpClient client = new HttpClient();
        public Vieworder(string username)
        {
            InitializeComponent();
            supplierUsername = username;
            supp.Text = $"Welcome, {supplierUsername}";
            LoadOrder();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            SupplierMenu am = new SupplierMenu(supplierUsername);
            am.Show();

            this.Hide();
        }

        private async void LoadOrder()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost:50980/api/Order");
                if (response.IsSuccessStatusCode)
                {
                    string jsonData = await response.Content.ReadAsStringAsync();
                    List<Ordes> orderList = JsonConvert.DeserializeObject<List<Ordes>>(jsonData);

                    // 🔥 Filter the orders by the logged-in supplier's username
                    var filteredOrders = orderList.Where(order => order.Supplier == supplierUsername).ToList();

                    dataGridView1.DataSource = filteredOrders; // Bind filtered data to DataGridView
                }
                else
                {
                    MessageBox.Show("Failed to load orders.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private void Vieworder_Load(object sender, EventArgs e)
        {

        }


        private async void button1_Click(object sender, EventArgs e)
        {
           try
{
    if (dataGridView1.SelectedRows.Count > 0)
    {
        int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);
        string newStatus = comsup.Text.Trim(); // Get the new status (Pending / Cancel)

        var statusUpdate = new { Status = newStatus };
        string json = JsonConvert.SerializeObject(statusUpdate);
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Create PATCH request manually since PatchAsync is not available in .NET Framework
         var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"http://localhost:50980/api/Order/{id}/status")
         {
             Content = content
         };


                    HttpResponseMessage response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            MessageBox.Show("Order status updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadOrder(); // Refresh DataGridView
        }
        else
        {
            string errorMessage = await response.Content.ReadAsStringAsync();
            MessageBox.Show($"Failed to update order status! Server Response: {errorMessage}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    else
    {
        MessageBox.Show("Please select an order to update the status!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
catch (HttpRequestException httpEx)
{
    MessageBox.Show($"HTTP Error: {httpEx.Message}", "Network Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
catch (Exception ex)
{
    MessageBox.Show($"Unexpected Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
}


        }
    }
}
