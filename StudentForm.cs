using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MidDB26_2025CS10.UI
{
    public partial class StudentForm : Form
    {
        public StudentForm()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            UI.MainDashboard dash = new MainDashboard();
            dash.Show();
            this.Close();
        }

        private void StudentForm_Load(object sender, EventArgs e)
        {
            LoadStatus();
            ShowStudents();
            txtFirstName.Enabled = false;
            txtLastName.Enabled = false;
            txtContact.Enabled = false;
            txtEmail.Enabled = false;
            txtRegNo.Enabled = false;
            cmbStatus.Enabled = false;
        }
        private void LoadStatus()
        {
            try
            {
                using(MySqlConnection conn=dbConnection.getInternalConnection())
                {
                    conn.Open();
                    string query = "Select Name from lookup Where Category='Student_Status'";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    cmbStatus.Items.Clear();
                    while(reader.Read())
                    {
                        cmbStatus.Items.Add(reader["Name"].ToString());
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error Loading Status: " + ex.Message);
            }

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(txtFirstName.Text) || string.IsNullOrEmpty(txtRegNo.Text))
            {
                MessageBox.Show("Please enter at least Reg no and Name.");
                return;
            }
            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();

                    string checkQuery = "SELECT COUNT(*) FROM Student WHERE RegistrationNumber = @regNo";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@regNo", txtRegNo.Text);

                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (count > 0)
                    {
                        MessageBox.Show("A student with this Registration Number already exists!");
                        return;
                    }

                    string query = "Insert Into Student(FirstName,LastName,Contact,Email,RegistrationNumber,Status) " +
                                   "Values(@fName,@lName,@Contact,@email,@regNo,@status)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@fName", txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@lName", txtLastName.Text);
                    cmd.Parameters.AddWithValue("@Contact", txtContact.Text);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@regNo", txtRegNo.Text);

                    int statusId;
                    if(cmbStatus.Text == "Active")
                    {
                        statusId = 5;
                    }
                    else
                    {
                        statusId = 6;
                    }

                    cmd.Parameters.AddWithValue("@status", statusId);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Student Added Successfully.");
                    ShowStudents();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }
        private void ClearFields()
        {
            txtFirstName.Clear();
            txtLastName.Clear();
            txtContact.Clear();
            txtRegNo.Clear();
            txtEmail.Clear();
            cmbStatus.SelectedIndex = -1;
        }
        private void ShowStudents()
        {
            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    dgvStudents.AlternatingRowsDefaultCellStyle.BackColor = Color.LightSteelBlue;
                    string query = @"SELECT s.FirstName, s.LastName, s.Contact, s.Email, 
                 s.RegistrationNumber, l.Name as Status 
                 FROM Student s 
                 INNER JOIN lookup l ON s.Status = l.LookupId";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvStudents.DataSource = dt;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error Showing Students" + ex.Message);
            }
        }

        private void btnStartNew_Click(object sender, EventArgs e)
        {
            txtFirstName.Enabled = true;
            txtLastName.Enabled = true;
            txtContact.Enabled = true;
            cmbStatus.Enabled = true;
            txtEmail.Enabled = true;
            txtRegNo.Enabled = true;

        }

        private void cmbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if((string.IsNullOrEmpty(txtRegNo.Text)))
            {
                MessageBox.Show("Please select the student from the table first.");
                return;
            }
            DialogResult result = MessageBox.Show("Delete this student?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if(result==DialogResult.Yes)
            {
                try
                {
                    using(MySqlConnection conn=dbConnection.getInternalConnection())
                    {
                        conn.Open();
                        string query = "Delete From Student Where RegistrationNumber=@regNo";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@regNo", txtRegNo.Text);
                        cmd.ExecuteNonQuery();
                        ShowStudents();
                        ClearFields();
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error:" + ex.Message);
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    string query = @"Update Student
                                    SET FirstName=@fName, LastName=@lName, Contact=@contact, Email=@email, Status=@status 
                                    WHERE RegistrationNumber=@regNo";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@fName", txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@lName", txtLastName.Text);
                    cmd.Parameters.AddWithValue("@contact", txtContact.Text);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@regNo", txtRegNo.Text);
                    string  statusId;
                    if (cmbStatus.Text == "Active")
                    {
                        statusId = "Active";
                    }
                    else
                    {
                        statusId = "In Active";
                    }
                    cmd.Parameters.AddWithValue("@status", statusId); 
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Updated successfully!");
                    ShowStudents();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update Error: " + ex.Message);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection conn = dbConnection.getInternalConnection())
                {
                    conn.Open();
                    string query = @"SELECT s.FirstName, s.LastName, s.Contact, s.Email, 
                             s.RegistrationNumber, l.Name as Status 
                             FROM Student s 
                             INNER JOIN lookup l ON s.Status = l.LookupId
                             WHERE s.FirstName LIKE @search 
                             OR s.RegistrationNumber LIKE @search";

                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    da.SelectCommand.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvStudents.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void dgvStudents_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {           
                DataGridViewRow row = dgvStudents.Rows[e.RowIndex];               
                txtFirstName.Text = row.Cells["FirstName"].Value?.ToString();
                txtLastName.Text = row.Cells["LastName"].Value?.ToString();
                txtContact.Text = row.Cells["Contact"].Value?.ToString();
                txtEmail.Text = row.Cells["Email"].Value?.ToString();
                txtRegNo.Text = row.Cells["RegistrationNumber"].Value?.ToString();
                cmbStatus.Text = row.Cells["Status"].Value?.ToString();
                btnStartNew_Click(null, null);
                txtRegNo.Enabled = false;
            }
        }
    }       
}
    

