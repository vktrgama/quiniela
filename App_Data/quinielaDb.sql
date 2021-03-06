USE [master]
GO
/****** Object:  Database [quiniela]    Script Date: 7/3/2014 6:13:02 PM ******/
CREATE DATABASE [quiniela] ON  PRIMARY 
( NAME = N'quiniela', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\quiniela_data.mdf' , SIZE = 2304KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'quiniela_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\quiniela_log.ldf' , SIZE = 11200KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [quiniela] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [quiniela].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [quiniela] SET ANSI_NULL_DEFAULT ON 
GO
ALTER DATABASE [quiniela] SET ANSI_NULLS ON 
GO
ALTER DATABASE [quiniela] SET ANSI_PADDING ON 
GO
ALTER DATABASE [quiniela] SET ANSI_WARNINGS ON 
GO
ALTER DATABASE [quiniela] SET ARITHABORT ON 
GO
ALTER DATABASE [quiniela] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [quiniela] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [quiniela] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [quiniela] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [quiniela] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [quiniela] SET CURSOR_DEFAULT  LOCAL 
GO
ALTER DATABASE [quiniela] SET CONCAT_NULL_YIELDS_NULL ON 
GO
ALTER DATABASE [quiniela] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [quiniela] SET QUOTED_IDENTIFIER ON 
GO
ALTER DATABASE [quiniela] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [quiniela] SET  DISABLE_BROKER 
GO
ALTER DATABASE [quiniela] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [quiniela] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [quiniela] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [quiniela] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [quiniela] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [quiniela] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [quiniela] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [quiniela] SET RECOVERY FULL 
GO
ALTER DATABASE [quiniela] SET  MULTI_USER 
GO
ALTER DATABASE [quiniela] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [quiniela] SET DB_CHAINING OFF 
GO
USE [quiniela]
GO
/****** Object:  User [vktr]    Script Date: 7/3/2014 6:13:03 PM ******/
CREATE USER [vktr] FOR LOGIN [vktr] WITH DEFAULT_SCHEMA=[vktr]
GO
ALTER ROLE [db_owner] ADD MEMBER [vktr]
GO
/****** Object:  Schema [vktr]    Script Date: 7/3/2014 6:13:03 PM ******/
CREATE SCHEMA [vktr]
GO
/****** Object:  StoredProcedure [vktr].[sp_calculateMatchPoints]    Script Date: 7/3/2014 6:13:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [vktr].[sp_calculateMatchPoints]	
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
			IF (@SH - @SA) = 0
			BEGIN
				PRINT 'DRAW =>' + @MatchId + @TH + @TA 
				update t SET t.TotalPoints = t.TotalPoints + 1 
						FROM Users t inner join v_MatchByUser on v_MatchByUser.UserId = t.Email
							where MatchId = @MatchId
							and SH - SA = 0
			END
			ELSE
			BEGIN
				-- Home Won
				IF (@SH - @SA) > 0
					BEGIN
						PRINT 'HOME =>' + @MatchId + @TH + @TA 
						update t SET t.TotalPoints = t.TotalPoints + 1 
							FROM Users t inner join v_MatchByUser on v_MatchByUser.UserId = t.Email
								where MatchId = @MatchId
								and SH - SA > 0
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
/****** Object:  Table [dbo].[FinalScores]    Script Date: 7/3/2014 6:13:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FinalScores](
	[MatchId] [nvarchar](10) NOT NULL,
	[TeamHome] [nvarchar](3) NULL,
	[TeamAway] [nchar](10) NULL,
	[ScoreHome] [int] NULL,
	[ScoreAway] [int] NULL,
	[MatchPlayed] [bit] NULL,
 CONSTRAINT [PK_FinalScores] PRIMARY KEY CLUSTERED 
(
	[MatchId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MatchDate]    Script Date: 7/3/2014 6:13:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MatchDate](
	[MatchId] [nvarchar](10) NOT NULL,
	[MatchDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[MatchId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MatchScores]    Script Date: 7/3/2014 6:13:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MatchScores](
	[UserId] [varchar](250) NOT NULL,
	[MatchId] [varchar](10) NOT NULL,
	[Team] [varchar](3) NOT NULL,
	[Score] [int] NULL,
	[Type] [varchar](10) NULL,
 CONSTRAINT [PK_MatchScores] PRIMARY KEY NONCLUSTERED 
(
	[UserId] ASC,
	[MatchId] ASC,
	[Team] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[Users]    Script Date: 7/3/2014 6:13:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Email] [nvarchar](250) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[AccessCode] [nvarchar](4) NULL,
	[InviteCode] [nvarchar](12) NULL,
	[IpAddress] [nvarchar](50) NULL,
	[State] [nvarchar](50) NULL,
	[TotalPoints] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  View [dbo].[v_MatchByUser]    Script Date: 7/3/2014 6:13:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[v_MatchByUser]
AS
SELECT     a.UserId, a.MatchId, a.Team AS TH, b.Team AS TA, a.Score AS SH, b.Score AS SA
FROM         dbo.MatchScores AS a INNER JOIN
                          (SELECT     UserId, MatchId, Team, Score, Type
                            FROM          dbo.MatchScores
                            WHERE      (Type = 'away')) AS b ON a.MatchId = b.MatchId AND a.Type = 'home' AND a.UserId = b.UserId


GO
USE [master]
GO
ALTER DATABASE [quiniela] SET  READ_WRITE 
GO
