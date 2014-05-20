-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'sp_calculateMatchPoints'))
DROP PROCEDURE sp_calculateMatchPoints	
GO

CREATE PROCEDURE sp_calculateMatchPoints	
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
	FOR Select MatchId, TeamHome, TeamAway, ScoreHome, ScoreAway From FinalScores where MatchPlayed = 1 order by MatchId

	OPEN @Final
	FETCH NEXT FROM @Final
	INTO @MatchId,@TH, @TA, @SH, @SA

	BEGIN TRAN @TransactionName
	BEGIN TRY
		-- clear points to all
		update Users SET TotalPoints = 0
		
		WHILE @@FETCH_STATUS = 0 
		BEGIN 			
			-- It is a match
			update t SET t.TotalPoints = t.TotalPoints + 1 
						FROM Users t inner join v_MatchByUser on v_MatchByUser.UserId = t.Email
							where MatchId = @MatchId
							and SH = @SH
			
			update t SET t.TotalPoints = t.TotalPoints + 1 
						FROM Users t inner join v_MatchByUser on v_MatchByUser.UserId = t.Email
							where MatchId = @MatchId
							and SA = @SA

			-- No match			
			IF @SH = @SA
			BEGIN
				PRINT 'DRAW =>' + @MatchId + @TH + @TA 
				update t SET t.TotalPoints = t.TotalPoints + 1 
						FROM Users t inner join v_MatchByUser on v_MatchByUser.UserId = t.Email
							where MatchId = @MatchId
							and SH = SA
			END
			ELSE
			BEGIN
				-- Home Won
				IF (@SH - @SA) >= 0
					BEGIN
						PRINT 'HOME =>' + @MatchId + @TH + @TA 
						update t SET t.TotalPoints = t.TotalPoints + 1 
							FROM Users t inner join v_MatchByUser on v_MatchByUser.UserId = t.Email
								where MatchId = @MatchId
								and SH - SA >= 0
					END
				ELSE
				-- Away Won
					BEGIN
						PRINT 'AWAY =>' + @MatchId + @TH + @TA 
						update t SET t.TotalPoints = t.TotalPoints + 1 
								FROM Users t inner join v_MatchByUser on v_MatchByUser.UserId = t.Email
									where MatchId = @MatchId
									and SH - SA < 0
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
GO
