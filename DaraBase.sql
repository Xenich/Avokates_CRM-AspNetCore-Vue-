USE [LawyerCRM]
GO
/****** Object:  Table [dbo].[Auth]    Script Date: 15.03.2020 21:04:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Auth](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Login] [nvarchar](50) NOT NULL,
	[Password] [char](128) NOT NULL,
	[Salt] [varchar](16) NOT NULL,
	[EmployeeId] [int] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Case]    Script Date: 15.03.2020 21:04:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Case](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](500) NOT NULL,
	[Info] [nvarchar](max) NULL,
	[Date] [date] NULL,
	[UpdateDate] [datetime] NULL,
	[IsClosed] [bit] NOT NULL,
 CONSTRAINT [PK_Case] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Classificator]    Script Date: 15.03.2020 21:04:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Classificator](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProfessionName] [nvarchar](250) NOT NULL,
	[ProfessionCode] [nvarchar](20) NULL,
 CONSTRAINT [PK_Classificator] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Company]    Script Date: 15.03.2020 21:04:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Company](
	[UID] [char](32) NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyName] [nvarchar](250) NULL,
	[CompanyDirector] [nvarchar](250) NULL,
	[CompanyEmail] [nvarchar](50) NULL,
	[CompanyPhone] [nvarchar](20) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Employee]    Script Date: 15.03.2020 21:04:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employee](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClassificatorId] [int] NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Birthday] [date] NULL,
	[Phone] [nvarchar](50) NULL,
	[Email] [nvarchar](50) NULL,
	[Employee_CompanyUID] [char](32) NOT NULL,
	[Token] [nvarchar](250) NULL,
	[RoleId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmployeeCase]    Script Date: 15.03.2020 21:04:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeCase](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[CaseId] [int] NOT NULL,
	[IsOwner] [bit] NULL,
 CONSTRAINT [PK_EmployeeCase] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Figurant]    Script Date: 15.03.2020 21:04:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Figurant](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Phone] [nvarchar](20) NULL,
	[Email] [nvarchar](50) NULL,
	[FigurantRoleId] [int] NULL,
	[FigurantRoleName] [nvarchar](255) NULL,
	[CaseId] [int] NOT NULL,
 CONSTRAINT [PK_Figurant] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FigurantRole]    Script Date: 15.03.2020 21:04:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FigurantRole](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](255) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_FigurantRole] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Invite]    Script Date: 15.03.2020 21:04:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Invite](
	[Token] [varchar](255) NOT NULL,
	[Invite_CompanyUID] [char](32) NOT NULL,
	[ExpiresIn] [datetime] NOT NULL,
 CONSTRAINT [PK_Invite_1] PRIMARY KEY CLUSTERED 
(
	[Token] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MediaFile]    Script Date: 15.03.2020 21:04:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MediaFile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FilePath] [nvarchar](255) NULL,
	[NoteId] [int] NOT NULL,
 CONSTRAINT [PK_MediaFile] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Note]    Script Date: 15.03.2020 21:04:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Note](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Text] [nvarchar](max) NULL,
	[Date] [datetime] NULL,
	[Updatedate] [datetime] NULL,
	[CaseId] [int] NOT NULL,
	[EmployeeId] [int] NOT NULL,
 CONSTRAINT [PK_Note] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 15.03.2020 21:04:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Settings]    Script Date: 15.03.2020 21:04:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Settings](
	[Parameter] [nvarchar](50) NOT NULL,
	[Value] [nvarchar](250) NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_Settings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_EmployeeCase]    Script Date: 15.03.2020 21:04:45 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_EmployeeCase] ON [dbo].[EmployeeCase]
(
	[EmployeeId] ASC,
	[CaseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Case] ADD  CONSTRAINT [DF_IsClosed]  DEFAULT ((0)) FOR [IsClosed]
GO
ALTER TABLE [dbo].[Company] ADD  CONSTRAINT [DF_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Employee] ADD  CONSTRAINT [DF_RoleId]  DEFAULT ((3)) FOR [RoleId]
GO
ALTER TABLE [dbo].[Employee] ADD  CONSTRAINT [DF_IsActiveEmployee]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[FigurantRole] ADD  CONSTRAINT [DF_FigurantRoleIsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Auth]  WITH CHECK ADD  CONSTRAINT [FK_Auth_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employee] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Auth] CHECK CONSTRAINT [FK_Auth_Employee]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_Classificator] FOREIGN KEY([ClassificatorId])
REFERENCES [dbo].[Classificator] ([Id])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_Classificator]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_Company] FOREIGN KEY([Employee_CompanyUID])
REFERENCES [dbo].[Company] ([UID])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_Company]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_Role]
GO
ALTER TABLE [dbo].[EmployeeCase]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeCase_Case] FOREIGN KEY([CaseId])
REFERENCES [dbo].[Case] ([Id])
GO
ALTER TABLE [dbo].[EmployeeCase] CHECK CONSTRAINT [FK_EmployeeCase_Case]
GO
ALTER TABLE [dbo].[EmployeeCase]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeCase_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employee] ([Id])
GO
ALTER TABLE [dbo].[EmployeeCase] CHECK CONSTRAINT [FK_EmployeeCase_Employee]
GO
ALTER TABLE [dbo].[Figurant]  WITH CHECK ADD  CONSTRAINT [FK_Figurant_Case] FOREIGN KEY([CaseId])
REFERENCES [dbo].[Case] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Figurant] CHECK CONSTRAINT [FK_Figurant_Case]
GO
ALTER TABLE [dbo].[Figurant]  WITH CHECK ADD  CONSTRAINT [FK_Figurant_FigurantRole] FOREIGN KEY([FigurantRoleId])
REFERENCES [dbo].[FigurantRole] ([Id])
GO
ALTER TABLE [dbo].[Figurant] CHECK CONSTRAINT [FK_Figurant_FigurantRole]
GO
ALTER TABLE [dbo].[Invite]  WITH CHECK ADD  CONSTRAINT [FK_Invite_Company] FOREIGN KEY([Invite_CompanyUID])
REFERENCES [dbo].[Company] ([UID])
GO
ALTER TABLE [dbo].[Invite] CHECK CONSTRAINT [FK_Invite_Company]
GO
ALTER TABLE [dbo].[MediaFile]  WITH CHECK ADD  CONSTRAINT [FK_MediaFile_Note] FOREIGN KEY([NoteId])
REFERENCES [dbo].[Note] ([Id])
GO
ALTER TABLE [dbo].[MediaFile] CHECK CONSTRAINT [FK_MediaFile_Note]
GO
ALTER TABLE [dbo].[Note]  WITH CHECK ADD  CONSTRAINT [FK_Note_Case] FOREIGN KEY([CaseId])
REFERENCES [dbo].[Case] ([Id])
GO
ALTER TABLE [dbo].[Note] CHECK CONSTRAINT [FK_Note_Case]
GO
ALTER TABLE [dbo].[Note]  WITH CHECK ADD  CONSTRAINT [FK_Note_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employee] ([Id])
GO
ALTER TABLE [dbo].[Note] CHECK CONSTRAINT [FK_Note_Employee]
GO
/****** Object:  StoredProcedure [dbo].[prAuthorization]    Script Date: 15.03.2020 21:04:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Leachman Dmitry>
-- Create date: <24-02-2020>
-- Description:	<Авторизация - Получение информации о юзере по логину и паролю>
-- =============================================
CREATE PROCEDURE [dbo].[prAuthorization]
	@Login nvarchar(50),
	@Password nvarchar(50)
	--	@Outret nvarchar(50) OUTPUT
AS
BEGIN
DECLARE @passHash CHAR(128);
DECLARE @salt VARCHAR(16);
SET @passHash = (SELECT a.[Password] FROM Auth a WHERE a.[Login] = @Login);
SET @salt = (SELECT a.Salt FROM Auth a WHERE a.[Login] = @Login);

IF (CONVERT(char(128), HASHBYTES('SHA2_512', CONCAT(@Password,@salt)),2) = @passHash) 
	BEGIN

		SELECT r.RoleName, e.[Name], e.Token, e.IsActive, @Login as Login, e.Employee_CompanyUID as CompanyUID, e.Id as EmployeeId
		FROM Auth a
		left JOIN Employee e on e.id = a.EmployeeId
		left join Role r on r.Id = e.RoleId
		where a.Login = @Login
	END
ELSE
	RETURN 6	-- пароль или логин неправильный
END
GO
/****** Object:  StoredProcedure [dbo].[prCreateNewCase]    Script Date: 15.03.2020 21:04:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Leachman,Dmitry>
-- Create date: <15-03-2020>
-- Description:	<Создание нового дела>
-- =============================================
CREATE PROCEDURE [dbo].[prCreateNewCase] 
	@title nvarchar(500),
	@info nvarchar(max),
	@figurants nvarchar(max),		-- JSON
	@employeeId int,
	@companyUID char(32)
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION

	IF @companyUID != ( SELECT e.Employee_CompanyUID FROM Employee e WHERE e.Id = @employeeId)
	BEGIN
		rollback;
		RETURN 6
	END

	DECLARE @caseId int;
	DECLARE @figurantsTable TABLE (Name NVARCHAR(255), Phone NVARCHAR(20), Email NVARCHAR(50), FigurantRoleId int, FigurantRoleName NVARCHAR(255));

	INSERT INTO [Case] ([Title], [Info], [Date], [UpdateDate])
	VALUES (@title, @info, GETDATE(), GETDATE())
	SET @CaseId = @@IDENTITY;

	INSERT INTO EmployeeCase ([EmployeeId], [CaseId], [IsOwner])
	VALUES (@employeeId, @caseId, 1)

	INSERT INTO @figurantsTable
	SELECT [Name], Phone, Email, FigurantRoleId, FigurantRoleName
	FROM OPENJSON(@figurants, '$')
	WITH
	(
		[Name] varchar(50) N'$.Name',
		Phone varchar(50) N'$.Phone',
		Email varchar(50) N'$.Email',
		FigurantRoleId varchar(50) N'$.FigurantRoleId',
		FigurantRoleName varchar(50) N'$.FigurantRoleName'
	)

	INSERT INTO Figurant ([Name], [Phone], [Email], [FigurantRoleId], [FigurantRoleName], [CaseId] )
	SELECT 
		 ft.[Name]
		, ft.Phone 
		, ft.Email 
		, CASE 
			WHEN ft.FigurantRoleId IS NULL THEN 1		-- Другое 
			WHEN NOT EXISTS ( SELECT 1 FROM [dbo].[FigurantRole] WHERE id = ft.FigurantRoleId) THEN 1		-- Другое
			ELSE ft.FigurantRoleId
		  END

		, FigurantRoleName
		, @caseId
	FROM @figurantsTable ft


	COMMIT TRANSACTION
END
GO
/****** Object:  StoredProcedure [dbo].[prRegistration]    Script Date: 15.03.2020 21:04:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Leachman Dmitry>
-- Create date: <24-02-2020>
-- Description:	<Регистрация нового пользователя>
-- =============================================
CREATE PROCEDURE [dbo].[prRegistration]
	@Login nvarchar(50),
	@Password nvarchar(50),
	@InvitingToken char(250),
	@Name nvarchar(250),
    @Birthday Date,
    @Phone nvarchar(50),
    @Email nvarchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION
	DECLARE @CompanyUID char(32);
	IF (SELECT i.ExpiresIn FROM Invite i WHERE i.Token = @InvitingToken)>GETDATE()
		SET @CompanyUID = (SELECT i.Invite_CompanyUID FROM Invite i WHERE i.Token = @InvitingToken)
	ELSE
	BEGIN
		rollback;
		RETURN 7			-- пригласительный токен просрочен
	END
	IF EXISTS (SELECT 1 FROM Auth a WHERE a.[Login] = @Login)
	BEGIN
		rollback;
		RETURN 6
	END

	DECLARE @length int = cast(rand()*5 +10 as int);
	DECLARE @salt NVARCHAR(15);
	DECLARE @passwordHash CHAR(128);
	DECLARE @EmploeeId int;
	
	SET @salt = SUBSTRING( CONVERT ( NVARCHAR(128), NEWID()) , 1,@length)

	SET @passwordHash = CONVERT(char(128), HASHBYTES('SHA2_512', CONCAT(@Password,@salt)),2)
	--SELECt @passwordHash

	INSERT INTO [dbo].[Employee]
	([Name], [Birthday], [Phone], [Email], [Employee_CompanyUID] )
	VALUES
	(
		  @Name
		, @Birthday
		, @Phone
		, @Email
		, (SELECT i.Invite_CompanyUID FROM Invite i WHERE i.Token = @InvitingToken)
	)
	SET @EmploeeId =@@IDENTITY;

	INSERT INTO [dbo].[Auth] 
	([Login], [Password], [Salt], [EmployeeId])
	VALUES
	(@Login, @passwordHash, @salt, @EmploeeId)

	DELETE [dbo].[Invite] WHERE Token = @InvitingToken

	COMMIT TRANSACTION

END







GO
