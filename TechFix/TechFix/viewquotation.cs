using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static TechFix.Quotation;

namespace TechFix
{
    public partial class viewquotation : Form
    {
        private string supplierUsername;
        private static readonly HttpClient client = new HttpClient();
        public viewquotation(string username)
        {
            InitializeComponent();
            supplierUsername = username;
            supp.Text = $"Welcome, {supplierUsername}";
            LoadQuotations();
        }

        private  void Quotation_Load(object sender, EventArgs e)
        {
          
        }
        private void button1_Click(object sender, EventArgs e)
        {
            SupplierMenu menu = new SupplierMenu(supplierUsername);
            menu.Show();

            this.Hide();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void LoadQuotations()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost:50980/api/Quotation");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var quotations = JsonConvert.DeserializeObject<List<quotation>>(json);

                    // 🔥 Filter the quotations by the logged-in supplier's username
                    var filteredQuotations = quotations.Where(q => q.Supplier == supplierUsername).ToList();

                    dataGridView1.DataSource = filteredQuotations; // Set DataGridView source to filtered quotations
                }
                else
                {
                    MessageBox.Show("Failed to retrieve data!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
        public class quotation
        {
            public int Id { get; set; }  // Ensure your API returns an ID
            public string ItemName { get; set; }
            public int Quantity { get; set; }
            public string Supplier { get; set; }

        
            public string Description { get; set; }
            
            public string Status { get; set; }
            public string ResponseInfo { get; set; }

            public string ResponseStatus { get; set; }
            public DateTime Date { get; set; }
        }



        private async  void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);
                    string newStatus = supcom.Text.Trim(); // Get the new status (Pending / Cancel)

                    var statusUpdate = new { Status = newStatus };
                    string json = JsonConvert.SerializeObject(statusUpdate);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Create PATCH request manually since PatchAsync is not available in .NET Framework
                    var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"http://localhost:50980/api/Quotation/{id}/status")
                    {
                        Content = content
                    };


                    HttpResponseMessage response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Order status updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadQuotations();// Refresh DataGridView
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

        private void updatebtn_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            SupplierMenu sm = new SupplierMenu(supplierUsername);
            sm.Show();
            this.Hide();
        }

        private void viewquotation_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private async void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);
                    string responseInfo = resdes.Text.Trim(); // Get text from textbox
                    string responseStatus = resupdate.Text.Trim(); // Get text from combo box

                    var responseUpdate = new { ResponseInfo = responseInfo, ResponseStatus = responseStatus };

                    string json = JsonConvert.SerializeObject(responseUpdate);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Create PATCH request manually
                    var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"http://localhost:50980/api/Quotation/{id}/response")
                    {
                        Content = content
                    };

                    HttpResponseMessage response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Response updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadQuotations(); // Refresh DataGridView
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Failed to update response! Server Response: {errorMessage}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a quotation to update the response!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
