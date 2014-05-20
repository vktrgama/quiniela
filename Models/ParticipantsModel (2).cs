using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace quiniela.Models
{
    public class ParticipantsModel
    {
        public string UserListTitle { get; set; }
        public string Participants { get; set; }
        public string ParticipantDesc { get; set; }
        public string ParticipantNode { get; set; }
        public string InviteFriendText { get; set; }
        public string FieldValEmail { get; set; }
        public string FieldRequired { get; set; }
        public string SendMessage { get; set; }
        public string PageTitle { get; set; }
        public object InviteFriend { get; set; }
        public object InvitationSuccessMsg { get; set; }
        public object FormErrMsg { get; set; }
        public object FieldEmail { get; set; }
    }
}