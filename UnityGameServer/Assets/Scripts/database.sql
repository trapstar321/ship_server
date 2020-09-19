create database ship;
use ship;

create table player
(
	ID int auto_increment not null,
    USERNAME varchar(1000) not null,
    PRIMARY KEY(id)
)DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci; 

create table item
(
	ID int NOT NULL auto_increment,
    NAME varchar(1000) not null,
    ICON_NAME varchar(1000) not null,
    IS_DEFAULT_ITEM tinyint not null,
    PRIMARY KEY(id)
)DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci; 

create table player_item
(
	ID int not null auto_increment,
    ITEM_ID int not null,
    PLAYER_ID int not null,
    PRIMARY KEY(id),
    FOREIGN KEY(ITEM_ID) REFERENCES item(ID),
    FOREIGN KEY(PLAYER_ID) REFERENCES player(ID)
)DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci; 

create table inventory_slot
(
	ID int NOT NULL auto_increment,
    ITEM_ID int Not null,
    SLOT_ID int not null,
    QUANTITY int null,
    PRIMARY KEY(ID),
    FOREIGN KEY(ITEM_ID) references player_item(ID)
)DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci; 

create table temporary_storage
(
	ID int NOT NULL auto_increment,
    ITEM_ID int Not null,    
    PRIMARY KEY(ID),
    FOREIGN KEY(ITEM_ID) references player_item(ID)
)DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci; 

create table inventory
(
	ID int not null auto_increment,
    PLAYER_ID int not null,
    SLOT_ID int not null,
    PRIMARY KEY(ID),
    foreign key(player_id) references player(id),
    foreign key(slot_id) references inventory_slot(id)
)DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci; 

insert into item
(name, icon_name, is_default_item)
select 'Wood log', 'wood.png', 0
union all
select 'Wood plank', 'WoodPlank.png', 0

insert into player
(username)
select 'player1'

insert into player_item
(item_id, player_id)
select 1, 1
union all
select 2,1

insert into inventory_slot
(item_id, slot_id, quantity)
select 1, 1, 1
union all
select 2,2,1

insert into inventory
(player_id, slot_id)
select 1, 1
union all
select 1,2