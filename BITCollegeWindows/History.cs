using BITCollege_CS.Data;
using BITCollege_CS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace BITCollegeWindows
{
    public partial class History : Form
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
        public History(ConstructorData constructorData)
        {
            InitializeComponent();
            this.constructorData = constructorData;
            studentBindingSource.DataSource = constructorData.Student;
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
        /// given:  Open this form in top right corner of the frame.
        /// further code required:
        /// </summary>
        private void History_Load(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);

            try
            {
                var registrationCourseJoin = from registration in db.Registrations
                                             join course in db.Courses
                                             on registration.CourseId equals course.CourseId
                                             where registration.StudentId == constructorData.Student.StudentId
                                             select new
                                             {
                                                 registrationNumber = registration.RegistrationNumber,
                                                 registrationDate = registration.RegistrationDate,
                                                 course = course.Title,
                                                 grade = registration.Grade,
                                                 notes = registration.Notes
                                             };

                registrationBindingSource.DataSource = registrationCourseJoin.ToList();
            }
            catch (Exception error)
            {
                string caption = "An error has occured!";
                string errorMessage = error.Message;
                MessageBox.Show(errorMessage, caption, MessageBoxButtons.OK);
            }  
        }
    }
}
