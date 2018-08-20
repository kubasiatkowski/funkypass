/****** Object:  Table [dbo].[dictionaries]    Script Date: 8/20/2018 10:11:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[dictionaries](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[langcode] [nvarchar](50) NOT NULL,
	[langname] [nvarchar](max) NOT NULL,
	[maxid] [int] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[words_dk]    Script Date: 8/20/2018 10:11:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[words_dk](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[word] [nvarchar](50) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[words_en]    Script Date: 8/20/2018 10:11:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[words_en](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[word] [nvarchar](50) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[words_hu]    Script Date: 8/20/2018 10:11:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[words_hu](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[word] [nvarchar](50) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[words_la]    Script Date: 8/20/2018 10:11:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[words_la](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[word] [nvarchar](50) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[words_nl]    Script Date: 8/20/2018 10:11:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[words_nl](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[word] [nvarchar](50) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[words_pl]    Script Date: 8/20/2018 10:11:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[words_pl](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[word] [nvarchar](50) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[words_shakespeare]    Script Date: 8/20/2018 10:11:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[words_shakespeare](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[word] [nvarchar](50) NOT NULL
) ON [PRIMARY]
GO


/***** Indexes *****/
CREATE UNIQUE CLUSTERED INDEX [ipl] ON [dbo].[words_pl]
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO

CREATE UNIQUE CLUSTERED INDEX [idk] ON [dbo].[words_dk]
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO

CREATE UNIQUE CLUSTERED INDEX [ien] ON [dbo].[words_en]
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO

CREATE UNIQUE CLUSTERED INDEX [ihu] ON [dbo].[words_hu]
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO

CREATE UNIQUE CLUSTERED INDEX [ila] ON [dbo].[words_la]
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO

CREATE UNIQUE CLUSTERED INDEX [inl] ON [dbo].[words_nl]
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO

CREATE UNIQUE CLUSTERED INDEX [ishakespeare] ON [dbo].[words_shakespeare]
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO

