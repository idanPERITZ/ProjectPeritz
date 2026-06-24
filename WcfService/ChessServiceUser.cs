using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using ViewModel;

namespace WcfService
{
    // WCF Service class providing user-level operations for the chess system
    // Implements IChessServiceUser with duplex callback support
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ChessServiceUser : IChessServiceUser
    {
        // Static collection to track online players and their callback channels
        private static readonly Dictionary<int, IChessServiceUserCallback> callbacks = new Dictionary<int, IChessServiceUserCallback>();
        private static readonly List<Player> onlinePlayers = new List<Player>();
        private static readonly object lockObj = new object();

        // Get the callback channel for the current client
        private IChessServiceUserCallback CurrentCallback
        {
            get
            {
                return OperationContext.Current.GetCallbackChannel<IChessServiceUserCallback>();
            }
        }

        // ============= USER/PLAYER OPERATIONS =============

        public void InsertUser(Player user)
        {
            new UserDB().Insert(user);
        }

        public void UpdateUser(Player user)
        {
            new UserDB().Update(user);
        }

        public void DeleteUser(Player user)
        {
            new UserDB().Delete(user);
        }

        public PlayerList GetAllUsers()
        {
            return new UserDB().SelectAll();
        }

        public Player GetUserByID(int userID)
        {
            return new UserDB().SelectById(userID);
        }

        // ============= GAME OPERATIONS =============

        public GameList GetAllGames()
        {
            return new GameDB().SelectAll();
        }

        public GameList GetActiveGames()
        {
            return new GameDB().SelectAll();
        }

        public Game GetGameByID(int gameID)
        {
            return new GameDB().SelectById(gameID);
        }

        public Game GetGameByID(Game game)
        {
            if (game == null) return null;
            return new GameDB().SelectById(game.Id);
        }

        public Game InsertGame(Game game)
        {
            GameDB db = new GameDB();
            db.Insert(game);
            return db.GetGame(game);
        }

        public Game InsertGameAndReturn(Game game)
        {
            GameDB db = new GameDB();
            int newId = db.InsertAndReturnId(game);
            return db.SelectById(newId);
        }

        public void UpdateGame(Game game)
        {
            new GameDB().Update(game);
        }

        public void DeleteGame(Game game)
        {
            new GameDB().Delete(game);
        }

        public GameList GetGamesByPlayer(int playerID)
        {
            return new GameDB().SelectByPlayer(playerID);
        }

        public GameList GetGamesByPlayer(Player player)
        {
            if (player == null) return new GameList();
            return new GameDB().SelectByPlayer(player.Id);
        }

        public Game GetLatestGameForPlayer(int playerID)
        {
            GameList list = new GameDB().SelectByPlayer(playerID);
            if (list == null || list.Count == 0)
                return null;
            return list.Last();
        }

        public bool IsGameFinished(int gameID)
        {
            Game game = new GameDB().SelectById(gameID);
            return game != null && game.Result != null;
        }

        // ============= MOVE OPERATIONS =============

        public void InsertMove(MoveRecord move)
        {
            new MoveDB().Insert(move);
        }

        public void UpdateMove(MoveRecord move)
        {
            new MoveDB().Update(move);
        }

        public void DeleteMove(MoveRecord move)
        {
            new MoveDB().Delete(move);
        }

        public MoveList GetMovesByGameID(int gameID)
        {
            return new MoveDB().SelectByGame(gameID);
        }

        public MoveList GetMovesByPlayerID(int playerID)
        {
            return new MoveDB().SelectByPlayer(playerID);
        }

        public MoveRecord GetMoveByID(int moveID)
        {
            MoveDB moveDB = new MoveDB();
            MoveList moves = moveDB.SelectAll();
            return moves.FirstOrDefault(m => m.Id == moveID);
        }

        public MoveRecord GetLastMoveByGameID(int gameID)
        {
            MoveList moves = new MoveDB().SelectByGame(gameID);
            if (moves == null || moves.Count == 0)
                return null;
            return moves.Last();
        }

        public MoveRecord ReturnLastMoveByGameID(int gameID)
        {
            return GetLastMoveByGameID(gameID);
        }

        public MoveRecord UndoLastMove(int gameID)
        {
            MoveDB moveDB = new MoveDB();
            MoveList moves = moveDB.SelectByGame(gameID);
            if (moves == null || moves.Count == 0)
                return null;
            MoveRecord lastMove = moves.Last();
            moveDB.Delete(lastMove);
            return lastMove;
        }

        // ============= GAME LOGIC =============

        public bool IsPlayerTurn(int gameID, int playerID)
        {
            MoveList moves = new MoveDB().SelectByGame(gameID);
            Game game = new GameDB().SelectById(gameID);
            if (game == null)
                return false;
            if (moves.Count == 0)
                return playerID == game.WhitePlayerUserID.Id;
            bool isWhiteTurn = moves.Count % 2 == 0;
            if (isWhiteTurn)
                return playerID == game.WhitePlayerUserID.Id;
            else
                return playerID == game.BlackPlayerUserID.Id;
        }

        public void UpdatePlayerStats(int playerID, bool won)
        {
            Player player = new UserDB().SelectById(playerID);
            if (player == null)
                return;
            player.GamesPlayed++;
            if (won)
                player.Wins++;
            else
                player.Losses++;
            new UserDB().Update(player);
        }

        public void UpdatePlayerDraw(int playerID)
        {
            Player player = new UserDB().SelectById(playerID);
            if (player == null) return;
            player.GamesPlayed++;
            player.Draws++;
            new UserDB().Update(player);
        }

        // ============= AUTHENTICATION =============

        public async Task<string> SignUp(string email, string password)
        {
            FirebaseAuthService authService = new FirebaseAuthService();
            return await authService.SignUp(email, password);
        }

        public Player Login(string email, string password)
        {
            FirebaseAuthService authService = new FirebaseAuthService();
            var signInResult = authService.SignIn(email, password).GetAwaiter().GetResult();

            if (signInResult.Success)
            {
                Player player = new UserDB().Login(signInResult.LocalId, email);
                if (player == null)
                    return null;
                return player;
            }
            return null;
        }

        // ============= ONLINE PLAYERS & MULTIPLAYER =============

        public void PlayerJoin(Player player)
        {
            if (player == null) return;

            lock (lockObj)
            {
                // Remove if already exists (reconnect scenario)
                onlinePlayers.RemoveAll(p => p.Id == player.Id);
                callbacks.Remove(player.Id);

                // Add player to online list
                onlinePlayers.Add(player);
                callbacks[player.Id] = CurrentCallback;
            }

            // Notify all other clients that this player joined
            NotifyAllExcept(player.Id, callback => callback.PlayerJoined(player));
        }

        public void PlayerLeave(Player player)
        {
            if (player == null) return;

            lock (lockObj)
            {
                onlinePlayers.RemoveAll(p => p.Id == player.Id);
                callbacks.Remove(player.Id);
            }

            // Notify all clients that this player left
            NotifyAll(callback => callback.PlayerLeft(player));
        }

        public PlayerList GetOnlinePlayers()
        {
            lock (lockObj)
            {
                return new PlayerList(onlinePlayers);
            }
        }

        public void InvitePlayer(Player inviter, Player invited, bool isWhite)
        {
            if (invited == null) return;

            lock (lockObj)
            {
                if (callbacks.TryGetValue(invited.Id, out var callback))
                {
                    try
                    {
                        callback.RecieveInvitation(inviter, isWhite);
                    }
                    catch { }
                }
            }
        }

        public void RespondToInvitation(Player inviter, Player invited, bool accept, bool isWhite)
        {
            if (inviter == null) return;

            Game game = null;
            if (accept)
            {
                // Create a new game
                game = new Game
                {
                    WhitePlayerUserID = isWhite ? invited : inviter,
                    BlackPlayerUserID = isWhite ? inviter : invited,
                    GameDate = DateTime.Now,
                    Result = null
                };
                game = InsertGameAndReturn(game);
            }

            lock (lockObj)
            {
                if (callbacks.TryGetValue(inviter.Id, out var callback))
                {
                    try
                    {
                        callback.RecieveInvitationResponse(invited, accept, game);
                    }
                    catch { }
                }
            }
        }

        public void SendMove(MoveRecord move)
        {
            if (move == null) return;

            // Insert the move into the database
            InsertMove(move);

            // Get the game to find the opponent
            Game game = GetGameByID(move.GameID);
            if (game == null) return;

            // Get all moves to determine whose turn it was
            MoveList moves = GetMovesByGameID(move.GameID);

            // Determine who made the move based on move count
            // If move index is even (0, 2, 4...), white made the move
            // If move index is odd (1, 3, 5...), black made the move
            bool whiteMadeMove = move.MoveIndex % 2 == 0;

            // Determine opponent ID
            int opponentId = whiteMadeMove 
                ? game.BlackPlayerUserID.Id 
                : game.WhitePlayerUserID.Id;

            // Notify opponent of the move
            lock (lockObj)
            {
                if (callbacks.TryGetValue(opponentId, out var callback))
                {
                    try
                    {
                        callback.RecievedMove(move);
                    }
                    catch { }
                }
            }
        }

        public void LeaveGame(Game game, Player leavingPlayer)
        {
            if (game == null || leavingPlayer == null) return;

            // Determine opponent ID
            int opponentId = (leavingPlayer.Id == game.WhitePlayerUserID.Id)
                ? game.BlackPlayerUserID.Id
                : game.WhitePlayerUserID.Id;

            // Notify opponent that player left
            lock (lockObj)
            {
                if (callbacks.TryGetValue(opponentId, out var callback))
                {
                    try
                    {
                        callback.OpponentLeftGame();
                    }
                    catch { }
                }
            }
        }

        // ============= FRIENDSHIP OPERATIONS =============

        public FriendshipList GetAcceptedFriendsByPlayer(int playerID)
        {
            return new FriendshipDB().SelectAcceptedByUser(playerID);
        }

        public FriendshipList GetAcceptedFriendsByPlayer(Player player)
        {
            if (player == null) return new FriendshipList();
            return new FriendshipDB().SelectAcceptedByUser(player.Id);
        }

        public FriendshipList GetPendingFriendRequestsForPlayer(int playerID)
        {
            return new FriendshipDB().SelectPendingForUser(playerID);
        }

        public FriendshipList GetPendingFriendRequestsForPlayer(Player player)
        {
            if (player == null) return new FriendshipList();
            return new FriendshipDB().SelectPendingForUser(player.Id);
        }

        public bool FriendshipExists(int userA, int userB)
        {
            return new FriendshipDB().FriendshipExists(userA, userB);
        }

        public int SendFriendRequest(int requesterID, int receiverID)
        {
            Friendship f = new Friendship
            {
                RequesterID = requesterID,
                ReceiverID = receiverID
            };
            return new FriendshipDB().InsertAndReturnId(f);
        }

        public int SendFriendRequest(Friendship friendship)
        {
            if (friendship == null) return 0;
            return new FriendshipDB().InsertAndReturnId(friendship);
        }

        public void AcceptFriendRequest(int friendshipID)
        {
            Friendship f = new Friendship { Id = friendshipID };
            new FriendshipDB().Update(f);
        }

        public void AcceptFriendRequest(Friendship friendship)
        {
            if (friendship == null) return;
            new FriendshipDB().Update(friendship);
        }

        public void DeleteFriendship(int friendshipID)
        {
            Friendship f = new Friendship { Id = friendshipID };
            new FriendshipDB().Delete(f);
        }

        public void DeleteFriendship(Friendship friendship)
        {
            if (friendship == null) return;
            new FriendshipDB().Delete(friendship);
        }

        public void DeclineFriendRequest(int friendshipID)
        {
            Friendship f = new Friendship { Id = friendshipID };
            new FriendshipDB().Delete(f);
        }

        public void DeclineFriendRequest(Friendship friendship)
        {
            if (friendship == null) return;
            new FriendshipDB().Delete(friendship);
        }

        // ============= HELPER METHODS FOR CALLBACKS =============

        private void NotifyAll(Action<IChessServiceUserCallback> action)
        {
            lock (lockObj)
            {
                foreach (var callback in callbacks.Values.ToList())
                {
                    try
                    {
                        action(callback);
                    }
                    catch
                    {
                        // Ignore failed callbacks (disconnected clients)
                    }
                }
            }
        }

        private void NotifyAllExcept(int exceptPlayerId, Action<IChessServiceUserCallback> action)
        {
            lock (lockObj)
            {
                foreach (var kvp in callbacks.ToList())
                {
                    if (kvp.Key == exceptPlayerId) continue;
                    try
                    {
                        action(kvp.Value);
                    }
                    catch
                    {
                        // Ignore failed callbacks (disconnected clients)
                    }
                }
            }
        }
    }
}