using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    // Database access class for Game entities
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
            // Read white player's ELO rating
            game.WhiteElo = int.Parse(reader["WhiteElo"].ToString());
            // Read black player's ELO rating
            game.BlackElo = int.Parse(reader["BlackElo"].ToString());
            // Read game ID
            game.Id = int.Parse(reader["GameID"].ToString());

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

        // Method: Selects game by ID
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

        // Method: Selects all games where player participated
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

        // Override: Inserts new game into database
        public override int Insert(BaseEntity entity)
        {
            // Cast entity to Game type
            Game game = entity as Game;

            // Set SQL command for insertion
            command.CommandText = @"INSERT INTO TableGames 
                (WhitePlayerUserID, BlackPlayerUserID, GameDate, Result, WhiteElo, BlackElo)
                VALUES (@WhitePlayerUserID, @BlackPlayerUserID, @GameDate, @Result, @WhiteElo, @BlackElo)";

            // Clear previous parameters
            command.Parameters.Clear();
            // Add white player ID parameter
            command.Parameters.AddWithValue("@WhitePlayerUserID", game.WhitePlayerUserID.Id);
            // Add black player ID parameter
            command.Parameters.AddWithValue("@BlackPlayerUserID", game.BlackPlayerUserID.Id);
            // Add game date parameter
            command.Parameters.AddWithValue("@GameDate", game.GameDate);

            // If there's a winner
            if (game.Result != null)
            {
                // Add winner's ID as result
                command.Parameters.AddWithValue("@Result", game.Result.Id);
            }
            else
            {
                // No winner (draw) - add null
                command.Parameters.AddWithValue("@Result", DBNull.Value);
            }

            // Add white player's ELO parameter
            command.Parameters.AddWithValue("@WhiteElo", game.WhiteElo);
            // Add black player's ELO parameter
            command.Parameters.AddWithValue("@BlackElo", game.BlackElo);

            // Execute and return number of affected rows
            return ExecuteChange();
        }

        // Override: Updates existing game in database
        public override int Update(BaseEntity entity)
        {
            // Cast entity to Game type
            Game game = entity as Game;

            // Set SQL command for update
            command.CommandText = @"UPDATE TableGames SET
                WhitePlayerUserID = @WhitePlayerUserID,
                BlackPlayerUserID = @BlackPlayerUserID,
                GameDate = @GameDate,
                Result = @Result,
                WhiteElo = @WhiteElo,
                BlackElo = @BlackElo
                WHERE GameID = @GameID";

            // Clear previous parameters
            command.Parameters.Clear();
            // Add white player ID parameter
            command.Parameters.AddWithValue("@WhitePlayerUserID", game.WhitePlayerUserID.Id);
            // Add black player ID parameter
            command.Parameters.AddWithValue("@BlackPlayerUserID", game.BlackPlayerUserID.Id);
            // Add game date parameter
            command.Parameters.AddWithValue("@GameDate", game.GameDate);

            // If there's a winner
            if (game.Result != null)
            {
                // Add winner's ID as result
                command.Parameters.AddWithValue("@Result", game.Result.Id);
            }
            else
            {
                // No winner (draw) - add null
                command.Parameters.AddWithValue("@Result", DBNull.Value);
            }

            // Add white player's ELO parameter
            command.Parameters.AddWithValue("@WhiteElo", game.WhiteElo);
            // Add black player's ELO parameter
            command.Parameters.AddWithValue("@BlackElo", game.BlackElo);
            // Add game ID parameter
            command.Parameters.AddWithValue("@GameID", game.Id);

            // Execute and return number of affected rows
            return ExecuteChange();
        }

        // Override: Deletes game from database
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
    }
}