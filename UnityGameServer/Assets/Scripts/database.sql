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
    ITEM_TYPE varchar(1000) not null,
    ATTACK int null,
	HEALTH int null,
	DEFENCE int null,
	ROTATION int null,
	SPEED int null,
	VISIBILITY int null,
	CANNON_RELOAD_SPEED int null
    PRIMARY KEY(id)
)DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci; 

alter table item add ATTACK int not null default 0
alter table item add HEALTH int not null default 0
alter table item add DEFENCE int not null default 0
alter table item add ROTATION int not null default 0
alter table item add SPEED int not null default 0
alter table item add VISIBILITY int not null default 0
alter table item add CANNON_RELOAD_SPEED int not null default 0
alter table item add CRIT_CHANCE int not null default 0
alter table item add CANNON_FORCE int not null default 0

select* from item

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
    ITEM_ID bigint null,   
    PLAYER_ID int not null,
    ITEM_TYPE varchar(1000) not null,
    PRIMARY KEY(ID),
    FOREIGN KEY(ITEM_ID) references player_item(ID),
    FOREIGN KEY(PLAYER_ID) references player(ID)
)DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci; 

create table player_equipment
(
	ID bigint NOT NULL auto_increment,
    ITEM_ID bigint null,   
    PLAYER_ID int not null,
    ITEM_TYPE varchar(1000) not null,
    PRIMARY KEY(ID),
    FOREIGN KEY(ITEM_ID) references player_item(ID),
    FOREIGN KEY(PLAYER_ID) references player(ID)
)DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci; 

create table base_stats
(
	ID bigint NOT NULL auto_increment,
    LEVEL int NOT NULL,
    ATTACK int not null default 0,
	HEALTH int not null default 0,
	DEFENCE int not null default 0,
	ROTATION int not null default 0,
	SPEED int not null default 0,
	VISIBILITY int not null default 0,
	CANNON_RELOAD_SPEED int not null default 0,
	CRIT_CHANCE int not null default 0,
    PRIMARY KEY(ID)
)DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

select* from base_stats 

alter table base_stats add CANNON_FORCE int not null default 0

select* from base_stats

update base_stats set speed=5 where id=1

insert into base_stats
(level, attack, health, defence, rotation, speed, visibility, cannon_reload_speed, crit_chance)
select 1, 100, 500, 20, 2, 10, 50, 4, 20

create table experience
(
	ID bigint NOT NULL auto_increment,
    LEVEL int NOT NULL,
    FROM_ int not null,
    TO_ int not null,
    PRIMARY KEY(ID)
)DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci; 

insert into item
(name, icon_name, is_default_item)
select 'Wood log', 'wood.png', 0
union all
select 'Wood plank', 'WoodPlank.png', 0

insert into player
(username)
select 'player2'

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

insert into ship_equipment
(item_id, player_id, item_type)
select 23, 2, 'crows_nest'

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

select* from inventory_slot as a
inner join inventory as b
on a.id=b.slot_id
where b.player_id=1
order by a.slot_id asc

use ship
select* from item

select ID, NAME, ICON_NAME, IS_DEFAULT_ITEM, ITEM_TYPE
from item

select* from inventory_slot

truncate table inventory_slot

update inventory_slot set quantity=1 where id=38

delete from inventory_slot where id between 111 and 159

update inventory_slot set item_id=8 where id=69
update inventory_slot set item_id=9 where id=70

SELECT b.SLOT_ID, b.QUANTITY, c.id, d.id as item_id, d.name, d.icon_name, d.is_default_item, d.item_type 
                        FROM inventory a 
                        inner join inventory_slot as b 
                        on a.slot_id=b.id 
                        inner join player_item as c 
                        on b.item_id=c.id 
                        and a.PLAYER_ID=c.PLAYER_ID
                        inner join item as d
                        on c.item_id=d.id
                        where a.player_id=1

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
select 3,2
union all
select 4, 2
union all
select 5, 2
union all
select 6, 2
union all
select 7, 2
union all
select 8, 2
union all
select 9, 2
union all
select 10, 2
union all
select 11, 2
union all
select 12, 2
union all
select 13, 2
union all
select 14, 2

insert into inventory_slot
(item_id, slot_id, quantity)
select 1, 1,1
union all
select 2,2,1
union all
select 3,3,1
union all
select 4,4,1
union all
select 5,5,1
union all
select 6,6,1
union all
select 7,7,1
union all
select 8,8,1
union all
select 9,9,1
union all
select 10,10,1
union all
select 11,11,1
union all
select 12,12,1
union all
select 13,13,1
union all
select 14,14,1
union all
select 15,15,1
union all
select 16,16,1
union all
select 17,17,1
union all
select 18,18,1

insert into inventory
(player_id, slot_id)
select 1, id
from inventory_slot

truncate table inventory
truncate table ship_equipment
truncate table inventory_slot

select* from inventory_slot as a
where slot_id in (2)
inner join inventory as b
on a.id=b.slot_id
where b.player_id=1
order by a.slot_id asc

1
2
3
4
5
6

delete from inventory_slot where id between 483 and 545

update inventory_slot set item_id=4 where id=485
update inventory_slot set item_id=2,quantity=1 where id=524

select* from player

alter table player add LEVEL int
alter table player add EXPERIENCE int

update player set level=1, experience=0 where id in (1,2)
