using quiniela.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace quiniela.Models
{
    public class HomeModel
    {
        public string UserId { get; set; }
        public string UserState { get; set; }
        public string DomainPath { get; set; }
        public string SiteSubtitle { get; set; }
        public string Title { get; set; }
        public string LogOut { get; set; }
        public string ContentParagraph1 { get; set; }
        public string ContentParagraph2 { get; set; }
        public string LearnMore { get; set; }
        public string SignIn { get; set; }
        public string UserLoggedIn { get; set; }
        public string UserLoginFail { get; set; }
        public string FieldValEmail { get; set; }
        public string FieldRequired { get; set; }
        public string FieldValPin { get; set; }
        public string FieldPin { get; set; }
        public string LogIn { get; set; }
        public string TopWinDesc { get; set; }
        public string TopWinTitle { get; set; }
        public string TopWinner { get; set; }
        public string TopWinnerPrice { get; set; }
        public string TopWinPrizeDesc { get; set; }
        public string MnuRules { get; set; }
        public string MnuParticipants { get; set; }
        public string MnuMatches { get; set; }
        public string MnuChat { get; set; }
        public string MnuReg { get; set; }
        public string MoveUp { get; set; }
        public string MoveDown { get; set; }
        public string PayPalFormCode { get; set; }
        public string PayPalButton { get; set; }
        public object WelcomeUser { get; set; }
        
        public ScoresModal ScoresModal { get; set; }

        public object WinningPrize { get; set; }

        public object PrivatePolicyTitle { get; set; }

        public string UserName { get; set; }
    }
}