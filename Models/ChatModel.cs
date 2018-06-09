using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace quiniela.Models
{
    public class ChatModel : BaseClass
    {
        public string PageTitle { get; set; }
        public string ChatTitle { get; set; }
        public string ChatSubTitle { get; set; }
        public string LinksTitle { get; set; }
        public string Link1Text { get; set; }
        public string Link2Text { get; set; }
        public string Link3Text { get; set; }
        public string Link4Text { get; set; }
        public string Link5Text { get; set; }
        public string Link1Url { get; set; }
        public string Link2Url { get; set; }
        public string SendMessage { get; set; }
        public string FieldMessage { get; set; }

    }
}