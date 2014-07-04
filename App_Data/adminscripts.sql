/*
select name from Users where Email  in (
select UserId from MatchScores where matchId in ('300186488', '300186485'))

update Users set state = 'Active' where state <> 'Admin'

*/