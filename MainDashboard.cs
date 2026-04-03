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
    public partial class MainDashboard : Form
    {
        public MainDashboard()
        {
            InitializeComponent();
        }

        private void MainDashboard_Load(object sender, EventArgs e)
        {

        }

        private void btnStudents_Click(object sender, EventArgs e)
        {
            UI.StudentForm studentWin = new StudentForm();
            studentWin.Show();
            this.Hide();
        }

        private void btnClo_Click(object sender, EventArgs e)
        {
            UI.CloForm cloWin = new CloForm();
            cloWin.Show();
            this.Hide();
        }

        private void btnRubrics_Click(object sender, EventArgs e)
        {
            UI.RubricForm rubticWin=new RubricForm();
            rubticWin.Show();
            this.Hide();
        }

        private void btnAssessments_Click(object sender, EventArgs e)
        {
            UI.AssessmentForm assessmentWin=new AssessmentForm();
            assessmentWin.Show();
            this.Hide();
        }

        private void btnRubricLevels_Click(object sender, EventArgs e)
        {
            UI.RubricLevelsForm levelWin=new RubricLevelsForm();
            levelWin.Show();
            this.Hide();
        }

        private void btnEvaluation_Click(object sender, EventArgs e)
        {
            UI.EvaluationForm evaluationWin=new EvaluationForm();
            evaluationWin.Show();
            this.Hide();
        }

        private void btnAssessmentComponent_Click(object sender, EventArgs e)
        {
            UI.AssessmentComponentForm compForm = new UI.AssessmentComponentForm();
            compForm.Show();
            this.Hide();
        }

        private void btnAttendanceManagement_Click(object sender, EventArgs e)
        {
            AttendanceManagementForm frm = new AttendanceManagementForm();
            frm.Show();
            this.Hide();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
