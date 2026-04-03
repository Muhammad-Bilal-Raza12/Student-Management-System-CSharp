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
    public partial class RubricForm : Form
    {
        private int selectedRubricId = -1;

        public RubricForm()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            MainDashboard dash = new MainDashboard();
            dash.Show();
            this.Close();
        }

        private void RubricForm_Load(object sender, EventArgs e)
        {
            LoadClos();
            ShowRubrics();
            dgvRubrics.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRubrics.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRubrics.AllowUserToAddRows = false;
        }

        private void LoadClos()
        {
            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    string query = "SELECT Id, Name FROM Clo";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cmbClo.DataSource = dt;
                    cmbClo.DisplayMember = "Name";
                    cmbClo.ValueMember = "Id";
                    cmbClo.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading CLOs: " + ex.Message);
            }
        }

        private void ShowRubrics()
        {
            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    dgvRubrics.AlternatingRowsDefaultCellStyle.BackColor = Color.LightSteelBlue;

                    string query = "SELECT r.Id, r.Details, c.Name AS CloName FROM Rubric r JOIN Clo c ON r.CloId = c.Id";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvRubrics.DataSource = dt;

                    if (dgvRubrics.Columns.Contains("Id")) dgvRubrics.Columns["Id"].Visible = true;
                    dgvRubrics.Columns["Id"].HeaderText = "Rubric ID";
                    dgvRubrics.Columns["Details"].HeaderText = "Rubric Description";
                    dgvRubrics.Columns["CloName"].HeaderText = "Associated CLO";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private bool IsIdDuplicate(string id)
        {
            using (MySqlConnection conn = dbConnection.getInternalConnection())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Rubric WHERE Id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtId.Text) || string.IsNullOrEmpty(txtRubric.Text) || cmbClo.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill all fields including the ID");
                return;
            }

            if (!int.TryParse(txtId.Text, out _))
            {
                MessageBox.Show("ID must be a numeric value");
                return;
            }

            try
            {
                if (IsIdDuplicate(txtId.Text))
                {
                    MessageBox.Show("This ID already exists. Use a unique ID.");
                    return;
                }

                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO Rubric (Id, Details, CloId) VALUES (@id, @details, @cloId)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", txtId.Text);
                    cmd.Parameters.AddWithValue("@details", txtRubric.Text);
                    cmd.Parameters.AddWithValue("@cloId", cmbClo.SelectedValue);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Rubric Added");
                    ShowRubrics();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedRubricId == -1 || string.IsNullOrEmpty(txtRubric.Text) || cmbClo.SelectedIndex == -1)
            {
                MessageBox.Show("Select a rubric and fill all fields");
                return;
            }

            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    string query = "UPDATE Rubric SET Details=@details, CloId=@cloId WHERE Id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@details", txtRubric.Text);
                    cmd.Parameters.AddWithValue("@cloId", cmbClo.SelectedValue);
                    cmd.Parameters.AddWithValue("@id", selectedRubricId);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Rubric Updated");
                    ShowRubrics();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedRubricId == -1)
            {
                MessageBox.Show("Select a rubric to delete");
                return;
            }

            if (MessageBox.Show("Delete this rubric?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection conn = dbConnection.getInternalConnection())
                    {
                        conn.Open();
                        string query = "DELETE FROM Rubric WHERE Id=@id";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", selectedRubricId);

                        cmd.ExecuteNonQuery();
                        ShowRubrics();
                        ClearFields();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void dgvRubrics_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvRubrics.Rows[e.RowIndex];
                selectedRubricId = Convert.ToInt32(row.Cells["Id"].Value);
                txtId.Text = row.Cells["Id"].Value.ToString();
                txtRubric.Text = row.Cells["Details"].Value.ToString();
                cmbClo.Text = row.Cells["CloName"].Value.ToString();
                txtId.Enabled = false;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    string query = "SELECT r.Id, r.Details, c.Name AS CloName FROM Rubric r JOIN Clo c ON r.CloId = c.Id WHERE r.Details LIKE @search OR r.Id LIKE @search";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    da.SelectCommand.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvRubrics.DataSource = dt;
                    //
                }
            }
            catch 
            { 

            }
        }

        private void ClearFields()
        {
            txtId.Clear();
            txtId.Enabled = true;
            txtRubric.Clear();
            cmbClo.SelectedIndex = -1;
            selectedRubricId = -1;
        }
    }
}