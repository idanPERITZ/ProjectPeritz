using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ViewModel;

namespace WcfService
{
    // WCF Service Contract interface defining all admin operations for the chess system
    // All methods marked with [OperationContract] are exposed as web service endpoints
    [ServiceContract]
    public interface IChessServiceAdmin
    {
        // ============= GAME OPERATIONS =============

        // Operation: Retrieves all games from the database
        [OperationContract]
        GameList GetAllGames();

        // Operation: Retrieves a specific game by its ID
        [OperationContract]
        Game GetGameByID(int gameID);

        // Operation: Inserts a new game into the database and returns it
        [OperationContract]
        Game InsertGame(Game game);

        // Operation: Inserts a new game and returns it with its auto-generated ID
        [OperationContract]
        Game InsertGameAndReturn(Game game);

        // Operation: Updates an existing game in the database
        [OperationContract]
        void UpdateGame(Game game);

        // Operation: Updates only the result of a game (winner or draw)
        [OperationContract]
        void UpdateGameResult(Game game);

        // Operation: Deletes a game from the database
        // Note: Delete all moves first using DeleteMovesByGameID to avoid FK constraint errors
        [OperationContract]
        void DeleteGame(Game game);

        // Operation: Retrieves all games where a specific player participated
        [OperationContract]
        GameList GetGamesByPlayer(int playerID);

        // Operation: Retrieves the most recent game for a specific player
        [OperationContract]
        Game GetLatestGameForPlayer(int playerID);

        // Operation: Checks if a game has finished (has a result)
        [OperationContract]
        bool IsGameFinished(int gameID);

        // Operation: Retrieves all active/ongoing games
        [OperationContract]
        GameList GetActiveGames();

        // ============= MOVE OPERATIONS =============

        // Operation: Inserts a new move into the database
        [OperationContract]
        void InsertMove(MoveRecord move);

        // Operation: Updates an existing move in the database
        [OperationContract]
        void UpdateMove(MoveRecord move);

        // Operation: Deletes a single move from the database
        [OperationContract]
        void DeleteMove(MoveRecord move);

        // Operation: Deletes all moves associated with a specific game
        // Must be called before DeleteGame to avoid FK constraint errors
        [OperationContract]
        void DeleteMovesByGameID(int gameID);

        // Operation: Retrieves all moves from the database
        [OperationContract]
        MoveList GetAllMoves();

        // Operation: Retrieves all moves for a specific game ordered by move index
        [OperationContract]
        MoveList GetMovesByGameID(int gameID);

        // Operation: Retrieves all moves made by a specific player across all games
        [OperationContract]
        MoveList GetMovesByPlayerID(int playerID);

        // Operation: Returns the most recent move in a specific game
        [OperationContract]
        MoveRecord ReturnLastMoveByGameID(int gameID);

        // Operation: Undoes the last move in a game by deleting it from the database
        [OperationContract]
        MoveRecord UndoLastMove(int gameID);

        // ============= USER/PLAYER OPERATIONS =============

        // Operation: Retrieves all users from the database
        [OperationContract]
        PlayerList GetAllUsers();

        // Operation: Retrieves a specific user by their ID
        [OperationContract]
        Player GetUserByID(int userID);

        // Operation: Retrieves a user by their exact username
        [OperationContract]
        Player GetUserByName(string name);

        // Operation: Searches for users whose username matches a pattern
        [OperationContract]
        PlayerList SearchUsersByName(string name);

        // Operation: Inserts a new user into the database
        [OperationContract]
        void InsertUser(Player user);

        // Operation: Updates an existing user in the database
        [OperationContract]
        void UpdateUser(Player user);

        // Operation: Deletes a user from the database
        [OperationContract]
        void DeleteUser(Player user);

        // ============= GAME STATISTICS & LOGIC =============

        // Operation: Updates player statistics after a game ends
        // Increments GamesPlayed and either Wins or Losses based on the won parameter
        [OperationContract]
        void UpdatePlayerStats(int playerID, bool won);

        // Operation: Updates player statistics after a draw
        // Increments GamesPlayed and Draws for the specified player
        [OperationContract]
        void UpdatePlayerDraw(int playerID);

        // Operation: Checks if it's a specific player's turn in a game
        // Based on move count: even number of moves = white's turn, odd = black's turn

        // Operation: Reverts player statistics when a game is deleted
        [OperationContract]
        void RevertPlayerStats(int playerID, bool won);

        // Operation: Reverts player draw statistics when a game is deleted
        [OperationContract]
        void RevertPlayerDraw(int playerID);

        [OperationContract]
        bool IsPlayerTurn(int gameID, int playerID);

        // ============= AUTHENTICATION =============

        // Operation: Signs in an admin user using Firebase authentication
        // Returns the admin Player object if successful, null otherwise
        [OperationContract]
        Player SignIn(string email, string password);



        // ============= FRIENDSHIP OPERATIONS =============
        [OperationContract] FriendshipList GetAllFriendships();
        [OperationContract] FriendshipList GetAcceptedFriendsByUser(int userID);
        [OperationContract] FriendshipList GetPendingFriendRequestsForUser(int userID);
        [OperationContract] bool FriendshipExists(int userA, int userB);
        [OperationContract] int SendFriendRequest(int requesterID, int receiverID);
        [OperationContract] void AcceptFriendRequest(int friendshipID);
        [OperationContract] void DeleteFriendship(int friendshipID);

        [OperationContract] void DeclineFriendRequest(int friendshipID);
    }
}