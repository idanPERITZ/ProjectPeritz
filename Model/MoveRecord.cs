using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    // Class representing a single move in a chess game
    [DataContract]
    public class MoveRecord : BaseEntity
    {
        // Field: The ID of the game this move belongs to
        private int gameID;

        // Property: Gets or sets the game ID reference
        [DataMember]
        public int GameID
        {
            // Return the game ID
            get { return gameID; }
            // Set the game ID
            set { gameID = value; }
        }

        // Field: Sequential index of the move in the game (0, 1, 2...)
        private int moveIndex;

        // Property: Gets or sets the move index
        [DataMember]
        public int MoveIndex
        {
            // Return the move index
            get { return moveIndex; }
            // Set the move index
            set { moveIndex = value; }
        }

        // Field: Starting position of the move (e.g., "e2")
        private string from;

        // Property: Gets or sets the starting position
        [DataMember]
        public string From
        {
            // Return the starting position
            get { return from; }
            // Set the starting position
            set { from = value; }
        }

        // Field: Ending position of the move (e.g., "e4")
        private string to;

        // Property: Gets or sets the ending position
        [DataMember]
        public string To
        {
            // Return the ending position
            get { return to; }
            // Set the ending position
            set { to = value; }
        }

        // Field: Type of move (e.g., "Normal", "CastleKingSide", "EnPassant", "Promotion")
        private string moveType;

        // Property: Gets or sets the move type
        [DataMember]
        public string MoveType
        {
            // Return the move type
            get { return moveType; }
            // Set the move type
            set { moveType = value; }
        }

        // Field: Piece type to promote to ("Queen", "Rook", "Bishop", "Knight".)
        private string promotion;

        // Property: Gets or sets the promotion piece type
        [DataMember]
        public string Promotion
        {
            // Return the promotion piece type
            get { return promotion; }
            // Set the promotion piece type
            set { promotion = value; }
        }
    }

    // Collection class for managing a list of moves
    [CollectionDataContract]
    public class MoveList : List<MoveRecord>
    {
        // Constructor: Creates an empty move list
        public MoveList() { }

        // Constructor: Creates a move list from an existing collection of moves
        public MoveList(IEnumerable<MoveRecord> list) : base(list) { }

        // Constructor: Creates a move list from base entities (casting to MoveRecord)
        public MoveList(IEnumerable<BaseEntity> list)
            : base(list.Cast<MoveRecord>().ToList()) { }
    }
}
