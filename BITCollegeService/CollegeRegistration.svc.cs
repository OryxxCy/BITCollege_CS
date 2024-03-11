using BITCollege_CS.Data;
using BITCollege_CS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Utility;

namespace BITCollegeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CollegeRegistration" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select CollegeRegistration.svc or CollegeRegistration.svc.cs at the Solution Explorer and start debugging.
 
    /// <summary>
    /// Handles the changes in registration using WCF Services.
    /// </summary>
    public class CollegeRegistration : ICollegeRegistration
    {
        protected static BITCollege_CSContext db = new BITCollege_CSContext();

        /// <summary>
        /// Deletes a registration.
        /// </summary>
        /// <param name="registrationId">The registrationId of the registration that will get deleted.</param>
        /// <returns>Returns true if the process is successfuk and false if it is not.</returns>
        public bool DropCourse(int registrationId)
        {
            try
            {
                Registration registration = db.Registrations.Find(registrationId);
                db.Registrations.Remove(registration);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a new registration.
        /// </summary>
        /// <param name="studentId">The studentId of teh new registration.</param>
        /// <param name="courseId">The courseId of teh new registration.</param>
        /// <param name="notes">The notes of teh new registration.</param>
        /// <returns>Returns an error code if a problem exist.</returns>
        public int RegisterCourse(int studentId, int courseId, string notes)
        {
            int errorCode = 0;

            IQueryable<Registration> registrations = db.Registrations.
                                                     Where(x => x.StudentId == studentId && x.CourseId == courseId);
            Student student = db.Students.Find(studentId);
            Course course = db.Courses.Find(courseId);

            IEnumerable<Registration> nullRegistrations = registrations.Where(x => x.Grade == null);

            if (nullRegistrations.Count() > 0) 
            {
                errorCode =  -100;
            }

            if (BusinessRules.CourseTypeLookup(course.CourseType) == CourseType.MASTERY) 
            {
                IEnumerable<Registration> notNullRegistrations = registrations.Where(x => x.Grade != null);
                int maximumAttempts = ((MasteryCourse)course).MaximumAttempts;

                if (notNullRegistrations.Count() >= maximumAttempts) 
                {
                    errorCode =  -200;
                }   
            }

            if (errorCode == 0) 
            {
                try
                {
                    Registration newRegistration = new Registration();

                    newRegistration.StudentId = studentId;
                    newRegistration.CourseId = courseId;
                    newRegistration.Notes = notes;
                    newRegistration.RegistrationDate = DateTime.Now;
                    newRegistration.SetNextRegistrationNumber();
                    double tuitionAmount = course.TuitionAmount * student.GradePointState.TuitionRateAdjustment(student);
                    student.OutstandingFees += tuitionAmount;

                    db.Registrations.Add(newRegistration);
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    errorCode = -300;
                }
            }

            return errorCode;
        }

        /// <summary>
        /// Chnages the grade of a registration.
        /// </summary>
        /// <param name="grade">The new grade of the registration.</param>
        /// <param name="registrationId">The registrationId of the registration that will get updated.</param>
        /// <param name="notes">The notes grade of the registration.</param>
        /// <returns>The new GPA of the student in this registration.</returns>
        public double? UpdateGrade(double grade, int registrationId, string notes)
        {
            Registration registration = db.Registrations.Find(registrationId);
            registration.Grade = grade;
            registration.Notes = notes;
            db.SaveChanges();
            double? calculatedGradePointAverage = CalculateGradePointAverage(registration.StudentId);
            return calculatedGradePointAverage;
        }

        /// <summary>
        /// Calculates the new GPA of a student.
        /// </summary>
        /// <param name="studentId">The studentId of the student that will have a new GPA.</param>
        /// <returns>The new GPA of a student.</returns>
        private double? CalculateGradePointAverage(int studentId) 
        {
            double grade;
            CourseType courseType;
            double gradePoint;
            double gradePointValue;
            double totalCreditHours = 0;
            double totalGradePointValue = 0;
            double? calculatedGradePointAverage;

            IQueryable<Registration> registrations = db.Registrations.
                                                     Where(x => x.StudentId == studentId && x.Grade != null);

            foreach (Registration record in registrations.ToList()) 
            {
                grade = (double)record.Grade;
                courseType = BusinessRules.CourseTypeLookup(record.Course.CourseType);
                if (courseType != CourseType.AUDIT) 
                {
                    gradePoint = BusinessRules.GradeLookup(grade,courseType);
                    gradePointValue = gradePoint * record.Course.CreditHours;
                    totalGradePointValue += gradePointValue;
                    totalCreditHours += record.Course.CreditHours;
                }
            }

            calculatedGradePointAverage = totalCreditHours == 0 ? null : (double?)(totalGradePointValue / totalCreditHours);

            Student student = db.Students.Find(studentId);
            student.GradePointAverage = calculatedGradePointAverage;
            student.ChangeState();
            db.SaveChanges();
            
            return calculatedGradePointAverage;
        }
    }
}
