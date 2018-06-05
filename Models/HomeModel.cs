﻿using quiniela.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace quiniela.Models
{
    public class HomeModel : BaseClass
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
        public string WelcomeUser { get; set; }
        
        public ScoresModal ScoresModal { get; set; }

        public string WinningPrize { get; set; }
        public string PrivatePolicyTitle { get; set; }
        public string UserName { get; set; }
        public double CurrencyExchange { get; set; }
        public string CurrenctExchangeTitle { get; set; }

        public string InvitationSuccessMsg { get; set; }
        public string FormErrMsg { get; set; }
        public string FieldEmail { get; set; }
        public string BtnInvite { get; set; }
        public string WantToJoin { get; set; }

        public string WeTakeCC { get; set; }
        public string CheckScores { get; set; }
        public string ChatTitle { get; set; }
        public string ChatSubTitle { get; set; }
        public string SendMessage { get; set; }
        public string FieldMessage { get; set; }

        public string FinalScoresMsg { get; set; }
    }
}