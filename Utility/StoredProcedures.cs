using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// Contains Stored Procedures that can be used in the database.
    /// </summary>
    public static class StoredProcedures
    {
        /// <summary>
        /// Retrieves the next available number based on the discriminator, but returns null if an exception happens.
        /// </summary>
        /// <param name="discriminator">The type of the NextUniqueNumber that the next number will be based on.</param>
        /// <returns>the next available number based on the discriminator.</returns>
        public static long? NextNumber(string discriminator)
        {
            try
            {
                long? returnValue = 0;
                SqlConnection connection = new SqlConnection("Data Source=ORYXX\\CYRUS; " +
                             "Initial Catalog=BITCollege_CSContext;Integrated Security=True");
                SqlCommand storedProcedure = new SqlCommand("next_number", connection);
                storedProcedure.CommandType = CommandType.StoredProcedure;
                storedProcedure.Parameters.AddWithValue("@Discriminator", discriminator);
                SqlParameter outputParameter = new SqlParameter("@NewVal", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.Output
                };
                storedProcedure.Parameters.Add(outputParameter);
                connection.Open();
                storedProcedure.ExecuteNonQuery();
                connection.Close();
                returnValue = (long?)outputParameter.Value;
                return returnValue;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
