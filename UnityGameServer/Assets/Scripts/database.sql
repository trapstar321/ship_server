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
select* from item where id>20
select* from ship_equipment

insert into player
(username, level, experience)
select 'player3', 1, 0

alter table player add PASSWORD varchar(100) null

update player set password='' where id in (1,2,3)

update item set drop_chance=40 where id=21
update item set drop_chance=30 where id=22
update item set drop_chance=30 where id=23
update item set drop_chance=15 where id=24
update item set drop_chance=35 where id=25
update item set drop_chance=25 where id=26
update item set drop_chance=20 where id=27
update item set drop_chance=30 where id=28
update item set drop_chance=45 where id=29

alter table item add DROP_CHANCE int

delete from inventory where id=745
delete from inventory_slot where id=752

select b.id, b.slot_id from inventory as a
inner join inventory_slot as b
on a.slot_id=b.id
where a.player_id=1 and b.slot_id=1

alter table item add item_type varchar(1000)

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
union all
select 14, 1


insert into inventory_slot
(item_id, slot_id, quantity)
select 10,10,1
union all
select 11, 11, 1
union all
select 12, 12, 1
union all
select 12, 12, 1
union all
select 13, 13, 1
union all
select 14, 14, 1
union all
select 15, 15, 1
union all
select 16, 16, 1
union all
select 17, 17, 1
union all
select 18, 18, 1
union all
select 19, 19, 1
union all
select 20, 20, 1
union all
select 21, 21, 1

insert into inventory
(player_id, slot_id)
select 1, 147
union all
select 1, 148
union all
select 1, 149
union all
select 1, 150
union all
select 1, 151
union all
select 1, 152
union all
select 1, 153
union all
select 1, 154
union all
select 1, 155
union all
select 1, 156
union all
select 1, 157
union all
select 1, 158
union all
select 1, 159

SELECT a.id,b.SLOT_ID, b.QUANTITY, c.id, d.id as item_id, d.name, d.icon_name, d.is_default_item, d.item_type 
                        FROM inventory a 
                        inner join inventory_slot as b 
                        on a.slot_id=b.id 
                        inner join player_item as c 
                        on b.item_id=c.id 
                        and a.PLAYER_ID=c.PLAYER_ID
                        inner join item as d
                        on c.item_id=d.id
                        where a.player_id=1
                        
                        update item set item_type='jib_sail' where id in (5)
                        update item set item_type='hull' where id in (9)
                        
                        delete from inventory where id=148

create table skill
(
	ID bigint not null auto_increment,
    SKILL_NAME varchar(1000) not null,
    PRIMARY KEY(ID)    
)DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;                         
                        
select* from skill      

insert into skill
(skill_name)
select 'Woodcutting'

insert into skill
(skill_name)
select 'Mining'

create table skill_level
(
	ID bigint NOT NULL auto_increment,
    SKILL_ID bigint null,   
    LVL int not null,
    DMG int not null,
    EXP_START int not null,
    EXP_END int not null,
    PRIMARY KEY(ID),
    FOREIGN KEY(SKILL_ID) references skill(ID)
)DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci; 

insert into skill_level
(skill_id, lvl, dmg, exp_start, exp_end)
select 1, 1, 10, 0, 1000
union all
select 1, 2, 12, 1000, 4000
union all
select 1, 3, 16, 4000, 10000
union all
select 1, 4, 22, 10000, 30000
union all
select 1, 5, 28, 30000, 100000
                  
select* from skill_level                  