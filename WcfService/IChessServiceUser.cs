using Model;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using ViewModel;

namespace WcfService
{
    // Callback interface for duplex communication (server -> client notifications)
    public interface IChessServiceUserCallback
    {
        // Callback: Notifies client when a player joins the system
        [OperationContract(IsOneWay = true)]
        void PlayerJoined(Player player);

        // Callback: Notifies client when a player leaves the system
        [OperationContract(IsOneWay = true)]
        void PlayerLeft(Player player);

        // Callback: Notifies client when a move is received in their game
        [OperationContract(IsOneWay = true)]
        void RecievedMove(MoveRecord move);

        // Callback: Notifies client when they receive a game invitation
        [OperationContract(IsOneWay = true)]
        void RecieveInvitation(Player inviter, bool isWhite);

        // Callback: Notifies client of invitation response (accept/decline)
        [OperationContract(IsOneWay = true)]
        void RecieveInvitationResponse(Player inviter, bool accept, Game game);

        // Callback: Notifies client when opponent leaves the game
        [OperationContract(IsOneWay = true)]
        void OpponentLeftGame();
    }

    // WCF Service Contract interface defining all user-level operations for the chess system
    // All methods marked with [OperationContract] are exposed as web service endpoints
    // Supports duplex communication for real-time notifications
    [ServiceContract(CallbackContract = typeof(IChessServiceUserCallback))]
    public interface IChessServiceUser
    {
        // ============= GAME OPERATIONS =============

        // Operation: Retrieves all games from the database
        [OperationContract]
        GameList GetAllGames();

        // Operation: Retrieves all active/ongoing games (no result yet)
        [OperationContract]
        GameList GetActiveGames();

        // Operation: Retrieves a specific game by its ID
        [OperationContract]
        Game GetGameByID(int gameID);

        // Alternative signature: Get game by passing a Game object
        [OperationContract(Name = "GetGameByIDWithGame")]
        Game GetGameByID(Game game);

        // Operation: Inserts a new game into the database and returns it
        [OperationContract]
        Game InsertGame(Game game);

        // Operation: Inserts a new game and returns it with its auto-generated ID
        [OperationContract]
        Game InsertGameAndReturn(Game game);

        // Operation: Updates an existing game in the database
        [OperationContract]
        void UpdateGame(Game game);

        // Alias for backward compatibility - updates game result
        [OperationContract(Name = "UpdateGameResult")]
        void UpdateGameResult(Game game);

        // Operation: Deletes a game from the database
        [OperationContract]
        void DeleteGame(Game game);

        // Alias for backward compatibility - deletes open game
        [OperationContract(Name = "DeleteOpen")]
        void DeleteOpen(Game game);

        // Operation: Retrieves all games where a specific player participated
        [OperationContract]
        GameList GetGamesByPlayer(int playerID);

        // Alternative signature: Get games by Player object
        [OperationContract(Name = "GetGamesByPlayerWithPlayer")]
        GameList GetGamesByPlayer(Player player);

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

        // Alias for backward compatibility with old WPF client
        [OperationContract(Name = "GetAllplayers")]
        PlayerList GetAllplayers();

        // Operation: Retrieves a specific user by their ID
        [OperationContract]
        Player GetUserByID(int userID);

        // Operation: Inserts a new user into the database
        [OperationContract]
        void InsertUser(Player user);

        // Alias for backward compatibility - takes player and password
        [OperationContract(Name = "InsertPlayer")]
        void InsertPlayer(Player player, string password);

        // Operation: Updates an existing user in the database
        [OperationContract]
        void UpdateUser(Player user);

        // Operation: Deletes a user from the database
        [OperationContract]
        void DeleteUser(Player user);

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

        // Alternative name for getting last move
        [OperationContract(Name = "ReturnLastMoveByGameID")]
        MoveRecord ReturnLastMoveByGameID(int gameID);

        // Operation: Undoes the last move in a game by deleting it from the database
        [OperationContract]
        MoveRecord UndoLastMove(int gameID);

        // ============= GAME LOGIC =============

        // Operation: Checks if it's a specific player's turn in a game
        // Based on move count: even number of moves = white's turn, odd = black's turn
        [OperationContract]
        bool IsPlayerTurn(int gameID, int playerID);

        // Operation: Updates player statistics after a game ends
        // Increments GamesPlayed and either Wins or Losses based on the won parameter
        [OperationContract]
        void UpdatePlayerStats(int playerID, bool won);

        // Operation: Updates player statistics after a draw
        // Increments GamesPlayed and Draws for the specified player
        [OperationContract]
        void UpdatePlayerDraw(int playerID);

        // ============= AUTHENTICATION =============

        // Operation: Registers a new user with Firebase authentication
        // Returns a success or error message string
        [OperationContract]
        Task<string> SignUp(string email, string password);

        // Operation: Signs in a user using Firebase authentication
        // Returns the authenticated Player object or null if authentication fails
        [OperationContract]
        Player Login(string email, string password);

        // ============= ONLINE PLAYERS & MULTIPLAYER =============

        // Operation: Player joins the online system (registers their callback)
        [OperationContract]
        void PlayerJoin(Player player);

        // Operation: Player leaves the online system (unregisters their callback)
        [OperationContract]
        void PlayerLeave(Player player);

        // Operation: Gets list of all currently online players
        [OperationContract]
        PlayerList GetOnlinePlayers();

        // Operation: Sends a game invitation from one player to another
        [OperationContract]
        void InvitePlayer(Player inviter, Player invited, bool isWhite);

        // Operation: Responds to a game invitation (accept or decline)
        [OperationContract]
        void RespondToInvitation(Player inviter, Player invited, bool accept, bool isWhite);

        // Operation: Sends a move in real-time to the opponent
        [OperationContract]
        void SendMove(MoveRecord move);

        // Operation: Leaves an active game (notifies opponent)
        [OperationContract]
        void LeaveGame(Game game, Player leavingPlayer);

        // ============= FRIENDSHIP OPERATIONS =============

        // Operation: Returns all accepted friendships for a specific user
        [OperationContract]
        FriendshipList GetAcceptedFriendsByPlayer(int playerID);

        // Alternative signature: Get accepted friends by Player object
        [OperationContract(Name = "GetAcceptedFriendsByPlayerWithPlayer")]
        FriendshipList GetAcceptedFriendsByPlayer(Player player);

        // Operation: Returns all pending friend requests sent TO a specific user
        [OperationContract]
        FriendshipList GetPendingFriendRequestsForPlayer(int playerID);

        // Alternative signature: Get pending requests by Player object
        [OperationContract(Name = "GetPendingFriendRequestsForPlayerWithPlayer")]
        FriendshipList GetPendingFriendRequestsForPlayer(Player player);

        // Operation: Checks if friendship exists between two players
        [OperationContract]
        bool FriendshipExists(int userA, int userB);

        // Operation: Sends a friend request from requester to receiver
        [OperationContract]
        int SendFriendRequest(int requesterID, int receiverID);

        // Alternative signature: Send friend request with Friendship object
        [OperationContract(Name = "SendFriendRequestWithObject")]
        int SendFriendRequest(Friendship friendship);

        // Operation: Accepts a pending friend request
        [OperationContract]
        void AcceptFriendRequest(int friendshipID);

        // Alternative signature: Accept friend request with Friendship object
        [OperationContract(Name = "AcceptFriendRequestWithObject")]
        void AcceptFriendRequest(Friendship friendship);

        // Operation: Deletes a friendship record entirely
        [OperationContract]
        void DeleteFriendship(int friendshipID);

        // Alternative signature: Delete friendship with Friendship object
        [OperationContract(Name = "DeleteFriendshipWithObject")]
        void DeleteFriendship(Friendship friendship);

        // Operation: Declines a pending friend request by deleting it
        [OperationContract]
        void DeclineFriendRequest(int friendshipID);

        // Alternative signature: Decline friend request with Friendship object
        [OperationContract(Name = "DeclineFriendRequestWithObject")]
        void DeclineFriendRequest(Friendship friendship);

    }
}