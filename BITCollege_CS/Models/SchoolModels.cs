/*
 * Name: Cyrusbien Sarceno
 * Program: Business Information Technology
 * Course: ADEV-3008 Programming 3
 * Created: 2023-09-04
 * Updated: 2023-10-27
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using Utility;
using BITCollege_CS.Data;
using System.Web.Compilation;

namespace BITCollege_CS.Models
{
    /// <summary>
    /// GradePointState Model - to represent GradePointState in the database.
    /// </summary>
    public abstract class GradePointState 
    {
        protected static BITCollege_CSContext db = new BITCollege_CSContext();
        
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key]
        public int GradePointStateId { get; set; }

        [Required]
        [DisplayFormat(DataFormatString ="{0:0.00}")]
        [Display(Name ="Lower\nLimit")]
        public double LowerLimit { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        [Display(Name = "Upper\nLimit")]
        public double UpperLimit { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:0.0#}")]
        [Display(Name = "Tuition\nRate\nFactor")]
        public double TuitionRateFactor { get; set; }

        [Display(Name = "State")]
        public string Description 
        {
            get
            {
                return BusinessRules.ParseString(GetType().Name, "State");
            }
        }

        /// <summary>
        /// Changes the tuition rate of a student.
        /// </summary>
        /// <param name="student">an object of the Student class that receives a change in their grade point average.</param>
        /// <returns>a new tuition rate.</returns>
        public abstract double TuitionRateAdjustment(Student student);


        /// <summary>
        /// Changes the GradePointState state of a student base on their grade point average.
        /// </summary>
        /// <param name="student">an object of the Student class that receives a change in their grade point average.</param>
        public abstract void StateChangeCheck(Student student);

        //Navigation Properties
        public virtual ICollection<Student> Student { get; set; }
    }

    /// <summary>
    /// SuspendedState Model - to represent SuspendedState in the database.
    /// </summary>
    public class SuspendedState : GradePointState
    {
        private static SuspendedState suspendedState;

        /// <summary>
        /// Create and initialize the single instance of SuspendedState.
        /// </summary>
        private SuspendedState() 
        {
            LowerLimit = 0.00;
            UpperLimit = 1.00;
            TuitionRateFactor = 1.1;
        }

        /// <summary>
        /// Returns the single instance of SuspendedState if it is null it create a new instance then return it.
        /// </summary>
        /// <returns>The single instance of SuspendedState</returns>
        public static SuspendedState GetInstance() 
        {
            if (suspendedState == null) 
            {
                suspendedState = db.SuspendedStates.SingleOrDefault();
                if (suspendedState == null) 
                {
                    suspendedState = new SuspendedState();
                    db.SuspendedStates.Add(suspendedState);
                    db.SaveChanges();
                }
            }
            return suspendedState;
        }

        /// <summary>
        /// Changes the tuition rate of a student.
        /// </summary>
        /// <param name="student">an object of the Student class that receives a change in their grade point average.</param>
        /// <returns>a new tuition rate.</returns>
        public override double TuitionRateAdjustment(Student student)
        {
            double localValue = this.TuitionRateFactor;

            if (student.GradePointAverage < 0.50)
            {
                localValue += 0.05;
            }
            else if (student.GradePointAverage < 0.75) 
            {
                localValue += 0.02;
            }

            return localValue;
        }

        /// <summary>
        /// Changes the GradePointState state of a student into probation state if their GPA is greater than 1.
        /// </summary>
        /// <param name="student">an object of the Student class that receives a change in their grade point average.</param>
        public override void StateChangeCheck(Student student)
        {
            if (student.GradePointAverage > this.UpperLimit)
            {
                student.GradePointStateId = ProbationState.GetInstance().GradePointStateId;
            }
        }
    }

    /// <summary>
    /// ProbationState Model - to represent ProbationState in the database.
    /// </summary>
    public class ProbationState : GradePointState 
    {
        private static ProbationState probationState;

        /// <summary>
        /// Create and initialize the single instance of ProbationState.
        /// </summary>
        private ProbationState()
        {
            LowerLimit = 1.00;
            UpperLimit = 2.00;
            TuitionRateFactor = 1.075;
        }

        /// <summary>
        /// Returns the single instance of ProbationState if it is null it create a new instance then return it.
        /// </summary>
        /// <returns>The single instance of ProbationState</returns>
        public static ProbationState GetInstance() 
        {
            if (probationState == null) 
            {
                probationState = db.ProbationStates.SingleOrDefault();
                if (probationState == null)
                {
                    probationState = new ProbationState();
                    db.ProbationStates.Add(probationState);
                    db.SaveChanges();
                }
            }
            return probationState;
        }

        /// <summary>
        /// Changes the tuition rate of a student.
        /// </summary>
        /// <param name="student">an object of the Student class that receives a change in their grade point average.</param>
        /// <returns>a new tuition rate.</returns>
        public override double TuitionRateAdjustment(Student student)
        {
            double localValue = this.TuitionRateFactor;

            IQueryable<Registration> studentCourses = db.Registrations.Where(x => x.StudentId == student.StudentId
                                                      && x.Grade != null);
            int numberOfCourses = studentCourses.Count();

            if (numberOfCourses >= 5)
            {
                localValue = 1.035;
            }
            return localValue;
        }

        /// <summary>
        /// Changes the GradePointState state of a student into suspended state if their GPA is less than 1
        /// or into regular state if their gpa is greater than 2.
        /// </summary>
        /// <param name="student">an object of the Student class that receives a change in their grade point average.</param>
        public override void StateChangeCheck(Student student)
        {
            if (student.GradePointAverage < this.LowerLimit)
            {
                student.GradePointStateId = SuspendedState.GetInstance().GradePointStateId;
            }
            else if (student.GradePointAverage > this.UpperLimit) 
            {
                student.GradePointStateId = RegularState.GetInstance().GradePointStateId;
            }
        }
    }

    /// <summary>
    /// RegularState Model - to represent RegularState in the database.
    /// </summary>
    public class RegularState : GradePointState
    {
        private static RegularState regularState;

        /// <summary>
        /// Create and initialize the single instance of RegularState.
        /// </summary>
        private RegularState() 
        {
            LowerLimit = 2.00;
            UpperLimit = 3.70;
            TuitionRateFactor = 1.0;
        }

        /// <summary>
        /// Returns the single instance of RegularState if it is null it create a new instance then return it.
        /// </summary>
        /// <returns>The single instance of RegularState</returns>
        public static RegularState GetInstance()
        {
            if (regularState == null)
            {
                regularState = db.RegularStates.SingleOrDefault();
                if (regularState == null) 
                {
                    regularState = new RegularState();
                    db.RegularStates.Add(regularState);
                    db.SaveChanges();
                }
            }
            return regularState;
        }

        /// <summary>
        /// Changes the tuition rate of a student.
        /// </summary>
        /// <param name="student">an object of the Student class that receives a change in their grade point average.</param>
        /// <returns>a new tuition rate.</returns>
        public override double TuitionRateAdjustment(Student student)
        {
            return this.TuitionRateFactor;
        }

        /// <summary>
        /// Changes the GradePointState state of a student into probation state if their GPA is less than 2
        /// or into honours state if their gpa is greater than 3.7.
        /// </summary>
        /// <param name="student">an object of the Student class that receives a change in their grade point average.</param>
        public override void StateChangeCheck(Student student)
        {
            if (student.GradePointAverage < this.LowerLimit)
            {
                student.GradePointStateId = ProbationState.GetInstance().GradePointStateId;
            }
            else if (student.GradePointAverage > this.UpperLimit)
            {
                student.GradePointStateId = HonoursState.GetInstance().GradePointStateId;
            }
        }
    }

    /// <summary>
    /// HonoursState Model - to represent HonoursState in the database.
    /// </summary>
    public class HonoursState : GradePointState
    {
        private static HonoursState honoursState;

        /// <summary>
        /// Create and initialize the single instance of HonoursState.
        /// </summary>
        private HonoursState() 
        {
            LowerLimit = 3.70;
            UpperLimit = 4.50;
            TuitionRateFactor = 0.9;
        }

        /// <summary>
        /// Returns the single instance of HonoursState if it is null it create a new instance then return it.
        /// </summary>
        /// <returns>The single instance of HonoursState</returns>
        public static HonoursState GetInstance() 
        {
            if (honoursState == null)
            {
                honoursState = db.HonoursStates.SingleOrDefault();
                if (honoursState == null)
                {
                    honoursState = new HonoursState();
                    db.HonoursStates.Add(honoursState);
                    db.SaveChanges();
                }
            }
            return honoursState;
        }

        /// <summary>
        /// Changes the tuition rate of a student.
        /// </summary>
        /// <param name="student">an object of the Student class that receives a change in their grade point average.</param>
        /// <returns>a new tuition rate.</returns>
        public override double TuitionRateAdjustment(Student student)
        {
            double localValue = this.TuitionRateFactor;

            IQueryable<Registration> studentCourses = db.Registrations.Where(x => x.StudentId == student.StudentId
                                                      && x.Grade != null);
            int numberOfCourses = studentCourses.Count();

            if (numberOfCourses >= 5)
            {
                localValue -= 0.05;
            }
            if (student.GradePointAverage > 4.25)
            {
                localValue -= 0.02;
            }
            return localValue;
        }

        /// <summary>
        /// Changes the GradePointState state of a student into regular state if their GPA is less than 3.7.
        /// </summary>
        /// <param name="student">an object of the Student class that receives a change in their grade point average.</param>
        public override void StateChangeCheck(Student student)
        {
            if (student.GradePointAverage < this.LowerLimit)
            {
                student.GradePointStateId = RegularState.GetInstance().GradePointStateId;
            }
        }
    }

    /// <summary>
    ///  Course Model - to represent Course in the database.
    /// </summary>
    public abstract class Course
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key]
        public int CourseId { get; set; }

        [ForeignKey("AcademicProgram")]
        public int? AcademicProgramId { get; set; }

        [Display(Name = "Course\nNumber")]
        public string CourseNumber { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        [Display(Name = "Credit\nHours")]
        public double CreditHours { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Tuition")]
        public double TuitionAmount { get; set; }

        [Display(Name = "Course\nType")]
        public string CourseType
        {
            get
            {
                return BusinessRules.ParseString(GetType().Name, "Course");
            }
        }

        public string Notes { get; set; }

        /// <summary>
        /// Sets the next courseNumber
        /// </summary>
        public abstract void SetNextCourseNumber();

        //Navigation Properties
        public virtual AcademicProgram AcademicProgram { get; set; }
        public virtual ICollection<Registration> Registration { get; set; }
    }

    /// <summary>
    /// GradedCourse Model - to represent the GradedCourse table in the database.
    /// </summary>
    public class GradedCourse : Course
    {
        [Required]
        [Display(Name ="Assignments")]
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double AssignmentWeight { get; set; }

        [Required]
        [Display(Name = "Exams")]
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double ExamWeight { get; set; }

        /// <summary>
        /// Sets the next Graded course courseNumber. 
        /// </summary>
        public override void SetNextCourseNumber()
        {
            long? nextNumber = StoredProcedures.NextNumber("NextGradedCourse");
            if (nextNumber != null)
            {
                CourseNumber = "G-" + nextNumber.ToString();
            }
        }
    }

    /// <summary>
    /// AuditCourse Model - to represent the AuditCourse table in the database.
    /// </summary>
    public class AuditCourse : Course 
    {
        /// <summary>
        /// Sets the next Audit course courseNumber.
        /// </summary>
        public override void SetNextCourseNumber()
        {
            long? nextNumber = StoredProcedures.NextNumber("NextAuditCourse");
            if (nextNumber != null)
            {
                CourseNumber = "A-" + nextNumber.ToString();
            }
        }
    }

    /// <summary>
    /// MasteryCourse Model - to represent the MasteryCourse table in the database.
    /// </summary>
    public class MasteryCourse : Course 
    {
        [Required]
        [Display(Name ="Maximum\nAttempts")]
        public int MaximumAttempts { get; set; }

        /// <summary>
        /// Sets the next Mastery course courseNumber.
        /// </summary>
        public override void SetNextCourseNumber()
        {
            long? nextNumber = StoredProcedures.NextNumber("NextMasteryCourse");
            if (nextNumber != null)
            {
                CourseNumber = "M-" + nextNumber.ToString();
            }
        }
    }

    /// <summary>
    /// AcademicProgram Model - to represent the AcademicProgram table in the database.
    /// </summary>
    public class AcademicProgram
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int AcademicProgramId { get; set; }

        [Required]
        [Display(Name ="Program")]
        public string ProgramAcronym { get; set; }

        [Required]
        [Display(Name ="Program\nName")]
        public string Description { get; set; }

        //Navigation Properties
        public virtual ICollection<Student> Student { get; set; }
        public virtual ICollection<Course> Course { get; set; }
    }

    /// <summary>
    /// Registration Model - to represent the Registration table in the database.
    /// </summary>
    public class Registration 
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RegistrationId { get; set; }

        [Required]
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [Required]
        [ForeignKey("Course")]
        public int CourseId { get; set; }

        [Display(Name = "Registration\nNumber")]
        public long RegistrationNumber { get; set; }

        [Required]
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime RegistrationDate { get; set; }

        [DisplayFormat(NullDisplayText ="Ungraded")]
        [Range(0,1)]
        public double? Grade { get; set; }

        public string Notes { get; set; }

        /// <summary>
        /// Sets the next RegistrationNumber
        /// </summary>
        public void SetNextRegistrationNumber() 
        {
            long? nextNumber = StoredProcedures.NextNumber("NextRegistration");
            if (nextNumber != null)
            {
                RegistrationNumber = (long)nextNumber;
            }
        }

        //Navigation Properties
        public virtual Student Student { get; set; }
        public virtual Course Course { get; set; }
    }

    /// <summary>
    /// Student Model - to represent the Student table in the database.
    /// </summary>
    public class Student
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int StudentId { get; set; }

        [Required]
        [ForeignKey("GradePointState")]
        public int GradePointStateId { get; set; }

        [ForeignKey("AcademicProgram")]
        public int? AcademicProgramId { get; set; }

        [Display(Name = "Student\nNumber")]
        public long StudentNumber { get; set; }

        [Required]
        [Display(Name = "First\nName")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last\nName")]
        public string LastName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        [RegularExpression("^(N[BLSTU]|[AMN]B|[BQ]C|ON|PE|SK|YT)", ErrorMessage = "Please enter a valid Canadian province code")]
        public string Province { get; set; }

        [Required]
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Grade Point\nAverage")]
        [DisplayFormat(DataFormatString ="{0:0.00}")]
        [Range(0,4.5)]
        public double? GradePointAverage { get; set; }

        [Required]
        [Display(Name ="Fees")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public double OutstandingFees { get; set; }

        public string Notes { get; set; }

        [Display(Name = "Name")]
        public string FullName 
        {
            get
            {
                return String.Format("{0} {1}", FirstName, LastName);
            }
        }

        [Display(Name = "Address")]
        public string FullAddress
        {
            get
            {
                return String.Format("{0} {1} {2}", Address, City, Province);
            }
        }

        /// <summary>
        /// Changes the GradePointState of a student.
        /// </summary>
        public void ChangeState() 
        {
            BITCollege_CSContext db = new BITCollege_CSContext();
            GradePointState before = db.GradePointStates.Find(this.GradePointStateId);
            int after = 0;

            while (before.GradePointStateId != after)
            {
                before.StateChangeCheck(this);
                after = before.GradePointStateId;
                before = db.GradePointStates.Find(this.GradePointStateId);
            }
        }

        /// <summary>
        /// Sets the next studentNumber.
        /// </summary>
        public void SetNextStudentNumber() 
        {
            long? nextNumber = StoredProcedures.NextNumber("NextStudent");
            if (nextNumber != null)
            {
                StudentNumber = (long)nextNumber;
            }
        }

        //Navigation Properties
        public virtual AcademicProgram AcademicProgram { get; set; }
        public virtual GradePointState GradePointState { get; set; }
        public virtual ICollection<Registration> Registration { get; set; }
    }

    /// <summary>
    /// NextUniqueNumber - to represent the NextUniqueNumber table in the database.
    /// </summary>
    public abstract class NextUniqueNumber
    {
        protected static BITCollege_CSContext db = new BITCollege_CSContext();

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int NextUniqueNumberId { get; set; }

        [Required]
        public long NextAvailableNumber{ get; set; }
    }

    /// <summary>
    /// NextStudent model - to represent the NextStudent in the database and used to determine the next Student's StudentNumber.
    /// </summary>
    public class NextStudent : NextUniqueNumber
    {
        private static NextStudent nextStudent;

        /// <summary>
        /// Create and initialize the single instance of NextStudent.
        /// </summary>
        private NextStudent()
        {
            NextAvailableNumber = 20000000;
        }

        /// <summary>
        /// Returns the single instance of NextStudent if it is null it create a new instance then return it.
        /// </summary>
        /// <returns>The single instance of NextStudent</returns>
        public static NextStudent GetInstance()
        {
            if (nextStudent == null)
            {
                nextStudent = db.NextStudents.SingleOrDefault();
                if (nextStudent == null)
                {
                    nextStudent = new NextStudent();
                    db.NextStudents.Add(nextStudent);
                    db.SaveChanges();
                }
            }
            return nextStudent;
        }
    }


    /// <summary>
    /// NextRegistration model - to represent the NextRegistration in the database and used to determine the next Registration's RegistrationNumber.
    /// </summary>
    public class NextRegistration : NextUniqueNumber
    {
        private static NextRegistration nextRegistration;

        /// <summary>
        /// Create and initialize the single instance of NextRegistration.
        /// </summary>
        private NextRegistration()
        {
            NextAvailableNumber = 700;
        }

        /// <summary>
        /// Returns the single instance of NextRegistration if it is null it create a new instance then return it.
        /// </summary>
        /// <returns>The single instance of NextRegistration</returns>
        public static NextRegistration GetInstance()
        {
            if (nextRegistration == null)
            {
                nextRegistration = db.NextRegistrations.SingleOrDefault();
                if (nextRegistration == null)
                {
                    nextRegistration = new NextRegistration();
                    db.NextRegistrations.Add(nextRegistration);
                    db.SaveChanges();
                }
            }
            return nextRegistration;
        }
    }

    /// <summary>
    /// NextGradedCourse model - to represent the NextGradedCourse in the database and used to determine the next GradedCourse's CourseNumber.
    /// </summary>
    public class NextGradedCourse : NextUniqueNumber
    {
        private static NextGradedCourse nextGradedCourse;

        /// <summary>
        /// Create and initialize the single instance of NextGradedCourse.
        /// </summary>
        private NextGradedCourse()
        {
            NextAvailableNumber = 200000;
        }

        /// <summary>
        /// Returns the single instance of NextGradedCourse if it is null it create a new instance then return it.
        /// </summary>
        /// <returns>The single instance of NextGradedCourse</returns>
        public static NextGradedCourse GetInstance()
        {
            if (nextGradedCourse == null)
            {
                nextGradedCourse = db.NextGradedCourses.SingleOrDefault();
                if (nextGradedCourse == null)
                {
                    nextGradedCourse = new NextGradedCourse();
                    db.NextGradedCourses.Add(nextGradedCourse);
                    db.SaveChanges();
                }
            }
            return nextGradedCourse;
        }
    }

    /// <summary>
    /// NextAuditCourse model - to represent the NextAuditCourse in the database and used to determine the next AuditCourse's CourseNumber.
    /// </summary>
    public class NextAuditCourse : NextUniqueNumber
    {
        private static NextAuditCourse nextAuditCourse;

        /// <summary>
        /// Create and initialize the single instance of NextAuditCourse.
        /// </summary>
        private NextAuditCourse()
        {
            NextAvailableNumber = 2000;
        }

        /// <summary>
        /// Returns the single instance of NextAuditCourse if it is null it create a new instance then return it.
        /// </summary>
        /// <returns>The single instance of NextAuditCourse</returns>
        public static NextAuditCourse GetInstance()
        {
            if (nextAuditCourse == null)
            {
                nextAuditCourse = db.NextAuditCourses.SingleOrDefault();
                if (nextAuditCourse == null)
                {
                    nextAuditCourse = new NextAuditCourse();
                    db.NextAuditCourses.Add(nextAuditCourse);
                    db.SaveChanges();
                }
            }
            return nextAuditCourse;
        }
    }

    /// <summary>
    /// NextMasteryCourse model - to represent the NextMasteryCourse in the database and used to determine the next MasteryCourse's CourseNumber.
    /// </summary>
    public class NextMasteryCourse : NextUniqueNumber
    {
        private static NextMasteryCourse nextMasteryCourse;

        /// <summary>
        /// Create and initialize the single instance of NextMasteryCourse.
        /// </summary>
        private NextMasteryCourse()
        {
            NextAvailableNumber = 20000;
        }

        /// <summary>
        /// Returns the single instance of NextMasteryCourse if it is null it create a new instance then return it.
        /// </summary>
        /// <returns>The single instance of NextMasteryCourse</returns>
        public static NextMasteryCourse GetInstance()
        {
            if (nextMasteryCourse == null)
            {
                nextMasteryCourse = db.NextMasteryCourses.SingleOrDefault();
                if (nextMasteryCourse == null)
                {
                    nextMasteryCourse = new NextMasteryCourse();
                    db.NextMasteryCourses.Add(nextMasteryCourse);
                    db.SaveChanges();
                }
            }
            return nextMasteryCourse;
        }
    }
}