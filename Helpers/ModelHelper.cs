using quiniela.Entities;
using quiniela.Models;
using quiniela.Services;
using System.Configuration;
using System.Globalization;

namespace quiniela.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public static class ModelHelper
    {
        /// <summary>
        /// Gets the home model.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="TopWinner">The top winner.</param>
        /// <param name="currencyExchange">The currency exchange.</param>
        /// <returns></returns>
        public static HomeModel GetHomeModel(Paticipant user, Winner TopWinner, double currencyExchange)
        {

            var culture = Localizer.GetCulture();
            var currentCode = new RegionInfo(culture.LCID).ISOCurrencySymbol;

            // Build Submit Scores Modal
            ScoresModal submitModal = new ScoresModal
            {
                Title = Localizer.Get("SSModalTitle"),
                SubTitle = Localizer.Get("SSModalSubTitle"),
                Bullet1 = string.Format(Localizer.Get("SSModalBullet1"), Localizer.Get("BetAmount")),
                Bullet2 = Localizer.Get("SSModalBullet2"),
                Bullet3 = Localizer.Get("SSModalBullet3"),
                Bullet4 = Localizer.Get("SSModalBullet4"),
                Bullet5 = Localizer.Get("SSModalBullet5"),
                Warning = Localizer.Get("SSModalWarning"),
                WarningText = Localizer.Get("SSModalWarningTxt"),
                PayPalToolTip = Localizer.Get("SSModalPayPalToolTip")
            };

            var homeModel = new HomeModel
            {
                UserId = user.Email,
                UserState = user.State,
                UserName = user.Name,
                WelcomeUser = string.Format(Localizer.Get("WelcomeUser"), string.Empty),
                DomainPath = ConfigurationManager.AppSettings["DomainPath"],
                BgImage = ConfigurationManager.AppSettings["BgImage"],
                SiteSubtitle = Localizer.Get("SiteSubtitle"),
                MnuRules = Localizer.Get("MnuRules"),
                MnuParticipants = Localizer.Get("MnuParticipants"),
                MnuMatches = Localizer.Get("MnuMatches"),
                MnuChat = Localizer.Get("MnuChat"),
                MnuReg = Localizer.Get("MnuReg"),
                Title = string.Format(Localizer.Get("Title"), ConfigurationManager.AppSettings["WorldCupYear"]),
                TopWinDesc = Localizer.Get("TopWinDesc"),
                TopWinner = TopWinner.topWinner,
                TopWinnerPrice = string.Format("{0:C} {1}", TopWinner.prize, (currentCode != "USD") ? "MXN" : currentCode),
                TopWinTitle = Localizer.Get("TopWinTitle"),
                TopWinPrizeDesc = Localizer.Get("TopWinPrizeDesc"),
                WinningPrize = Localizer.Get("WinnerPrize"),
                UserLoggedIn = Localizer.Get("UserLoggedIn"),
                UserLoginFail = Localizer.Get("UserLoginFail"),
                PayPalFormCode = Localizer.Get("PayPalFormCode"),
                PayPalButton = Localizer.Get("PayPalButton"),
                ScoresModal = submitModal,
                ContentParagraph1 = Localizer.Get("ContentParagraph1"),
                ContentParagraph2 = Localizer.Get("ContentParagraph2"),
                FieldRequired = Localizer.Get("FieldRequired"),
                FieldValEmail = Localizer.Get("FieldValEmail"),
                FieldValPin = Localizer.Get("FieldValPin"),
                FieldPin = Localizer.Get("FieldPin"),
                LearnMore = Localizer.Get("LearnMore"),
                SignIn = Localizer.Get("SignIn"),
                LogIn = Localizer.Get("LogIn"),
                LogOut = Localizer.Get("LogOut"),
                MoveUp = Localizer.Get("MoveUp"),
                MoveDown = Localizer.Get("MoveDown"),
                PrivatePolicyTitle = Localizer.Get("PrivatePolicyTitle"),
                CurrenctExchangeTitle = Localizer.Get("CurrenctExchange"),
                InvitationSuccessMsg = Localizer.Get("InvitationSuccessMsg"),
                FormErrMsg = Localizer.Get("FormErrMsg"),
                FieldEmail = Localizer.Get("FieldEmail"),
                BtnInvite = Localizer.Get("BtnInvite"),
                WantToJoin = Localizer.Get("WantToJoin"),
                WeTakeCC = Localizer.Get("WeTakeCC"),
                CheckScores = Localizer.Get("CheckScores"),
                ChatTitle = Localizer.Get("ChatTitle"),
                ChatSubTitle = Localizer.Get("ChatSubTitle"),
                SendMessage = Localizer.Get("SendMessage"),
                FieldMessage = Localizer.Get("FieldMessage"),
                FinalScoresMsg = Localizer.Get("FinalScoresMsg"),
                RegisterNow = Localizer.Get("RegisterNow"),
                TitlePreviousWC = Localizer.Get("TitlePreviousWC"),
                MsgPreviousWC = Localizer.Get("MsgPreviousWC")
            };

            if (Localizer.Get("CurrenctExchange") != "")
            {
                homeModel.CurrenctExchangeTitle += " " + currencyExchange;
                homeModel.CurrencyExchange = currencyExchange;
            }

            return homeModel;
        }

        /// <summary>
        /// Gets the participant model.
        /// </summary>
        /// <returns></returns>
        public static ParticipantsModel GetParticipantModel()
        {
            var model = new ParticipantsModel()
            {
                PageTitle = Localizer.Get("MnuParticipants"),
                UserListTitle = Localizer.Get("UserListTitle"),
                Participants = Localizer.Get("Participants"),
                ParticipantDesc = Localizer.Get("ParticipantDesc"),
                ParticipantNode = Localizer.Get("ParticipantNode"),
                InviteFriendText = Localizer.Get("InviteFriendText"),
                FieldRequired = Localizer.Get("FieldRequired"),
                FieldValEmail = Localizer.Get("FieldValEmail"),
                SendMessage = Localizer.Get("SendMessage"),
                InviteFriend = Localizer.Get("InviteFriendText"),
                InvitationSuccessMsg = Localizer.Get("InvitationSuccessMsg"),
                FieldEmail = Localizer.Get("FieldEmail"),
                FormErrMsg = Localizer.Get("FormErrMsg")
            };

            return model;
        }

        /// <summary>
        /// Gets the matches model.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static MatchesModel GetMatchesModel(Paticipant user)
        {
            return new MatchesModel {
                UserId = user.Email,
                UserName = user.Name,
                UserState = user.State,
                DomainPath = ConfigurationManager.AppSettings["DomainPath"],
                PageTitle = Localizer.Get("MnuMatches"),
            };
        }

        public static LogOut GetLogOutModel()
        {
            return new LogOut
            {
                LogOutDescription = Localizer.Get("LogOutDescription"),
                LogOutMessage = Localizer.Get("LogOutMessage"),
                PageTitle = Localizer.Get("LogOutPageTitle"),
            };
        }

        /// <summary>
        /// Gets the chat model.
        /// </summary>
        /// <returns></returns>
        public static ChatModel GetChatModel()
        {
            return new ChatModel
            {
                PageTitle = Localizer.Get("MnuChat"),
                ChatTitle = Localizer.Get("ChatTitle"),
                ChatSubTitle = Localizer.Get("ChatSubTitle"),
                LinksTitle = Localizer.Get("LinksTitle"),
                Link1Text = Localizer.Get("Link1Text"),
                Link1Url = Localizer.Get("Link1Url"),
                Link2Text = Localizer.Get("Link2Text"),
                Link2Url = Localizer.Get("Link2Url"),
                SendMessage = Localizer.Get("SendMessage"),
                FieldMessage = Localizer.Get("FieldMessage")
            };
        }

        /// <summary>
        /// Gets the rules model.
        /// </summary>
        /// <returns></returns>
        public static RulesModel GetRulesModel()
        {
            return new RulesModel
            {
                PageTitle = Localizer.Get("MnuRules"),
                RulesTitle = Localizer.Get("RulesTitle"),
                DomainPath = ConfigurationManager.AppSettings["DomainPath"],
                MyScores = Localizer.Get("MnuMatches"),
                RegTitle = Localizer.Get("MnuReg"),
                MyScoresTitle = Localizer.Get("MnuMatches"),
                CalcTitle = Localizer.Get("CalcTitle"),
                WinnerTitle = Localizer.Get("WinnerTitle"),
                DonationTitle = Localizer.Get("DonationTitle"),
                Rules1 = string.Format(Localizer.Get("Rules1"), "_link2", ConfigurationManager.AppSettings["DomainPath"] + "/Home/Registration"),
                Rules2 = string.Format(Localizer.Get("Rules2"), "_link2", ConfigurationManager.AppSettings["DomainPath"] + "/Home/Matches", Localizer.Get("MnuMatches")),
                Rules3 = string.Format(Localizer.Get("Rules3"), "_link2", ConfigurationManager.AppSettings["DomainPath"] + "/Home/Matches", Localizer.Get("MnuMatches")),
                Rules4 = string.Format(Localizer.Get("Rules4"), "_link2", ConfigurationManager.AppSettings["DomainPath"] + "/Home/Matches", Localizer.Get("MnuMatches")),
                Rules5 = Localizer.Get("Rules5"),
                Rules6 = Localizer.Get("Rules6"),
                Rules7 = Localizer.Get("Rules7"),
                Rules8 = Localizer.Get("Rules8"),
                Rules9 = string.Format(Localizer.Get("Rules9"), "_link2"),
                Rules10 = Localizer.Get("Rules10"),
                Rules11 = Localizer.Get("Rules11"),
                Rules12 = Localizer.Get("Rules12"),
                Rules13 = Localizer.Get("Rules13"),
                Rules14 = Localizer.Get("Rules14"),
                Rules15 = Localizer.Get("Rules15"),
                Rules16 = Localizer.Get("Rules16"),
                Rules17 = Localizer.Get("Rules17"),  
                Rules18 = string.Format(Localizer.Get("Rules18"), Localizer.Get("BetAmount")),
                Rules19 = string.Format(Localizer.Get("Rules19"), "_link2"),
                Rules20 = Localizer.Get("Rules20"),
                Rules21 = Localizer.Get("Rules21"),
                Rules22 = Localizer.Get("Rules22"),
                FeedbackTitle = Localizer.Get("FeedbackTitle"),
                FeedbackSuccessMsg = Localizer.Get("FeedbackSuccessMsg"),
                FeedbackSuccessMsg1 = Localizer.Get("RegSuccessMsg1"),
                FeedbackFailMsg = Localizer.Get("FeedbackFailMsg"),
                FeedbackFailMsg1 = Localizer.Get("PleaseVerify"),
                FieldValMessage = Localizer.Get("FieldValMessage"),
                SendFeedbackMsg = Localizer.Get("SendFeedbackMsg"),
                FieldName = Localizer.Get("FieldName"),
                FieldEmail = Localizer.Get("FieldEmail"),
                FieldRequired = Localizer.Get("FieldRequired"),
                FieldValEmail = Localizer.Get("FieldValEmail"),
                FieldValName = Localizer.Get("FieldValName"),
                FieldMessage = Localizer.Get("FieldMessage"),
                BtnClear = Localizer.Get("BtnClear")
            };
        }

        /// <summary>
        /// Gets the reg model.
        /// </summary>
        /// <returns></returns>
        public static RegModel GetRegModel(){
            return new RegModel
            {
                PageTitle = Localizer.Get("MnuReg"),
                NewUserTitle = Localizer.Get("NewUserTitle"),
                InvitationDesc = Localizer.Get("InvitationDesc"),
                RegSuccessMsg = Localizer.Get("RegSuccessMsg"),
                RegSuccessMsg1 = Localizer.Get("RegSuccessMsg1"),
                RegFailMsg = Localizer.Get("RegFailMsg"),
                FieldValName = Localizer.Get("FieldValName"),
                FieldRequired = Localizer.Get("FieldRequired"),
                FieldValEmail = Localizer.Get("FieldValEmail"),
                FieldValInviteCode = Localizer.Get("FieldValInviteCode"),
                FieldValPin = Localizer.Get("FieldValPin2"),
                BtnClear = Localizer.Get("BtnClear"),
                BtnReg = Localizer.Get("BtnReg"),
                InviteFriend = Localizer.Get("InviteFriendText"),
                InvitationSuccessMsg = Localizer.Get("InvitationSuccessMsg"),
                FormErrMsg = Localizer.Get("FormErrMsg"),
                BtnSendMsg = Localizer.Get("SendMessage"),
                FieldValMessage = Localizer.Get("FieldValMessage"),
                FieldName = Localizer.Get("FieldName"),
                FieldPin = Localizer.Get("FieldPin"),
                FieldPinConfirmation = Localizer.Get("FieldPinConfirmation"),
                FieldEmail = Localizer.Get("FieldEmail"),
                FieldInvitation = Localizer.Get("FieldInvitation"),
                FieldMessage = Localizer.Get("FieldMessage")
            };
        }

        /// <summary>
        /// Gets the privacy model.
        /// </summary>
        /// <returns></returns>
        public static PrivacyModel GetPrivacyModel()
        {
            return new PrivacyModel
            {
                PageTitle = Localizer.Get("PrivatePolicyTitle"),
                PrivatePolicyTitle = Localizer.Get("PrivatePolicyTitle"),
                PrivacyMsg = Localizer.Get("PrivacyMsg"),
                PrivacyMsg1 = Localizer.Get("PrivacyMsg1"),
                PrivacyMsg2 = Localizer.Get("PrivacyMsg2"),
                PrivacyMsg3 = Localizer.Get("PrivacyMsg3"),
                PrivacyMsg4 = Localizer.Get("PrivacyMsg4"),
                PrivacyMsg5 = Localizer.Get("PrivacyMsg5")
            };
        }

        /// <summary>
        /// Gets the admin model.
        /// </summary>
        /// <returns></returns>
        public static AdminModel GetAdminModel()
        {
            return new AdminModel
            {
                InviteFriendText = Localizer.Get("InviteFriendText"),
                FieldRequired = Localizer.Get("FieldRequired"),
                FieldValEmail = Localizer.Get("FieldValEmail"),
                SendMessage = Localizer.Get("SendMessage")
            };
        }

    }
}