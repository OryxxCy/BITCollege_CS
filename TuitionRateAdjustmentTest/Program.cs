/*
 * Name: Cyrusbien Sarceno
 * Program: Business Information Technology
 * Course: ADEV-3008 Programming 3
 * Created: 2023-09-04
 * Updated: 2023-10-09
 */

using BITCollege_CS.Models;
using BITCollege_CS.Data;
using Utility;
using System;
using System.Security.Policy;

namespace TuitionRateAdjustmentTest
{
    public class Program
    {
        private static BITCollege_CSContext db = new BITCollege_CSContext();

        static void Main(string[] args)
        {
            Console.WriteLine("----------SuspendedState----------");
            Console.WriteLine("Test 1");
            SuspendedState_GradePointAverage_044();
            Console.WriteLine("Test 2");
            SuspendedState_GradePointAverage_060();
            Console.WriteLine("Test 3");
            SuspendedState_GradePointAverage_080();

            Console.WriteLine("----------ProbationState----------");
            Console.WriteLine("Test 1");
            ProbationState_GradePointAverage_115_Courses_3();
            Console.WriteLine("Test 2");
            ProbationState_GradePointAverage_115_Courses_7();

            Console.WriteLine("----------RegularState----------");
            Console.WriteLine("Test 1");
            RegularState_GradePointAverage_250();

            Console.WriteLine("----------HonoursState----------");
            Console.WriteLine("Test 1");
            HonoursState_GradePointAverage_390_Courses_3();
            Console.WriteLine("Test 2");
            HonoursState_GradePointAverage_427_Courses_4();
            Console.WriteLine("Test 3");
            HonoursState_GradePointAverage_440_Courses_7();
            Console.WriteLine("Test 4");
            HonoursState_GradePointAverage_410_Courses_7();

            Console.Write("Press any key to close this window...");
            Console.ReadKey();
        }

        //Suspended States

        /// <summary>
        /// Test 1: A student with a GradePointAverage 0.44 registering for a course that has a 
        /// TuitionAmount of $1000 will be charged $1,150
        /// </summary>
        static void SuspendedState_GradePointAverage_044()
        {
            Student student = db.Students.Find(1);
            student.GradePointAverage = 0.44;
            student.GradePointStateId = 1;
            db.SaveChanges();

            GradePointState state = db.GradePointStates.Find(student.GradePointStateId);
            double tuitionRate = 1000 * state.TuitionRateAdjustment(student);

            Console.WriteLine("Expected: 1150");
            Console.WriteLine("Actual: " + tuitionRate);
        }

        /// <summary>
        /// Test 2: A student with a GradePointAverage of 0.60 registering for a course that has a
        /// TuitionAmount of $1000 will be charged $1,120
        /// </summary>
        static void SuspendedState_GradePointAverage_060()
        {
            Student student = db.Students.Find(1);
            student.GradePointAverage = 0.60;
            student.GradePointStateId = 1;
            db.SaveChanges();

            GradePointState state = db.GradePointStates.Find(student.GradePointStateId);
            double tuitionRate = 1000 * state.TuitionRateAdjustment(student);

            Console.WriteLine("Expected: 1120");
            Console.WriteLine("Actual: " + tuitionRate);
        }

        /// <summary>
        /// Test 3: A student with a GPA 0.80 registering for a course that has a 
        /// TuitionAmount of $1000 will be charged $1,100.
        /// </summary>
        static void SuspendedState_GradePointAverage_080()
        {
            Student student = db.Students.Find(1);
            student.GradePointAverage = 0.80;
            student.GradePointStateId = 1;
            db.SaveChanges();

            GradePointState state = db.GradePointStates.Find(student.GradePointStateId);
            double tuitionRate = 1000 * state.TuitionRateAdjustment(student);

            Console.WriteLine("Expected: 1100");
            Console.WriteLine("Actual: " + tuitionRate);
        }

        //Probation States

        /// <summary>
        /// Test 1: A student with a GradePointAverage of 1.15 and has completed 3 courses, registering
        /// for a course that has a TuitionAmount of $1000 will be charged $1,075.
        /// </summary>
        static void ProbationState_GradePointAverage_115_Courses_3()
        {
            Student student = db.Students.Find(2);
            student.GradePointAverage = 1.15;
            student.GradePointStateId = 2;
            db.SaveChanges();

            GradePointState state = db.GradePointStates.Find(student.GradePointStateId);
            double tuitionRate = 1000 * state.TuitionRateAdjustment(student);

            Console.WriteLine("Expected: 1075");
            Console.WriteLine("Actual: " + tuitionRate);
        }

        /// <summary>
        /// Test 2: A student with a GradePointAverage 1.15 who has completed 7 courses, registering for
        /// a course that has a TuitionAmount of $1000 will be charged $1,035
        /// </summary>
        static void ProbationState_GradePointAverage_115_Courses_7()
        {
            Student student = db.Students.Find(4);
            student.GradePointAverage = 1.15;
            student.GradePointStateId = 2;
            db.SaveChanges();

            GradePointState state = db.GradePointStates.Find(student.GradePointStateId);
            double tuitionRate = 1000 * state.TuitionRateAdjustment(student);

            Console.WriteLine("Expected: 1035");
            Console.WriteLine("Actual: " + tuitionRate);
        }

        //Regular States

        /// <summary>
        /// Test 1: A student with a GradePointAverage of 2.50, registering for a course that has a
        /// TuitionAmount of $1000 will be charged $1,000
        /// </summary>
        static void RegularState_GradePointAverage_250()
        {
            Student student = db.Students.Find(1);
            student.GradePointAverage = 2.50;
            student.GradePointStateId = 3;
            db.SaveChanges();

            GradePointState state = db.GradePointStates.Find(student.GradePointStateId);
            double tuitionRate = 1000 * state.TuitionRateAdjustment(student);

            Console.WriteLine("Expected: 1000");
            Console.WriteLine("Actual: " + tuitionRate);
        }

        //Honours States

        /// <summary>
        /// Test 1: A student with a GradePointAverage 3.90 and has completed 3 courses, registering for a
        /// course that has a TuitionAmount of $1000 will be charged $900.
        /// </summary>
        static void HonoursState_GradePointAverage_390_Courses_3()
        {
            Student student = db.Students.Find(2);
            student.GradePointAverage = 3.90;
            student.GradePointStateId = 4;
            db.SaveChanges();

            GradePointState state = db.GradePointStates.Find(student.GradePointStateId);
            double tuitionRate = 1000 * state.TuitionRateAdjustment(student);

            Console.WriteLine("Expected: 900");
            Console.WriteLine("Actual: " + tuitionRate);
        }

        /// <summary>
        /// Test 2: A student with a GradePointAverage of 4.27 and has completed 4 courses, registering
        /// for a course that has a TuitionAmount of $1000 will be charged $880
        /// </summary>
        static void HonoursState_GradePointAverage_427_Courses_4()
        {
            Student student = db.Students.Find(5);
            student.GradePointAverage = 4.27;
            student.GradePointStateId = 4;
            db.SaveChanges();

            GradePointState state = db.GradePointStates.Find(student.GradePointStateId);
            double tuitionRate = 1000 * state.TuitionRateAdjustment(student);

            Console.WriteLine("Expected: 880");
            Console.WriteLine("Actual: " + tuitionRate);
        }

        /// <summary>
        /// Test 3: A student with a GradePointAverage of 4.40 and has completed 7 courses, registering
        /// for a course that has a TuitionAmount of $1000 will be charged $830
        /// </summary>
        static void HonoursState_GradePointAverage_440_Courses_7()
        {
            Student student = db.Students.Find(4);
            student.GradePointAverage = 4.40;
            student.GradePointStateId = 4;
            db.SaveChanges();

            GradePointState state = db.GradePointStates.Find(student.GradePointStateId);
            double tuitionRate = 1000 * state.TuitionRateAdjustment(student);

            Console.WriteLine("Expected: 830");
            Console.WriteLine("Actual: " + tuitionRate);
        }

        /// <summary>
        /// Test 4: A student with a GradePointAverage of 4.10 and has completed 7 courses, registering
        /// for a course that has a TuitionAmount of $1000 will be charged $850
        /// </summary>
        static void HonoursState_GradePointAverage_410_Courses_7()
        {
            Student student = db.Students.Find(4);
            student.GradePointAverage = 4.10;
            student.GradePointStateId = 4;
            db.SaveChanges();

            GradePointState state = db.GradePointStates.Find(student.GradePointStateId);
            double tuitionRate = 1000 * state.TuitionRateAdjustment(student);

            Console.WriteLine("Expected: 850");
            Console.WriteLine("Actual: " + tuitionRate);
        }
    }
}
