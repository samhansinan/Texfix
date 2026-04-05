using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace TechFix
{
    public partial class SupplierMenu : Form
    {
        private string loggedInUsername;
        public SupplierMenu(string username)
        {
            InitializeComponent();
            loggedInUsername = username;
            supp.Text = "Welcome, " + loggedInUsername;

        }


        private void button3_Click(object sender, EventArgs e)
        {
            store st = new store(loggedInUsername);
            st.Show();

            this.Hide();
        }

        private void logout_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();


            this.Close();


            clear loginWindow = new clear();
            loginWindow.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            viewquotation viewquotation = new viewquotation(loggedInUsername);
            viewquotation.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Vieworder vo =new Vieworder(loggedInUsername);
            vo.Show();
            this.Hide();
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            Salesreport sr = new Salesreport(loggedInUsername);
            sr.Show();
            this.Hide();
        }

        private void SupplierMenu_Load(object sender, EventArgs e)
        {
            supp.Text = "Welcome, " + loggedInUsername;

        }
    }
}
