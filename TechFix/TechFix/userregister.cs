using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TechFix
{
    public partial class userregister : Form
    {
        private static readonly HttpClient client = new HttpClient();
        public userregister()
        {
            InitializeComponent();
            LoadData();
        }

        
        private void userregister_Load(object sender, EventArgs e)
        {
           
        }

        

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var login = new
                {
                    Username = usernametxt.Text,
                    Password = passwordtxt.Text,
                    UserType = usercombo.Text,
                    Email  = emailtxt.Text,
                    Contact = contacttxt.Text
                   
                };

                string json = JsonConvert.SerializeObject(login);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("http://localhost:50980/api/login", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Registration   successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                HttpResponseMessage response = await client.GetAsync("http://localhost:50980/api/login");
                if (response.IsSuccessStatusCode)
                {
                    string jsonData = await response.Content.ReadAsStringAsync();
                    List<Login> loginList = JsonConvert.DeserializeObject<List<Login>>(jsonData);

                    dataGridView1.DataSource = loginList; // Bind data to DataGridView
                }
                else
                {
                    MessageBox.Show("Failed to load Login.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        public class Login
        {
            public int id { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string usertype { get; set; }
            public string email { get; set; }
            public string contact { get; set; }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
           
        }

        private void logout_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();


            this.Close();


            clear loginWindow = new clear();
            loginWindow.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            AdminMenu st = new AdminMenu();
            st.Show();

            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
      
        private async void deletebtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);

                    HttpResponseMessage response = await client.DeleteAsync($"http://localhost:50980/api/login/{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Acount deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData(); // Refresh data after deletion
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete item!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please select an Account to delete!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            AdminMenu am = new AdminMenu();
            am.Show();
            this.Hide();
        }

        private  void button7_Click(object sender, EventArgs e)
        {
           
        }
    }
}
