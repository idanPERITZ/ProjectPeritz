using Model;
using System;
using System.Collections.Generic;

namespace ViewModel
{
    public class FriendshipDB : BaseDB
    {
        public override BaseEntity NewEntity() => new Friendship();

        public override BaseEntity CreateModel(BaseEntity entity)
        {
            Friendship f = entity as Friendship;
            f.Id = int.Parse(reader["FriendshipID"].ToString());
            f.RequesterID = int.Parse(reader["RequesterID"].ToString());
            f.ReceiverID = int.Parse(reader["ReceiverID"].ToString());
            f.IsAccepted = (bool)reader["IsAccepted"];
            f.FriendshipDate = reader["FriendshipDate"] == DBNull.Value
                ? (DateTime?)null
                : DateTime.Parse(reader["FriendshipDate"].ToString());
            return f;
        }

        // Get all friendships in the database
        public FriendshipList SelectAll()
        {
            command.CommandText = "SELECT * FROM TableFriendships";
            return new FriendshipList(Execute());
        }

        // Get all ACCEPTED friendships for a specific user
        public FriendshipList SelectAcceptedByUser(int userID)
        {
            command.CommandText = @"SELECT * FROM TableFriendships 
                WHERE (RequesterID=@ID OR ReceiverID=@ID) AND IsAccepted=1";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@ID", userID);
            return new FriendshipList(Execute());
        }

        // Get all PENDING requests sent TO a specific user
        public FriendshipList SelectPendingForUser(int userID)
        {
            command.CommandText = @"SELECT * FROM TableFriendships 
                WHERE ReceiverID=@ID AND IsAccepted=0";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@ID", userID);
            return new FriendshipList(Execute());
        }

        // Check if any friendship (pending or accepted) already exists between two users
        public bool FriendshipExists(int userA, int userB)
        {
            command.CommandText = @"SELECT COUNT(*) FROM TableFriendships
                WHERE (RequesterID=@A AND ReceiverID=@B) 
                   OR (RequesterID=@B AND ReceiverID=@A)";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@A", userA);
            command.Parameters.AddWithValue("@B", userB);
            try
            {
                connection.Open();
                return (int)command.ExecuteScalar() > 0;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
            }
        }

        // Insert a new pending friend request (IsAccepted = 0, FriendshipDate = NULL)
        public override int Insert(BaseEntity entity)
        {
            Friendship f = entity as Friendship;
            command.CommandText = @"INSERT INTO TableFriendships (RequesterID, ReceiverID, IsAccepted)
                VALUES (@RequesterID, @ReceiverID, 0)";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@RequesterID", f.RequesterID);
            command.Parameters.AddWithValue("@ReceiverID", f.ReceiverID);
            return ExecuteChange();
        }

        // Insert a new pending friend request and return the auto-generated ID
        public int InsertAndReturnId(Friendship f)
        {
            command.CommandText = @"INSERT INTO TableFriendships (RequesterID, ReceiverID, IsAccepted)
                VALUES (@RequesterID, @ReceiverID, 0);
                SELECT CAST(SCOPE_IDENTITY() AS INT)";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@RequesterID", f.RequesterID);
            command.Parameters.AddWithValue("@ReceiverID", f.ReceiverID);
            try
            {
                connection.Open();
                return (int)command.ExecuteScalar();
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
            }
        }

        // Accept a friend request: sets IsAccepted=1 and records the date
        public override int Update(BaseEntity entity)
        {
            Friendship f = entity as Friendship;
            command.CommandText = @"UPDATE TableFriendships 
                SET IsAccepted=1, FriendshipDate=@FriendshipDate 
                WHERE FriendshipID=@ID";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@FriendshipDate", DateTime.Now);
            command.Parameters.AddWithValue("@ID", f.Id);
            return ExecuteChange();
        }

        // Delete a friendship by its ID
        public override int Delete(BaseEntity entity)
        {
            Friendship f = entity as Friendship;
            command.CommandText = "DELETE FROM TableFriendships WHERE FriendshipID=@ID";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@ID", f.Id);
            return ExecuteChange();
        }
    }
}