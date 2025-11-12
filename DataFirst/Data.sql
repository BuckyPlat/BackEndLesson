create database GameVuiVL

use GameVuiVL

create table player(
ID tinyint primary key identity(1,1),
Username nvarchar(20) not null,
Levi int,
Gold int,
)

create table shop(
shopID tinyint primary key identity(1,1),
ItemName nvarchar(20) not null,
Price int,
Quality int,
)

insert into player(Username,Levi,Gold) values
('hieunt',1,20000),
('sangtq',10,9999),
('thannt',15,1111),
('namvip',9,12345);

insert into shop(ItemName,Price,Quality) values
('Gun',1111,12);