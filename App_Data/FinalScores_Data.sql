insert [quiniela].[dbo].[FinalScores]
select a.MatchId, a.Team, b.Team, 0, 0, 0, 2018 from 
	[quiniela].[dbo].[MatchScores] a 
		inner join 
		(select * from [quiniela].[dbo].[MatchScores] where Type = 'away') b
	on a.MatchId = b.MatchId and a.Type = 'home' and a.UserId = 'vktr@vgama.com'
	and a.MatchId not in (Select MatchId from [quiniela].[dbo].[FinalScores])