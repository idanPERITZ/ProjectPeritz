using Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ViewModel
{
    public class MoveDB : BaseDB
    {
        public override BaseEntity NewEntity()
        {
            return new MoveRecord();
        }

        public override BaseEntity CreateModel(BaseEntity entity)
        {
            MoveRecord move = entity as MoveRecord;
            move.Id = int.Parse(reader["MoveID"].ToString());
            move.GameID = int.Parse(reader["GameID"].ToString());
            move.MoveIndex = int.Parse(reader["MoveIndex"].ToString());
            move.From = reader["FromPosition"].ToString();
            move.To = reader["ToPosition"].ToString();
            move.MoveType = reader["MoveType"].ToString();

            if (reader["Promotion"] != DBNull.Value)
                move.Promotion = reader["Promotion"].ToString();

            else
                move.Promotion = null;

            return move;
        }

        public MoveList SelectAll()
        {
            command.CommandText = "SELECT * FROM TableMoves ORDER BY MoveIndex";
            return new MoveList(Execute());
        }

        public MoveList SelectByGame(int gameID)
        {
            command.CommandText =
                "SELECT * FROM TableMoves WHERE GameID = @GameID ORDER BY MoveIndex";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@GameID", gameID);
            return new MoveList(Execute());
        }

        public MoveList SelectByPlayer(int playerID)
        {
            command.CommandText = @"
                SELECT m.* FROM TableMoves m
                INNER JOIN TableGames g ON m.GameID = g.GameID
                WHERE g.WhitePlayerUserID = @PlayerID OR g.BlackPlayerUserID = @PlayerID
                ORDER BY m.GameID, m.MoveIndex";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@PlayerID", playerID);
            return new MoveList(Execute());
        }

        public override int Insert(BaseEntity entity)
        {
            MoveRecord move = entity as MoveRecord;

            command.CommandText = @"
        INSERT INTO TableMoves
        (GameID, MoveIndex, FromPosition, ToPosition, MoveType, Promotion)
        VALUES
        (@GameID, @MoveIndex, @FromPosition, @ToPosition, @MoveType, @Promotion)";

            command.Parameters.Clear();

            command.Parameters.AddWithValue("@GameID", move.GameID);
            command.Parameters.AddWithValue("@MoveIndex", move.MoveIndex);
            command.Parameters.AddWithValue("@FromPosition", move.From ?? "");
            command.Parameters.AddWithValue("@ToPosition", move.To ?? "");
            command.Parameters.AddWithValue("@MoveType", move.MoveType ?? "Normal");

            if (string.IsNullOrEmpty(move.Promotion))
                command.Parameters.AddWithValue("@Promotion", DBNull.Value);

            else
                command.Parameters.AddWithValue("@Promotion", move.Promotion);

            return ExecuteChange();
        }

        public override int Update(BaseEntity entity)
        {
            MoveRecord move = entity as MoveRecord;

            command.CommandText = @"
                UPDATE TableMoves SET
                MoveIndex    = @MoveIndex,
                FromPosition = @FromPosition,
                ToPosition   = @ToPosition,
                MoveType     = @MoveType,
                Promotion    = @Promotion
                WHERE MoveID = @MoveID";

            command.Parameters.Clear();
            command.Parameters.AddWithValue("@MoveIndex", move.MoveIndex);
            command.Parameters.AddWithValue("@FromPosition", move.From);
            command.Parameters.AddWithValue("@ToPosition", move.To);
            command.Parameters.AddWithValue("@MoveType", move.MoveType ?? "Normal");

            if (move.Promotion != null)
                command.Parameters.AddWithValue("@Promotion", move.Promotion);

            else
                command.Parameters.AddWithValue("@Promotion", DBNull.Value);

            command.Parameters.AddWithValue("@MoveID", move.Id);

            return ExecuteChange();
        }

        public override int Delete(BaseEntity entity)
        {
            MoveRecord move = entity as MoveRecord;
            command.CommandText = "DELETE FROM TableMoves WHERE MoveID = @MoveID";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@MoveID", move.Id);
            return ExecuteChange();
        }

        public void DeleteByGame(int gameID)
        {
            command.CommandText = "DELETE FROM TableMoves WHERE GameID = @GameID";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@GameID", gameID);
            ExecuteChange();
        }
    }
}