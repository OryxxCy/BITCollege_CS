using BITCollege_CS.Data;
using BITCollege_CS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utility;

namespace BITCollegeWindows
{
    public partial class Grading : Form
    {
        protected static BITCollege_CSContext db = new BITCollege_CSContext();

        ///given:  student and registration data will passed throughout 
        ///application. This object will be used to store the current
        ///student and selected registration
        ConstructorData constructorData;

        /// <summary>
        /// given:  This constructor will be used when called from the
        /// Student form.  This constructor will receive 
        /// specific information about the student and registration
        /// further code required:  
        /// </summary>
        /// <param name="constructorData">constructorData object containing
        /// specific student and registration data.</param>
        public Grading(ConstructorData constructor)
        {
            InitializeComponent();
            constructorData = constructor;
        }

        /// <summary>
        /// given: This code will navigate back to the Student form with
        /// the specific student and registration data that launched
        /// this form.
        /// </summary>
        private void lnkReturn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //return to student with the data selected for this form
            StudentData student = new StudentData(constructorData);
            student.MdiParent = this.MdiParent;
            student.Show();
            this.Close();
        }

        /// <summary>
        /// given:  Always open in this form in the top right corner of the frame.
        /// further code required:
        /// </summary>
        private void Grading_Load(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);
            studentBindingSource.DataSource = constructorData.Student;
            registrationBindingSource.DataSource = constructorData.Registration;

            courseNumberMaskedLabel.Mask = BusinessRules.CourseFormat(constructorData.Registration.Course.CourseType);

            if (constructorData.Registration.Grade != null)
            {
                gradeTextBox.Enabled = false;
                lnkUpdate.Enabled = false;
                lblExisting.Visible = true;
            }
            else 
            {
                gradeTextBox.Enabled = true;
                lnkUpdate.Enabled = true;
                lblExisting.Visible = false;
            }
        }

        /// <summary>
        /// Handles the logic for updating a student grade
        /// </summary>
        private void lnkUpdate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string gradeInPercentage = BusinessRules.ParseString(gradeTextBox.Text, "%");

                if (Numeric.IsNumeric(gradeInPercentage, NumberStyles.AllowDecimalPoint))
                {
                    double gradeData = double.Parse(gradeInPercentage) / 100;
                    if (gradeData > 1 || gradeData < 0)
                    {
                        string caption = "An error has occured!";
                        string errorMessage = "Invalid Input : Grade must be entered as a decimal value between 0 and 1";
                        MessageBox.Show(errorMessage, caption, MessageBoxButtons.OK);
                    }
                    else
                    {
                        CollegeRegistrationService.CollegeRegistrationClient collegeService = 
                                                    new CollegeRegistrationService.CollegeRegistrationClient();

                        collegeService.UpdateGrade(gradeData, constructorData.Registration.RegistrationId, constructorData.Registration.Notes);
                     
                        gradeTextBox.Enabled = false;
                    }
                }
                else
                {
                    string caption = "An error has occured!";
                    string errorMessage = "Invalid Input : Grade must be entered as a decimal value between 0 and 1";
                    MessageBox.Show(errorMessage, caption, MessageBoxButtons.OK);
                }
            }
            catch 
            {
                string caption = "An error has occured!";
                string errorMessage = "Invalid Input. Please don't leave the grade empty and use numeric data not words.";
                MessageBox.Show(errorMessage, caption, MessageBoxButtons.OK);
            }
        }
    }
}
