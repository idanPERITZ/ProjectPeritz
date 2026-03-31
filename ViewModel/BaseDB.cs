using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace ViewModel
{
    // Abstract base class for all database operations - provides common functionality
    // All database classes (GameDB, UserDB, MoveDB) inherit from this class
    public abstract class BaseDB
    {
        // Field: Connection string for database access
        protected string connectionString;
        // Field: SQL connection object for connecting to the database
        protected SqlConnection connection;
        // Field: SQL command object for executing queries
        protected SqlCommand command;
        // Field: SQL data reader for reading query results row by row
        protected SqlDataReader reader;

        // Abstract method: Creates a new instance of the specific entity type
        // Each subclass implements this to return its own entity type
        public abstract BaseEntity NewEntity();

        // Abstract method: Fills entity with data from database reader
        // Each subclass implements this to map columns to entity properties
        public abstract BaseEntity CreateModel(BaseEntity entity);

        // Abstract method: Inserts a new entity into the database
        // Returns number of affected rows
        public abstract int Insert(BaseEntity entity);

        // Abstract method: Updates an existing entity in the database
        // Returns number of affected rows
        public abstract int Update(BaseEntity entity);

        // Abstract method: Deletes an entity from the database
        // Returns number of affected rows
        public abstract int Delete(BaseEntity entity);

        // Constructor: Initializes database connection and command objects
        public BaseDB()
        {
            // Set the connection string to the local database file
            connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Path() + @"\ChessDatabase.mdf;Integrated Security=True";
            // Create new SQL connection with the connection string
            connection = new SqlConnection(connectionString);
            // Create new SQL command and assign the connection
            command = new SqlCommand
            {
                Connection = connection
            };
        }

        // Method: Executes a SELECT query and returns list of entities
        public List<BaseEntity> Execute()
        {
            // Create empty list to store results
            List<BaseEntity> list = new List<BaseEntity>();

            try
            {
                // Open connection to database
                connection.Open();
                // Execute the command and get data reader
                reader = command.ExecuteReader();

                // Loop through all rows returned by the query
                while (reader.Read())
                {
                    // Create new entity instance of the specific type
                    BaseEntity entity = NewEntity();
                    // Fill entity with data from current row and add to list
                    list.Add(CreateModel(entity));
                }
            }

            catch (Exception ex)
            {
                // Log error to console if query fails
                Console.WriteLine("A general error occurred: " + ex.Message);
            }

            finally
            {
                // Close the reader if it's open
                reader?.Close();
                // Close connection if it's open
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            // Return the list of entities (empty if error occurred)
            return list;
        }

        // Method: Executes INSERT, UPDATE, or DELETE command
        // Returns number of rows affected by the operation
        public int ExecuteChange()
        {
            // Variable to store number of affected rows
            int records = 0;

            try
            {
                // Open connection if not already open
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                // Execute the command and get number of affected rows
                records = command.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                // Log error to console if command fails
                Console.WriteLine("A general error occurred: " + ex.Message);
            }

            finally
            {
                // Close connection if it's open
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            // Return number of affected rows (0 if error occurred)
            return records;
        }

        // Private method: Builds the path to the database file
        // Navigates from the current directory back to the ViewModel folder
        private static string Path()
        {
            // Get the current working directory where the project runs
            string s = Environment.CurrentDirectory;
            // Split the path into parts by backslash
            string[] sub = s.Split('\\');

            // Go back 3 levels from current directory
            int index = sub.Length - 3;
            // Replace that folder name with "ViewModel"
            sub[index] = "ViewModel";
            // Resize the array to only include up to the ViewModel folder
            Array.Resize(ref sub, index + 1);

            // Rejoin the path parts into a full path string
            s = String.Join("\\", sub);
            return s;
        }
    }
}