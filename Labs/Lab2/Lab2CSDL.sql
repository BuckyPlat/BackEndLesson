create database lab2

use lab2

create table USSR(
UssrID tinyint primary key identity(1,1),
UssrName nvarchar(20) not null,
RegionID tinyint not null,
LinkAvatar nvarchar(50),
IsDeleted bit default(1),
RoleID tinyint not null,
OTP numeric(11)
)

foreign key (

create table Region(
RegionID tinyint primary key identity(1,1),
RegionName nvarchar(40) not null,
)

create table Rolle(
RoleID tinyint primary key identity(1,1),
RoleName nvarchar(20)
)

create table GameLevel(
LevelID tinyint primary key identity(1,1),
Title nvarchar(20) not null,
Descriptions nvarchar(50),
)

create table Question(
QuestionID tinyint primary key identity(1,1),
ContentQuestion nvarchar(50) not null,
Answer nvarchar(50),
Option1 nvarchar(50),
Option2 nvarchar(50),
Option3 nvarchar(50),
Option4 nvarchar(50),
LevelID tinyint not null,
)

create table LevelResult(
QuizzResult tinyint primary key identity(1,1),
UssrID tinyint not null,
LevelID tinyint not null,
Score int,
CompletionDate Date default GETDATE(),
)