using quiniela.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace quiniela.Models
{
    public class AdminModel : BaseClass
    {
        public string InviteFriendText { get; set; }
        public string FieldValEmail { get; set; }
        public string FieldRequired { get; set; }
        public string SendMessage { get; set; }
        public string PageTitle { get; set; }
        public string MatchesList { get; set; }
    }
}