USE [quiniela]
GO

USE [quiniela]
GO

-----------------------
/****** Object:  Table [dbo].[MatchScores]    Script Date: 05/14/2014 11:22:37 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MatchScores]') AND type in (N'U'))
DROP TABLE [dbo].[MatchScores]
GO

USE [quiniela]
GO

/****** Object:  Table [dbo].[MatchScores]    Script Date: 05/14/2014 11:22:37 ******/
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING ON
GO
------------------------
/****** Object:  Table [dbo].[FinalScores]    Script Date: 05/14/2014 11:23:29 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FinalScores]') AND type in (N'U'))
DROP TABLE [dbo].[FinalScores]
GO

USE [quiniela]
GO

/****** Object:  Table [dbo].[FinalScores]    Script Date: 05/14/2014 11:23:29 ******/
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
-----------------------------------------
/****** Object:  Table [dbo].[Users]    Script Date: 05/14/2014 11:23:58 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
DROP TABLE [dbo].[Users]
GO

USE [quiniela]
GO

/****** Object:  Table [dbo].[Users]    Script Date: 05/14/2014 11:23:58 ******/
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
-----------------------------------

/****** Object:  View [dbo].[v_MatchByUser]    Script Date: 05/14/2014 11:48:55 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[v_MatchByUser]'))
DROP VIEW [dbo].[v_MatchByUser]
GO

USE [quiniela]
GO

/****** Object:  View [dbo].[v_MatchByUser]    Script Date: 05/14/2014 11:48:55 ******/
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

