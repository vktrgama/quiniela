using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace quiniela.Entities
{
    public class ParticipantList
    {
        public List<Paticipant> UserList { get; set; }
        public string Error { get; set; }
    }
}