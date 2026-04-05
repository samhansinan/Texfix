using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TechFix
{
    public partial class AdminMenu : Form
    {
        public AdminMenu()
        {
            InitializeComponent();
        }

        private void logout_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();

            
            this.Close();

            
          clear loginWindow = new clear();
            loginWindow.Show();
        }

        private void AdminMenu_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            userregister st = new userregister();
            st.Show();

            this.Hide();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            userregister st = new userregister();
            st.Show();

            this.Hide();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            manageinventory mi = new manageinventory();
            mi.Show();
            this.Hide();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Order order = new Order();
            order.Show();
            this.Hide();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Quotation am = new Quotation();
            am.Show();

            this.Hide();
        }
    }
}
