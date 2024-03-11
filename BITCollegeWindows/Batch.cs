using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using BITCollege_CS.Data;
using BITCollege_CS.Models;
using System.ServiceModel.Configuration;
using Utility;
using System.Globalization;

namespace BITCollegeWindows
{
    /// <summary>
    /// Batch:  This class provides functionality that will validate
    /// and process incoming xml files.
    /// </summary>
    public class Batch
    {
        protected static BITCollege_CSContext db = new BITCollege_CSContext();
        CollegeRegistrationService.CollegeRegistrationClient service =
                new CollegeRegistrationService.CollegeRegistrationClient();

        private String inputFileName;
        private String logFileName;
        private String logData;

        /// <summary>
        /// Process all detail errors found within the current file being processed.
        /// </summary>
        /// <param name="beforeQuery">The records that existed before the round of validation.</param>
        /// <param name="afterQuery">The records that remained following the round of validation.</param>
        /// <param name="message">The error message that is to be written to the log file based on the record failing the round of validation.</param>
        private void ProcessErrors(IEnumerable<XElement> beforeQuery, IEnumerable<XElement> afterQuery, String message)
        {
            IEnumerable<XElement> failedTransactions = beforeQuery.Except(afterQuery);

            foreach (XElement failedTransaction in failedTransactions)
            {
                logData += "\r\n---------ERROR----------";
                logData += "\r\nFile: " + inputFileName;
                logData += "\r\nProgram: " + failedTransaction.Element("program");
                logData += "\r\nStudent Number: " + failedTransaction.Element("student_no");
                logData += "\r\nCourse Number: " + failedTransaction.Element("course_no");
                logData += "\r\nRegistration Number: " + failedTransaction.Element("registration_no");
                logData += "\r\nType: " + failedTransaction.Element("type");
                logData += "\r\nGrade: " + failedTransaction.Element("grade");
                logData += "\r\nNotes: " + failedTransaction.Element("notes");
                logData += "\r\nNode Count: " + failedTransaction.Nodes().Count();
                logData += "\r\nError Message: " + message;
                logData += "\r\n------------------------";
            }
        }

        /// <summary>
        /// Verify the attributes of the xml file’s root element.
        /// </summary>
        private void ProcessHeader()
        {
            XDocument xDocument = XDocument.Load(inputFileName);

            XElement root = xDocument.Element("student_update");

            if (root.Attributes().Count() != 3)
            {
                throw new Exception("student_update element does not have 3 attributes.");
            }

            if (root.Attribute("date").Value != DateTime.Now.ToString("yyyy-MM-dd"))
            {
                throw new Exception("The date attribute does not match the current date.");
            }

            string program = root.Attribute("program").Value;

            IEnumerable<AcademicProgram> programs = 
                        db.AcademicPrograms.Where(x => x.ProgramAcronym == program);

            if (programs.Count() == 0)
            {
                throw new Exception("The program attribute does not match an academic program acronym");
            }

            long sumOfStudentNumbers = root.Descendants("student_no").Sum(x => long.Parse(x.Value));

            if (sumOfStudentNumbers.ToString() != root.Attribute("checksum").Value) 
            {
                throw new Exception("The checksum attribute is not the total of all student_no elements.");
            }
        }

        /// <summary>
        /// Check the contents of the file and sort out invalid transactions.
        /// </summary>
        private void ProcessDetails()
        {
            XDocument xDocument = XDocument.Load(inputFileName);

            IEnumerable<XElement> first = xDocument.Descendants().Elements("transaction");

            IEnumerable<XElement> second = first.Where(x => x.Nodes().Count() == 7);

            ProcessErrors(first, second, "Nodes count is not 7.");

            IEnumerable<XElement> third =
                        second.Where(x => x.Element("program").Value == xDocument.Root.Attribute("program").Value);

            ProcessErrors(second, third, "Transaction program does not match the root program.");

            IEnumerable<XElement> fourth =
                        third.Where(x => Numeric.IsNumeric(x.Element("type").Value, NumberStyles.Integer));

            ProcessErrors(third, fourth, "Transaction type is not a number.");

            IEnumerable<XElement> fifth =
                        fourth.Where(x => Numeric.IsNumeric(x.Element("grade").Value, NumberStyles.Any) || x.Element("grade").Value == "*");

            ProcessErrors(fourth, fifth, "Transaction grade is not a number or equal to *.");

            IEnumerable<XElement> sixth =
                        fifth.Where(x => x.Element("type").Value == "1" || x.Element("type").Value == "2");

            ProcessErrors(fifth, sixth, "Transaction type is not equal to 1 or 2.");

            IEnumerable<XElement> seventh =
                        sixth.Where(x =>
                            (x.Element("type").Value == "1" && x.Element("grade").Value == "*") ||
                            (x.Element("type").Value == "2" && (double.Parse(x.Element("grade").Value) <= 100 && double.Parse(x.Element("grade").Value) >= 0)));

            ProcessErrors(sixth, seventh, "Transaction grade is not equal to * when the type is 1 or Transaction grade is not between 0 adn 100 when the type is 2.");

            IEnumerable<long> studentNumbers = db.Students.Select(x => x.StudentNumber).ToList();

            IEnumerable<XElement> eight = 
                        seventh.Where(x => studentNumbers.Contains(long.Parse(x.Element("student_no").Value)));

            ProcessErrors(seventh, eight, "Transaction student_no is not in the database.");

            IEnumerable<string> courseNumbers = db.Courses.Select(x => x.CourseNumber).ToList();

            IEnumerable<XElement> ninth =
                        eight.Where(x =>
                            (x.Element("type").Value == "2" && x.Element("course_no").Value == "*") ||
                            (x.Element("type").Value == "1" && courseNumbers.Contains(x.Element("course_no").Value)));

            ProcessErrors(eight, ninth, "Transaction course_no is not in the database when its type 1 or its not * when its type 2.");

            IEnumerable<long> registrationNumbers = db.Registrations.Select(x => x.RegistrationNumber).ToList();

            IEnumerable<XElement> tenth =
                        ninth.Where(x =>
                            (x.Element("type").Value == "1" && x.Element("registration_no").Value == "*") ||
                            (x.Element("type").Value == "2" && registrationNumbers.Contains(long.Parse(x.Element("registration_no").Value))));

            ProcessErrors(ninth, tenth, "Transaction registration_no is not in the database when its type 2 or its not * when its type 1.");

            ProcessTransactions(tenth);
        }

        /// <summary>
        /// Use the contents of all valid transactions in a WCF Service.
        /// </summary>
        /// <param name="transactionRecords">The valid transactions</param>
        private void ProcessTransactions(IEnumerable<XElement> transactionRecords)
        {
            foreach (XElement transaction in transactionRecords) 
            {
                long studentNumber = long.Parse(transaction.Element("student_no").Value);
                Student student = db.Students.Where(x => x.StudentNumber == studentNumber).SingleOrDefault();
                int studentId = student.StudentId;
                string type = transaction.Element("type").Value;
                string notes = transaction.Element("notes").Value;

                if (type == "1")
                {
                    string courseNumber = transaction.Element("course_no").Value;
                    Course course = db.Courses.Where(x => x.CourseNumber == courseNumber).SingleOrDefault();
                    int courseId = course.CourseId;
                    int registrationCode = service.RegisterCourse(studentId, courseId, notes);

                    if (registrationCode != 0)
                    {
                        string registrationErrorMessage = BusinessRules.RegisterError(registrationCode);

                        logData += "\r\nRegistration Error: " + registrationErrorMessage;
                    }
                    else
                    {
                        logData += "\r\nStudent: " + studentNumber + " has successfuly registered for " + courseNumber +".";
                    }
                }
                else 
                {
                    try
                    {
                        double grade = double.Parse(transaction.Element("grade").Value);
                        long registrationNumber = long.Parse(transaction.Element("registration_no").Value);
                        Registration registration = db.Registrations.Where(x => x.RegistrationNumber == registrationNumber).SingleOrDefault();
                        int registrationId = registration.RegistrationId;

                        double updatedGrade = grade / 100;

                        double? gradePointAverage = service.UpdateGrade(updatedGrade, registrationId, notes);

                        if (gradePointAverage == null)
                        {
                            logData += "\r\nError A grade of: " + grade + " has not been applied to registration: " + registrationNumber + ".";
                        }
                        else
                        {
                            logData += "\r\nA grade of: " + grade + " has been successfully applied to registration: " + registrationNumber + ".";
                        }
                    }
                    catch (Exception e)
                    {
                        logData += "\r\nException :" + e.Message + "has occured.";
                    }
                }
             
            }
        }

        /// <summary>
        /// Writes the information in te logData into a file.
        /// </summary>
        /// <returns>The captured logging data</returns>
        public String WriteLogData()
        {
            StreamWriter log = new StreamWriter(logFileName);
            log.Write(logData);
            log.Close();
            string capturedLogData = logData;
            logData = "";
            logFileName = "";
            return capturedLogData;
        }

        /// <summary>
        /// Initiate the batch process by determining the appropriate filename.
        /// </summary>
        /// <param name="programAcronym">The program acronym that appears in the file name.</param>
        public void ProcessTransmission(String programAcronym)
        {
            inputFileName = DateTime.Now.Year + "-" + DateTime.Now.DayOfYear + "-" +
                            programAcronym + ".xml";
            logFileName = "LOG " + inputFileName.Replace("xml", "txt");

            if (File.Exists(inputFileName))
            {
                try
                {
                    ProcessHeader();
                    ProcessDetails();
                }
                catch(Exception e) 
                {
                    logData += "\r\n" + e.Message + "exception has occured.";
                }
                    
            }
            else
            {
                logData += "\r\nFile: " + inputFileName + " does not exist.";
            }

        }
}
}
