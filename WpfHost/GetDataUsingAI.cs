using System;
using System.Collections.Generic;
using ViewModel;
using Model;

namespace WcfService
{
    /// <summary>
    /// This class adds realistic Players to the database automatically.
    /// It holds a predefined list of 50 players and can add them all to the database.
    /// </summary>
    public class GetDataUsingAI
    {
        // A predefined list of 50 players with usernames, emails, join dates, and Elo rating.
        public PlayerList Users = new PlayerList
        {
            new Player { UserName = "ShadowStrike", Email = "shadowstrike@example.com", DateJoined = new DateTime(2010, 3, 15), Elo = 200, UserType = "Admin", GamesPlayed = 120, Wins = 78, Losses = 42},
            new Player { UserName = "NovaBlast", Email = "novablast@example.com", DateJoined = new DateTime(2012, 7, 21), Elo = 200, UserType = "Registered", GamesPlayed = 90, Wins = 45, Losses = 45},
            new Player { UserName = "IronWolf", Email = "ironwolf@example.com", DateJoined = new DateTime(2014, 11, 9), Elo = 200, UserType = "Registered", GamesPlayed = 110, Wins = 60, Losses = 50},
            new Player { UserName = "BlueViper", Email = "blueviper@example.com", DateJoined = new DateTime(2008, 5, 30), Elo = 200, UserType = "Admin", GamesPlayed = 150, Wins = 95, Losses = 55},
            new Player { UserName = "CrystalKnight", Email = "crystalknight@example.com", DateJoined = new DateTime(2011, 1, 2), Elo = 200, UserType = "Registered", GamesPlayed = 70, Wins = 40, Losses = 30},
            new Player { UserName = "NightFury", Email = "nightfury@example.com", DateJoined = new DateTime(2009, 8, 18), Elo = 200, UserType = "Registered", GamesPlayed = 130, Wins = 82, Losses = 48},
            new Player { UserName = "PixelGhost", Email = "pixelghost@example.com", DateJoined = new DateTime(2013, 12, 25), Elo = 200, UserType = "Guest", GamesPlayed = 20, Wins = 8, Losses = 12},
            new Player { UserName = "StormRider", Email = "stormrider@example.com", DateJoined = new DateTime(2007, 6, 14), Elo = 200, UserType = "Admin", GamesPlayed = 200, Wins = 130, Losses = 70},
            new Player { UserName = "ToxicFlame", Email = "toxicflame@example.com", DateJoined = new DateTime(2015, 4, 9), Elo = 200, UserType = "Registered", GamesPlayed = 60, Wins = 33, Losses = 27},
            new Player { UserName = "SilentArrow", Email = "silentarrow@example.com", DateJoined = new DateTime(2010, 9, 1), Elo = 200, UserType = "Registered", GamesPlayed = 100, Wins = 58, Losses = 42},
            new Player { UserName = "DragonSoul", Email = "dragonsoul@example.com", DateJoined = new DateTime(2006, 11, 22), Elo = 200, UserType = "Admin", GamesPlayed = 180, Wins = 120, Losses = 60},
            new Player { UserName = "CyberNinja", Email = "cyberninja@example.com", DateJoined = new DateTime(2013, 3, 19), Elo = 200, UserType = "Registered", GamesPlayed = 85, Wins = 46, Losses = 39},
            new Player { UserName = "FrostByte", Email = "frostbyte@example.com", DateJoined = new DateTime(2009, 2, 28), Elo = 200, UserType = "Registered", GamesPlayed = 95, Wins = 50, Losses = 45},
            new Player { UserName = "RedSpecter", Email = "redspecter@example.com", DateJoined = new DateTime(2008, 12, 5), Elo = 200, UserType = "Registered", GamesPlayed = 140, Wins = 88, Losses = 52},
            new Player { UserName = "HunterX", Email = "hunterx@example.com", DateJoined = new DateTime(2011, 7, 11), Elo = 200, UserType = "Registered", GamesPlayed = 75, Wins = 39, Losses = 36},
            new Player { UserName = "MegaNova", Email = "meganova@example.com", DateJoined = new DateTime(2012, 10, 30), Elo = 200, UserType = "Registered", GamesPlayed = 88, Wins = 44, Losses = 44},
            new Player { UserName = "RapidFalcon", Email = "rapidfalcon@example.com", DateJoined = new DateTime(2014, 6, 17), Elo = 200, UserType = "Registered", GamesPlayed = 66, Wins = 34, Losses = 32},
            new Player { UserName = "DarkWarden", Email = "darkwarden@example.com", DateJoined = new DateTime(2007, 1, 24), Elo = 200, UserType = "Admin", GamesPlayed = 210, Wins = 140, Losses = 70},
            new Player { UserName = "SkyBreaker", Email = "skybreaker@example.com", DateJoined = new DateTime(2015, 9, 6), Elo = 200, UserType = "Guest", GamesPlayed = 15, Wins = 6, Losses = 9},
            new Player { UserName = "VenomEdge", Email = "venomedge@example.com", DateJoined = new DateTime(2010, 5, 13), Elo = 200, UserType = "Registered", GamesPlayed = 105, Wins = 60, Losses = 45},
            new Player { UserName = "CrimsonReaper", Email = "crimsonreaper@example.com", DateJoined = new DateTime(2016, 2, 1), Elo = 200, UserType = "Registered", GamesPlayed = 55, Wins = 31, Losses = 24},
            new Player { UserName = "AquaStriker", Email = "aquastriker@example.com", DateJoined = new DateTime(2017, 8, 25), Elo = 200, UserType = "Registered", GamesPlayed = 48, Wins = 26, Losses = 22},
            new Player { UserName = "ElectroMage", Email = "electromage@example.com", DateJoined = new DateTime(2018, 1, 10), Elo = 200, UserType = "Registered", GamesPlayed = 42, Wins = 21, Losses = 21},
            new Player { UserName = "GoldenPhoenix", Email = "goldenphoenix@example.com", DateJoined = new DateTime(2015, 12, 3), Elo = 200, UserType = "Registered", GamesPlayed = 90, Wins = 55, Losses = 35}

        };

        /// <summary>
        /// Adds all predefined users from the Users list to the database.
        /// </summary>
        /// <returns>Returns a list of all users that were added.</returns>
        public PlayerList AddAllUsers()
        {
            UserDB userDb = new UserDB();

            // Loop through each player in the Users list and insert into the database
            foreach (Player user in Users)
            {
                userDb.Insert(user);
            }

            // Return the full list after insertion
            return Users;
        }

        

    }
}
