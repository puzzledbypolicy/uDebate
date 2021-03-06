USE [DNN6]
GO
/****** Object:  Table [dbo].[uDebate_Forum_PostTypes]    Script Date: 21/05/2013 12:22:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[uDebate_Forum_PostTypes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](50) NOT NULL,
	[ThreadGroupId] [int] NOT NULL,
	[ReplyByID] [nvarchar](50) NOT NULL,
	[NotReplyByID] [nvarchar](50) NOT NULL,
	[StartingType] [int] NOT NULL,
	[Description] [nvarchar](1024) NULL,
 CONSTRAINT [PK_2uDebate_Forum_PostTypes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[uDebate_Forum_ThreadGroup]    Script Date: 21/05/2013 12:22:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[uDebate_Forum_ThreadGroup](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](1024) NULL,
 CONSTRAINT [PK_2uDebate_Forum_ThreadGroup] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[uDebate_Forum_ThreadStatus]    Script Date: 21/05/2013 12:22:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[uDebate_Forum_ThreadStatus](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](256) NULL,
 CONSTRAINT [PK_uDebate_Forum_ThreadStatus] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[uDebate_Forums]    Script Date: 21/05/2013 12:22:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[uDebate_Forums](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ModuleID] [int] NULL,
	[Language] [nvarchar](50) NULL,
	[Description] [nvarchar](1024) NOT NULL,
	[Summary] [nvarchar](4000) NULL,
	[Active] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[CREATED] [datetime] NOT NULL,
	[LAST_UPDATED] [datetime] NOT NULL,
	[CREATED_BY] [int] NOT NULL,
	[UPDATED_BY] [int] NOT NULL,
	[Opened_Date] [datetime] NULL,
	[Closed_Date] [datetime] NULL,
 CONSTRAINT [PK_2uDebate_Forums] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[uDebate_Forum_PostTypes] ON 

INSERT [dbo].[uDebate_Forum_PostTypes] ([ID], [Title], [ThreadGroupId], [ReplyByID], [NotReplyByID], [StartingType], [Description]) VALUES (1, N'Issue', 1, N'2,8,3,4', N'1', 1, NULL)
INSERT [dbo].[uDebate_Forum_PostTypes] ([ID], [Title], [ThreadGroupId], [ReplyByID], [NotReplyByID], [StartingType], [Description]) VALUES (2, N'Alternative', 1, N'3,4', N'1,2,8', 0, NULL)
INSERT [dbo].[uDebate_Forum_PostTypes] ([ID], [Title], [ThreadGroupId], [ReplyByID], [NotReplyByID], [StartingType], [Description]) VALUES (3, N'Argument', 1, N'3,4', N'1,2,8', 0, NULL)
INSERT [dbo].[uDebate_Forum_PostTypes] ([ID], [Title], [ThreadGroupId], [ReplyByID], [NotReplyByID], [StartingType], [Description]) VALUES (4, N'Gegenargument', 1, N'3,4', N'1,2,8', 0, NULL)
INSERT [dbo].[uDebate_Forum_PostTypes] ([ID], [Title], [ThreadGroupId], [ReplyByID], [NotReplyByID], [StartingType], [Description]) VALUES (5, N'Question', 2, N'6,7', N'5', 1, NULL)
INSERT [dbo].[uDebate_Forum_PostTypes] ([ID], [Title], [ThreadGroupId], [ReplyByID], [NotReplyByID], [StartingType], [Description]) VALUES (6, N'Answer', 2, N'7', N'5,6', 0, NULL)
INSERT [dbo].[uDebate_Forum_PostTypes] ([ID], [Title], [ThreadGroupId], [ReplyByID], [NotReplyByID], [StartingType], [Description]) VALUES (7, N'Comment', 2, N'7', N'5,6', 0, NULL)
INSERT [dbo].[uDebate_Forum_PostTypes] ([ID], [Title], [ThreadGroupId], [ReplyByID], [NotReplyByID], [StartingType], [Description]) VALUES (8, N'Comment', 1, N'8,3,4', N'1,2', 0, NULL)
SET IDENTITY_INSERT [dbo].[uDebate_Forum_PostTypes] OFF
SET IDENTITY_INSERT [dbo].[uDebate_Forum_ThreadGroup] ON 

INSERT [dbo].[uDebate_Forum_ThreadGroup] ([ID], [Title], [Description]) VALUES (1, N'Forum Type I', NULL)
INSERT [dbo].[uDebate_Forum_ThreadGroup] ([ID], [Title], [Description]) VALUES (2, N'Forum Type II', NULL)
SET IDENTITY_INSERT [dbo].[uDebate_Forum_ThreadGroup] OFF
SET IDENTITY_INSERT [dbo].[uDebate_Forum_ThreadStatus] ON 

INSERT [dbo].[uDebate_Forum_ThreadStatus] ([ID], [Title], [Description]) VALUES (1, N'Created', NULL)
INSERT [dbo].[uDebate_Forum_ThreadStatus] ([ID], [Title], [Description]) VALUES (2, N'Opened', NULL)
INSERT [dbo].[uDebate_Forum_ThreadStatus] ([ID], [Title], [Description]) VALUES (3, N'Closed', NULL)
SET IDENTITY_INSERT [dbo].[uDebate_Forum_ThreadStatus] OFF
SET IDENTITY_INSERT [dbo].[uDebate_Forums] ON 

INSERT [dbo].[uDebate_Forums] ([ID], [ModuleID], [Language], [Description], [Summary], [Active], [Status], [CREATED], [LAST_UPDATED], [CREATED_BY], [UPDATED_BY], [Opened_Date], [Closed_Date]) VALUES (4, 459, N'en-GB', N'uDebate', N'Discussion about immigration', 1, 1, CAST(0x00009F6F0100612A AS DateTime), CAST(0x00009F6F0100612A AS DateTime), 4, 10, CAST(0x00009B5B0100612A AS DateTime), NULL)
INSERT [dbo].[uDebate_Forums] ([ID], [ModuleID], [Language], [Description], [Summary], [Active], [Status], [CREATED], [LAST_UPDATED], [CREATED_BY], [UPDATED_BY], [Opened_Date], [Closed_Date]) VALUES (5, 459, N'el-GR', N'uDebate', N'Discussion about immigration', 1, 1, CAST(0x00009F6F0100612A AS DateTime), CAST(0x00009F6F0100612A AS DateTime), 4, 4, CAST(0x00009B5B0100612A AS DateTime), NULL)
INSERT [dbo].[uDebate_Forums] ([ID], [ModuleID], [Language], [Description], [Summary], [Active], [Status], [CREATED], [LAST_UPDATED], [CREATED_BY], [UPDATED_BY], [Opened_Date], [Closed_Date]) VALUES (6, 459, N'it-IT', N'uDebate', N'Discussion about immigration', 1, 1, CAST(0x00009F6F0100612A AS DateTime), CAST(0x00009F6F0100612A AS DateTime), 4, 4, CAST(0x00009B5B0100612A AS DateTime), NULL)
INSERT [dbo].[uDebate_Forums] ([ID], [ModuleID], [Language], [Description], [Summary], [Active], [Status], [CREATED], [LAST_UPDATED], [CREATED_BY], [UPDATED_BY], [Opened_Date], [Closed_Date]) VALUES (7, 459, N'hu-HU', N'uDebate', N'Discussion about immigration', 1, 1, CAST(0x00009F6F0100612A AS DateTime), CAST(0x00009F6F0100612A AS DateTime), 4, 4, CAST(0x00009B5B0100612A AS DateTime), NULL)
INSERT [dbo].[uDebate_Forums] ([ID], [ModuleID], [Language], [Description], [Summary], [Active], [Status], [CREATED], [LAST_UPDATED], [CREATED_BY], [UPDATED_BY], [Opened_Date], [Closed_Date]) VALUES (8, 459, N'es-ES', N'uDebate', N'Discussion about immigration', 1, 1, CAST(0x00009F6F0100612A AS DateTime), CAST(0x00009F6F0100612A AS DateTime), 4, 4, CAST(0x00009B5B0100612A AS DateTime), NULL)
INSERT [dbo].[uDebate_Forums] ([ID], [ModuleID], [Language], [Description], [Summary], [Active], [Status], [CREATED], [LAST_UPDATED], [CREATED_BY], [UPDATED_BY], [Opened_Date], [Closed_Date]) VALUES (9, 460, N'en-GB', N'uDebate', N'Discussion about immigration', 1, 1, CAST(0x00009FC00106E129 AS DateTime), CAST(0x00009FC00106E129 AS DateTime), 10, 10, NULL, NULL)
SET IDENTITY_INSERT [dbo].[uDebate_Forums] OFF
ALTER TABLE [dbo].[uDebate_Forums] ADD  CONSTRAINT [DF_uDebate_Forums_CREATED]  DEFAULT (getdate()) FOR [CREATED]
GO
ALTER TABLE [dbo].[uDebate_Forums] ADD  CONSTRAINT [DF_uDebate_Forums_LAST_UPDATED]  DEFAULT (getdate()) FOR [LAST_UPDATED]
GO
ALTER TABLE [dbo].[uDebate_Forums]  WITH NOCHECK ADD  CONSTRAINT [FK_uDebate_Forums_UPDATED_BY] FOREIGN KEY([ID])
REFERENCES [dbo].[uDebate_Forums] ([ID])
GO
ALTER TABLE [dbo].[uDebate_Forums] CHECK CONSTRAINT [FK_uDebate_Forums_UPDATED_BY]
GO


CREATE TABLE [dbo].[uDebate_Attachments](
	[FileID] [int] NOT NULL,
	[PostID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[LocalFileName] [nvarchar](255) NOT NULL,
	[Inline] [bit] NOT NULL,
	[AttachmentID] [int] IDENTITY(1,1) NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[uDebate_Forum_Notifications]    Script Date: 21/05/2013 12:23:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[uDebate_Forum_Notifications](
	[userID] [int] NOT NULL,
	[threadID] [int] NOT NULL,
	[userEmail] [nvarchar](50) NULL,
	[insertedOn] [datetime] NULL,
 CONSTRAINT [PK_uDebate_Forum_Notifications] PRIMARY KEY CLUSTERED 
(
	[userID] ASC,
	[threadID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[uDebate_Forum_Posts]    Script Date: 21/05/2013 12:23:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[uDebate_Forum_Posts](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ModuleID] [int] NULL,
	[ThreadID] [int] NULL,
	[ParentID] [int] NULL,
	[UserID] [int] NOT NULL,
	[PostLevel] [int] NOT NULL,
	[SortOrder] [int] NOT NULL,
	[Subject] [nvarchar](512) NULL,
	[Message] [ntext] NULL,
	[PostDate] [datetime] NOT NULL,
	[IsPublished] [int] NOT NULL,
	[PostType] [int] NOT NULL,
	[Active] [int] NOT NULL,
	[Published_Date] [datetime] NULL,
	[Complaint_Count] [int] NULL,
 CONSTRAINT [PK_2uDebate_Forum_Posts] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[uDebate_Forum_Threads]    Script Date: 21/05/2013 12:23:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[uDebate_Forum_Threads](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ModuleID] [int] NULL,
	[TopicID] [int] NOT NULL,
	[Language] [nvarchar](50) NULL,
	[EuRelated] [bit] NULL,
	[Description] [nvarchar](1024) NULL,
	[Summary] [nvarchar](4000) NULL,
	[Text] [nvarchar](max) NOT NULL,
	[UserID] [int] NOT NULL,
	[Complain_email] [nvarchar](512) NULL,
	[Contact_email] [nvarchar](512) NULL,
	[CREATED] [datetime] NOT NULL,
	[LAST_UPDATED] [datetime] NOT NULL,
	[CREATED_BY] [int] NOT NULL,
	[UPDATED_BY] [int] NOT NULL,
	[Closed_Date] [datetime] NULL,
	[Opened_Date] [datetime] NULL,
	[STATUS] [int] NOT NULL,
	[Active] [int] NOT NULL,
	[View_Count] [int] NULL,
	[Delete_Count] [int] NULL,
	[GroupID] [int] NOT NULL,
	[StatusID] [int] NOT NULL,
	[FacilitatorNotes] [nvarchar](max) NULL,
	[Position] [int] NULL,
 CONSTRAINT [PK_2uDebate_Forum_Threads] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[uDebate_Forum_Topics]    Script Date: 21/05/2013 12:23:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[uDebate_Forum_Topics](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ModuleID] [int] NULL,
	[ForumID] [int] NOT NULL,
	[Language] [nvarchar](50) NULL,
	[Description] [nvarchar](1024) NOT NULL,
	[Summary] [nvarchar](4000) NULL,
	[Active] [int] NOT NULL,
	[Text] [ntext] NULL,
	[Position] [int] NULL,
	[Status] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[CREATED] [datetime] NOT NULL,
	[LAST_UPDATED] [datetime] NOT NULL,
	[CREATED_BY] [int] NOT NULL,
	[UPDATED_BY] [int] NOT NULL,
	[Opened_Date] [datetime] NULL,
	[Closed_Date] [datetime] NULL,
 CONSTRAINT [PK_2uDebate_Forum_Topics] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[uDebate_Forum_Threads] ADD  CONSTRAINT [DF_uDebate_Forum_Threads_CREATED]  DEFAULT (getdate()) FOR [CREATED]
GO
ALTER TABLE [dbo].[uDebate_Forum_Threads] ADD  CONSTRAINT [DF_uDebate_Forum_Threads_LAST_UPDATED]  DEFAULT (getdate()) FOR [LAST_UPDATED]
GO
ALTER TABLE [dbo].[uDebate_Forum_Threads] ADD  CONSTRAINT [DF_uDebate_Forum_Threads_View_Count]  DEFAULT ((0)) FOR [View_Count]
GO
ALTER TABLE [dbo].[uDebate_Forum_Threads] ADD  CONSTRAINT [DF_uDebate_Forum_Threads_Delete_Count]  DEFAULT ((0)) FOR [Delete_Count]
GO
ALTER TABLE [dbo].[uDebate_Forum_Threads] ADD  CONSTRAINT [DF_uDebate_Forum_Threads_GroupID]  DEFAULT ((1)) FOR [GroupID]
GO
ALTER TABLE [dbo].[uDebate_Forum_Threads] ADD  CONSTRAINT [DF_2uDebate_Forum_Threads_StatusID]  DEFAULT ((1)) FOR [StatusID]
GO
ALTER TABLE [dbo].[uDebate_Forum_Topics] ADD  CONSTRAINT [DF_uDebate_Forum_Topics_CREATED]  DEFAULT (getdate()) FOR [CREATED]
GO
ALTER TABLE [dbo].[uDebate_Forum_Topics] ADD  CONSTRAINT [DF_uDebate_Forum_Topics_LAST_UPDATED]  DEFAULT (getdate()) FOR [LAST_UPDATED]
GO
ALTER TABLE [dbo].[uDebate_Attachments]  WITH NOCHECK ADD  CONSTRAINT [FK_uDebate_Attachments_Files] FOREIGN KEY([FileID])
REFERENCES [dbo].[Files] ([FileId])
ON DELETE CASCADE
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[uDebate_Attachments] CHECK CONSTRAINT [FK_uDebate_Attachments_Files]
GO
ALTER TABLE [dbo].[uDebate_Forum_Posts]  WITH NOCHECK ADD  CONSTRAINT [FK_uDebate_Forum_Posts_ThreadID] FOREIGN KEY([ThreadID])
REFERENCES [dbo].[uDebate_Forum_Threads] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[uDebate_Forum_Posts] CHECK CONSTRAINT [FK_uDebate_Forum_Posts_ThreadID]
GO
ALTER TABLE [dbo].[uDebate_Forum_Threads]  WITH NOCHECK ADD  CONSTRAINT [FK_uDebate_Forum_Threads_TOPIC_ID] FOREIGN KEY([TopicID])
REFERENCES [dbo].[uDebate_Forum_Topics] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[uDebate_Forum_Threads] CHECK CONSTRAINT [FK_uDebate_Forum_Threads_TOPIC_ID]
GO
ALTER TABLE [dbo].[uDebate_Forum_Topics]  WITH NOCHECK ADD  CONSTRAINT [FK_uDebate_Forum_Topics_uDebate_Forums] FOREIGN KEY([ForumID])
REFERENCES [dbo].[uDebate_Forums] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[uDebate_Forum_Topics] CHECK CONSTRAINT [FK_uDebate_Forum_Topics_uDebate_Forums]
GO
