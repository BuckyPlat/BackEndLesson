create database Project_Bakamitai

use Project_Bakamitai

create table Users(
UserId int primary key identity(1,1),
UserName Nvarchar(50) not null,
Email varchar(80) not null unique,
PasswordHash varbinary(32) not null,
CreateAt Datetime default getutcdate()
)

create table PlayerProfile(
ProfileId int primary key identity(1,1),
UserId int not null unique,
DisplayName nvarchar(50) not null,
AvatarUrl varchar(200),
Levels int default 1,
Exps int default 0,
Gold int default 0,
Gem int default 0

foreign key (UserId) references Users(UserId)
)

create table Items(
ItemId int primary key identity(1,1),
ItemName Nvarchar(100) not null,
Description Nvarchar(255),
ProductImage varchar(200),
PriceGold int default 0,
PriceGem int default 0,
ItemType varchar(50),
IsShow bit default 1
)

create table ShopProducts(
ShopItemId int primary key identity(1,1),
ItemId int not null,
IsAvailable bit default 1,

foreign key (ItemId) references Items(ItemId)
)

create table Inventory(
InvenID int primary key identity(1,1),
UserId int not null,
ItemId int not null,
Quantity int default 1,
PurchasePriceGold int,
PurchasePriceGem int,
PurchasedAt Datetime default getdate(),

foreign key (UserId) references Users(UserId),
foreign key (ItemId) references Items(ItemId)
)

create table Transactions(
TransactionId int primary key identity(1,1),
UserId int not null,
ItemId int not null,
TransactionType varchar(10),
CurrencyType varchar(10),
Amount int not null,
Quantity int default 1,
CreateAt datetime default getdate(),

foreign key (UserId) references Users(UserId),
foreign key (ItemId) references Items(ItemId)
)

SELECT 
    name 
FROM sys.default_constraints 
WHERE parent_object_id = OBJECT_ID('Users')
  AND parent_column_id = (
        SELECT column_id FROM sys.columns 
        WHERE name = 'CreateAt' AND object_id = OBJECT_ID('Users')
  );

ALTER TABLE Users
DROP CONSTRAINT DF__Users__CreateAt__4AB81AF0;

ALTER TABLE Users
ADD CONSTRAINT DF_Users_CreateAt_UTC
DEFAULT (GETUTCDATE()) FOR CreateAt;

