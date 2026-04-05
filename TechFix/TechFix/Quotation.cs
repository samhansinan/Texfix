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
    public partial class Quotation : Form
    {
        private static readonly HttpClient client = new HttpClient();
       
        public Quotation()
        {
            InitializeComponent();
            


        }

        private async void Quotation_Load(object sender, EventArgs e)
        {
            await LoadQuotations();
        }


        private async void insertbtn_Click(object sender, EventArgs e)
        {
            try
            {
                var quontity = new
                {
                    ItemName = producttxt.Text,
                    Quantity = int.Parse(qtytxt.Text),
                    Supplier = combosup.Text,
                    Description = richdes.Text
                };

                string json = JsonConvert.SerializeObject(quontity);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("http://localhost:50980/api/Quotation", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Quotation Requested successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadQuotations(); // Refresh DataGridView
                }
                else
                {
                    MessageBox.Show("Failed to insert Quotation!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async Task LoadQuotations()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost:50980/api/Quotation");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var quotations = JsonConvert.DeserializeObject<List<quotationfo>>(json);

                    dataGridView1.DataSource = quotations; // Set DataGridView source
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


        private void refreshbtn_Click(object sender, EventArgs e)
        {
           
           
        }

        private void button9_Click(object sender, EventArgs e)
        {
            AdminMenu form2 = new AdminMenu();
            // Show Form2
            form2.Show();
            // Optionally, hide Form1
            this.Hide();
        }

        public class quotationfo
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

        private async void deletebtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);

                    HttpResponseMessage response = await client.DeleteAsync($"http://localhost:50980/api/Quotation/{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Quotation deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await LoadQuotations(); // Refresh data after deletion
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete item!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please select an item to Quotation!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        ItemName = producttxt.Text.Trim(),
                        Quantity = quantity,
                        Supplier = combosup.Text.Trim(),
                        Description = richdes.Text.Trim()
                    };

                    string json = JsonConvert.SerializeObject(quotation);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync($"http://localhost:50980/api/Quotation/{id}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Quotation updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await LoadQuotations(); // Refresh DataGridView
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

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView1.SelectedRows[0];
                producttxt.Text = selectedRow.Cells["ItemName"].Value.ToString();
                qtytxt.Text = selectedRow.Cells["Quantity"].Value.ToString();
                combosup.Text = selectedRow.Cells["Supplier"].Value.ToString();
                richdes.Text = selectedRow.Cells["Description"].Value.ToString();
            }
        }

        private void savebtn_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            AdminMenu am = new AdminMenu();
            am.Show();

            this.Hide();
                
        }

        private async  void button1_Click(object sender, EventArgs e)
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
                HttpResponseMessage response = await client.GetAsync($"http://localhost:50980/api/Quotation/search?itemName={Uri.EscapeDataString(itemName)}");

                if (response.IsSuccessStatusCode)
                {
                    string jsonData = await response.Content.ReadAsStringAsync();
                    List<quotationfo> searchResults = JsonConvert.DeserializeObject<List<quotationfo>>(jsonData);

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

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);
                    string newStatus = conform.Text.Trim(); // Get the new status (Pending / Cancel)

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
                        await LoadQuotations();// Refresh DataGridView
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
