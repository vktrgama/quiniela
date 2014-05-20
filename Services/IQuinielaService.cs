using quiniela.Entities;
using System.Collections.Generic;

namespace quiniela.Services
{
    public interface IQuinielaService
    {
        ParticipantList GetParticipants();
        QException SaveScores(List<MatchScore> scores, string userId, bool submitted);
        string GetUserState(string userId);
        IList<MatchScore> RetrieveScores(string userId);
        string LogUser(string userId, string password);
        QException VerifyInvitation(string name, string email, string invitecode, string pin, string ipaddress);
        string CreateInvite(string email);
        Paticipant GetUser(string userId);
        Winner GetTopWinner();
        ParticipantList GetAllUsers();
        string UpdateField(string id, string fieldName, string fieldValue);
        string DeleteUser(string id);
        double GetCurrenctRate();
    }
}
