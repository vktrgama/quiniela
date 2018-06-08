using quiniela.Entities;
using quiniela.Helpers;
using quiniela.Services;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace quiniela.Controllers
{
    public class HomeController : Controller
    {
        private IQuinielaService _quinielaService;

        public HomeController()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="quinielaService">The quiniela service.</param>
        public HomeController(IQuinielaService quinielaService)
        {
            _quinielaService = quinielaService;
        }
        
        /// <summary>
        /// Indexes the specified dummy.
        /// </summary>
        /// <param name="dummy">The dummy.</param>
        /// <param name="userid">The userid.</param>
        /// <returns></returns>
        public ActionResult Index(string dummy, string userid)
        {
            Paticipant user = new Paticipant();
            if (!string.IsNullOrEmpty(userid))
            {
                userid = HttpUtility.UrlDecode(userid);
                user = _quinielaService.GetUser(userid);
            }

            var winner = _quinielaService.GetTopWinner();
            var currencyExchange = Math.Round(_quinielaService.GetCurrenctRate(), 2);

            // return View("Index", ModelHelper.GetHomeModel(user, winner, currencyExchange));
            return View("pronto");
        }

        /// <summary>
        /// Participantses this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Participants()
        {
            return View("participants", ModelHelper.GetParticipantModel());
        }

        /// <summary>
        /// Matcheses this instance.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult Matches(string userid)
        {
            Paticipant user = new Paticipant();
            if (!string.IsNullOrEmpty(userid))
            {
                userid = HttpUtility.UrlDecode(userid);
                user = _quinielaService.GetUser(userid);
                return View("matches_" + Localizer.GetCulture().TwoLetterISOLanguageName, ModelHelper.GetMatchesModel(user));
            } else
            {
                return View("LogOut", ModelHelper.GetLogOutModel());
            }

        }

        /// <summary>
        /// Registrations this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Registration()
        {
            return View("registration", ModelHelper.GetRegModel());
        }

        /// <summary>
        /// Ruleses this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Rules()
        {
            return View("rules", ModelHelper.GetRulesModel());
        }

        /// <summary>
        /// Chats this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Chat()
        {
            return View("chat", ModelHelper.GetChatModel());
        }

        /// <summary>
        /// Privacies this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Privacy()
        {
            return View("privacy", ModelHelper.GetPrivacyModel());
        }

        /// <summary>
        /// Admins this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Admin()
        {
            var model = ModelHelper.GetAdminModel();

            var matchList = _quinielaService.GetMatchList();
            if (matchList.Count>0){
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(matchList);
                model.MatchesList = json;
            }

            return View("admin", model);
        }
    }
}
