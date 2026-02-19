using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    // Database access class for Player/User entities
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

            // Read basic user information from database
            player.Id = int.Parse(reader["UserID"].ToString());
            player.UserName = reader["UserName"].ToString();
            player.Email = reader["Email"].ToString();
            player.DateJoined = DateTime.Parse(reader["DateJoined"].ToString());
            player.Elo = int.Parse(reader["Elo"].ToString());
            player.UserType = reader["UserType"].ToString();
            player.GamesPlayed = int.Parse(reader["GamesPlayed"].ToString());
            player.Wins = int.Parse(reader["Wins"].ToString());


            player.Losses = int.Parse(reader["Losses"].ToString());
            player.Draws = int.Parse(reader["Draws"].ToString());

            // Return the filled player
            return player;
        }

        // Method: Selects all players from database
        public PlayerList SelectAll()
        {
            // Set SQL command to select all users with statistics
            command.CommandText = "SELECT * FROM TableUsers";
            // Execute query and return list of players
            return new PlayerList(Execute());
        }

        // Method: Selects player by ID
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

        // Method: Selects players by name pattern
        public PlayerList SelectByName(string name)
        {
            // Set SQL command to search by name
            command.CommandText = "SELECT * FROM TableUsers WHERE UserName LIKE @Name";
            // Clear previous parameters
            command.Parameters.Clear();
            // Add name parameter with wildcards for partial match
            command.Parameters.AddWithValue("@Name", "%" + name + "%");

            // Execute query and return list of players
            return new PlayerList(Execute());
        }

        // Override: Inserts new player into database
        public override int Insert(BaseEntity entity)
        {
            // Cast entity to Player type
            Player player = entity as Player;

            // Set SQL command for insertion (including statistics fields)
            command.CommandText = @"INSERT INTO TableUsers 
                (UserName, Email, DateJoined, Elo, UserType, GoogleID, GamesPlayed, Wins, Losses, Draws)
                VALUES (@UserName, @Email, @DateJoined, @Elo, @UserType, @GoogleId, @GamesPlayed, @Wins, @Losses, @Draws)";

            // Clear previous parameters
            command.Parameters.Clear();
            // Add all parameters
            command.Parameters.AddWithValue("@UserName", player.UserName);
            command.Parameters.AddWithValue("@Email", player.Email);
            command.Parameters.AddWithValue("@DateJoined", player.DateJoined);
            command.Parameters.AddWithValue("@Elo", player.Elo);
            command.Parameters.AddWithValue("@UserType", player.UserType);
            command.Parameters.AddWithValue("@GoogleId", ""); // Changed from 0 to empty string for GoogleID
            command.Parameters.AddWithValue("@GamesPlayed", player.GamesPlayed);
            command.Parameters.AddWithValue("@Wins", player.Wins);
            command.Parameters.AddWithValue("@Losses", player.Losses);
            command.Parameters.AddWithValue("@Draws", player.Draws);

            // Execute and return number of affected rows
            return ExecuteChange();
        }

        // Override: Deletes player from database
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

        // Override: Updates existing player in database
        public override int Update(BaseEntity entity)
        {
            // Cast entity to Player type
            Player player = entity as Player;

            command.CommandText = @"UPDATE TableUsers SET
                UserName = @UserName,
                Email = @Email,
                DateJoined = @DateJoined,
                Elo = @Elo,
                UserType = @UserType,
                GamesPlayed = @GamesPlayed,
                Wins = @Wins,
                Losses = @Losses,
                Draws = @Draws
                WHERE UserID = @UserID";

            // Clear previous parameters
            command.Parameters.Clear();
            // Add all parameters
            command.Parameters.AddWithValue("@UserName", player.UserName);
            command.Parameters.AddWithValue("@Email", player.Email);
            command.Parameters.AddWithValue("@DateJoined", player.DateJoined);
            command.Parameters.AddWithValue("@Elo", player.Elo);
            command.Parameters.AddWithValue("@UserType", player.UserType);
            command.Parameters.AddWithValue("@GamesPlayed", player.GamesPlayed);
            command.Parameters.AddWithValue("@Wins", player.Wins);
            command.Parameters.AddWithValue("@Losses", player.Losses);
            command.Parameters.AddWithValue("@Draws", player.Draws);
            command.Parameters.AddWithValue("@UserID", player.Id);

            // Execute and return number of affected rows
            return ExecuteChange();
        }

        // Method: Login user by GoogleID and Email
        public Player Login(string googleID, string email)
        {
            // Set SQL command to find user
            command.CommandText = "SELECT * FROM TableUsers WHERE GoogleID = @GoogleID AND Email = @Email";
            // Clear previous parameters
            command.Parameters.Clear();
            // Add parameters
            command.Parameters.AddWithValue("@GoogleID", googleID);
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