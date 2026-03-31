using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    // Database access class for Game entities
    // Inherits from BaseDB and provides CRUD operations for TableGames
    public class GameDB : BaseDB
    {
        // Override: Creates a new Game entity
        public override BaseEntity NewEntity()
        {
            // Return new Game instance
            return new Game();
        }

        // Override: Fills Game entity with data from database reader
        public override BaseEntity CreateModel(BaseEntity entity)
        {
            // Cast entity to Game type
            Game game = entity as Game;
            // Create UserDB instance to load player data
            UserDB userDB = new UserDB();

            // Load white player by ID from database
            game.WhitePlayerUserID = userDB.SelectById(int.Parse(reader["WhitePlayerUserID"].ToString()));
            // Load black player by ID from database
            game.BlackPlayerUserID = userDB.SelectById(int.Parse(reader["BlackPlayerUserID"].ToString()));

            // Check if Result field has value (not null means someone won)
            if (reader["Result"] != DBNull.Value)
            {
                // Load winner player by ID
                game.Result = userDB.SelectById(int.Parse(reader["Result"].ToString()));
            }

            else
            {
                // No winner means draw - set Result to null
                game.Result = null;
            }

            // Read game date from database
            game.GameDate = DateTime.Parse(reader["GameDate"].ToString());
            // Read game ID from GameID column
            game.Id = int.Parse(reader["GameID"].ToString());
            // Sync GameID property with Id to ensure both are consistent
            game.GameID = game.Id;

            // Return the filled game
            return game;
        }

        // Method: Selects all games from database
        public GameList SelectAll()
        {
            // Set SQL command to select all games
            command.CommandText = "SELECT * FROM TableGames";
            // Execute query and return list of games
            return new GameList(Execute());
        }

        // Method: Selects a single game by its ID
        public Game SelectById(int id)
        {
            // Set SQL command to select by ID
            command.CommandText = "SELECT * FROM TableGames WHERE GameID = @GameID";
            // Clear previous parameters
            command.Parameters.Clear();
            // Add game ID parameter
            command.Parameters.AddWithValue("@GameID", id);

            // Execute query and get list
            GameList list = new GameList(Execute());
            // If found, return first item
            if (list.Count > 0)
                return list[0];
            // Otherwise return null
            return null;
        }

        // Method: Selects all games where a specific player participated (as white or black)
        public GameList SelectByPlayer(int playerId)
        {
            // Set SQL command to find games where player was white or black
            command.CommandText = @"SELECT * FROM TableGames 
                WHERE WhitePlayerUserID = @PlayerID OR BlackPlayerUserID = @PlayerID";
            // Clear previous parameters
            command.Parameters.Clear();
            // Add player ID parameter
            command.Parameters.AddWithValue("@PlayerID", playerId);

            // Execute query and return list of games
            return new GameList(Execute());
        }

        // Method: Finds a specific game by its white player, black player, and date
        // Used after insertion to retrieve the newly created game
        public Game GetGame(Game game)
        {
            // Set SQL command to find game by both players and date
            command.CommandText = @"SELECT * FROM TableGames 
                WHERE WhitePlayerUserID = @WhitePlayerID AND BlackPlayerUserID = @BlackPlayerID AND GameDate = @GameDate";
            // Clear previous parameters
            command.Parameters.Clear();
            // Add white player ID parameter
            command.Parameters.AddWithValue("@WhitePlayerID", game.WhitePlayerUserID.Id);
            // Add black player ID parameter
            command.Parameters.AddWithValue("@BlackPlayerID", game.BlackPlayerUserID.Id);
            // Add game date parameter
            command.Parameters.AddWithValue("@GameDate", game.GameDate);

            // Execute query and get list
            GameList list = new GameList(Execute());
            // If found, return first item
            if (list.Count > 0)
                return list[0];
            // Otherwise return null
            return null;
        }

        // Override: Inserts a new game into the database
        // Returns number of affected rows
        public override int Insert(BaseEntity entity)
        {
            // Cast entity to Game type
            Game game = entity as Game;

            // Set SQL command for insertion
            command.CommandText = @"INSERT INTO TableGames 
                (WhitePlayerUserID, BlackPlayerUserID, GameDate, Result)
                VALUES (@WhitePlayerUserID, @BlackPlayerUserID, @GameDate, @Result)";

            // Clear previous parameters
            command.Parameters.Clear();
            // Add white player ID parameter
            command.Parameters.AddWithValue("@WhitePlayerUserID", game.WhitePlayerUserID.Id);
            // Add black player ID parameter
            command.Parameters.AddWithValue("@BlackPlayerUserID", game.BlackPlayerUserID.Id);
            // Add game date parameter
            command.Parameters.AddWithValue("@GameDate", game.GameDate);

            // If there's a winner, add winner's ID - otherwise add null for draw
            if (game.Result != null)
                command.Parameters.AddWithValue("@Result", game.Result.Id);

            else
                command.Parameters.AddWithValue("@Result", DBNull.Value);

            // Execute and return number of affected rows
            return ExecuteChange();
        }

        // Override: Updates an existing game in the database
        // Returns number of affected rows
        public override int Update(BaseEntity entity)
        {
            // Cast entity to Game type
            Game game = entity as Game;

            // Set SQL command to update all game fields
            command.CommandText = @"UPDATE TableGames SET
                WhitePlayerUserID = @WhitePlayerUserID,
                BlackPlayerUserID = @BlackPlayerUserID,
                GameDate = @GameDate,
                Result = @Result
                WHERE GameID = @GameID";

            // Clear previous parameters
            command.Parameters.Clear();
            // Add all field parameters
            command.Parameters.AddWithValue("@WhitePlayerUserID", game.WhitePlayerUserID.Id);
            command.Parameters.AddWithValue("@BlackPlayerUserID", game.BlackPlayerUserID.Id);
            command.Parameters.AddWithValue("@GameDate", game.GameDate);

            // If there's a winner, add winner's ID - otherwise add null for draw
            if (game.Result != null)
                command.Parameters.AddWithValue("@Result", game.Result.Id);

            else
                command.Parameters.AddWithValue("@Result", DBNull.Value);

            // Add game ID for WHERE clause
            command.Parameters.AddWithValue("@GameID", game.Id);

            // Execute and return number of affected rows
            return ExecuteChange();
        }

        // Override: Deletes a game from the database by its ID
        // Returns number of affected rows
        public override int Delete(BaseEntity entity)
        {
            // Cast entity to Game type
            Game game = entity as Game;

            // Set SQL command for deletion
            command.CommandText = "DELETE FROM TableGames WHERE GameID = @GameID";
            // Clear previous parameters
            command.Parameters.Clear();
            // Add game ID parameter
            command.Parameters.AddWithValue("@GameID", game.Id);

            // Execute and return number of affected rows
            return ExecuteChange();
        }

        // Method: Inserts a new game and returns its auto-generated ID
        // Uses SCOPE_IDENTITY() to retrieve the new ID immediately after insertion
        public int InsertAndReturnId(Game game)
        {
            // Set SQL command to insert and return the new ID
            command.CommandText = @"INSERT INTO TableGames 
                (WhitePlayerUserID, BlackPlayerUserID, GameDate, Result)
                VALUES (@WhitePlayerUserID, @BlackPlayerUserID, @GameDate, @Result);
                SELECT CAST(SCOPE_IDENTITY() AS INT)";

            // Clear previous parameters
            command.Parameters.Clear();
            // Add all field parameters
            command.Parameters.AddWithValue("@WhitePlayerUserID", game.WhitePlayerUserID.Id);
            command.Parameters.AddWithValue("@BlackPlayerUserID", game.BlackPlayerUserID.Id);
            command.Parameters.AddWithValue("@GameDate", game.GameDate);

            // If there's a winner, add winner's ID - otherwise add null for draw
            if (game.Result != null)
                command.Parameters.AddWithValue("@Result", game.Result.Id);
            else
                command.Parameters.AddWithValue("@Result", DBNull.Value);

            // Variable to store the new game ID
            int newId = 0;

            try
            {
                // Open connection and execute scalar to get the new ID
                connection.Open();
                newId = (int)command.ExecuteScalar();
            }

            catch (Exception ex)
            {
                // Log error to console if insertion fails
                Console.WriteLine("Error: " + ex.Message);
            }

            finally
            {
                // Close connection if it's open
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
            }

            // Return the new game ID (0 if error occurred)
            return newId;
        }
    }
}