using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    // Database access class for Player/User entities
    // Inherits from BaseDB and provides full CRUD operations against the TableUsers table
    public class PlayerDB : BaseDB
    {
        // Override: Creates and returns a blank Player instance
        // Used by the base class Execute() method to instantiate entities from query results
        public override BaseEntity NewEntity()
        {
            return new Player();
        }

        // Override: Maps a single database reader row to a fully populated Player entity
        // Called by the base class for each row returned by a SELECT query
        public override BaseEntity CreateModel(BaseEntity entity)
        {
            Player player = new Player();

            // Map each column from the reader to the corresponding Player property
            player.Id = int.Parse(reader["UserID"].ToString());
            player.UserName = reader["UserName"].ToString();
            player.Email = reader["Email"].ToString();
            player.DateJoined = DateTime.Parse(reader["DateJoined"].ToString());
            player.UserType = reader["UserType"].ToString();
            player.GamesPlayed = int.Parse(reader["GamesPlayed"].ToString());
            player.Wins = int.Parse(reader["Wins"].ToString());
            player.Losses = int.Parse(reader["Losses"].ToString());
            player.Draws = int.Parse(reader["Draws"].ToString());
            // Map the Google OAuth ID (empty string if the player has no linked Google account)
            player.GoogleId = reader["GoogleId"].ToString();

            return player;
        }

        // Method: Retrieves all players from the database
        public PlayerList SelectAll()
        {
            command.CommandText = "SELECT * FROM TableUsers";
            return new PlayerList(Execute());
        }

        // Method: Retrieves a single player by their unique database ID
        // Returns null if no player with the given ID exists
        public Player SelectById(int id)
        {
            command.CommandText = "SELECT * FROM TableUsers WHERE UserID = @UserID";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@UserID", id);

            PlayerList list = new PlayerList(Execute());
            // Return the first match or null if not found
            return list.Count > 0 ? list[0] : null;
        }

        // Method: Retrieves all players whose username contains the given search string
        // Uses a LIKE query with wildcards to support partial name matching
        public PlayerList SelectByName(string name)
        {
            command.CommandText = "SELECT * FROM TableUsers WHERE UserName LIKE @Name";
            command.Parameters.Clear();
            // Wrap name in wildcards to match any username containing the search term
            command.Parameters.AddWithValue("@Name", "%" + name + "%");

            return new PlayerList(Execute());
        }

        // Override: Inserts a new player record into the database
        // GoogleID defaults to 0 for admin-created accounts that are not linked to Google
        // Returns the number of rows affected (1 on success, 0 on failure)
        public override int Insert(BaseEntity entity)
        {
            Player player = entity as Player;

            command.CommandText = @"INSERT INTO TableUsers 
                (UserName, Email, DateJoined, UserType, GoogleID, GamesPlayed, Wins, Losses, Draws)
                VALUES (@UserName, @Email, @DateJoined, @UserType, @GoogleId, @GamesPlayed, @Wins, @Losses, @Draws)";

            command.Parameters.Clear();
            command.Parameters.AddWithValue("@UserName", player.UserName);
            command.Parameters.AddWithValue("@Email", player.Email);
            command.Parameters.AddWithValue("@DateJoined", player.DateJoined);
            command.Parameters.AddWithValue("@UserType", player.UserType);
            command.Parameters.AddWithValue("@GoogleId", player.GoogleId);
            command.Parameters.AddWithValue("@GamesPlayed", player.GamesPlayed);
            command.Parameters.AddWithValue("@Wins", player.Wins);
            command.Parameters.AddWithValue("@Losses", player.Losses);
            command.Parameters.AddWithValue("@Draws", player.Draws);

            return ExecuteChange();
        }

        // Override: Deletes a player record from the database by their unique ID
        // Returns the number of rows affected (1 on success, 0 if player not found)
        public override int Delete(BaseEntity entity)
        {
            Player player = entity as Player;

            command.CommandText = "DELETE FROM TableUsers WHERE UserID = @UserID";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@UserID", player.Id);

            return ExecuteChange();
        }

        // Override: Updates all fields of an existing player record in the database
        // Includes stats (wins, losses, draws) and Google ID for OAuth account linking
        // Returns the number of rows affected (1 on success, 0 if player not found)
        public override int Update(BaseEntity entity)
        {
            Player player = entity as Player;

            command.CommandText = @"UPDATE TableUsers SET
                UserName    = @UserName,
                Email       = @Email,
                DateJoined  = @DateJoined,
                UserType    = @UserType,
                GamesPlayed = @GamesPlayed,
                Wins        = @Wins,
                Losses      = @Losses,
                Draws       = @Draws,
                GoogleId    = @GoogleId
                WHERE UserID = @UserID";

            command.Parameters.Clear();
            command.Parameters.AddWithValue("@UserName", player.UserName);
            command.Parameters.AddWithValue("@Email", player.Email);
            command.Parameters.AddWithValue("@DateJoined", player.DateJoined);
            command.Parameters.AddWithValue("@UserType", player.UserType);
            command.Parameters.AddWithValue("@GamesPlayed", player.GamesPlayed);
            command.Parameters.AddWithValue("@Wins", player.Wins);
            command.Parameters.AddWithValue("@Losses", player.Losses);
            command.Parameters.AddWithValue("@Draws", player.Draws);
            // WHERE clause parameter — identifies which record to update
            command.Parameters.AddWithValue("@UserID", player.Id);
            // Updated Google ID for account linking or unlinking scenarios
            command.Parameters.AddWithValue("@GoogleId", player.GoogleId);

            return ExecuteChange();
        }

        // Method: Looks up a player by their Google ID and email for Google OAuth login
        // Both fields must match to prevent unauthorized access with a stolen email or Google ID alone
        // Returns the matching Player, or null if no account is found
        public Player Login(string googleID, string email)
        {
            command.CommandText = "SELECT * FROM TableUsers WHERE GoogleID = @GoogleID AND Email = @Email";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@GoogleID", googleID);
            command.Parameters.AddWithValue("@Email", email);

            PlayerList list = new PlayerList(Execute());
            // Return the matched player or null if credentials don't match any record
            return list.Count > 0 ? list[0] : null;
        }
    }
}