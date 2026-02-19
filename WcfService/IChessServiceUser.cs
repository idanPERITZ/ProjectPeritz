using Model;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using ViewModel;

namespace WcfService
{
    // WCF Service Contract interface defining all user operations for the chess system
    [ServiceContract]
    public interface IChessServiceUser
    {
        // ============= GAME OPERATIONS =============

        // Operation: Retrieves all games where a specific player participated
        [OperationContract]
        GameList GetGamesByPlayer(int playerID);

        // Operation: Retrieves the most recent game for a specific player
        [OperationContract]
        Game GetLatestGameForPlayer(int playerID);

        // Operation: Retrieves a specific game by its ID
        [OperationContract]
        Game GetGameByID(int gameID);

        // ============= USER/PLAYER OPERATIONS =============

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

        // Operation: Retrieves all users from the database
        [OperationContract]
        PlayerList GetAllUsers();

        // ============= MOVE OPERATIONS =============

        // Operation: Retrieves all moves for a specific game
        [OperationContract]
        MoveList GetMovesByGameID(int gameID);

        // Operation: Retrieves all moves made by a specific player
        [OperationContract]
        MoveList GetMovesByPlayerID(int playerID);

        // Operation: Returns the last move in a specific game
        // ✅ FIX: Changed return type from Move to MoveRecord
        [OperationContract]
        MoveRecord GetLastMoveByGameID(int gameID);

        // Operation: Retrieves a specific move by its ID
        // ✅ FIX: Changed return type from Move to MoveRecord
        [OperationContract]
        MoveRecord GetMoveByID(int moveID);

        // ============= GAME LOGIC =============

        // Operation: Checks if it's a specific player's turn in a game
        [OperationContract]
        bool IsPlayerTurn(int gameID, int playerID);

        // Operation: Checks if a game has finished (has a result)
        [OperationContract]
        bool IsGameFinished(int gameID);

        // ============= AUTHENTICATION =============

        // Operation: Registers a new user with Firebase authentication
        [OperationContract]
        Task<string> SignUp(string email, string password);

        // Operation: Signs in a user using Firebase authentication
        [OperationContract]
        Player Login(string email, string password);
    }
}