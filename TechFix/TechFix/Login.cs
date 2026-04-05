using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Net.Http;
using Newtonsoft.Json;
using static TechFix.userregister;
namespace TechFix
{
    public partial class clear : Form
    {
        private static readonly HttpClient client = new HttpClient();
        public clear()
        {
            InitializeComponent();
        }

       

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void txtusername_TextChanged(object sender, EventArgs e)
        {

        }

        private async void loginbtn_Click(object sender, EventArgs e)
        {
            try
            {
                var login = new
                {
                    Username = txtusername.Text,
                    Password = txtpassword.Text
                };

                string json = JsonConvert.SerializeObject(login);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("http://localhost:50980/api/login/validate", content);

                if (response.IsSuccessStatusCode)
                {
                    string jsonData = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<LoginResponse>(jsonData);

                    if (user != null)
                    {
                        if (user.usertype == "Admin")
                        {
                            AdminMenu adminMenu = new AdminMenu();
                            adminMenu.Show();
                            this.Hide();
                        }
                        else if (user.usertype == "Supplier")
                        {
                            SupplierMenu supplierPage = new SupplierMenu(user.username); // Pass username
                            supplierPage.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Invalid user type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Invalid username or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }


        public class LoginResponse
        {
            public int id { get; set; }
            public string username { get; set; }
            public string usertype { get; set; }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            txtusername.Clear();
            txtpassword.Clear();

            txtusername.Focus();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            userregister form2 = new userregister();
            form2.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
