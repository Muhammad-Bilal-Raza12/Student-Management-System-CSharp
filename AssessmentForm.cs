using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace MidDB26_2025CS10.UI
{
    public partial class AssessmentForm : Form
    {
        private int selectedAssessmentId = -1;

        public AssessmentForm()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            MainDashboard dash = new MainDashboard();
            dash.Show();
            this.Close();
        }

        private void AssessmentForm_Load(object sender, EventArgs e)
        {
            ShowAssessments();
            dgvAssessments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAssessments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAssessments.AllowUserToAddRows = false;
        }

        private void ShowAssessments()
        {
            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    string query = "SELECT Id, Title, DateCreated, TotalMarks, TotalWeightage FROM Assessment";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvAssessments.DataSource = dt;
                    if (dgvAssessments.Columns.Contains("Id")) dgvAssessments.Columns["Id"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTitle.Text) || string.IsNullOrEmpty(txtTotalMarks.Text) || string.IsNullOrEmpty(txtWeightage.Text))
            {
                MessageBox.Show("Please fill all fields");
                return;
            }

            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO Assessment (Title, DateCreated, TotalMarks, TotalWeightage) VALUES (@title, @date, @marks, @weightage)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@title", txtTitle.Text);
                    cmd.Parameters.AddWithValue("@date", dtpDate.Value);
                    cmd.Parameters.AddWithValue("@marks", txtTotalMarks.Text);
                    cmd.Parameters.AddWithValue("@weightage", txtWeightage.Text);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Assessment Added Successfully");
                    ShowAssessments();
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
            if (selectedAssessmentId == -1)
            {
                MessageBox.Show("Select an assessment to update");
                return;
            }

            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    string query = "UPDATE Assessment SET Title=@title, DateCreated=@date, TotalMarks=@marks, TotalWeightage=@weightage WHERE Id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@title", txtTitle.Text);
                    cmd.Parameters.AddWithValue("@date", dtpDate.Value);
                    cmd.Parameters.AddWithValue("@marks", txtTotalMarks.Text);
                    cmd.Parameters.AddWithValue("@weightage", txtWeightage.Text);
                    cmd.Parameters.AddWithValue("@id", selectedAssessmentId);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Updated Successfully");
                    ShowAssessments();
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
            if (selectedAssessmentId == -1)
            {
                MessageBox.Show("Select an assessment to delete");
                return;
            }

            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM Assessment WHERE Id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", selectedAssessmentId);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Deleted Successfully");
                    ShowAssessments();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvAssessments_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvAssessments.Rows[e.RowIndex];
                selectedAssessmentId = Convert.ToInt32(row.Cells["Id"].Value);
                txtTitle.Text = row.Cells["Title"].Value.ToString();
                dtpDate.Value = Convert.ToDateTime(row.Cells["DateCreated"].Value);
                txtTotalMarks.Text = row.Cells["TotalMarks"].Value.ToString();
                txtWeightage.Text = row.Cells["TotalWeightage"].Value.ToString();
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    string query = "SELECT Id, Title, DateCreated, TotalMarks, TotalWeightage FROM Assessment WHERE Title LIKE @search";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    da.SelectCommand.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvAssessments.DataSource = dt;
                }
            }
            catch { }
        }

        private void ClearFields()
        {
            txtTitle.Clear();
            txtTotalMarks.Clear();
            txtWeightage.Clear();
            dtpDate.Value = DateTime.Now;
            selectedAssessmentId = -1;
        }
    }
}