Declare @SQL nvarchar(max);
SET @SQL = ' USE [LawyerCRM]

CREATE TABLE [dbo].[Auth](
	[Login] [nvarchar](50) NOT NULL,
	[PassHash] [char](128) NOT NULL,
	[Salt] [varchar](16) NOT NULL,
	[EmployeeUID] [uniqueidentifier] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_Auth] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
/****** Object:  Table [dbo].[Case]    Script Date: 14.08.2021 21:29:18 ******/
SET ANSI_NULLS ON
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
SET QUOTED_IDENTIFIER ON
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
CREATE TABLE [dbo].[Case](
	[UID] [uniqueidentifier] NOT NULL,
	[CompanyUID] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](500) NOT NULL,
	[Info] [varbinary](max) NULL,
	[Date] [date] NULL,
	[UpdateDate] [datetime] NULL,
	[IsClosed] [bit] NOT NULL,
	[IdPerCompany] [int] NOT NULL,
 CONSTRAINT [PK_Case] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
/****** Object:  Table [dbo].[Classificator]    Script Date: 14.08.2021 21:29:18 ******/
SET ANSI_NULLS ON
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
SET QUOTED_IDENTIFIER ON
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
CREATE TABLE [dbo].[Classificator](
	[UID] [uniqueidentifier] NOT NULL,
	[ProfessionName] [nvarchar](250) NOT NULL,
	[ProfessionCode] [nvarchar](20) NULL,
 CONSTRAINT [PK_Classificator] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
/****** Object:  Table [dbo].[Company]    Script Date: 14.08.2021 21:29:18 ******/
SET ANSI_NULLS ON
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
SET QUOTED_IDENTIFIER ON
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
CREATE TABLE [dbo].[Company](
	[UID] [uniqueidentifier] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyName] [nvarchar](250) NULL,
	[CompanyDirector] [nvarchar](250) NULL,
	[CompanyEmail] [nvarchar](50) NULL,
	[CompanyPhone] [nvarchar](20) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Company_1] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
/****** Object:  Table [dbo].[Employee]    Script Date: 14.08.2021 21:29:18 ******/
SET ANSI_NULLS ON
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
SET QUOTED_IDENTIFIER ON
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
CREATE TABLE [dbo].[Employee](
	[UID] [uniqueidentifier] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyUID] [uniqueidentifier] NOT NULL,
	[ClassificatorUID] [uniqueidentifier] NULL,
	[RoleUID] [uniqueidentifier] NULL,
	[Name] [nvarchar](250) NULL,
	[Surname] [nvarchar](250) NULL,
	[SecondName] [nvarchar](250) NULL,
	[Birthday] [date] NULL,
	[Phone] [nvarchar](50) NULL,
	[Email] [nvarchar](50) NULL,
	[Token] [nvarchar](250) NULL,
	[IsActive] [bit] NOT NULL,
	[PublicKey] [varbinary](8000) NULL,
 CONSTRAINT [PK_Employee_1] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
/****** Object:  Table [dbo].[EmployeeCase]    Script Date: 14.08.2021 21:29:18 ******/
SET ANSI_NULLS ON
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
SET QUOTED_IDENTIFIER ON
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
CREATE TABLE [dbo].[EmployeeCase](
	[IsOwner] [bit] NOT NULL,
	[EmployeeUID] [uniqueidentifier] NOT NULL,
	[CaseUID] [uniqueidentifier] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EncriptedAesKey] [varbinary](8000) NULL,
 CONSTRAINT [PK_EmployeeCase] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
/****** Object:  Table [dbo].[Figurant]    Script Date: 14.08.2021 21:29:18 ******/
SET ANSI_NULLS ON
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
SET QUOTED_IDENTIFIER ON
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
CREATE TABLE [dbo].[Figurant](
	[UID] [uniqueidentifier] NOT NULL,
	[CaseUID] [uniqueidentifier] NOT NULL,
	[FigurantRoleUID] [uniqueidentifier] NULL,
	[Name] [nvarchar](255) NULL,
	[Phone] [nvarchar](20) NULL,
	[Email] [nvarchar](50) NULL,
	[FigurantRoleName] [nvarchar](255) NULL,
	[SecondName] [nvarchar](255) NULL,
	[Surname] [nvarchar](255) NULL,
	[Description] [varbinary](max) NULL,
 CONSTRAINT [PK_Figurant_1] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
/****** Object:  Table [dbo].[FigurantRole]    Script Date: 14.08.2021 21:29:18 ******/
SET ANSI_NULLS ON
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
SET QUOTED_IDENTIFIER ON
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
CREATE TABLE [dbo].[FigurantRole](
	[UID] [uniqueidentifier] NOT NULL,
	[RoleName] [nvarchar](255) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CompanyUID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_FigurantRole] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
/****** Object:  Table [dbo].[Invite]    Script Date: 14.08.2021 21:29:18 ******/
SET ANSI_NULLS ON
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
SET QUOTED_IDENTIFIER ON
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
CREATE TABLE [dbo].[Invite](
	[CompanyUID] [uniqueidentifier] NOT NULL,
	[Token] [varchar](255) NOT NULL,
	[ExpiresIn] [datetime] NOT NULL,
 CONSTRAINT [PK_Invite_1] PRIMARY KEY CLUSTERED 
(
	[Token] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
/****** Object:  Table [dbo].[MediaFile]    Script Date: 14.08.2021 21:29:18 ******/
SET ANSI_NULLS ON
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
SET QUOTED_IDENTIFIER ON
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
CREATE TABLE [dbo].[MediaFile](
	[UID] [uniqueidentifier] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FilePath] [nvarchar](255) NULL,
	[NoteUID] [uniqueidentifier] NULL,
	[NameCripted] [varbinary](max) NULL,
 CONSTRAINT [PK_MediaFile] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
/****** Object:  Table [dbo].[Note]    Script Date: 14.08.2021 21:29:18 ******/
SET ANSI_NULLS ON
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
SET QUOTED_IDENTIFIER ON
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
CREATE TABLE [dbo].[Note](
	[UID] [uniqueidentifier] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CaseUID] [uniqueidentifier] NOT NULL,
	[EmployeeUID] [uniqueidentifier] NOT NULL,
	[Text] [varbinary](max) NULL,
	[Date] [datetime] NULL,
	[Updatedate] [datetime] NULL,
	[Title] [varbinary](max) NULL,
 CONSTRAINT [PK_Note] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
/****** Object:  Table [dbo].[Role]    Script Date: 14.08.2021 21:29:18 ******/
SET ANSI_NULLS ON
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
SET QUOTED_IDENTIFIER ON
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
CREATE TABLE [dbo].[Role](
	[UID] [uniqueidentifier] NOT NULL,
	[RoleName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
/****** Object:  Table [dbo].[Settings]    Script Date: 14.08.2021 21:29:18 ******/
SET ANSI_NULLS ON
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
SET QUOTED_IDENTIFIER ON
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
CREATE TABLE [dbo].[Settings](
	[Parameter] [nvarchar](50) NOT NULL,
	[Value] [nvarchar](250) NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_Settings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
'; EXEC (@SQL); 
 SET @SQL = ' USE [LawyerCRM]
/****** Object:  Index [IX_Case_Company]    Script Date: 14.08.2021 21:29:18 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Case_Company] ON [dbo].[Case]
(
	[IdPerCompany] ASC,
	[CompanyUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
/****** Object:  Index [IX_EmployeeCase]    Script Date: 14.08.2021 21:29:18 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_EmployeeCase] ON [dbo].[EmployeeCase]
(
	[CaseUID] ASC,
	[EmployeeUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Case] ADD  CONSTRAINT [DF__Case__UID__640DD89F]  DEFAULT (newsequentialid()) FOR [UID]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Case] ADD  CONSTRAINT [DF_IsClosed]  DEFAULT ((0)) FOR [IsClosed]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Classificator] ADD  CONSTRAINT [DF__Classificat__UID__57A801BA]  DEFAULT (newsequentialid()) FOR [UID]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Company] ADD  CONSTRAINT [DF_Company_UID]  DEFAULT (newsequentialid()) FOR [UID]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Company] ADD  CONSTRAINT [DF_IsActive]  DEFAULT ((1)) FOR [IsActive]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Company] ADD  CONSTRAINT [DF_Company_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Employee] ADD  CONSTRAINT [DF_Employee_UID]  DEFAULT (newsequentialid()) FOR [UID]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Employee] ADD  CONSTRAINT [DF_IsActiveEmployee]  DEFAULT ((1)) FOR [IsActive]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[EmployeeCase] ADD  CONSTRAINT [DF_EmployeeCase_IsOwner]  DEFAULT ((0)) FOR [IsOwner]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Figurant] ADD  CONSTRAINT [DF_Figurant_UID]  DEFAULT (newsequentialid()) FOR [UID]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[FigurantRole] ADD  CONSTRAINT [DF__FigurantRol__UID__762C88DA]  DEFAULT (newsequentialid()) FOR [UID]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[FigurantRole] ADD  CONSTRAINT [DF_FigurantRoleIsActive]  DEFAULT ((1)) FOR [IsActive]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[MediaFile] ADD  CONSTRAINT [DF__MediaFile__UID__7AF13DF7]  DEFAULT (newsequentialid()) FOR [UID]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Note] ADD  CONSTRAINT [DF__Note__GUID__6BAEFA67]  DEFAULT (newsequentialid()) FOR [UID]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Role] ADD  CONSTRAINT [DF__Role__UID__589C25F3]  DEFAULT (newsequentialid()) FOR [UID]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Auth]  WITH CHECK ADD  CONSTRAINT [FK_Auth_Employee] FOREIGN KEY([EmployeeUID])
REFERENCES [dbo].[Employee] ([UID])
ON UPDATE CASCADE
ON DELETE CASCADE
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Auth] CHECK CONSTRAINT [FK_Auth_Employee]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Case]  WITH CHECK ADD  CONSTRAINT [FK_Case_Company] FOREIGN KEY([CompanyUID])
REFERENCES [dbo].[Company] ([UID])
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Case] CHECK CONSTRAINT [FK_Case_Company]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_Classificator] FOREIGN KEY([ClassificatorUID])
REFERENCES [dbo].[Classificator] ([UID])
ON UPDATE CASCADE
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_Classificator]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_Company] FOREIGN KEY([CompanyUID])
REFERENCES [dbo].[Company] ([UID])
ON UPDATE CASCADE
ON DELETE CASCADE
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_Company]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_Role] FOREIGN KEY([RoleUID])
REFERENCES [dbo].[Role] ([UID])
ON UPDATE CASCADE
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_Role]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[EmployeeCase]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeCase_Case] FOREIGN KEY([CaseUID])
REFERENCES [dbo].[Case] ([UID])
ON UPDATE CASCADE
ON DELETE CASCADE
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[EmployeeCase] CHECK CONSTRAINT [FK_EmployeeCase_Case]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[EmployeeCase]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeCase_Employee] FOREIGN KEY([EmployeeUID])
REFERENCES [dbo].[Employee] ([UID])
ON UPDATE CASCADE
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[EmployeeCase] CHECK CONSTRAINT [FK_EmployeeCase_Employee]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Figurant]  WITH CHECK ADD  CONSTRAINT [FK_Figurant_Case] FOREIGN KEY([CaseUID])
REFERENCES [dbo].[Case] ([UID])
ON UPDATE CASCADE
ON DELETE CASCADE
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Figurant] CHECK CONSTRAINT [FK_Figurant_Case]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Figurant]  WITH CHECK ADD  CONSTRAINT [FK_Figurant_FigurantRole] FOREIGN KEY([FigurantRoleUID])
REFERENCES [dbo].[FigurantRole] ([UID])
ON UPDATE CASCADE
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Figurant] CHECK CONSTRAINT [FK_Figurant_FigurantRole]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[FigurantRole]  WITH CHECK ADD  CONSTRAINT [FK_FigurantRole_Company] FOREIGN KEY([CompanyUID])
REFERENCES [dbo].[Company] ([UID])
ON UPDATE CASCADE
ON DELETE CASCADE
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[FigurantRole] CHECK CONSTRAINT [FK_FigurantRole_Company]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Invite]  WITH CHECK ADD  CONSTRAINT [FK_Invite_Company] FOREIGN KEY([CompanyUID])
REFERENCES [dbo].[Company] ([UID])
ON UPDATE CASCADE
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Invite] CHECK CONSTRAINT [FK_Invite_Company]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[MediaFile]  WITH CHECK ADD  CONSTRAINT [FK_MediaFile_Note] FOREIGN KEY([NoteUID])
REFERENCES [dbo].[Note] ([UID])
ON UPDATE CASCADE
ON DELETE CASCADE
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[MediaFile] CHECK CONSTRAINT [FK_MediaFile_Note]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Note]  WITH CHECK ADD  CONSTRAINT [FK_Note_Case] FOREIGN KEY([CaseUID])
REFERENCES [dbo].[Case] ([UID])
ON UPDATE CASCADE
ON DELETE CASCADE
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Note] CHECK CONSTRAINT [FK_Note_Case]
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Note]  WITH CHECK ADD  CONSTRAINT [FK_Note_Employee] FOREIGN KEY([EmployeeUID])
REFERENCES [dbo].[Employee] ([UID])
ON UPDATE CASCADE
'; EXEC (@SQL); SET @SQL = ' USE [LawyerCRM]
ALTER TABLE [dbo].[Note] CHECK CONSTRAINT [FK_Note_Employee]
'; EXEC (@SQL);  SET @SQL = ' USE [LawyerCRM]
SET QUOTED_IDENTIFIER ON
'; EXEC (@SQL); 