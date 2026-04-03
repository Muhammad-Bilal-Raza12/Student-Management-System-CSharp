using MySql.Data.MySqlClient;
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
    public partial class CloForm : Form
    {
        private int selectedCloId = -1;

        public CloForm()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            MainDashboard dash = new MainDashboard();
            dash.Show();
            this.Close();
        }

        private void CloForm_Load(object sender, EventArgs e)
        {
            ShowClos();
        }

        private void ShowClos()
        {
            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    dgvClos.AlternatingRowsDefaultCellStyle.BackColor = Color.LightSteelBlue;
                    string query = "SELECT Id, Name, DateCreated, DateUpdated FROM Clo";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvClos.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCloName.Text))
            {
                MessageBox.Show("Please enter CLO Name");
                return;
            }

            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();

                    string check = "SELECT COUNT(*) FROM Clo WHERE Name = @name";
                    MySqlCommand checkCmd = new MySqlCommand(check, conn);
                    checkCmd.Parameters.AddWithValue("@name", txtCloName.Text);
                    if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                    {
                        MessageBox.Show("CLO already exists!");
                        return;
                    }

                    string query = "INSERT INTO Clo (Name, DateCreated, DateUpdated) VALUES (@name, @date, @date)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", txtCloName.Text);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Added successfully");
                    ShowClos();
                    txtCloName.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedCloId == -1 || string.IsNullOrEmpty(txtCloName.Text))
            {
                MessageBox.Show("Please select a CLO to update");
                return;
            }

            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    string query = "UPDATE Clo SET Name=@name, DateUpdated=@date WHERE Id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", txtCloName.Text);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);
                    cmd.Parameters.AddWithValue("@id", selectedCloId);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Updated successfully");
                    ShowClos();
                    txtCloName.Clear();
                    selectedCloId = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedCloId == -1)
            {
                MessageBox.Show("Please select a CLO to delete");
                return;
            }

            if (MessageBox.Show("Are you sure?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection conn = dbConnection.getInternalConnection())
                    {
                        conn.Open();
                        string query = "DELETE FROM Clo WHERE Id=@id";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", selectedCloId);
                        cmd.ExecuteNonQuery();

                        ShowClos();
                        txtCloName.Clear();
                        selectedCloId = -1;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void dgvClos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvClos.Rows[e.RowIndex];
                selectedCloId = Convert.ToInt32(row.Cells["Id"].Value);
                txtCloName.Text = row.Cells["Name"].Value.ToString();
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Clo WHERE Name LIKE @search";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    da.SelectCommand.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvClos.DataSource = dt;
                }
            }
            catch { }
        }
    }
}