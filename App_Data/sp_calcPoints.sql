USE [quiniela]
GO
/****** Object:  StoredProcedure [vktr].[sp_calculateMatchPoints]    Script Date: 06/06/2018 22:04:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [vktr].[sp_calculateMatchPoints] @Year nvarchar(4)
AS
BEGIN
	DECLARE @MatchId nvarchar(10)
	DECLARE @TH nvarchar(3) 
	DECLARE @TA nvarchar(3)
	DECLARE @SH int 
	DECLARE @SA int
	DECLARE @TransactionName varchar(5) = 'Tran1';

	DECLARE @Final CURSOR 
	SET @Final = CURSOR FAST_FORWARD 
	FOR Select MatchId, TeamHome, TeamAway, ScoreHome, ScoreAway From FinalScores where MatchPlayed = 1 and [Year] = @Year order by MatchId

	OPEN @Final
	FETCH NEXT FROM @Final
	INTO @MatchId,@TH, @TA, @SH, @SA

	BEGIN TRAN @TransactionName
	BEGIN TRY
		-- clear points to all
		update Users SET TotalPoints = 0 WHERE [Year] = @Year
		
		WHILE @@FETCH_STATUS = 0 
		BEGIN 			
			-- It is a match
			update u SET u.TotalPoints = u.TotalPoints + 1 
						FROM Users u inner join v_MatchByUser on v_MatchByUser.UserId = u.Email
							where MatchId = @MatchId
							and SH = @SH
							and u.[Year] = @Year
			
			update u SET u.TotalPoints = u.TotalPoints + 1 
						FROM Users u inner join v_MatchByUser on v_MatchByUser.UserId = u.Email
							where MatchId = @MatchId
							and SA = @SA
							and u.[Year] = @Year

			-- No match			
			IF (@SH - @SA) = 0
			BEGIN
				PRINT 'DRAW =>' + @MatchId + @TH + @TA 
				update u SET u.TotalPoints = u.TotalPoints + 1 
						FROM Users u inner join v_MatchByUser on v_MatchByUser.UserId = u.Email
							where MatchId = @MatchId
							and SH - SA = 0
							and u.[Year] = @Year
			END
			ELSE
			BEGIN
				-- Home Won
				IF (@SH - @SA) > 0
					BEGIN
						PRINT 'HOME =>' + @MatchId + @TH + @TA 
						update u SET u.TotalPoints = u.TotalPoints + 1 
							FROM Users u inner join v_MatchByUser on v_MatchByUser.UserId = u.Email
								where MatchId = @MatchId
								and SH - SA > 0
								and u.[Year] = @Year
					END
				ELSE
				-- Away Won
					BEGIN
						PRINT 'AWAY =>' + @MatchId + @TH + @TA 
						update u SET u.TotalPoints = u.TotalPoints + 1 
								FROM Users u inner join v_MatchByUser on v_MatchByUser.UserId = u.Email
									where MatchId = @MatchId
									and SH - SA < 0
									and u.[Year] = @Year
					END
			END
			
			FETCH NEXT FROM @Final 
			INTO @MatchId,@TH, @TA, @SH, @SA
		END

		COMMIT TRANSACTION @TransactionName;
		CLOSE @Final 
		DEALLOCATE @Final 
	
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION @TransactionName
	END CATCH
END

GRANT EXECUTE ON [vktr].[sp_calculateMatchPoints]
    TO PUBLIC;  
GO 