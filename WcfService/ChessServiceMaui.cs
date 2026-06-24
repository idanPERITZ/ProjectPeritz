using Model;
using System.Threading.Tasks;
using ViewModel;

namespace WcfService
{
    public class ChessServiceMaui : IChessServiceMaui
    {
        public Player Login(string email, string password)
        {
            string signInResult = new FirebaseAuthService().SignIn(email, password).GetAwaiter().GetResult();

            if (signInResult == null || signInResult.ToLower().Contains("error"))
                return null;

            return new PlayerDB().Login(signInResult, email);
        }

        public async Task<string> SignUp(string username, string email, string password)
        {
            return await new FirebaseAuthService().SignUp(email, password);
        }

        public GameList GetGamesByPlayer(Player player)
        {
            if (player == null)
                return new GameList();

            return new GameDB().SelectByPlayer(player.Id);
        }

        public MoveList GetMovesByGameID(Game game)
        {
            if (game == null)
                return new MoveList();

            return new MoveDB().SelectByGame(game.Id);
        }
    }
}