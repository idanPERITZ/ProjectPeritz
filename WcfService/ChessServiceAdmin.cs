using System.Windows;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using ViewModel;

namespace WcfService
{
    // WCF Service class providing admin operations for the chess system
    // Implements IChessServiceAdmin and handles all admin-level requests
    public class ChessServiceAdmin : IChessServiceAdmin
    {
        // ============= GAME OPERATIONS =============

        // Method: Inserts a new game into the database and returns it
        public Game InsertGame(Game game)
        {
            // Create GameDB instance and insert the game
            GameDB db = new GameDB();
            db.Insert(game);
            // Return the game with its new ID by searching for it
            return db.GetGame(game);
        }

        // Method: Updates an existing game in the database
        public void UpdateGame(Game game)
        {
            // Create GameDB instance and update the game
            new GameDB().Update(game);
        }

        // Method: Deletes a game from the database
        public void DeleteGame(Game game)
        {
            // Create GameDB instance and delete the game
            new GameDB().Delete(game);
        }

        // Method: Retrieves all games from the database
        public GameList GetAllGames()
        {
            // Create GameDB instance and select all games
            return new GameDB().SelectAll();
        }

        // Method: Retrieves all active games
        // TODO: Add logic to filter only games without Result (ongoing games)
        public GameList GetActiveGames()
        {
            // Currently returns all games
            return new GameDB().SelectAll();
        }

        // Method: Retrieves a specific game by its ID
        public Game GetGameByID(int gameID)
        {
            // Create GameDB instance and select game by ID
            return new GameDB().SelectById(gameID);
        }

        // Method: Retrieves all games where a specific player participated
        public GameList GetGamesByPlayer(int playerID)
        {
            // Create GameDB instance and select games by player ID
            return new GameDB().SelectByPlayer(playerID);
        }

        // Method: Retrieves the most recent game for a specific player
        public Game GetLatestGameForPlayer(int playerID)
        {
            // Get all games for the player
            GameList list = new GameDB().SelectByPlayer(playerID);
            // If list is null or empty, return null
            if (list == null || list.Count == 0)
                return null;
            // Return the last game in the list
            return list.Last();
        }

        // Method: Checks if a game has finished (has a result)
        public bool IsGameFinished(int gameID)
        {
            // Get the game by ID
            Game game = new GameDB().SelectById(gameID);
            // Return true if game exists and has a result (winner or draw)
            return game != null && game.Result != null;
        }

        // Method: Inserts a new game and returns it with its auto-generated ID
        public Game InsertGameAndReturn(Game game)
        {
            // Create GameDB instance
            GameDB db = new GameDB();
            // Insert the game and get the new auto-generated ID
            int newId = db.InsertAndReturnId(game);
            // Return the full game object with the new ID
            return db.SelectById(newId);
        }

        // Method: Updates only the result of a game (winner or draw)
        public void UpdateGameResult(Game game)
        {
            // Create GameDB instance and update the game
            new GameDB().Update(game);
        }

        // ============= MOVE OPERATIONS =============

        // Method: Inserts a new move into the database
        public void InsertMove(MoveRecord move)
        {
            // Create MoveDB instance and insert the move
            new MoveDB().Insert(move);
        }

        // Method: Updates an existing move in the database
        public void UpdateMove(MoveRecord move)
        {
            // Create MoveDB instance and update the move
            new MoveDB().Update(move);
        }

        // Method: Deletes a single move from the database
        public void DeleteMove(MoveRecord move)
        {
            // Create MoveDB instance and delete the move
            new MoveDB().Delete(move);
        }

        // Method: Deletes all moves associated with a specific game
        // Must be called before deleting a game to avoid FK constraint errors
        public void DeleteMovesByGameID(int gameID)
        {
            // Create MoveDB instance and delete all moves for the game
            new MoveDB().DeleteByGame(gameID);
        }

        // Method: Retrieves all moves from the database
        public MoveList GetAllMoves()
        {
            // Create MoveDB instance and select all moves
            return new MoveDB().SelectAll();
        }

        // Method: Retrieves all moves for a specific game ordered by move index
        public MoveList GetMovesByGameID(int gameID)
        {
            // Create MoveDB instance and select moves by game ID
            return new MoveDB().SelectByGame(gameID);
        }

        // Method: Retrieves all moves made by a specific player across all games
        public MoveList GetMovesByPlayerID(int playerID)
        {
            // Create MoveDB instance and select moves by player ID
            return new MoveDB().SelectByPlayer(playerID);
        }

        // Method: Returns the most recent move in a specific game
        public MoveRecord ReturnLastMoveByGameID(int gameID)
        {
            // Get all moves for the game
            MoveList list = new MoveDB().SelectByGame(gameID);
            // If list is null or empty, return null
            if (list == null || list.Count == 0)
                return null;
            // Return the last move in the list
            return list.Last();
        }

        // Method: Undoes the last move in a game by deleting it from the database
        public MoveRecord UndoLastMove(int gameID)
        {
            // Create MoveDB instance
            MoveDB moveDB = new MoveDB();
            // Get all moves for the game
            MoveList moves = moveDB.SelectByGame(gameID);
            // If list is null or empty, return null
            if (moves == null || moves.Count == 0)
                return null;
            // Get the last move
            MoveRecord lastMove = moves.Last();
            // Delete the last move from database
            moveDB.Delete(lastMove);
            // Return the deleted move
            return lastMove;
        }

        // ============= USER/PLAYER OPERATIONS =============

        // Method: Inserts a new user into the database
        public void InsertUser(Player user)
        {
            // Create UserDB instance and insert the user
            new UserDB().Insert(user);
        }

        // Method: Updates an existing user in the database
        public void UpdateUser(Player user)
        {
            // Create UserDB instance and update the user
            new UserDB().Update(user);
        }

        // Method: Deletes a user from the database
        public void DeleteUser(Player user)
        {
            // Create UserDB instance and delete the user
            new UserDB().Delete(user);
        }

        // Method: Retrieves a specific user by their ID
        public Player GetUserByID(int userID)
        {
            // Create UserDB instance and select user by ID
            return new UserDB().SelectById(userID);
        }

        // Method: Retrieves a user by their exact username
        public Player GetUserByName(string name)
        {
            // Search for users with matching name
            PlayerList players = new UserDB().SelectByName(name);
            // If list is null or empty, return null
            if (players == null || players.Count == 0)
                return null;
            // Return the first matching user
            return players[0];
        }

        // Method: Retrieves all users from the database
        public PlayerList GetAllUsers()
        {
            // Create UserDB instance and select all users
            return new UserDB().SelectAll();
        }

        // Method: Searches for users whose username matches a pattern
        public PlayerList SearchUsersByName(string name)
        {
            // Create UserDB instance and search by name pattern
            return new UserDB().SelectByName(name);
        }

        // Method: Signs in an admin user using Firebase authentication
        // Returns the admin player if authentication succeeds, null otherwise
        public Player SignIn(string email, string password)
        {
            // Create Firebase authentication service
            FirebaseAuthService authService = new FirebaseAuthService();
            // Attempt to sign in with email and password (wait for async result)
            var signInResult = authService.SignIn(email, password).GetAwaiter().GetResult();

            // If sign-in was successful
            if (signInResult.Success)
            {
                // Get player from database using Firebase LocalId and email
                Player player = new UserDB().Login(signInResult.LocalId, email);
                // If player not found in database, return null
                if (player == null)
                    return null;
                // If player is not an admin, return null (only admins allowed)
                if (player.UserType != "Admin")
                    return null;
                // Return the authenticated admin player
                return player;
            }
            // If sign-in failed, return null
            return null;
        }

        // Method: Updates player statistics after a game ends
        // Increments GamesPlayed and either Wins or Losses based on result
        public void UpdatePlayerStats(int playerID, bool won)
        {
            // Get the player from database
            Player player = new UserDB().SelectById(playerID);
            // If player not found, return without changes
            if (player == null)
                return;
            // Increment total games played
            player.GamesPlayed++;
            // Increment wins or losses based on game result
            if (won)
                player.Wins++;
            else
                player.Losses++;
            // Save updated statistics to database
            new UserDB().Update(player);
        }

        // Method: Updates player statistics after a draw
        // Increments GamesPlayed and Draws for the specified player
        public void UpdatePlayerDraw(int playerID)
        {
            // Get the player from database
            Player player = new UserDB().SelectById(playerID);
            // If player not found, return without changes
            if (player == null) return;
            // Increment total games played
            player.GamesPlayed++;
            // Increment draws count
            player.Draws++;
            // Save updated statistics to database
            new UserDB().Update(player);
        }

        // Method: Reverts player statistics when a game is deleted
        // Decrements GamesPlayed and either Wins or Losses
        public void RevertPlayerStats(int playerID, bool won)
        {
            Player player = new UserDB().SelectById(playerID);
            if (player == null) return;
            // Decrement total games played (minimum 0)
            if (player.GamesPlayed > 0) player.GamesPlayed--;
            // Decrement wins or losses
            if (won) { if (player.Wins > 0) player.Wins--; }
            else { if (player.Losses > 0) player.Losses--; }
            new UserDB().Update(player);
        }

        // Method: Reverts player draw statistics when a game is deleted
        // Decrements GamesPlayed and Draws
        public void RevertPlayerDraw(int playerID)
        {
            Player player = new UserDB().SelectById(playerID);
            if (player == null) return;
            // Decrement total games played (minimum 0)
            if (player.GamesPlayed > 0) player.GamesPlayed--;
            // Decrement draws count (minimum 0)
            if (player.Draws > 0) player.Draws--;
            new UserDB().Update(player);
        }

        // Method: Checks if it's a specific player's turn in a game
        // Based on move count: even = white's turn, odd = black's turn
        public bool IsPlayerTurn(int gameID, int playerID)
        {
            // Get all moves for the game
            MoveList moves = new MoveDB().SelectByGame(gameID);
            // Get the game details
            Game game = new GameDB().SelectById(gameID);
            // If game not found, return false
            if (game == null)
                return false;
            // If no moves have been made yet, it's white player's turn
            if (moves.Count == 0)
                return playerID == game.WhitePlayerUserID.Id;
            // Calculate whose turn it is based on number of moves
            // Even number of moves = white's turn, odd = black's turn
            bool isWhiteTurn = moves.Count % 2 == 0;
            // Return true if it's the specified player's turn
            if (isWhiteTurn)
                return playerID == game.WhitePlayerUserID.Id;
            else
                return playerID == game.BlackPlayerUserID.Id;
        }

        // ============= FRIENDSHIP OPERATIONS =============

        // Method: Returns all friendships in the database
        public FriendshipList GetAllFriendships()
        {
            return new FriendshipDB().SelectAll();
        }

        // Method: Returns all accepted friendships for a specific user
        public FriendshipList GetAcceptedFriendsByUser(int userID)
        {
            return new FriendshipDB().SelectAcceptedByUser(userID);
        }

        // Method: Returns all pending friend requests sent TO a specific user
        public FriendshipList GetPendingFriendRequestsForUser(int userID)
        {
            return new FriendshipDB().SelectPendingForUser(userID);
        }

        // Method: Returns true if any friendship (pending or accepted) exists between two users
        public bool FriendshipExists(int userA, int userB)
        {
            return new FriendshipDB().FriendshipExists(userA, userB);
        }

        // Method: Sends a friend request from requesterID to receiverID
        // Returns the new FriendshipID
        public int SendFriendRequest(int requesterID, int receiverID)
        {
            Friendship f = new Friendship
            {
                RequesterID = requesterID,
                ReceiverID = receiverID
            };
            return new FriendshipDB().InsertAndReturnId(f);
        }

        // Method: Accepts a pending friend request by setting IsAccepted=1 and recording the date
        public void AcceptFriendRequest(int friendshipID)
        {
            Friendship f = new Friendship { Id = friendshipID };
            new FriendshipDB().Update(f);
        }

        // Method: Deletes a friendship record entirely
        public void DeleteFriendship(int friendshipID)
        {
            Friendship f = new Friendship { Id = friendshipID };
            new FriendshipDB().Delete(f);
        }

        // Method: Declines a pending friend request by deleting it
        public void DeclineFriendRequest(int friendshipID)
        {
            Friendship f = new Friendship { Id = friendshipID };
            new FriendshipDB().Delete(f);
        }
    }
}