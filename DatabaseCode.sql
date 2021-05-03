CREATE DATABASE [YazilimYapimi]

GO

USE YazilimYapimi

GO

CREATE TABLE [dbo].[Users] (
    [Id]               INT             IDENTITY (1, 1) NOT NULL,
    [FirstName]        VARCHAR (50)    NOT NULL,
    [LastName]         VARCHAR (50)    NOT NULL,
    [UserName]         VARCHAR (50)    NOT NULL,
    [PasswordHash]     VARBINARY (500) NOT NULL,
    [PasswordSalt]     VARBINARY (500) NOT NULL,
    [TCIdentityNumber] CHAR (11)       NOT NULL,
    [PhoneNumber]      CHAR (11)       NOT NULL,
    [Email]            VARCHAR (50)    NOT NULL,
    [Address]          VARCHAR (250)   NOT NULL,
    CONSTRAINT [PK__Users__3214EC0709300D8B] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UC_User] UNIQUE NONCLUSTERED ([UserName] ASC)
);

GO

CREATE TABLE [dbo].[OperationClaims] (
    [Id]   INT           IDENTITY (1, 1) NOT NULL,
    [Name] VARCHAR (250) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO

INSERT INTO [dbo].[OperationClaims]
           ([Name])
     VALUES
           ('admin')
GO
INSERT INTO [dbo].[OperationClaims]
           ([Name])
     VALUES
           ('kullanıcı')
		   
GO

CREATE TABLE [dbo].[UserOperationClaims] (
    [Id]               INT IDENTITY (1, 1) NOT NULL,
    [UserId]           INT NOT NULL,
    [OperationClaimId] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserOperationClaims_OperationClaims] FOREIGN KEY ([OperationClaimId]) REFERENCES [dbo].[OperationClaims] ([Id]),
    CONSTRAINT [FK_UserOperationClaims_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
);

GO 

CREATE TABLE [dbo].[Wallet] (
    [Id]     INT   IDENTITY (1, 1) NOT NULL,
    [Amount] MONEY NOT NULL,
    [UserId] INT   NOT NULL,
    CONSTRAINT [PK_Wallet] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Wallet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
);

GO

CREATE trigger [dbo].[autoAddWallet]
on [dbo].[Users]
after insert
as
	declare @userId int
	select @userId=Id from inserted
	insert into Wallet(Amount,UserId) values(0,@userId)
	
GO

CREATE TABLE [dbo].[AddMoney] (
    [Id]     INT   IDENTITY (1, 1) NOT NULL,
    [Amount] MONEY NOT NULL,
    [Status] BIT   NOT NULL,
    [UserId] INT   NOT NULL,
    CONSTRAINT [PK__AddMoney__3214EC078D436E0F] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AddMoney_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
);


GO
CREATE trigger [dbo].[moneyAdded]
on [dbo].[AddMoney]
after update
as
	Declare @oldStatus bit,@status bit,@amount money,@userId int
	select @status=status,@amount=Amount,@userId=UserId from inserted
	select @oldStatus=status from deleted
	if @status=1 and @oldStatus !=1
	begin 
		if (select COUNT(*) from Wallet where UserId=@userId)>0
		begin
			update Wallet set Amount=((select Amount from Wallet where UserId=@userId)+@amount) where UserId=@userId
		end
		else
		begin
			insert into Wallet(Amount,UserId) values(@amount,@userId);
		end
	end
	else if @oldStatus=1 and @status=0 
	begin
		update Wallet set Amount=((select Amount from Wallet where UserId=@userId)-@amount) where UserId=@userId
	end
	
GO

CREATE TABLE [dbo].[Categories] (
    [Id]   INT           IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO

CREATE TABLE [dbo].[Products] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [CategoryId] INT            NOT NULL,
    [Quantity]   DECIMAL (7, 2) NOT NULL,
    [UnitPrice]  MONEY          NOT NULL,
    [SupplierId] INT            NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Products_Users] FOREIGN KEY ([SupplierId]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_Products_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([Id])
);


GO

CREATE TABLE [dbo].[AddProducts] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [SupplierId] INT            NOT NULL,
    [Quantity]   DECIMAL (7, 2) NOT NULL,
    [CategoryId] INT            NOT NULL,
    [UnitPrice]  MONEY          NOT NULL,
    [Status]     BIT            NOT NULL,
    CONSTRAINT [PK_AddProducts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AddProducts_Users] FOREIGN KEY ([SupplierId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AddProducts_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([Id])
);


GO

CREATE trigger [dbo].[productAdded]
on [dbo].[AddProducts]
after update
as
begin
	Declare @oldStatus bit,@status bit,@unitPrice money,@supplierId int,@quantity decimal(7,2),@categoryId int
	select @status=status,@unitPrice=UnitPrice,@categoryId=CategoryId,@supplierId=SupplierId,@quantity=Quantity from inserted
	select @oldStatus=status from deleted

	if @status=1 and @oldStatus !=1
	begin 
		if (select COUNT(*) from Products where SupplierId=@supplierId and CategoryId=@categoryId and UnitPrice=@unitPrice)>0
		begin
			update Products set Quantity=((select Quantity from Products where SupplierId=@supplierId and CategoryId=@categoryId and UnitPrice=@unitPrice)+@quantity) where SupplierId=@supplierId and CategoryId=@categoryId and UnitPrice=@unitPrice
		end
		else
		begin
			insert into Products(CategoryId,Quantity,UnitPrice,SupplierId) values(@categoryId,@quantity,@unitPrice,@supplierId);
		end
	end
	else if @oldStatus=1 and @status=0 
	begin
		update Products set Quantity=((select Quantity from Products where SupplierId=@supplierId and CategoryId=@categoryId and UnitPrice=@unitPrice)-@quantity) where SupplierId=@supplierId and CategoryId=@categoryId and UnitPrice=@unitPrice
	end
end

GO

CREATE TABLE [dbo].[Orders] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [OrderDate]  DATE           NOT NULL,
    [CustomerId] INT            NOT NULL,
    [CategoryId] INT            NOT NULL,
    [Quantity]   DECIMAL (7, 2) NOT NULL,
    CONSTRAINT [PK__Orders__3214EC07C932DDCA] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Orders_Users] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Users] ([Id])
);

GO

CREATE TABLE [dbo].[OrderDetails] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [ProductId] INT            NOT NULL,
    [Quantity]  DECIMAL (7, 2) NOT NULL,
    [OrderId]   INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_OrderDetails_Products] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products] ([Id]),
    CONSTRAINT [FK_OrderDetails_Orders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id]) ON DELETE CASCADE
);
