/*

select name from Users where Email  in (
select UserId from MatchScores where matchId in ('300186488', '300186485'))

update Users set state = 'Active' where state <> 'Admin'

select * from (
select a.matchId, a.UserId, a.Score s1, b.score s2, case when a.score <> b.score THEN 'ALERT' ELSE '_' END msg 
	from MatchScores a 
	inner join MatchScores$ b on a.MatchId = b.matchId 
		and a.UserId = b.UserId and a.Type = b.Type
	) as t
	where t.msg <> '_'

*/
