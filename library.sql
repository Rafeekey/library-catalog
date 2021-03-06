USE [library]
GO
/****** Object:  Table [dbo].[authors]    Script Date: 3/1/2017 5:12:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[authors](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](255) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[books]    Script Date: 3/1/2017 5:12:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[books](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[title] [varchar](255) NULL,
	[publication_date] [date] NULL,
	[copy_count] [int] NULL,
	[author_count] [int] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[books_authors]    Script Date: 3/1/2017 5:12:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[books_authors](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[books_id] [int] NULL,
	[authors_id] [int] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[checkout]    Script Date: 3/1/2017 5:12:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[checkout](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[checkout_date] [date] NULL,
	[return_date] [date] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[patron]    Script Date: 3/1/2017 5:12:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[patron](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[checkout_id] [int] NULL
) ON [PRIMARY]

GO
