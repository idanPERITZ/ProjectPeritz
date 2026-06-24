using Model;
using System.ServiceModel;
using System.Threading.Tasks;
using ViewModel;

namespace WcfService
{
    [ServiceContract]
    public interface IChessServiceMaui
    {
        [OperationContract]
        Player Login(string email, string password);

        [OperationContract]
        Task<string> SignUp(string username, string email, string password);

        [OperationContract]
        GameList GetGamesByPlayer(Player player);

        [OperationContract]
        MoveList GetMovesByGameID(Game game);
    }
}