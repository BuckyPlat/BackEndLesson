create database projectbakamitai
go

use projectbakamitai
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

SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Players';

ALTER TABLE Players
ALTER COLUMN PasswordHash VARBINARY(32) NOT NULL;

insert into GameMode(ModeName) values
('Survival'),
('Creative'),
('Spectator')

insert into Items (ItemName, ItemType, Price) values
('Wooden_Boat', 'Transportation', 200),
('Horse', 'Transportation', 400),
('Ender_Dragon', 'Transportation', 5000),
('Diamond_Sword', 'Weapon', 800),
('Diamond_Pickaxe', 'Tool', 400),
('Iron_Axe', 'Tool', 80),
('Diamond_Shovel', 'Tool', 300),
('Diamond', 'Resource', 1000),
('Iron', 'Resource', 800),
('Iron_Sword', 'Weapon', 300),
('Coal', 'Resource', 10),
('Cobblestone', 'Resource', 5),
('Wooden_Plank', 'Resource', 8),
('Lapis', 'Resource', 850),
('Stone_Spear', 'Weapon', 20),
('Stone_Bricks', 'Resource', 8),
('Seeds', 'Resource', 1),
('Obsidian', 'Resource', 500),
('Happy_Ghast', 'Transportation', 100),
('Lantern', 'Tool', 80),
('Minecart', 'Transportation', 90);


insert into Shop(ShopName) values
('Transportaion_Shop'),
('Weapon_Shop'),
('Tool_Shop'),
('Resource_Shop')

insert into Missions(Title,Description,EXPReward,GoldReward) values
('Clear_The_enemies_wave','Help the villagers defeat the invade of pillagers',400,40),
('Create_potion_of_healing','Help the local clinic treat villagers wound',600,100),
('Defeat_the_leader','Slain the leader of the pillagers',800,180),
('Rebuild_the_village','Help the local rebuild their home',100,80),
('Find_coal','Mine some coals',60,100),
('Gather_wood','Chop down tree',40,0),
('Defeat_boss','Slain the 1000 years old wolves',450,60)

select * from Players
select * from Missions
select * from Shop
select * from GameMode