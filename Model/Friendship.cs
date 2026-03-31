using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace Model
{
    [DataContract]
    public class Friendship : BaseEntity
    {
        private int requesterID;
        private int receiverID;
        private bool isAccepted;
        private DateTime? friendshipDate;

        [DataMember] public int RequesterID { get { return requesterID; } set { requesterID = value; } }
        [DataMember] public int ReceiverID { get { return receiverID; } set { receiverID = value; } }
        [DataMember] public bool IsAccepted { get { return isAccepted; } set { isAccepted = value; } }
        [DataMember] public DateTime? FriendshipDate { get { return friendshipDate; } set { friendshipDate = value; } }
    }

    [CollectionDataContract]
    public class FriendshipList : List<Friendship>
    {
        public FriendshipList() { }
        public FriendshipList(IEnumerable<Friendship> list) : base(list) { }
        public FriendshipList(IEnumerable<BaseEntity> list) : base(list.Cast<Friendship>()) { }
    }
}