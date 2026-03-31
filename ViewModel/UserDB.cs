using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    // Database access class for Player/User entities
    // Inherits from BaseDB and provides CRUD operations for TableUsers
    public class UserDB : BaseDB
    {
        // Override: Creates a new Player entity
        public override BaseEntity NewEntity()
        {
            // Return new Player instance
            return new Player();
        }

        // Override: Fills Player entity with data from database reader
        public override BaseEntity CreateModel(BaseEntity entity)
        {
            // Create new Player instance
            Player player = new Player();

            // Read user ID from database
            player.Id = int.Parse(reader["UserID"].ToString());
            // Read username from database
            player.UserName = reader["UserName"].ToString();
            // Read email address from database
            player.Email = reader["Email"].ToString();
            // Read date the player joined from database
            player.DateJoined = DateTime.Parse(reader["DateJoined"].ToString());
            // Read user type (Admin/Registered) from database
            player.UserType = reader["UserType"].ToString();
            // Read total games played count from database
            player.GamesPlayed = int.Parse(reader["GamesPlayed"].ToString());
            // Read total wins count from database
            player.Wins = int.Parse(reader["Wins"].ToString());
            // Read total losses count from database
            player.Losses = int.Parse(reader["Losses"].ToString());
            // Read total draws count from database
            player.Draws = int.Parse(reader["Draws"].ToString());

            // Return the filled player
            return player;
        }

        // Method: Selects all players from database
        public PlayerList SelectAll()
        {
            // Set SQL command to select all users
            command.CommandText = "SELECT * FROM TableUsers";
            // Execute query and return list of players
            return new PlayerList(Execute());
        }

        // Method: Selects a single player by their ID
        public Player SelectById(int id)
        {
            // Set SQL command to select by ID
            command.CommandText = "SELECT * FROM TableUsers WHERE UserID = @UserID";
            // Clear previous parameters
            command.Parameters.Clear();
            // Add ID parameter
            command.Parameters.AddWithValue("@UserID", id);

            // Execute query and get list
            PlayerList list = new PlayerList(Execute());
            // If found, return first item
            if (list.Count > 0)
                return list[0];
            // Otherwise return null
            return null;
        }

        // Method: Selects players whose username matches a search pattern
        public PlayerList SelectByName(string name)
        {
            // Set SQL command to search by name using LIKE for partial match
            command.CommandText = "SELECT * FROM TableUsers WHERE UserName LIKE @Name";
            // Clear previous parameters
            command.Parameters.Clear();
            // Add name parameter with wildcards for partial match
            command.Parameters.AddWithValue("@Name", "%" + name + "%");

            // Execute query and return list of matching players
            return new PlayerList(Execute());
        }

        // Override: Inserts a new player into the database
        // Returns number of affected rows
        public override int Insert(BaseEntity entity)
        {
            // Cast entity to Player type
            Player player = entity as Player;

            // Set SQL command for insertion including all player fields and statistics
            command.CommandText = @"INSERT INTO TableUsers 
                (UserName, Email, DateJoined, UserType, GoogleID, GamesPlayed, Wins, Losses, Draws)
                VALUES (@UserName, @Email, @DateJoined, @UserType, @GoogleId, @GamesPlayed, @Wins, @Losses, @Draws)";

            // Clear previous parameters
            command.Parameters.Clear();
            // Add username parameter
            command.Parameters.AddWithValue("@UserName", player.UserName);
            // Add email parameter
            command.Parameters.AddWithValue("@Email", player.Email);
            // Add date joined parameter
            command.Parameters.AddWithValue("@DateJoined", player.DateJoined);
            // Add user type parameter
            command.Parameters.AddWithValue("@UserType", player.UserType);
            // Add Google ID parameter (default 0 for admin-created users)
            command.Parameters.AddWithValue("@GoogleId", 0);
            // Add games played count parameter
            command.Parameters.AddWithValue("@GamesPlayed", player.GamesPlayed);
            // Add wins count parameter
            command.Parameters.AddWithValue("@Wins", player.Wins);
            // Add losses count parameter
            command.Parameters.AddWithValue("@Losses", player.Losses);
            // Add draws count parameter
            command.Parameters.AddWithValue("@Draws", player.Draws);

            // Execute and return number of affected rows
            return ExecuteChange();
        }

        // Override: Deletes a player from the database by their ID
        // Returns number of affected rows
        public override int Delete(BaseEntity entity)
        {
            // Cast entity to Player type
            Player player = entity as Player;

            // Set SQL command for deletion
            command.CommandText = "DELETE FROM TableUsers WHERE UserID = @UserID";
            // Clear previous parameters
            command.Parameters.Clear();
            // Add user ID parameter
            command.Parameters.AddWithValue("@UserID", player.Id);

            // Execute and return number of affected rows
            return ExecuteChange();
        }

        // Override: Updates an existing player's information in the database
        // Returns number of affected rows
        public override int Update(BaseEntity entity)
        {
            // Cast entity to Player type
            Player player = entity as Player;

            // Set SQL command to update all player fields
            command.CommandText = @"UPDATE TableUsers SET
                UserName   = @UserName,
                Email      = @Email,
                DateJoined = @DateJoined,
                UserType   = @UserType,
                GamesPlayed = @GamesPlayed,
                Wins       = @Wins,
                Losses     = @Losses,
                Draws      = @Draws
                WHERE UserID = @UserID";

            // Clear previous parameters
            command.Parameters.Clear();
            // Add username parameter
            command.Parameters.AddWithValue("@UserName", player.UserName);
            // Add email parameter
            command.Parameters.AddWithValue("@Email", player.Email);
            // Add date joined parameter
            command.Parameters.AddWithValue("@DateJoined", player.DateJoined);
            // Add user type parameter
            command.Parameters.AddWithValue("@UserType", player.UserType);
            // Add games played count parameter
            command.Parameters.AddWithValue("@GamesPlayed", player.GamesPlayed);
            // Add wins count parameter
            command.Parameters.AddWithValue("@Wins", player.Wins);
            // Add losses count parameter
            command.Parameters.AddWithValue("@Losses", player.Losses);
            // Add draws count parameter
            command.Parameters.AddWithValue("@Draws", player.Draws);
            // Add user ID parameter for WHERE clause
            command.Parameters.AddWithValue("@UserID", player.Id);

            // Execute and return number of affected rows
            return ExecuteChange();
        }

        // Method: Finds a player by their Google ID and email for Firebase authentication
        // Used during login to verify the player exists in the database
        public Player Login(string googleID, string email)
        {
            // Set SQL command to find user by Google ID and email
            command.CommandText = "SELECT * FROM TableUsers WHERE GoogleID = @GoogleID AND Email = @Email";
            // Clear previous parameters
            command.Parameters.Clear();
            // Add Google ID parameter
            command.Parameters.AddWithValue("@GoogleID", googleID);
            // Add email parameter
            command.Parameters.AddWithValue("@Email", email);

            // Execute query and get list
            PlayerList list = new PlayerList(Execute());
            // If found, return first item
            if (list.Count > 0)
                return list[0];
            // Otherwise return null
            return null;
        }
    }
}