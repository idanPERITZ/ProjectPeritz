using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Model
{
    // Class representing a chess game with all its details
    [DataContract]
    public class Game : BaseEntity
    {
        // Field: Unique identifier for the game
        private int gameID;

        // Property: Gets or sets the game's unique ID
        [DataMember]
        public int GameID
        {
            // Return the game ID
            get { return gameID; }
            // Set the game ID
            set { gameID = value; }
        }

        // Field: The player who played as white
        private Player whitePlayerUserId;

        // Property: Gets or sets the white player
        [DataMember]
        public Player WhitePlayerUserID
        {
            // Return the white player
            get { return whitePlayerUserId; }
            // Set the white player
            set { whitePlayerUserId = value; }
        }

        // Field: The player who played as black
        private Player blackPlayerUserId;

        // Property: Gets or sets the black player
        [DataMember]
        public Player BlackPlayerUserID
        {
            // Return the black player
            get { return blackPlayerUserId; }
            // Set the black player
            set { blackPlayerUserId = value; }
        }

        // Field: The date and time when the game was played
        private DateTime gameDate;

        // Property: Gets or sets the game date
        [DataMember]
        public DateTime GameDate
        {
            // Return the game date
            get { return gameDate; }
            // Set the game date
            set { gameDate = value; }
        }

        // Field: The winner of the game (or null for draw)
        private Player result;

        // Property: Gets or sets the game result (winner)
        [DataMember]
        public Player Result
        {
            // Return the result (winner)
            get { return result; }
            // Set the result (winner)
            set { result = value; }
        }
    }

    // Collection class for managing a list of games
    [CollectionDataContract]
    public class GameList : List<Game>
    {
        // Constructor: Creates an empty game list
        public GameList() { }

        // Constructor: Creates a game list from an existing collection of games
        public GameList(IEnumerable<Game> list) : base(list) { }

        // Constructor: Creates a game list from base entities (casting to Game)
        public GameList(IEnumerable<BaseEntity> list) : base(list.Cast<Game>().ToList()) { }
    }

    // Enum representing different types of chess pieces
    [DataContract(Name = "PieceType")]
    public enum PieceType
    {
        // King piece
        [EnumMember] King,
        // Queen piece
        [EnumMember] Queen,
        // Rook piece
        [EnumMember] Rook,
        // Bishop piece
        [EnumMember] Bishop,
        // Knight piece
        [EnumMember] Knight,
        // Pawn piece
        [EnumMember] Pawn
    }
}