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
    public abstract class BaseDB
    {
        // Field: Connection string for database access
        protected string connectionString;
        // Field: SQL connection object
        protected SqlConnection connection;
        // Field: SQL command object for executing queries
        protected SqlCommand command;
        // Field: SQL data reader for reading query results
        protected SqlDataReader reader;

        // Abstract method: Creates a new instance of the specific entity type
        public abstract BaseEntity NewEntity();
        // Abstract method: Fills entity with data from database reader
        public abstract BaseEntity CreateModel(BaseEntity entity);
        // Abstract method: Inserts a new entity into the database
        public abstract int Insert(BaseEntity entity);
        // Abstract method: Updates an existing entity in the database
        public abstract int Update(BaseEntity entity);
        // Abstract method: Deletes an entity from the database
        public abstract int Delete(BaseEntity entity);

        // Constructor: Initializes database connection and command objects
        public BaseDB()
        {
            // Set the connection string to the local database file
            connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename="+Path()+@"\ChessDatabase.mdf;Integrated Security=True";
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

                // Loop through all rows returned
                while (reader.Read())
                {
                    // Create new entity instance
                    BaseEntity entity = NewEntity();
                    // Fill entity with data and add to list
                    list.Add(CreateModel(entity));
                }
            }

            catch (Exception ex)
            {
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

            // Return the list of entities
            return list;
        }

        // Method: Executes INSERT, UPDATE, or DELETE command
        public int ExecuteChange()
        {
            // Variable to store number of affected rows
            int records=0;

            try
            {
                // Open connection if not already open
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                // Execute the command and get number of affected rows
                records = command.ExecuteNonQuery();
            }

            catch(Exception ex)
            {
                Console.WriteLine("A general error occurred: " + ex.Message);
            }

            finally
            {
                // Close connection if it's open
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            // Return number of affected rows
            return records;
        }

        private static string Path()
        {
            string s = Environment.CurrentDirectory; // המיקום שבו רץ הפרויקט
            string[] sub = s.Split('\\'); // פירוק מחרוזת הכתובת למערך לפי תיקיות

            int index = sub.Length - 3; // חזרה אחורה 2 תיקיות
            sub[index] = "ViewModel"; // שינוי התיקיה לתיקיה המתאימה
            Array.Resize(ref sub, index + 1); // תיקון של אורך המערך, לאורך המתאים לתיקייה

            s = String.Join("\\", sub); // חיבור מחדש של המערך
            return s;
        }
    }
}