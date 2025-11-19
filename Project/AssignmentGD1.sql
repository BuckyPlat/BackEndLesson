create database GD1
go

use GD1
go

create table player(
    playerid tinyint primary key identity(1,1),
    username nvarchar(30) not null unique,
    email varchar(80) not null unique,
    passwordhash varbinary(32) not null,
    createdate datetime default getdate()
);

create table characters(
    characterid tinyint primary key identity(1,1),
    playerid tinyint not null,
    charactername nvarchar(40) not null,
    level int default 1,
    experience int default 0,
    health int default 100,
    hunger int default 100,
    createdat datetime default getdate(),

    constraint fk_characters_player foreign key(playerid)
        references player(playerid)
);

create table items(
    itemid tinyint primary key identity(1,1),
    itemname nvarchar(40) not null,
    itemtype varchar(30),
    description nvarchar(255),
    price int not null default 0
);

create table inventory(
    inventoryid tinyint primary key identity(1,1),
    characterid tinyint not null,

    constraint fk_inventory_character foreign key(characterid)
        references characters(characterid)
);

create table inventoryitems(
    inventoryitemid tinyint primary key identity(1,1),
    inventoryid tinyint not null,
    itemid tinyint not null,
    quantity int default 1,

    constraint fk_inventoryitems_inventory foreign key(inventoryid)
        references inventory(inventoryid),

    constraint fk_inventoryitems_item foreign key(itemid)
        references items(itemid)
);

create table mission(
    missionid tinyint primary key identity(1,1),
    missionname nvarchar(50) not null,
    difficulty tinyint default 1,
    rewardgold int default 0,
    rewarditemid tinyint,

    constraint fk_mission_rewarditem foreign key(rewarditemid)
        references items(itemid)
);

create table playermission(
    playermissionid tinyint primary key identity(1,1),
    characterid tinyint not null,
    missionid tinyint not null,
    status tinyint default 0,
    updatedat datetime default getdate(),

    constraint fk_playermission_character foreign key(characterid)
        references characters(characterid),

    constraint fk_playermission_mission foreign key(missionid)
        references mission(missionid)
);

create table transactions(
    transactionid tinyint primary key identity(1,1),
    playerid tinyint not null,
    itemid tinyint,
    amount int not null,
    description nvarchar(120),
    createdat datetime default getdate(),

    constraint fk_transaction_player foreign key(playerid)
        references player(playerid),

    constraint fk_transaction_item foreign key(itemid)
        references items(itemid)
);
