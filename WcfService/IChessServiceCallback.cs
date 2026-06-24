using Model;
using System.ServiceModel;

namespace WcfService
{
    public interface IChessServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void PlayerJoined(Player player);

        [OperationContract(IsOneWay = true)]
        void PlayerLeft(Player player);

        [OperationContract(IsOneWay = true)]
        void RecievedMove(MoveRecord move);

        [OperationContract(IsOneWay = true)]
        void RecieveInvitation(Player inviter, bool isWhite);

        [OperationContract(IsOneWay = true)]
        void RecieveInvitationResponse(Player inviter, bool accept, Game game);

        [OperationContract(IsOneWay = true)]
        void OpponentLeftGame();
    }
}