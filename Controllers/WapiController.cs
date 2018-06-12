using quiniela.Entities;
using quiniela.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace quiniela.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class WapiController : Controller
    {
        private IQuinielaService _quinielaService;

        public WapiController()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WapiController"/> class.
        /// </summary>
        /// <param name="quinielaService">The quiniela service.</param>
        public WapiController(IQuinielaService quinielaService)
        {
            _quinielaService = quinielaService;
        }

        /// <summary>
        /// Gets the participants.
        /// </summary>
        /// <returns></returns>
        public JsonResult GetParticipants()
        {
            var result = _quinielaService.GetParticipants();
            return Json(new { err = result.Error, users = result.UserList }, "", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAllUsers()
        {
            var result = _quinielaService.GetAllUsers();
            return Json(new { err = result.Error, users = result.UserList }, "", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the states.
        /// </summary>
        /// <returns>list of user states</returns>
        public JsonResult GetStates()
        {
            return Json(new { states = _quinielaService.GetStates() }, "", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Sets the language.
        /// </summary>
        /// <param name="lang">The language.</param>
        public void SetLanguage(string lang)
        {
            Localizer.SetCulture(lang);
            return;
        }

        /// <summary>
        /// Users the registration.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public JsonResult UserRegistration(string name, string email, string invitecode, string pin)
        {
            var subject = Localizer.Get("RegFormSubjet");
            var message = Localizer.Get("RegFormMessage");

            QException result = _quinielaService.VerifyInvitation(name, email, invitecode, pin,
                HttpContext.Request.ServerVariables["REMOTE_ADDR"]);

            try
            {
                // Smtp requires authorization fro Gmail before sending any email
                if (result.Error == 0 && result.Message != "")
                {
                    new Smtp().SendEmail(email, subject, string.Format(message, ConfigurationManager.AppSettings["SiteDomain"]));
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
           

            return Json(new { err = result.Error, msg = result.Message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Submits the match scores.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="submitted">if set to <c>true</c> [submitted].</param>
        /// <param name="scores">The scores.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SubmitMatchScores(string userId, bool submitted, Array scores)
        {
            var feedback = new QException();
            var jss = new JavaScriptSerializer();
            var scoreList = jss.Deserialize<List<MatchScore>>(scores.GetValue(0).ToString());
            if (scoreList.Count > 0)
            {
                feedback = _quinielaService.SaveScores(scoreList, userId, submitted);
            }

            return Json(new { err = feedback.Error, msg = feedback.Message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Loads the user scores.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public JsonResult LoadUserScores(string userId)
        {
            var scores = _quinielaService.RetrieveScores(userId);
            return Json(new { scores = scores }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Users the login.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <param name="formTyp">The form typ.</param>
        /// <returns></returns>
        public JsonResult UserLogin(string email, string pin)
        {
            var feedback = _quinielaService.LogUser(email, pin);

            return (feedback == "success")
                ? Json(new { err = 0 }, JsonRequestBehavior.AllowGet)
                : Json(new { err = 1 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Sends the invite.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="emailFrom">The email from.</param>
        /// <returns>
        /// 0 successful
        /// </returns>
        public JsonResult SendInvite(string email, string emailFrom)
        {
            var msg = Localizer.Get("RegFormFail");

            try
            {
                var invitationCode = _quinielaService.CreateInvite(email.Trim());
                if (!string.IsNullOrEmpty(invitationCode))
                {
                    Paticipant user = _quinielaService.GetUser(emailFrom);
                    if (user.Name != null)
                    {
                        var subject = string.Format(Localizer.Get("SendInviteSubject"), user.Name);
                        var message = string.Format(Localizer.Get("SendInviteMessage"), invitationCode, ConfigurationManager.AppSettings["SiteDomain"]);

                        // Smtp requires authorization fro Gmail before sending any email
                        new Smtp().SendEmail(email.Trim(), subject, message);
                    }
                }
                return Json(new { err = 0 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { Console.Write(ex.Message); }

            return Json(new { err = 1, msg }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Sends the reminder.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="emailFrom">The email from.</param>
        /// <returns></returns>
        public JsonResult SendReminder(string email, string emailFrom)
        {
            // var subject = Localizer.Get("ReminderEmailSubjectCatchup");
            var subject = Localizer.Get("ReminderEmailSubject");
            var message = string.Format(Localizer.Get("ReminderEmailBody")
                , ConfigurationManager.AppSettings["SiteDomain"]);

            try
            {
                ParticipantList participants = _quinielaService.GetAllUsers();
                foreach (Paticipant user in participants.UserList)
                {
                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        // Smtp requires authorization fro Gmail before sending any email
                        new Smtp().SendEmail(user.Email.Trim(),
                            string.Format(subject, user.Name),
                            string.Format(message + "&lt;br/&gt;&lt;br/&gt;Email: {0}, Your/Tu Code/Codigo is/es: {1}&lt;br/&gt;&lt;br/&gt;", user.Email, user.InviteCode));
                    }
                }
                return Json(new { err = 0 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { Console.Write(ex.Message); }

            var msg = Localizer.Get("RegFormFail");
            return Json(new { err = 1, msg }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Sends the invites.
        /// </summary>
        /// <param name="emails">The emails.</param>
        /// <returns></returns>
        public JsonResult SendInvites(string emails)
        {
            var emailList = emails.Split(',');
            foreach (var email in emailList)
            {
                var result = SendInvite(email.Trim(), ConfigurationManager.AppSettings["ownerEmail"]);
            }

            return Json(new { err = 0 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Users the feedback.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="email">The email.</param>
        /// <param name="msg">The MSG.</param>
        /// <returns></returns>
        public JsonResult UserFeedback(string name, string email, string msg)
        {
            try
            {
                // Smtp requires authorization fro Gmail before sending any email
                new Smtp().SendEmail(email, "FIFA World Cup Feedback from " + name, msg);

                return Json(new { err = 0 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { Console.Write(ex.Message); }

            return Json(new { err = 1 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Successes the payment.
        /// </summary>
        /// <returns></returns>
        public JsonResult successPayment(object custom, string email)
        {
            try
            {
                // new Smtp().SendEmail(email, "Quiniela Feedback from " + name, msg);
                return Json(new { err = 0, msg = "sucessful payment", callback = custom, email = email }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return Json(new { err = 1, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Updates the field.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateField(string id, string fieldName, string fieldValue)
        {
            var result = _quinielaService.UpdateField(id, fieldName, fieldValue);

            return Json(new { result = result });
        }

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteUser(string userId)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(userId))
            {
                result = _quinielaService.DeleteUser(userId);
            }

            return (result == "success") ? Json(new { err = 0 }) : Json(new { err = 1 });
        }

        /// <summary>
        /// Calculates the points.
        /// </summary>
        /// <param name="matchId">The match identifier.</param>
        /// <param name="th">The th.</param>
        /// <param name="ta">The ta.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CalcPoints(string matchId, string th, string ta)
        {
            try
            {
                if (string.IsNullOrEmpty(th) && string.IsNullOrEmpty(ta))
                {
                    _quinielaService.UpdateTodayScores();
                }
                _quinielaService.CalcPoints(matchId, th, ta);

                return Json(new { err = 0 });
            }
            catch (Exception ex)
            {
                return Json(new { err = 1, msg = ex.Message });
            }
        }

        /// <summary>
        /// Loads the final results.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public JsonResult LoadFinalResults()
        {
            var scores = _quinielaService.LoadFinalResults();
            return Json(new { scores = scores }, JsonRequestBehavior.AllowGet);
        }
    }
}
