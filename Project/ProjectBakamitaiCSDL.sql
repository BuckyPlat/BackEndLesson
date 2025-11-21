create database projectbakamitai
go

use bakamitai
go

create table Players(
PlayerID tinyint primary key identity(1,1),
PlayerName nvarchar(50) not null,
Email varchar(50) not null,
PasswordHash varbinary not null,
CreateDate datetime default getdate()
)

create table GameMode(
GamemodeID tinyint primary key identity(1,1),
ModeName nvarchar(50) not null
)

create table Characters(
CharacterID tinyint primary key identity(1,1),
PlayerID tinyint not null,
CharacterName nvarchar(40) not null,
GamemodeID tinyint not null,
Heatlh int default 100,
Hunger int default 100,
Experience int default 0,
Gold int default 0,
CreateDate datetime default getdate(),

foreign key (PlayerID) references Players(PlayerID),
foreign key (GamemodeID) references GameMode(GamemodeID)
)

create table Missions(
MissionID tinyint primary key identity(1,1),
Title nvarchar(80) not null,
Description TEXT,
EXPReward int default 0,
GoldReward int default 0
);

create table CharacterMission(
CharacterID tinyint not null,
MissionID tinyint not null,
IsCompleted BIT default 0,
CompletionDate Datetime,
primary key (CharacterID, MissionID),
Foreign key (CharacterID) References Characters(CharacterID),
Foreign key (MissionID) references Missions(MissionID)
)

create table Items(
ItemID tinyint primary key identity(1,1),
ItemName nvarchar(50) not null,
ItemType nvarchar(50) not null,
Price int not null
)

create table Shop(
ShopID tinyint primary key identity(1,1),
ShopName nvarchar(40) not null
)

create table ShopItem(
ShopID tinyint not null,
ItemID tinyint not null,
Primary key(ShopID, ItemID),
foreign key (ShopID) references Shop(ShopID),
foreign key (ItemID) references Items(ItemID)
)

create table Inventory(
CharacterID tinyint not null,
ItemID tinyint not null,
Quantity int default 1,
AcquireDate Datetime default getdate(),
Primary key (CharacterID, ItemID),
foreign key (CharacterID) references Characters(CharacterID),
foreign key (ItemID) references Items(ItemID)
);