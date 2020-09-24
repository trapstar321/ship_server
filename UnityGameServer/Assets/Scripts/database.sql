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
    ITEM_TYPE varchar(1000) not null
    PRIMARY KEY(id)
)DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci; 

create table player_item
(
	ID bigint not null auto_increment,
    ITEM_ID int not null,
    PLAYER_ID int not null,
    PRIMARY KEY(id),
    FOREIGN KEY(ITEM_ID) REFERENCES item(ID),
    FOREIGN KEY(PLAYER_ID) REFERENCES player(ID)
)DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci; 

create table inventory_slot
(
	ID bigint NOT NULL auto_increment,
    ITEM_ID bigint Not null,
    SLOT_ID int not null,
    QUANTITY int null,
    PRIMARY KEY(ID),
    FOREIGN KEY(ITEM_ID) references player_item(ID)
)DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci; 

create table inventory
(
	ID bigint not null auto_increment,
    PLAYER_ID int not null,
    SLOT_ID bigint not null,
    PRIMARY KEY(ID),
    foreign key(player_id) references player(id),
    foreign key(slot_id) references inventory_slot(id)
)DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci; 

create table ship_equipment
(
	ID bigint NOT NULL auto_increment,
    ITEM_ID int null,   
    PLAYER_ID int not null,
    ITEM_TYPE varchar(1000) not null,
    PRIMARY KEY(ID),
    FOREIGN KEY(ITEM_ID) references player_item(ID),
    FOREIGN KEY(PLAYER_ID) references player(ID)
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

use ship
select* from inventory
select* from inventory_slot
select* from player_item
select* from item
select* from ship_equipment

delete from inventory where id=745
delete from inventory_slot where id=752

select b.id, b.slot_id from inventory as a
inner join inventory_slot as b
on a.slot_id=b.id
where a.player_id=1 and b.slot_id=1

insert into inventory_slot
(item_id, slot_id, quantity)
select 1, 1, 1
union all
select 2,2,1
union all
select 4,3,1
union all
select 5,4,1
union all
select 8,6,1

insert into inventory
(player_id, slot_id)
select 1,758
union all
select 1,759
union all
select 1,760
union all
select 1,761
union all
select 1,762

select* from item
select* from inventory_slot
select* from player_item

select* from inventory

update item set item_type='resource' where id in (1,2)

insert into item
(name, icon_name, is_default_item, item_type)
select 'Gold crows nest', 'crows_nest_gold.png', 0, 'crows_nest'
union all
select 'Bow sprite', 'bow_sprite.png', 0, 'bow_sprite'
union all
select 'Jib sail', 'jib_sail.png', 0, 'jib_sail'
union all
select 'Fore sail', 'fore_sail.png', 0, 'fore_sail'
union all
select 'Rudder', 'rudder.png', 0, 'rudder'
union all
select 'Keel', 'keel.png', 0, 'keel'
union all
select 'Hull', 'hull.png', 0, 'hull'
union all
select 'Main mast', 'main_mast.png', 0, 'main_mast'
union all
select 'Rigging', 'rigging.png', 0, 'rigging'
union all
select 'Captains cabin', 'captains_cabin.png', 0, 'captains_cabin'
union all
select 'Main sail', 'main_sail.png', 0, 'main_sail'
union all
select 'Crows nest', 'crows_nest.png', 0, 'crows_nest'

insert into player_item
(item_id, player_id)
select 18,1
union all
select 3, 1
union all
select 4, 1
union all
select 5, 1
union all
select 6, 1
union all
select 7, 1
union all
select 8, 1
union all
select 9, 1
union all
select 10, 1
union all
select 11, 1
union all
select 12, 1
union all
select 13, 1

insert into inventory_slot
(item_id, slot_id, quantity)
select 23,13,1
union all
select 8, 1, 1
union all
select 9, 2, 1
union all
select 10, 3, 1
union all
select 11, 4, 1
union all
select 12, 5, 1
union all
select 13, 6, 1
union all
select 14, 7, 1
union all
select 15, 8, 1
union all
select 16, 9, 1
union all
select 17, 11, 1
union all
select 18, 12, 1

insert into inventory
(player_id, slot_id)
select 1, 260
union all
select 1, 245
union all
select 1,246
union all
select 1,247
union all
select 1, 248
union all
select 1, 249
union all
select 1, 250
union all
select 1, 251
union all
select 1,252
union all
select 1, 253
union all
select 1, 254
union all
select 1,255
