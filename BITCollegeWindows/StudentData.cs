using BITCollege_CS.Data;
using BITCollege_CS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BITCollegeWindows
{
    public partial class StudentData : Form
    {
        protected static BITCollege_CSContext db = new BITCollege_CSContext();

        ///Given: Student and Registration data will be retrieved
        ///in this form and passed throughout application
        ///These variables will be used to store the current
        ///Student and selected Registration
        ConstructorData constructorData = new ConstructorData();

        /// <summary>
        /// This constructor will be used when this form is opened from
        /// the MDI Frame.
        /// </summary>
        public StudentData()
        {
            InitializeComponent();
        }

        /// <summary>
        /// given:  This constructor will be used when returning to StudentData
        /// from another form.  This constructor will pass back
        /// specific information about the student and registration
        /// based on activites taking place in another form.
        /// </summary>
        /// <param name="constructorData">constructorData object containing
        /// specific student and registration data.</param>
        public StudentData (ConstructorData constructor)
        {
            InitializeComponent();
            //Further code to be added.
            constructorData = constructor;
            studentNumberMaskedTextBox.Text = constructorData.Student.StudentNumber.ToString();
            studentNumberMaskedTextBox_Leave(null, null);
        }

        /// <summary>
        ///  Populate the constuctorData with the current data source of the binding sources of student and registration.
        /// </summary>
        private void PopulateConstructorData()
        {
            constructorData.Student = (Student)studentBindingSource.DataSource;
            constructorData.Registration = (Registration)registrationBindingSource.Current;
        }

        /// <summary>
        /// given: Open grading form passing constructor data.
        /// </summary>
        private void lnkUpdateGrade_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PopulateConstructorData();

            Grading grading = new Grading(constructorData);
            grading.MdiParent = this.MdiParent;
            grading.Show();
            this.Close();
        }


        /// <summary>
        /// given: Open history form passing constructor data.
        /// </summary>
        private void lnkViewDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PopulateConstructorData();

            History history = new History(constructorData);
            history.MdiParent = this.MdiParent;
            history.Show();
            this.Close();
        }

        /// <summary>
        /// given:  Opens the form in top right corner of the frame.
        /// </summary>
        private void StudentData_Load(object sender, EventArgs e)
        {
            //keeps location of form static when opened and closed
            this.Location = new Point(0, 0);
        }

        /// <summary>
        /// Handles the leave event of the studentNumberMaskedTextBox.
        /// </summary>
        private void studentNumberMaskedTextBox_Leave(object sender, EventArgs e)
        {
            long selectedStudentNumber = studentNumberMaskedTextBox.Text.Trim() == "" ? 
                                         0 : long.Parse(studentNumberMaskedTextBox.Text);
            Student student = db.Students.Where(x => x.StudentNumber == selectedStudentNumber).SingleOrDefault();

            if (student == null)
            {
                studentNumberMaskedTextBox.Focus();
                studentBindingSource.DataSource = typeof(Student);
                registrationBindingSource.DataSource = typeof(Registration);

                lnkUpdateGrade.Enabled = false;
                lnkViewDetails.Enabled = false;

                string caption = "Invalid Student Number";
                string message = String.Format("Student {0} does not exist.", studentNumberMaskedTextBox.Text);
                MessageBox.Show(message, caption, MessageBoxButtons.OK);
            }
            else 
            {
                studentBindingSource.DataSource = student;
                IQueryable<Registration> registrations = db.Registrations.
                                                     Where(x => x.StudentId == student.StudentId);
                if (registrations.Count() == 0)
                {
                    lnkUpdateGrade.Enabled = false;
                    lnkViewDetails.Enabled = false;
                    registrationBindingSource.DataSource = typeof(Registration);
                }
                else 
                {
                    registrationBindingSource.DataSource = registrations.ToList();
                    lnkUpdateGrade.Enabled = true;
                    lnkViewDetails.Enabled = true;

                    if (constructorData.Registration != null)
                    {
                        registrationNumberComboBox.Text = constructorData.Registration.RegistrationNumber.ToString();
                    }
                }
            } 
        }
    }
}
