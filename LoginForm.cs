using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MidDB26_2025CS10.UI
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if(txtRegNo.Text=="Admin" && txtPassword.Text=="123")
            {
                UI.MainDashboard dash = new UI.MainDashboard();
                dash.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Wrong username or password ! try Admin and 123");
            }
        }
    }
}
