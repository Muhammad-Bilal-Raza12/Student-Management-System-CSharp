using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace MidDB26_2025CS10.UI
{
    public partial class AttendanceManagementForm : Form
    {
        private bool isViewMode = false;
        private bool isWaitingForDate = false;

        public AttendanceManagementForm()
        {
            InitializeComponent();
        }

        private void AttendanceManagementForm_Load(object sender, EventArgs e)
        {
            SetupGrid();
            LoadStudents();
        }

        private void SetupGrid()
        {
            dgvAttendance.AutoGenerateColumns = false;
            dgvAttendance.Columns.Clear();
            dgvAttendance.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Name = "Id", Visible = false });
            dgvAttendance.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RegistrationNumber", HeaderText = "Reg No", Name = "RegNo", ReadOnly = true });
            dgvAttendance.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FirstName", HeaderText = "Name", Name = "FirstName", ReadOnly = true });

            DataGridViewComboBoxColumn combo = new DataGridViewComboBoxColumn();
            combo.HeaderText = "Status";
            combo.Name = "colStatus";
            combo.Items.AddRange("Present", "Absent", "Late", "Leave");
            dgvAttendance.Columns.Add(combo);

            dgvAttendance.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAttendance.AllowUserToAddRows = false;
        }

        private void LoadStudents()
        {
            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    string query = "SELECT Id, RegistrationNumber, FirstName FROM student";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvAttendance.DataSource = dt;
                    if (dgvAttendance.Columns.Contains("colStatus"))
                    {
                        dgvAttendance.Columns["colStatus"].Visible = true;
                        foreach (DataGridViewRow row in dgvAttendance.Rows) row.Cells["colStatus"].Value = "Present";
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btnViewClass_Click(object sender, EventArgs e)
        {
            if (!isViewMode && !isWaitingForDate)
            {
                dgvAttendance.DataSource = null;
                isWaitingForDate = true;
                btnViewClass.Text = "Fetch History";
                MessageBox.Show("Please select a date and click 'Fetch History'.");
            }
            else if (isWaitingForDate)
            {
                FetchAttendanceHistory();
            }
            else
            {
                isViewMode = false;
                isWaitingForDate = false;
                btnViewClass.Text = "View Class";
                dgvAttendance.AutoGenerateColumns = false;
                LoadStudents();
            }
        }

        private void FetchAttendanceHistory()
        {
            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    string query = @"SELECT s.RegistrationNumber, s.FirstName, 
                                   (CASE sa.AttendanceStatus WHEN 1 THEN 'Present' WHEN 2 THEN 'Absent' WHEN 3 THEN 'Late' ELSE 'Leave' END) as Status 
                                   FROM studentattendance sa 
                                   JOIN classattendance ca ON sa.AttendanceId = ca.Id 
                                   JOIN student s ON sa.StudentId = s.Id 
                                   WHERE ca.AttendanceDate = @date";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@date", dtpDate.Value.Date);
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        dgvAttendance.AutoGenerateColumns = true;
                        dgvAttendance.DataSource = dt;

                        if (dgvAttendance.Columns.Contains("colStatus")) dgvAttendance.Columns["colStatus"].Visible = false;

                        isViewMode = true;
                        isWaitingForDate = false;
                        btnViewClass.Text = "Back to Marking";
                    }
                    else
                    {
                        MessageBox.Show("No records found for " + dtpDate.Value.ToShortDateString());
                        isWaitingForDate = false;
                        btnViewClass.Text = "View Class";
                        LoadStudents();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (isViewMode || isWaitingForDate) return;

            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    DateTime selectedDate = dtpDate.Value.Date;
                    int attendanceId = -1;

                    string checkQuery = "SELECT Id FROM classattendance WHERE AttendanceDate = @date";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@date", selectedDate);
                    object result = checkCmd.ExecuteScalar();

                    if (result != null)
                    {
                        attendanceId = Convert.ToInt32(result);
                        new MySqlCommand("DELETE FROM studentattendance WHERE AttendanceId = " + attendanceId, conn).ExecuteNonQuery();
                    }
                    else
                    {
                        MySqlCommand insCmd = new MySqlCommand("INSERT INTO classattendance (AttendanceDate) VALUES (@date); SELECT LAST_INSERT_ID();", conn);
                        insCmd.Parameters.AddWithValue("@date", selectedDate);
                        attendanceId = Convert.ToInt32(insCmd.ExecuteScalar());
                    }

                    foreach (DataGridViewRow row in dgvAttendance.Rows)
                    {
                        if (row.Cells["Id"].Value != null)
                        {
                            MySqlCommand stdCmd = new MySqlCommand("INSERT INTO studentattendance (AttendanceId, StudentId, AttendanceStatus) VALUES (@aid, @sid, @status)", conn);
                            stdCmd.Parameters.AddWithValue("@aid", attendanceId);
                            stdCmd.Parameters.AddWithValue("@sid", row.Cells["Id"].Value);
                            string status = row.Cells["colStatus"].Value?.ToString() ?? "Present";
                            int statusId = (status == "Absent") ? 2 : (status == "Late") ? 3 : (status == "Leave") ? 4 : 1;
                            stdCmd.Parameters.AddWithValue("@status", statusId);
                            stdCmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("Attendance saved successfully");
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
            if (dgvAttendance.DataSource is DataTable dt)
            {
                dt.DefaultView.RowFilter = string.Format("RegistrationNumber LIKE '%{0}%' OR FirstName LIKE '%{0}%'", txtSearch.Text);
            }
        }
    }
}