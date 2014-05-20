using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace quiniela.Models
{
    public class RegModel
    {
        public string RegSuccessMsg { get; set; }
        public string RegSuccessMsg1 { get; set; }
        public string RegFailMsg { get; set; }
        public string FieldValName { get; set; }
        public string FieldRequired { get; set; }
        public string FieldValEmail { get; set; }
        public string FieldValInviteCode { get; set; }
        public string FieldValPin { get; set; }
        public string BtnClear { get; set; }
        public string BtnReg { get; set; }
        public string NewUserTitle { get; set; }
        public string InvitationDesc { get; set; }
        public string InviteFriend { get; set; }
        public object InvitationSuccessMsg { get; set; }
        public object FormErrMsg { get; set; }
        public string BtnSendMsg { get; set; }
        public object FeedbackSuccessMsg { get; set; }
        public string FeedbackSuccessMsg1 { get; set; }
        public object FeedbackFailMsg { get; set; }
        public string FeedbackFailMsg1 { get; set; }
        public object FieldValMessage { get; set; }
        public object SendFeedbackMsg { get; set; }
        public string FieldName { get; set; }
        public string FieldPin { get; set; }
        public string FieldEmail { get; set; }
        public string FieldPinConfirmation { get; set; }
        public string FieldInvitation { get; set; }

        public string PageTitle { get; set; }

        public object FeedbackTitle { get; set; }

        public object FieldMessage { get; set; }
    }
}