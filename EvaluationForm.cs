using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace MidDB26_2025CS10.UI
{
    public partial class EvaluationForm : Form
    {
        public EvaluationForm()
        {
            InitializeComponent();
        }

        private void EvaluationForm_Load(object sender, EventArgs e)
        {
            LoadAssessmentComponents();
            SetupGrid();
        }

        private void LoadAssessmentComponents()
        {
            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    string query = "SELECT Id, Name FROM assessmentcomponent";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    cmbAssessmentComponent.DataSource = dt;
                    cmbAssessmentComponent.DisplayMember = "Name";
                    cmbAssessmentComponent.ValueMember = "Id";
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void SetupGrid()
        {
            dgvEvaluation.AutoGenerateColumns = false;
            dgvEvaluation.Columns.Clear();

            dgvEvaluation.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", Name = "StudentId", Visible = false });
            dgvEvaluation.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RegistrationNumber", HeaderText = "Reg No", ReadOnly = true });
            dgvEvaluation.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FirstName", HeaderText = "Name", ReadOnly = true });

            DataGridViewComboBoxColumn rubricCol = new DataGridViewComboBoxColumn();
            rubricCol.HeaderText = "Select Rubric Level";
            rubricCol.Name = "colRubricLevel";
            dgvEvaluation.Columns.Add(rubricCol);

            dgvEvaluation.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvEvaluation.AllowUserToAddRows = false;

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    int componentId = Convert.ToInt32(cmbAssessmentComponent.SelectedValue);

                    
                    string query = @"SELECT s.Id, s.RegistrationNumber, s.FirstName, sr.RubricMeasurementId 
                           FROM student s 
                           LEFT JOIN studentresult sr ON s.Id = sr.StudentId 
                           AND sr.AssessmentComponentId = @aid 
                           WHERE s.Status = 5";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@aid", componentId);

                    MySqlDataAdapter daStd = new MySqlDataAdapter(cmd);
                    DataTable dtStd = new DataTable();
                    daStd.Fill(dtStd);
                    dgvEvaluation.DataSource = dtStd;

                    MySqlDataAdapter daRub = new MySqlDataAdapter("SELECT Id, Details FROM rubriclevel", conn);
                    DataTable dtRub = new DataTable();
                    daRub.Fill(dtRub);
                    DataGridViewComboBoxColumn combo = (DataGridViewComboBoxColumn)dgvEvaluation.Columns["colRubricLevel"];
                    combo.DataSource = dtRub;
                    combo.DisplayMember = "Details";
                    combo.ValueMember = "Id";

                   
                    combo.DataPropertyName = "RubricMeasurementId";
                }
            }
            catch (Exception ex)
            { 

            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    int componentId = Convert.ToInt32(cmbAssessmentComponent.SelectedValue);

                    foreach (DataGridViewRow row in dgvEvaluation.Rows)
                    {
                        if (row.Cells["colRubricLevel"].Value != null)
                        {
                            int studentId = Convert.ToInt32(row.Cells["StudentId"].Value);
                            int rubricId = Convert.ToInt32(row.Cells["colRubricLevel"].Value);

                            string sql = @"INSERT INTO studentresult (StudentId, AssessmentComponentId, RubricMeasurementId, EvaluationDate) 
                                         VALUES (@sid, @aid, @rid, @date) 
                                         ON DUPLICATE KEY UPDATE RubricMeasurementId = @rid, EvaluationDate = @date";

                            MySqlCommand cmd = new MySqlCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@sid", studentId);
                            cmd.Parameters.AddWithValue("@aid", componentId);
                            cmd.Parameters.AddWithValue("@rid", rubricId);
                            cmd.Parameters.AddWithValue("@date", DateTime.Now);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("Evaluation saved successfully!");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            new MainDashboard().Show();
            this.Close();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (dgvEvaluation.DataSource is DataTable dt)
            {
                dt.DefaultView.RowFilter = string.Format("RegistrationNumber LIKE '%{0}%' OR FirstName LIKE '%{0}%'", txtSearch.Text);
            }
        }

        private void cmbAssessmentComponent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAssessmentComponent.SelectedValue != null && cmbAssessmentComponent.ValueMember != "")
            {
                LoadData(); 
            }
        }
    }
}