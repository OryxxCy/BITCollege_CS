using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BITCollege_CS.Data
{
    public class BITCollege_CSContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public BITCollege_CSContext() : base("name=BITCollege_CSContext")
        {
        }

        public System.Data.Entity.DbSet<BITCollege_CS.Models.AcademicProgram> AcademicPrograms { get; set; }

        public System.Data.Entity.DbSet<BITCollege_CS.Models.Registration> Registrations { get; set; }

        public System.Data.Entity.DbSet<BITCollege_CS.Models.Course> Courses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_CS.Models.Student> Students { get; set; }

        public System.Data.Entity.DbSet<BITCollege_CS.Models.GradePointState> GradePointStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_CS.Models.GradedCourse> GradedCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_CS.Models.AuditCourse> AuditCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_CS.Models.MasteryCourse> MasteryCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_CS.Models.SuspendedState> SuspendedStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_CS.Models.ProbationState> ProbationStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_CS.Models.RegularState> RegularStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_CS.Models.HonoursState> HonoursStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_CS.Models.NextUniqueNumber> NextUniqueNumbers { get; set; }

        public System.Data.Entity.DbSet<BITCollege_CS.Models.NextAuditCourse> NextAuditCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_CS.Models.NextGradedCourse> NextGradedCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_CS.Models.NextMasteryCourse> NextMasteryCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_CS.Models.NextRegistration> NextRegistrations { get; set; }

        public System.Data.Entity.DbSet<BITCollege_CS.Models.NextStudent> NextStudents { get; set; }
    }
}
