using Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ViewModel
{
    // Database access class for MoveRecord entities
    public class MoveDB : BaseDB
    {
        // Method: Creates a new MoveRecord entity instance
        public override BaseEntity NewEntity()
        {
            // Return a new MoveRecord object
            return new MoveRecord();
        }

        // Method: Populates a MoveRecord entity from database reader
        public override BaseEntity CreateModel(BaseEntity entity)
        {
            // Cast the entity to MoveRecord type
            MoveRecord move = entity as MoveRecord; 
            // Read and assign the move ID from the database
            move.Id = int.Parse(reader["MoveID"].ToString());
            // Read and assign the game ID from the database
            move.GameID = int.Parse(reader["GameID"].ToString());
            // Read and assign the move index from the database
            move.MoveIndex = int.Parse(reader["MoveIndex"].ToString());
            // Read and assign the starting square from the database
            move.From = reader["FromSquare"].ToString();
            // Read and assign the destination square from the database
            move.To = reader["ToSquare"].ToString();
            // Read and assign the move type from the database
            move.MoveType = reader["MoveType"].ToString();
            // Read and assign the promotion piece from the database (can be null)
            if (reader["Promotion"] != DBNull.Value)
            {
                move.Promotion = reader["Promotion"].ToString();
            }

            else
            {
                move.Promotion = null;
            }


            // Return the populated entity
            return move;
        }

        // Method: Retrieves all moves from the database ordered by move index
        public MoveList SelectAll()
        {
            // Set the SQL command to select all moves ordered by index
            command.CommandText = "SELECT * FROM TableMoves ORDER BY MoveIndex";
            // Execute the query and create MoveList from results
            return new MoveList(Execute());
        }

        // Method: Retrieves all moves for a specific game
        public MoveList SelectByGame(int gameID)
        {
            // Set the SQL command to select moves by game ID, ordered by move index
            command.CommandText =
                "SELECT * FROM TableMoves WHERE GameID = @GameID ORDER BY MoveIndex";

            // Clear any existing parameters
            command.Parameters.Clear();
            // Add the game ID parameter
            command.Parameters.AddWithValue("@GameID", gameID);

            // Execute the query and create MoveList from results
            return new MoveList(Execute());
        }

        // Method: Retrieves all moves made by a specific player (from games where they participated)
        public MoveList SelectByPlayer(int playerID)
        {
            // SQL query to find all moves from games where the player participated
            command.CommandText = @"
                SELECT m.* FROM TableMoves m
                INNER JOIN TableGames g ON m.GameID = g.GameID
                WHERE g.WhitePlayerUserID = @PlayerID OR g.BlackPlayerUserID = @PlayerID
                ORDER BY m.GameID, m.MoveIndex";

            // Clear any existing parameters
            command.Parameters.Clear();
            // Add the player ID parameter
            command.Parameters.AddWithValue("@PlayerID", playerID);

            // Execute the query and create MoveList from results
            return new MoveList(Execute());
        }

        // Method: Inserts a new move record into the database
        public override int Insert(BaseEntity entity)
        {
            // Cast the entity to MoveRecord type
            MoveRecord move = entity as MoveRecord;

            // Set the SQL command to insert a new move
            command.CommandText = @"
                INSERT INTO TableMoves
                (GameID, MoveIndex, FromSquare, ToSquare, MoveType, Promotion)
                VALUES
                (@GameID, @MoveIndex, @From, @To, @MoveType, @Promotion)";

            // Clear any existing parameters
            command.Parameters.Clear();
            // Add the game ID parameter
            command.Parameters.AddWithValue("@GameID", move.GameID);
            // Add the move index parameter
            command.Parameters.AddWithValue("@MoveIndex", move.MoveIndex);
            // Add the starting square parameter
            command.Parameters.AddWithValue("@From", move.From);
            // Add the destination square parameter
            command.Parameters.AddWithValue("@To", move.To);
            // Add the move type parameter
            command.Parameters.AddWithValue("@MoveType", move.MoveType);
            // Add the promotion parameter (null if not a promotion)
            if (move.Promotion != null)
            {
                command.Parameters.AddWithValue("@Promotion", move.Promotion);
            }
            else
            {
                command.Parameters.AddWithValue("@Promotion", DBNull.Value);
            }

            // Execute the insert command and return rows affected
            return ExecuteChange();
        }

        // Method: Updates an existing move record in the database
        public override int Update(BaseEntity entity)
        {
            // Cast the entity to MoveRecord type
            MoveRecord move = entity as MoveRecord;

            // Set the SQL command to update the move
            command.CommandText = @"
                UPDATE TableMoves SET
                MoveIndex = @MoveIndex,
                FromSquare = @From,
                ToSquare = @To,
                MoveType = @MoveType,
                Promotion = @Promotion
                WHERE MoveID = @MoveID";

            // Clear any existing parameters
            command.Parameters.Clear();
            // Add the move index parameter
            command.Parameters.AddWithValue("@MoveIndex", move.MoveIndex);
            // Add the starting square parameter
            command.Parameters.AddWithValue("@From", move.From);
            // Add the destination square parameter
            command.Parameters.AddWithValue("@To", move.To);
            // Add the move type parameter
            command.Parameters.AddWithValue("@MoveType", move.MoveType);
            // Add the promotion parameter
            if (move.Promotion != null)
            {
                command.Parameters.AddWithValue("@Promotion", move.Promotion);
            }

            else
            {
                command.Parameters.AddWithValue("@Promotion", DBNull.Value);
            }

            // Add the move ID parameter for the WHERE clause
            command.Parameters.AddWithValue("@MoveID", move.Id);

            // Execute the update command and return rows affected
            return ExecuteChange();
        }

        // Method: Deletes a move record from the database
        public override int Delete(BaseEntity entity)
        {
            // Cast the entity to MoveRecord type
            MoveRecord move = entity as MoveRecord;

            // Set the SQL command to delete the move by ID
            command.CommandText =
                "DELETE FROM TableMoves WHERE MoveID = @MoveID";

            // Clear any existing parameters
            command.Parameters.Clear();
            // Add the move ID parameter
            command.Parameters.AddWithValue("@MoveID", move.Id);

            // Execute the delete command and return rows affected
            return ExecuteChange();
        }

        // Method: Deletes all moves for a specific game
        public void DeleteByGame(int gameID)
        {
            // Set the SQL command to delete all moves by game ID
            command.CommandText =
                "DELETE FROM TableMoves WHERE GameID = @GameID";

            // Clear any existing parameters
            command.Parameters.Clear();
            // Add the game ID parameter
            command.Parameters.AddWithValue("@GameID", gameID);

            // Execute the delete command
            ExecuteChange();
        }
    }
}