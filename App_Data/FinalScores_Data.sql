insert FinalScores
select a.MatchId, a.Team, b.Team, 0, 0, 0 from 
	MatchScores a inner join (select * from MatchScores where Type = 'away') b
	on a.MatchId = b.MatchId and a.Type = 'home' and a.UserId = 'vktrgama@outlook.com'
	and a.MatchId not in (Select MatchId from FinalScores)