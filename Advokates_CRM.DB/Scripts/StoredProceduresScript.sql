Declare @SQL nvarchar(max);
SET @SQL = '
Create PROCEDURE [dbo].[CheckConnection]
	@value int,
	@return int OUTPUT
	
	--	@Outret nvarchar(50) OUTPUT
AS
BEGIN
	SELECT @return = @value+1;
END

'
EXEC (@SQL);


SET @SQL = '
CREATE PROCEDURE [dbo].[prAuthorization]
	@Login nvarchar(50),
	@Password nvarchar(50)
	--	@Outret nvarchar(50) OUTPUT
AS
BEGIN
DECLARE @passHash CHAR(128);
DECLARE @salt VARCHAR(16);
SELECT @passHash = a.PassHash, @salt = a.Salt 
FROM Auth a WHERE a.[Login] = @Login;


IF (CONVERT(char(128), HASHBYTES(''SHA2_512'', CONCAT(@Password,@salt)),2) = @passHash) 
	BEGIN

		SELECT r.RoleName, e.[Name], e.Token, e.IsActive, @Login as Login, c.UID as CompanyUID, e.Id as EmployeeId, e.UID as EmployeeUid
		FROM Auth a
		left JOIN Employee e on e.UID = a.EmployeeUID
		left join Role r on r.UID = e.RoleUID
		left join Company c on c.UID = e.CompanyUID
		where a.Login = @Login
	END
ELSE
	RETURN 6	-- пароль или логин неправильный
END
'
EXEC (@SQL);



SET @SQL = '
CREATE PROCEDURE [dbo].[prRegistration]
	@Login nvarchar(50),
	@Password nvarchar(50),
	@InvitingToken char(255),
	@Name nvarchar(250),
    @Birthday Date,
    @Phone nvarchar(50),
    @Email nvarchar(50),
	@PublicKey varbinary(8000),
	@UserGuid uniqueidentifier OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @CompanyUID uniqueidentifier;
	DECLARE @EmploeeUID uniqueidentifier;


	BEGIN TRY
	BEGIN TRANSACTION

	IF (SELECT i.ExpiresIn FROM Invite i WHERE i.Token = @InvitingToken)>GETDATE()
		SELECT @CompanyUID = i.CompanyUID 
		FROM Invite i 
		WHERE i.Token = @InvitingToken
	ELSE
	BEGIN
		rollback;
		DELETE [dbo].[Invite] WHERE Token = @InvitingToken
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
	
	SET @salt = SUBSTRING( CONVERT ( NVARCHAR(128), NEWID()) , 1, @length)

	SET @passwordHash = CONVERT(char(128), HASHBYTES(''SHA2_512'', CONCAT(@Password,@salt)),2)
	SELECt @passwordHash

			-- создание нового employee
	DECLARE @EmploeeUIDTable TABLE (EmploeeUID uniqueidentifier)
	INSERT INTO [dbo].[Employee]
	([Name], [Birthday], [Phone], [Email], [CompanyUID], [PublicKey], [RoleUID] )
	OUTPUT inserted.UID INTO @EmploeeUIDTable
	VALUES
	(
		  @Name
		, @Birthday
		, @Phone
		, @Email
		, @CompanyUID
		, @PublicKey
		, (SELECT r.UID from [dbo].[Role] r WHERE [RoleName] = ''user'')
	)

	SET @EmploeeUID = (SELECT TOP 1 EmploeeUID FROM  @EmploeeUIDTable)

	INSERT INTO [dbo].[Auth] 
	([Login], [PassHash], [Salt], [EmployeeUID])
	VALUES
	(@Login, @passwordHash, @salt, @EmploeeUID)

	DELETE [dbo].[Invite] WHERE Token = @InvitingToken;
	SET @UserGuid = @EmploeeUID;

	COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
			rollback;
			RETURN 8;
	END CATCH
END
'
EXEC (@SQL);

