using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewModel;

namespace WcfService
{
    // WCF Service class providing user operations for the chess system
    public class ChessServiceUser : IChessServiceUser
    {
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

        // Method: Retrieves all users from the database
        public PlayerList GetAllUsers()
        {
            // Create UserDB instance and select all users
            return new UserDB().SelectAll();
        }

        // Method: Retrieves a specific user by their ID
        public Player GetUserByID(int userID)
        {
            // Create UserDB instance and select user by ID
            return new UserDB().SelectById(userID);
        }

        // ============= GAME OPERATIONS =============

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
            // If list is null or empty
            if (list == null || list.Count == 0)
                // Return null (no games found)
                return null;
            // Return the last game in the list
            return list.Last();
        }

        // ============= MOVE OPERATIONS =============

        // Method: Retrieves a specific move by its ID
        public MoveRecord GetMoveByID(int moveID)
        {
            // Create MoveDB instance
            MoveDB moveDB = new MoveDB();
            // Get all moves from database
            MoveList moves = moveDB.SelectAll();
            // Find and return the move with matching ID (or null if not found)
            return moves.FirstOrDefault(m => m.Id == moveID);
        }

        // Method: Retrieves all moves for a specific game
        public MoveList GetMovesByGameID(int gameID)
        {
            // Create MoveDB instance and select moves by game ID
            return new MoveDB().SelectByGame(gameID);
        }

        // Method: Retrieves all moves made by a specific player
        public MoveList GetMovesByPlayerID(int playerID)
        {
            // Create MoveDB instance and select moves by player ID
            return new MoveDB().SelectByPlayer(playerID);
        }

        // Method: Returns the last move in a specific game
        public MoveRecord GetLastMoveByGameID(int gameID)
        {
            // Get all moves for the game
            MoveList moves = new MoveDB().SelectByGame(gameID);
            // If list is null or empty
            if (moves == null || moves.Count == 0)
                // Return null (no moves found)
                return null;
            // Return the last move in the list
            return moves.Last();
        }

        // ============= GAME LOGIC =============

        // Method: Checks if it's a specific player's turn in a game
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

            // Return true if it's the player's turn
            if (isWhiteTurn)
                return playerID == game.WhitePlayerUserID.Id;
            else
                return playerID == game.BlackPlayerUserID.Id;
        }

        // Method: Checks if a game has finished (has a result)
        public bool IsGameFinished(int gameID)
        {
            // Get the game by ID
            Game game = new GameDB().SelectById(gameID);
            // Return true if game exists and has a result (winner or draw)
            return game != null && game.Result != null;
        }

        // ============= AUTHENTICATION =============

        // Method: Registers a new user with Firebase authentication
        public async Task<string> SignUp(string email, string password)
        {
            // Create Firebase authentication service
            FirebaseAuthService authService = new FirebaseAuthService();
            // Sign up the user and return the result (user ID or error)
            return await authService.SignUp(email, password);
        }

        // Method: Signs in a user using Firebase authentication
        public Player Login(string email, string password)
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
                // If player not found in database
                if (player == null)
                    // Return null (user doesn't exist in our database)
                    return null;

                // Return the authenticated player
                return player;
            }
            // If sign-in failed
            else
            {
                // Return null (authentication failed)
                return null;
            }
        }
    }
}