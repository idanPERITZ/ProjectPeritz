using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Model
{
    // Class representing a player in the chess system
    [DataContract]
    public class Player : BaseEntity
    {
        // Field: Player's username for login
        private string username;
        // Field: Player's email address
        private string email;
        // Field: Date when player joined the system
        private DateTime dateJoined;
        // Field: Player's ELO rating (skill level)
        private int elo;
        // Field: Type of user account ("regular", "admin")
        private string userType;
        // Field: Total number of games played
        private int gamesPlayed;
        // Field: Number of games won
        private int wins;
        // Field: Number of games lost
        private int losses;
        // Field: Number of games drawn 
        private int draws;


        // Property: Gets or sets the player's username
        [DataMember]
        public string UserName
        {
            // Return the username
            get { return username; }
            // Set the username
            set { username = value; }
        }

        // Property: Gets or sets the player's email
        [DataMember]
        public string Email
        {
            // Return the email
            get { return email; }
            // Set the email
            set { email = value; }
        }

        // Property: Gets or sets the date the player joined
        [DataMember]
        public DateTime DateJoined
        {
            // Return the join date
            get { return dateJoined; }
            // Set the join date
            set { dateJoined = value; }
        }

        // Property: Gets or sets the player's ELO rating
        [DataMember]
        public int Elo
        {
            // Return the ELO rating
            get { return elo; }
            // Set the ELO rating
            set { elo = value; }
        }

        // Property: Gets or sets the user account type
        [DataMember]
        public string UserType
        {
            // Return the user type
            get { return userType; }
            // Set the user type
            set { userType = value; }
        }

        // Property: Gets or sets the number of games played
        [DataMember]
        public int GamesPlayed
        {
            // Return games played count
            get { return gamesPlayed; }
            // Set games played count
            set { gamesPlayed = value; }
        }

        // Property: Gets or sets the number of wins
        [DataMember]
        public int Wins
        {
            // Return wins count
            get { return wins; }
            // Set wins count
            set { wins = value; }
        }

        // Property: Gets or sets the number of losses
        [DataMember]
        public int Losses
        {
            // Return losses count
            get { return losses; }
            // Set losses count
            set { losses = value; }
        }

        // Property: Gets or sets the number of draws
        [DataMember]
        public int Draws
        {
            // Return draws count
            get { return draws; }
            // Set draws count
            set { draws = value; }
        }
    }

    // Collection class for managing a list of players
    [CollectionDataContract]
    public class PlayerList : List<Player>
    {
        // Constructor: Creates an empty player list
        public PlayerList() { }

        // Constructor: Creates a player list from an existing collection of players
        public PlayerList(IEnumerable<Player> list) : base(list) { }

        // Constructor: Creates a player list from base entities (casting to Player)
        public PlayerList(IEnumerable<BaseEntity> list)
            : base(list.Cast<Player>().ToList()) { }
    }
}