using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BITCollegeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICollegeRegistration" in both code and config file together.

    /// <summary>
    /// Handles the changes in registration using WCF Services.
    /// </summary>
    [ServiceContract]
    public interface ICollegeRegistration
    {
        /// <summary>
        /// Deletes a registration.
        /// </summary>
        /// <param name="registrationId">The registrationId of the registration that will get deleted.</param>
        /// <returns>Returns true if the process is successfuk and false if it is not.</returns>
        [OperationContract]
        bool DropCourse(int registrationId);

        /// <summary>
        /// Creates a new registration.
        /// </summary>
        /// <param name="studentId">The studentId of teh new registration.</param>
        /// <param name="courseId">The courseId of teh new registration.</param>
        /// <param name="notes">The notes of teh new registration.</param>
        /// <returns>Returns an error code if a problem exist.</returns>
        [OperationContract]
        int RegisterCourse(int studentId, int courseId, string notes);

        /// <summary>
        /// Chnages the grade of a registration.
        /// </summary>
        /// <param name="grade">The new grade of the registration.</param>
        /// <param name="registrationId">The registrationId of the registration that will get updated.</param>
        /// <param name="notes">The notes grade of the registration.</param>
        /// <returns>The new GPA of the student in this registration.</returns>
        [OperationContract]
        double? UpdateGrade(double grade, int registrationId, string notes);
    }
}
