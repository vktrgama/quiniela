using quiniela.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;

namespace quiniela.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class QuinielaService : IQuinielaService
    {
        private SqlConnection conn = null;
        private double _donation = 20.00;
        public double _dollarExchangeRateToPesos = .13;

        public enum QuinielaState
        {
            New = 1,
            Active = 2,
            Submitted = 3,
            Playing = 4,
            Admin = 5
        }

        public ParticipantList GetParticipants()
        {
            var participants = new List<Paticipant>();

            try
            {
                OpenDatabase();

                var sql = string.Format("select * from dbo.Users where state <> '{0}' and year = {1} order by totalpoints desc, name", 
                                QuinielaState.Admin.ToString(),
                                ConfigurationManager.AppSettings["WorldCupYear"]);
                SqlCommand command = new SqlCommand(sql, conn);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        participants.Add(new Paticipant
                        {
                            Email = reader["Email"].ToString(),
                            Name = reader["Name"].ToString(),
                            IpAddress = reader["IpAddress"].ToString(),
                            State = reader["State"].ToString(),
                            TotalPoints = reader["TotalPoints"].ToString()
                        });
                    }
                }

                CloseDatabase();

                return new ParticipantList { Error = string.Empty, UserList = participants };
            }
            catch (Exception ex)
            {
                return new ParticipantList { Error = ex.Message, UserList = participants };
            }
        }

        /// <summary>
        /// Saves the scores.
        /// </summary>
        /// <param name="scores">The scores.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="submitted">The submitted.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public QException SaveScores(List<MatchScore> scores, string userId, bool submitted)
        {
            if (scores.Count == 0) new QException { Error = 0, Message = string.Empty };

            var validScores = scores.Where(s => s.value != string.Empty).ToList();
            if (validScores.Count == 0) new QException { Error = 0, Message = string.Empty };

            OpenDatabase();
            var trans = this.conn.BeginTransaction();

            SqlCommand command = conn.CreateCommand();
            command.Connection = conn;
            command.Transaction = trans;

            try
            {
                foreach (var score in validScores)
                {
                    var keys = score.name.Split('_');
                    try
                    {
                        command.CommandText = string.Format("insert into dbo.MatchScores (UserId, MatchId, Team, Score, Type, Year) values ('{0}', '{1}', '{2}', {3}, '{4}', {5})",
                                                             userId, keys[0], keys[1], score.value, keys[2], ConfigurationManager.AppSettings["WorldCupYear"]);
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException sqlex)
                    {
                        if (sqlex.Message.IndexOf("duplicate key") > 0)
                        {
                            command.CommandText = string.Format("update dbo.MatchScores set Score = {0}, Type = '{1}' where UserId = '{2}' and MatchId = '{3}' and Team = '{4}' and Year = {5}",
                                                             score.value, keys[2], userId, keys[0], keys[1], ConfigurationManager.AppSettings["WorldCupYear"]);
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            throw new Exception(sqlex.Source, sqlex);
                        }
                    }
                }

                if (submitted)
                {
                    command.CommandText = string.Format("update dbo.Users set state = '{0}' where Email = '{1}'", QuinielaState.Submitted.ToString(), userId);
                    command.ExecuteNonQuery();
                }

                trans.Commit();
                CloseDatabase();

                return new QException { Error = 0, Message = "success" };
            }
            catch (Exception ex)
            {
                trans.Rollback();
                Console.Write(ex.Message);
                return new QException { Error = 1, Message = ex.Message + ' ' + ex.InnerException };
            }
        }

        /// <summary>
        /// Gets the state of the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>User State</returns>
        public string GetUserState(string userId)
        {
            object UserState = string.Empty;
            try
            {
                OpenDatabase();

                var sql = string.Format("select State from dbo.Users where Email = '{0}' and Year = {1}", userId, ConfigurationManager.AppSettings["WorldCupYear"]);
                SqlCommand command = new SqlCommand(sql, conn);
                UserState = (string)command.ExecuteScalar();

                CloseDatabase();
            }
            catch (Exception ex) { Console.Write(ex.Message); }

            return UserState != null ? UserState.ToString() : string.Empty;
        }

        /// <summary>
        /// Retrieves the scores.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>List of scores by user</returns>
        public IList<MatchScore> RetrieveScores(string userId)
        {
            var scores = new List<MatchScore>();

            try
            {
                OpenDatabase();

                var sql = string.Format("select * from dbo.MatchScores where userId = '{0}' and Year = {1} order by MatchId", 
                            userId, ConfigurationManager.AppSettings["WorldCupYear"]);
                SqlCommand command = new SqlCommand(sql, conn);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        scores.Add(new MatchScore
                        {
                            name = string.Format("{0}_{1}_{2}", reader["MatchId"].ToString(), reader["Team"].ToString(), reader["Type"].ToString()),
                            value = reader["Score"].ToString()
                        });
                    }
                }

                CloseDatabase();
            }
            catch (Exception ex) { Console.Write(ex.Message); }

            return scores;
        }


        /// <summary>
        /// Logs the user.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public string LogUser(string userId, string password)
        {
            try
            {
                OpenDatabase();

                var sql = string.Format("select State from dbo.Users where Email = '{0}' and AccessCode = '{1}' and Year = {2}", 
                            userId, password, ConfigurationManager.AppSettings["WorldCupYear"]);
                SqlCommand command = new SqlCommand(sql, conn);
                var UserState = (string)command.ExecuteScalar();

                CloseDatabase();

                if (!string.IsNullOrEmpty(UserState) && UserState != "new")
                {
                    return "success";
                }
            }
            catch (Exception ex) { Console.Write(ex.Message); }

            return string.Empty;
        }

        /// <summary>
        /// Verifies the invitation.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="email">The email.</param>
        /// <param name="invitecode">The invitecode.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="ipaddress">The ipaddress.</param>
        /// <returns>
        /// True - valid user
        /// </returns>
        public QException VerifyInvitation(string name, string email, string invitecode, string pin, string ipaddress)
        {
            try
            {
                OpenDatabase();

                var sql = string.Format("select State from dbo.Users where InviteCode = '{1}' and Year = {2}", 
                    email,
                    invitecode,
                    ConfigurationManager.AppSettings["WorldCupYear"]);
                var command = new SqlCommand(sql, conn);
                var UserState = (string)command.ExecuteScalar();

                if (UserState != null)
                {
                    var newCommand = new SqlCommand(sql, conn);
                    newCommand.CommandText =
                        string.Format(
                            "update dbo.Users set state = '{0}', name = '{1}', accesscode='{2}', IpAddress = '{3}', Email = '{4}', Year = {5} where InviteCode = '{6}'",
                            QuinielaState.Active.ToString(), 
                            name, pin, ipaddress, email, 
                            ConfigurationManager.AppSettings["WorldCupYear"],
                            invitecode);
                    newCommand.ExecuteNonQuery();
                }
                else
                {
                    return new QException
                    {
                        Error = 1,
                        Message = Localizer.Get("RegInvalidCodeMsg")
                    };
                }

                CloseDatabase();
            }
            catch (Exception ex)
            {
                return new QException
                {
                    Error = 1,
                    Message = ex.Message
                };
            }

            return new QException
            {
                Error = 0,
                Message = "active"
            };
        }

        /// <summary>
        /// Creates the invite.
        /// </summary>
        /// <param name="email">The email.</param>
        public string CreateInvite(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                OpenDatabase();

                SqlCommand newCommand = new SqlCommand(string.Format("delete from dbo.Users where Email = '{0}' and Year = {1}", 
                    email,
                    ConfigurationManager.AppSettings["WorldCupYear"]), conn);
                newCommand.ExecuteNonQuery();

                var invitecode = Guid.NewGuid().ToString().Split('-')[0].ToString();
                var sql = string.Format("insert into dbo.Users (Email, InviteCode, state, TotalPoints, Year) values ('{0}', '{1}', '{2}', {3}, {4})", 
                            email, invitecode, QuinielaState.New.ToString(), 0, ConfigurationManager.AppSettings["WorldCupYear"]);

                SqlCommand command = new SqlCommand(sql, conn);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CloseDatabase();

                return invitecode;
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public Paticipant GetUser(string userId)
        {
            Paticipant user = new Paticipant();
            try
            {
                OpenDatabase();

                var sql = string.Format("select * from dbo.Users where Email = '{0}' and Year = {1}", userId, ConfigurationManager.AppSettings["WorldCupYear"]);
                SqlCommand command = new SqlCommand(sql, conn);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        user = new Paticipant
                        {
                            Email = reader["Email"].ToString(),
                            Name = reader["Name"].ToString(),
                            IpAddress = reader["IpAddress"].ToString(),
                            State = reader["State"].ToString(),
                            TotalPoints = reader["TotalPoints"].ToString(),
                            InviteCode = reader["InviteCode"].ToString()
                        };
                    }
                }

                CloseDatabase();
            }
            catch (Exception ex) { Console.Write(ex.Message); }

            return user;
        }

        /// <summary>
        /// Gets the top winner.
        /// </summary>
        /// <returns>Winner's Name</returns>
        public Winner GetTopWinner()
        {
            var winnerName = Localizer.Get("TopWinner");
            var winnerPrize = 0.00;

            try
            {
                OpenDatabase();

                var sql = string.Format("select name from dbo.Users where TotalPoints > 0 and Year = {0} order by TotalPoints desc", ConfigurationManager.AppSettings["WorldCupYear"]);
                SqlCommand command = new SqlCommand(sql, conn);
                var winner = (string)command.ExecuteScalar();

                if (!string.IsNullOrEmpty(winner))
                {
                    winnerName = winner;
                }

                command.CommandText = string.Format("select count(*) from dbo.Users where state <> 'Admin' and Year = {0}", ConfigurationManager.AppSettings["WorldCupYear"]);

                var numPartipants = (int)command.ExecuteScalar();
                if (numPartipants > 0)
                {
                    var totPrize = (numPartipants * _donation);
                    if (Localizer.GetCulture().TwoLetterISOLanguageName != "en")
                    {
                        totPrize = Math.Round(totPrize * GetCurrenctRate(), 2);
                    }
                    // winnerPrize = totPrize - (totPrize * .03) + .30; // paypal
                    winnerPrize = totPrize - (totPrize * .15); // quiniela
                }

                CloseDatabase();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            return new Winner() { topWinner = winnerName, prize = winnerPrize };
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns></returns>
        public ParticipantList GetAllUsers()
        {
            var participants = new List<Paticipant>();

            try
            {
                OpenDatabase();

                var sql = string.Format("select * from dbo.Users Where Year = {0} order by name", ConfigurationManager.AppSettings["WorldCupYear"]);
                SqlCommand command = new SqlCommand(sql, conn);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    var seq = 1;
                    while (reader.Read())
                    {
                        participants.Add(new Paticipant
                        {
                            Seq = seq,
                            Email = reader["Email"].ToString().Trim(),
                            Name = reader["Name"].ToString(),
                            AccessCode = reader["AccessCode"].ToString(),
                            InviteCode = reader["InviteCode"].ToString(),
                            IpAddress = reader["IpAddress"].ToString(),
                            State = reader["State"].ToString(),
                            TotalPoints = reader["TotalPoints"].ToString()
                        });
                        seq++;
                    }
                }

                CloseDatabase();

                return new ParticipantList { Error = string.Empty, UserList = participants };
            }
            catch (Exception ex)
            {
                return new ParticipantList { Error = ex.Message, UserList = participants };
            }
        }

        /// <summary>
        /// Updates the field.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns></returns>
        public string UpdateField(string id, string fieldName, string fieldValue)
        {
            try
            {
                OpenDatabase();
                SqlCommand command = new SqlCommand();
                command.Connection = conn;

                if (fieldName == "TotalPoints")
                {
                    command.CommandText = string.Format("update dbo.Users set {0} = {1} where Email = '{2}' and Year = {3}", 
                        fieldName, fieldValue, id, ConfigurationManager.AppSettings["WorldCupYear"]);
                }
                else
                {
                    command.CommandText = string.Format("update dbo.Users set {0} = '{1}' where Email = '{2}' and Year = {3}", 
                        fieldName, fieldValue, id, ConfigurationManager.AppSettings["WorldCupYear"]);
                }
                command.ExecuteNonQuery();

                CloseDatabase();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return "error";
            }

            return "success";
        }

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public string DeleteUser(string id)
        {
            OpenDatabase();
            var trans = this.conn.BeginTransaction();
            SqlCommand command = conn.CreateCommand();
            command.Connection = conn;
            command.Transaction = trans;

            try
            {
                command.CommandText = string.Format("delete from dbo.Users where Email = '{0}' and Year = {1}", id, ConfigurationManager.AppSettings["WorldCupYear"]);
                command.ExecuteNonQuery();
                command.CommandText = string.Format("delete from dbo.MatchScores where UserId = '{0}' and Year = {1}", id, ConfigurationManager.AppSettings["WorldCupYear"]);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                Console.Write(ex.Message);
                return "error";
            }

            trans.Commit();
            CloseDatabase();

            return "success";
        }

        /// <summary>
        /// Gets the currenct rate.
        /// </summary>
        /// <returns>Current exchange rate dollar/peso </returns>
        public double GetCurrenctRate()
        {
            try
            {
                WebRequest request = WebRequest.Create("http://free.currencyconverterapi.com/api/v5/convert?q=USD_MXN&compact=y");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();

                var json = new JavaScriptSerializer();
                var result = json.Deserialize<ExchangeRate>(responseFromServer);

                if (result != null && result.USD_MXN.val > 0)
                {
                   return result.USD_MXN.val;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            return _dollarExchangeRateToPesos * 100;
        }

        /// <summary>
        /// Gets the score fifa.
        /// </summary>
        /// <param name="matchId">The match identifier.</param>
        /// <returns></returns>
        public List<MatchScore> GetScoreFifa(string matchId)
        {
            var finalScore = new List<MatchScore>();
            try
            {
                WebRequest request = WebRequest.Create("http://www.fifa.com/worldcup/matches/index.html");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();

                CsQuery.CQ htmlBody = responseFromServer;
                var score = htmlBody[string.Format("div[data-id='{0}']", matchId)].Find(".fi-s__scoreText").Html();
                if (score.Contains('-'))
                {
                    finalScore.Add(new MatchScore { name = "home", value = score.Split('-')[0] });
                    finalScore.Add(new MatchScore { name = "away", value = score.Split('-')[1] });
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            return finalScore;
        }

        /// <summary>
        /// Updates the match dates.
        /// </summary>
        public void UpdateMatchDates()
        {
            try
            {
                WebRequest request = WebRequest.Create("http://www.fifa.com/worldcup/matches/index.html");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();

                CsQuery.CQ htmlBody = responseFromServer;
                var matches = htmlBody[string.Format("[data-id]")];
                if (matches.Length > 0)
                {
                    OpenDatabase();
                    foreach (var el in matches)
                    {
                        var id = el["data-id"];
                        var div = el.Cq();
                        var time = div.Find(".fi-s__scoreText").Html();
                        var dat = div.Find(".mu-i-date").Html();
                        if (time.Contains(':'))
                        {
                            time = Convert.ToDateTime(time).Subtract(new TimeSpan(0, 90, 0)).ToString("HH:mm");

                            SqlCommand command = new SqlCommand();
                            command.Connection = conn;
                            command.CommandText = string.Format("update dbo.MatchDate set MatchDate = convert(datetime, '{0}', 20) where MatchId = '{1}'", dat + ' ' + time, id);
                            command.ExecuteNonQuery();
                        }
                    }

                    CloseDatabase();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        /// <summary>
        /// Updates the today scores.
        /// </summary>
        public void UpdateTodayScores()
        {
            WebRequest request = WebRequest.Create("http://www.fifa.com/worldcup/matches/index.html");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();

            CsQuery.CQ htmlBody = responseFromServer;
            var todayMatch = htmlBody[string.Format("div[data-matchesdate='{0}{1}{2}']", DateTime.Now.Year, ("0" + DateTime.Now.Month).Substring(0, 2), DateTime.Now.Day)];
            var matches = todayMatch.Find("[data-id]");
            if (matches.Length > 0)
            {
                foreach (var match in matches)
                {
                    var id = match["data-id"];
                    // scape elements that are not related to match game
                    if (int.Parse(id) > 11000)
                    {
                        var div = match.Cq();
                        var score = div.Find(".fi-s__scoreText").Html();
                        if (score.Contains('-'))
                        {
                            OpenDatabase();
                            SqlCommand command = new SqlCommand();
                            command.Connection = conn;
                            command.CommandText = string.Format("update dbo.FinalScores set ScoreHome = {0}, ScoreAway = {1}, MatchPlayed = 1 where MatchId = '{2}', Year = {3}",
                                score.Split('-')[0], score.Split('-')[1], id, ConfigurationManager.AppSettings["WorldCupYear"]);
                            command.ExecuteNonQuery();
                            CloseDatabase();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Stores the match dates in memory.
        /// </summary>
        /// <returns></returns>
        public List<MatchDate> GetMatchDatesInMem()
        {
            var matchDates = new List<MatchDate>();

            OpenDatabase();

            var sql = "select * from dbo.MatchDate order by MatchDate";
            SqlCommand command = new SqlCommand(sql, conn);
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    DateTime dtime;
                    DateTime.TryParse(reader["MatchDate"].ToString(), out dtime);
                    matchDates.Add(new MatchDate
                    {
                        MatchId = reader["MatchId"].ToString(),
                        Date = dtime
                    });
                }
            }
            CloseDatabase();

            return matchDates;
        }

        /// <summary>
        /// Gets the states.
        /// </summary>
        /// <returns></returns>
        public List<string> GetStates()
        {
            return Enum.GetValues(typeof(QuinielaState)).Cast<QuinielaState>().Select(v => v.ToString()).ToList();
        }

        /// <summary>
        /// Calculates the points.
        /// </summary>
        /// <param name="matchId">The match identifier.</param>
        /// <param name="th">The th.</param>
        /// <param name="ta">The ta.</param>
        /// <returns>
        /// exception
        /// </returns>
        public void CalcPoints(string matchId, string th, string ta)
        {
            OpenDatabase();
            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            if (!string.IsNullOrEmpty(matchId) && !string.IsNullOrEmpty(th) && !string.IsNullOrEmpty(ta))
            {
                command.CommandText = string.Format("update dbo.FinalScores set ScoreHome = {0}, ScoreAway = {1}, MatchPlayed=1 where MatchId = '{2}' and Year = {3}", 
                    th, ta, matchId, ConfigurationManager.AppSettings["WorldCupYear"]);
                command.ExecuteNonQuery();
            }
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = string.Format("[vktr].[sp_calculateMatchPoints]");
            command.Parameters.Add("@Year", SqlDbType.VarChar).Value = ConfigurationManager.AppSettings["WorldCupYear"];
            command.ExecuteNonQuery();
            CloseDatabase();
        }

        /// <summary>
        /// Gets the match list.
        /// </summary>
        /// <returns></returns>
        public List<Match> GetMatchList()
        {
            OpenDatabase();
            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText = string.Format("select MatchId, TeamHome, TeamAway from dbo.FinalScores where Year = {0}", ConfigurationManager.AppSettings["WorldCupYear"]);
            var reader = command.ExecuteReader();

            var matchList = new List<Match>();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    matchList.Add(new Match
                    {
                        MatchId = reader["MatchId"].ToString(),
                        MatchName = string.Format("{0}-{1}-{2}",
                        reader["MatchId"].ToString(),
                        reader["TeamHome"].ToString(),
                        reader["TeamAway"].ToString())
                    });
                }
            }

            CloseDatabase();

            return matchList;
        }

        /// <summary>
        /// Loads the final results.
        /// </summary>
        /// <returns>
        /// List of final scores
        /// </returns>
        public List<MatchScore> LoadFinalResults()
        {
            var scores = new List<MatchScore>();
            try
            {
                OpenDatabase();

                var sql = string.Format("select * from dbo.FinalScores where MatchPlayed = 1 and Year = {0}", ConfigurationManager.AppSettings["WorldCupYear"]);
                SqlCommand command = new SqlCommand(sql, conn);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        scores.Add(new MatchScore
                        {
                            name = string.Format("{0}_{1}_home", reader["MatchId"].ToString(), reader["TeamHome"].ToString().Trim()),
                            value = reader["ScoreHome"].ToString()
                        });
                        scores.Add(new MatchScore
                        {
                            name = string.Format("{0}_{1}_away", reader["MatchId"].ToString(), reader["TeamAway"].ToString().Trim()),
                            value = reader["ScoreAway"].ToString()
                        });
                    }
                }

                CloseDatabase();
            }
            catch (Exception ex) { Console.Write(ex.Message); }

            return scores;
        }

        #region Database Connection

        /// <summary>
        /// Opens the database.
        /// </summary>
        private void OpenDatabase()
        {
            if (conn != null && conn.State != ConnectionState.Closed) return;
            try
            {
                var cs = GetConnectionStrings();
                conn = new SqlConnection(cs);
                conn.Open();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void CloseDatabase()
        {
            if (conn != null && conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }

        private static string GetConnectionStrings()
        {
            var connectionString = ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["CurrentDatabase"]].ConnectionString;
            return connectionString;
        }

        #endregion
    }
}