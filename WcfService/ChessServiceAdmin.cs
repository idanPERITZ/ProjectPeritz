using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using ViewModel;

namespace WcfService
{
    // WCF Service class providing admin operations for the chess system
    public class ChessServiceAdmin : IChessServiceAdmin
    {
        // ============= GAME OPERATIONS =============

        // Method: Inserts a new game into the database
        public void InsertGame(Game game)
        {
            // Create GameDB instance and insert the game
            new GameDB().Insert(game);
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

        // Method: Retrieves all active games (currently returns all games)
        public GameList GetActiveGames()
        {
            // Create GameDB instance and select all games
            // ✅ TODO: Add logic to filter only games without Result (ongoing games)
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
            // If list is null or empty
            if (list == null || list.Count == 0)
                // Return null (no games found)
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

        // Method: Deletes a move from the database
        public void DeleteMove(MoveRecord move)
        {
            // Create MoveDB instance and delete the move
            new MoveDB().Delete(move);
        }

        // Method: Deletes all moves associated with a specific game
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

        // Method: Returns the most recent move in a specific game
        public MoveRecord ReturnLastMoveByGameID(int gameID)
        {
            // Get all moves for the game
            MoveList list = new MoveDB().SelectByGame(gameID);
            // If list is null or empty
            if (list == null || list.Count == 0)
                // Return null (no moves found)
                return null;
            // Return the last move in the list
            return list.Last();
        }

        // Method: Undoes the last move in a game by deleting it
        public MoveRecord UndoLastMove(int gameID)
        {
            // Create MoveDB instance
            MoveDB moveDB = new MoveDB();
            // Get all moves for the game
            MoveList moves = moveDB.SelectByGame(gameID);

            // If list is null or empty
            if (moves == null || moves.Count == 0)
                // Return null (no moves to undo)
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
            // ✅ FIX: Changed from GameDB to UserDB
            new UserDB().Delete(user);
        }

        // Method: Retrieves a specific user by their ID
        public Player GetUserByID(int userID)
        {
            // Create UserDB instance and select user by ID
            return new UserDB().SelectById(userID);
        }

        // Method: Retrieves a user by their username
        public Player GetUserByName(string name)
        {
            // Search for users with matching name
            PlayerList players = new UserDB().SelectByName(name);
            // If list is null or empty
            if (players == null || players.Count == 0)
                // Return null (user not found)
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

        // Method: Searches for users by name pattern
        public PlayerList SearchUsersByName(string name)
        {
            // Create UserDB instance and search by name
            return new UserDB().SelectByName(name);
        }

        // Method: Signs in an admin user using Firebase authentication
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
                // If player not found in database
                if (player == null)
                    // Return null (user doesn't exist)
                    return null;

                // If player is not an admin
                if (player.UserType != "Admin")
                    // Return null (only admins can use this service)
                    return null;

                // Return the admin player
                return player;
            }
            // If sign-in failed
            else
            {
                // Return null (authentication failed)
                return null;
            }
        }

        // Method: Updates player statistics after a game
        public void UpdatePlayerStats(int playerID, bool won)
        {
            // Get the player from database
            Player player = new UserDB().SelectById(playerID);

            // If player not found, return
            if (player == null)
                return;

            // Increment games played
            player.GamesPlayed++;

            // Update wins or losses based on result
            if (won)
                player.Wins++;
            else
                player.Losses++;

            // Save updated player to database
            new UserDB().Update(player);
        }

        // Method: Updates ELO ratings for both players after a game
        public void UpdateEloRatings(int winnerID, int loserID, int winnerElo, int loserElo)
        {
            // Calculate expected scores using ELO formula
            double expectedWinner = 1 / (1 + Math.Pow(10, (loserElo - winnerElo) / 400.0));
            double expectedLoser = 1 / (1 + Math.Pow(10, (winnerElo - loserElo) / 400.0));

            // Calculate new ELO ratings (K-factor = 32)
            int newWinnerElo = (int)(winnerElo + 32 * (1 - expectedWinner));
            int newLoserElo = (int)(loserElo + 32 * (0 - expectedLoser));

            // Get players from database
            Player winner = new UserDB().SelectById(winnerID);
            Player loser = new UserDB().SelectById(loserID);

            // If either player not found, return
            if (winner == null || loser == null)
                return;

            // Update ELO ratings
            winner.Elo = newWinnerElo;
            loser.Elo = newLoserElo;

            // Save updated players to database
            new UserDB().Update(winner);
            new UserDB().Update(loser);
        }

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
    }
}