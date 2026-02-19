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

        // Operation: Inserts a new game into the database
        [OperationContract]
        void InsertGame(Game game);

        // Operation: Deletes a game from the database
        [OperationContract]
        void DeleteGame(Game game);

        // Operation: Updates an existing game in the database
        [OperationContract]
        void UpdateGame(Game game);

        // Operation: Retrieves the most recent game for a specific player
        [OperationContract]
        Game GetLatestGameForPlayer(int playerID);

        // Operation: Retrieves all games where a specific player participated
        [OperationContract]
        GameList GetGamesByPlayer(int playerID);

        // Operation: Checks if a game has finished (has a result)
        [OperationContract]
        bool IsGameFinished(int gameID);

        // Operation: Retrieves all active/ongoing games
        [OperationContract]
        GameList GetActiveGames();

        // ============= USER/PLAYER OPERATIONS =============

        // Operation: Retrieves all users from the database
        [OperationContract]
        PlayerList GetAllUsers();

        // Operation: Searches for users by name pattern
        [OperationContract]
        PlayerList SearchUsersByName(string name);

        // Operation: Retrieves a user by their exact username
        [OperationContract]
        Player GetUserByName(string name);

        // Operation: Retrieves a specific user by their ID
        [OperationContract]
        Player GetUserByID(int userID);

        // Operation: Inserts a new user into the database
        [OperationContract]
        void InsertUser(Player user);

        // Operation: Deletes a user from the database
        [OperationContract]
        void DeleteUser(Player user);

        // Operation: Updates an existing user in the database
        [OperationContract]
        void UpdateUser(Player user);

        // ============= MOVE OPERATIONS =============

        // Operation: Retrieves all moves from the database
        [OperationContract]
        MoveList GetAllMoves();

        // Operation: Retrieves all moves made by a specific player
        [OperationContract]
        MoveList GetMovesByPlayerID(int playerID);

        // Operation: Retrieves all moves for a specific game
        [OperationContract]
        MoveList GetMovesByGameID(int gameID);

        // Operation: Returns the most recent move in a specific game
        // ✅ FIX: Changed return type from Move to MoveRecord
        [OperationContract]
        MoveRecord ReturnLastMoveByGameID(int gameID);

        // Operation: Inserts a new move into the database
        // ✅ FIX: Changed parameter type from Move to MoveRecord
        [OperationContract]
        void InsertMove(MoveRecord move);

        // Operation: Deletes a move from the database
        // ✅ FIX: Changed parameter type from Move to MoveRecord
        [OperationContract]
        void DeleteMove(MoveRecord move);

        // Operation: Updates an existing move in the database
        // ✅ FIX: Changed parameter type from Move to MoveRecord
        [OperationContract]
        void UpdateMove(MoveRecord move);

        // Operation: Deletes all moves associated with a specific game
        [OperationContract]
        void DeleteMovesByGameID(int gameID);

        // Operation: Undoes the last move in a game by deleting it
        // ✅ FIX: Changed return type from Move to MoveRecord
        [OperationContract]
        MoveRecord UndoLastMove(int gameID);

        // ============= GAME STATISTICS & LOGIC =============

        // Operation: Updates player statistics after a game (wins/losses)
        [OperationContract]
        void UpdatePlayerStats(int playerID, bool won);

        // Operation: Updates ELO ratings for both players after a game
        [OperationContract]
        void UpdateEloRatings(int winnerID, int loserID, int winnerElo, int loserElo);

        // Operation: Checks if it's a specific player's turn in a game
        [OperationContract]
        bool IsPlayerTurn(int gameID, int playerID);

        // ============= AUTHENTICATION =============

        // Operation: Signs in an admin user using Firebase authentication
        [OperationContract]
        Player SignIn(string email, string password);
    }
}