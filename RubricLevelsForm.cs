using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace MidDB26_2025CS10.UI
{
    public partial class RubricLevelsForm : Form
    {
        private int selectedLevelId = -1;

        public RubricLevelsForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainDashboard dash = new MainDashboard();
            dash.Show();
            this.Close();
        }

        private void RubricLevelsForm_Load(object sender, EventArgs e)
        {
            LoadRubrics();
            ShowLevels();
            dgvLevels.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLevels.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLevels.AllowUserToAddRows = false;
        }

        private void LoadRubrics()
        {
            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    string query = "SELECT Id, Details FROM Rubric";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cmbRubric.DataSource = dt;
                    cmbRubric.DisplayMember = "Details";
                    cmbRubric.ValueMember = "Id";
                    cmbRubric.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ShowLevels()
        {
            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    string query = @"SELECT rl.Id, r.Details AS RubricName, rl.Details, rl.MeasurementLevel 
                                     FROM RubricLevel rl 
                                     JOIN Rubric r ON rl.RubricId = r.Id";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvLevels.DataSource = dt;

                    if (dgvLevels.Columns.Contains("Id")) dgvLevels.Columns["Id"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cmbRubric.SelectedIndex == -1 || string.IsNullOrEmpty(txtDetails.Text) || string.IsNullOrEmpty(cmbMeasurement.Text))
            {
                MessageBox.Show("Please fill all fields");
                return;
            }

            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO RubricLevel (RubricId, Details, MeasurementLevel) VALUES (@rid, @det, @lvl)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@rid", cmbRubric.SelectedValue);
                    cmd.Parameters.AddWithValue("@det", txtDetails.Text);
                    cmd.Parameters.AddWithValue("@lvl", cmbMeasurement.Text);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Rubric Level Added");
                    ShowLevels();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedLevelId == -1 || string.IsNullOrEmpty(txtDetails.Text))
            {
                MessageBox.Show("Select a record to update");
                return;
            }

            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    string query = "UPDATE RubricLevel SET RubricId=@rid, Details=@det, MeasurementLevel=@lvl WHERE Id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@rid", cmbRubric.SelectedValue);
                    cmd.Parameters.AddWithValue("@det", txtDetails.Text);
                    cmd.Parameters.AddWithValue("@lvl", cmbMeasurement.Text);
                    cmd.Parameters.AddWithValue("@id", selectedLevelId);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Updated Successfully");
                    ShowLevels();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedLevelId == -1)
            {
                MessageBox.Show("Select a record to delete");
                return;
            }

            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM RubricLevel WHERE Id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", selectedLevelId);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Deleted Successfully");
                    ShowLevels();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvLevels_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvLevels.Rows[e.RowIndex];
                selectedLevelId = Convert.ToInt32(row.Cells["Id"].Value);
                txtDetails.Text = row.Cells["Details"].Value.ToString();
                cmbRubric.Text = row.Cells["RubricName"].Value.ToString();
                cmbMeasurement.Text = row.Cells["MeasurementLevel"].Value.ToString();
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    string query = @"SELECT rl.Id, r.Details AS RubricName, rl.Details, rl.MeasurementLevel 
                                     FROM RubricLevel rl 
                                     JOIN Rubric r ON rl.RubricId = r.Id 
                                     WHERE rl.Details LIKE @search OR r.Details LIKE @search";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    da.SelectCommand.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvLevels.DataSource = dt;
                }
            }
            catch { }
        }

        private void ClearFields()
        {
            txtDetails.Clear();
            cmbRubric.SelectedIndex = -1;
            cmbMeasurement.SelectedIndex = -1;
            selectedLevelId = -1;
        }
    }
}