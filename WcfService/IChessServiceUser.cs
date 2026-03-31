using Model;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using ViewModel;

namespace WcfService
{
    // WCF Service Contract interface defining all user-level operations for the chess system
    // All methods marked with [OperationContract] are exposed as web service endpoints
    // Unlike IChessServiceAdmin, this interface only exposes operations safe for regular users
    [ServiceContract]
    public interface IChessServiceUser
    {
        // ============= GAME OPERATIONS =============

        // Operation: Retrieves a specific game by its ID
        [OperationContract]
        Game GetGameByID(int gameID);

        // Operation: Retrieves all games where a specific player participated
        [OperationContract]
        GameList GetGamesByPlayer(int playerID);

        // Operation: Retrieves the most recent game for a specific player
        [OperationContract]
        Game GetLatestGameForPlayer(int playerID);

        // Operation: Checks if a game has finished (has a result)
        [OperationContract]
        bool IsGameFinished(int gameID);

        // ============= USER/PLAYER OPERATIONS =============

        // Operation: Retrieves all users from the database
        [OperationContract]
        PlayerList GetAllUsers();

        // Operation: Retrieves a specific user by their ID
        [OperationContract]
        Player GetUserByID(int userID);

        // Operation: Inserts a new user into the database
        [OperationContract]
        void InsertUser(Player user);

        // Operation: Updates an existing user in the database
        [OperationContract]
        void UpdateUser(Player user);

        // Operation: Deletes a user from the database
        [OperationContract]
        void DeleteUser(Player user);

        // ============= MOVE OPERATIONS =============

        // Operation: Retrieves all moves for a specific game ordered by move index
        [OperationContract]
        MoveList GetMovesByGameID(int gameID);

        // Operation: Retrieves all moves made by a specific player across all games
        [OperationContract]
        MoveList GetMovesByPlayerID(int playerID);

        // Operation: Retrieves a specific move by its ID
        [OperationContract]
        MoveRecord GetMoveByID(int moveID);

        // Operation: Returns the last move made in a specific game
        [OperationContract]
        MoveRecord GetLastMoveByGameID(int gameID);

        // ============= GAME LOGIC =============

        // Operation: Checks if it's a specific player's turn in a game
        // Based on move count: even number of moves = white's turn, odd = black's turn
        [OperationContract]
        bool IsPlayerTurn(int gameID, int playerID);

        // ============= AUTHENTICATION =============

        // Operation: Registers a new user with Firebase authentication
        // Returns a success or error message string
        [OperationContract]
        Task<string> SignUp(string email, string password);

        // Operation: Signs in a user using Firebase authentication
        // Returns the authenticated Player object or null if authentication fails
        [OperationContract]
        Player Login(string email, string password);


    }
}