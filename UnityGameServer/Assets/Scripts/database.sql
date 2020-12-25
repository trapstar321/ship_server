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
SELECT* FROM player

select* from item

update item set item_type='general' where id=2

update player set is_on_ship=1 where id in (1)

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

SELECT a.id,b.id,b.SLOT_ID, b.QUANTITY, c.id, d.id as item_id, d.name, d.icon_name, d.is_default_item, d.item_type 
                        FROM inventory a 
                        inner join inventory_slot as b 
                        on a.slot_id=b.id 
                        inner join player_item as c 
                        on b.item_id=c.id 
                        and a.PLAYER_ID=c.PLAYER_ID
                        inner join item as d
                        on c.item_id=d.id
                        where a.player_id=1

delete from inventory where id=174
delete from inventory_slot where id=176
delete from player_item where id=50

select* from player_item where player_id=2
                        
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

create table player_skill_level
(
	ID bigint not null auto_increment,
    PLAYER_ID int not null,
    SKILL_LEVEL_ID bigint not null,
    EXPERIENCE int not null,
    PRIMARY KEY(ID),
    foreign key(player_id) references player(id),    
    foreign key(skill_level_id) references skill_level(id)    
)DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;        

insert into player_skill_level
(player_id, skill_level_id, experience)
select 1, 1, 0
union all
select 2, 1, 0
union all
select 3, 1, 0

select a.id, a.skill_id, lvl, dmg, exp_start, exp_end, skill_name
from skill_level as a
inner join skill as b
on a.skill_id=b.id
inner join player_skill_level as c
on a.id = c.skill_level_id
where c.player_id=1

select* from player_skill_level

select* from skill_level

select* from experience;

select* from item
 select* from player
 update player set x_player=32, y_player=1, z_player=45 where id=1
 
 select * from player_item where player_id=2
 
 delete from player_item where id=47
 
 create table resource_spawn
 (
	ID bigint NOT NULL auto_increment,
    ITEM_ID int null,   
    RESOURCE_TYPE int not null,
    RESOURCE_HP float not null,
    RESOURCE_COUNT int not null,
    RESPAWN_TIME float not null,
    X float not null,
    Y float not null,
    Z float not null,
    PRIMARY KEY(ID),
    FOREIGN KEY(ITEM_ID) references item(ID)    
)DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci; 

insert into resource_spawn
(item_id, resource_type, resource_hp, resource_count, respawn_time, x, y, z)
select 35, 5, 3000, 30, 15, 22, 0.93, 54
union all
select 36, 6, 1000, 10, 15, 21, 0.7, 50
union all
select 37, 7, 1500, 15, 15, 21, 0.7, 50

select* from resource_spawn where item_id=37
update resource_spawn set x=18, y=1, z=53 where item_id=37

select a.id, a.skill_id, lvl, modifier, exp_start, exp_end, skill_name, experience
                        from skill_level as a
                        inner join skill as b
                        on a.skill_id=b.id
                        inner join player_skill_level as c
                        on a.id = c.skill_level_id
                        where c.player_id=1

delete a
from player_skill_level as a
inner join skill_level as b
on a.skill_level_id=b.id
where player_id=1 and skill_id=1 and lvl=2

update player_skill_level a
inner join skill_level as b
on a.skill_level_id=b.id
set a.experience=999
where player_id=1 and skill_id=1
                        
select* from player_skill_level 

update player_skill_level set experience=3999 where id=82

delete from player_skill_level where player_id=1 and skill_level_id=2

insert into player_skill_level
(player_id, skill_level_id, experience)
select 1, 1, 999

select a.id, skill_id, lvl, modifier, exp_start, exp_end, skill_name
from skill_level as a
inner join skill as b
on a.skill_id=b.id

insert into player_skill_level
(player_id, skill_level_id, experience)
select 1, 1, 999

select* from resource_spawn
insert into player_skill_level
(player_id, skill_level_id, experience)
select 1, 6, 0                       
union all
select 2, 6, 0
union all
select 3, 6, 0
                        
select* from skill      
select* from skill_level    

insert into skill_level
(skill_id, lvl, dmg, exp_start, exp_end)
select 2, 1, 5, 0, 1000
union all              
select 2, 2, 8, 1000, 4000
union all
select 2, 3, 12, 4000, 10000
union all
select 2, 4, 15, 10000, 30000
union all
select 2, 5, 18, 30000, 100000

update resource_spawn set skill_type=2 where id<>1

 create table resource_spawn
 (
	ID bigint NOT NULL auto_increment,    
    RESOURCE_ID bigint not null,
    RESPAWN_TIME float not null,    
    X float not null,
    Y float not null,
    Z float not null,    
    PRIMARY KEY(ID),
    FOREIGN KEY(RESOURCE_ID) references resource(ID) 
)DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci; 



insert into resource_spawn
(resource_id, respawn_time, x,y,z)
select 1, 15, 32, 0.9, 46
union all
select 2, 10, 28, 0.8, 51
union all
select 3, 10, 29, 0.6, 48
union all
select 4, 15, 22, 0.9, 54
union all
select 5, 15, 21, 0.7, 50
union all
select 6, 15, 18, 1, 53

 create table resource
 (
	ID bigint NOT NULL auto_increment,
    ITEM_ID int null,   
    RESOURCE_TYPE int not null,
    RESOURCE_HP float not null,
    RESOURCE_COUNT int not null,
    EXPERIENCE float not null,
    PRIMARY KEY(ID),
    FOREIGN KEY(ITEM_ID) references item(ID)    
)DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci; 

select* from resource as a
inner join resource_spawn as b
on a.id=b.resource_id

select* from player_skill_level

update resource set experience=50 where id>=1

update skill_level set exp_end=999 where id=1
update skill_level set exp_end=3999 where id=2
update skill_level set exp_end=9999 where id=3
update skill_level set exp_end=29999 where id=4
update skill_level set exp_end=99999 where id=5
update skill_level set exp_end=999 where id=6
update skill_level set exp_end=3999 where id=7
update skill_level set exp_end=9999 where id=8
update skill_level set exp_end=29999 where id=9
update skill_level set exp_end=99999 where id=10

alter table skill_level rename column dmg to modifier

recipe (id, name, item_id, time_to_craft)
recipe_requirements(id, reciper_id, item_id, quantity)
recipe_skill_requirements(id, recipe_id, skill_id)

create table recipe
(
	ID bigint NOT NULL auto_increment,
    ITEM_ID int not null,   
    RECIPE_NAME varchar(1000) not null,
    TIME_TO_CRAFT float not null,
    PRIMARY KEY(ID),
    FOREIGN KEY(ITEM_ID) references item(ID)    
)DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci; 

create table recipe_item_requirements(
	ID bigint NOT NULL auto_increment,
    RECIPE_ID bigint not null, 
    ITEM_ID int not null,
    QUANTITY int not null,
    PRIMARY KEY(ID),
    FOREIGN KEY(ITEM_ID) references item(ID),    
    FOREIGN KEY(RECIPE_ID) references recipe(ID)    
)DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci; 

create table recipe_skill_requirements
(
	ID bigint NOT NULL auto_increment,
    RECIPE_ID bigint not null, 
    SKILL_LEVEL_ID bigint not null,
    PRIMARY KEY(ID),    
    FOREIGN KEY(RECIPE_ID) references recipe(ID)    ,
    FOREIGN KEY(SKILL_LEVEL_ID) references skill_level(ID)  
)DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci; 

insert into recipe_skill_requirements
(recipe_id, skill_level_id)
select 2, 32

update item set crafting_exp=10 where id=43

select* from item
insert into recipe
(item_id, recipe_name, time_to_craft)
select 41, 'Bread', 5

insert into recipe_item_requirements
(recipe_id, item_id, quantity)
select 2, 42, 2


select a.id, a.recipe_name, a.time_to_craft, b.crafting_exp
from recipe as a
inner join item as b
on a.item_id=b.id

select a.id, b.id as item_id, b.name, b.icon_name, quantity 
from recipe_item_requirements as a
inner join item as b
on a.item_id=b.id
where a.recipe_id=1

alter table 


select skill_level_id, lvl, skill_name, modifier
from recipe as a
inner join recipe_skill_requirements as b
on a.id=b.recipe_id
inner join skill_level as c
on b.skill_level_id=c.id
inner join skill as d
on c.skill_id=d.id
where a.id=1

select* from recipe
select* from inventory_slot as a
inner join inventory as b
on a.id=b.slot_id
where b.player_id=1 and a.slot_id=28
select* from inventory

update inventory_slot set item_id=57, quantity=100 where id=182
update inventory_slot set item_id=58, quantity=100 where id=155

select* from player_item

update inventory_slot a
inner join inventory as b
on a.id=b.slot_id
set a.quantity=930
where player_id = 1 and a.slot_id=11

select a.id, a.skill_id, lvl, modifier, exp_start, exp_end, skill_name, experience
                        from skill_level as a
                        inner join skill as b
                        on a.skill_id=b.id
                        inner join player_skill_level as c
                        on a.id = c.skill_level_id
                        where c.player_id=1

select* from skill_level where id=32
select* from skill

select a.id, b.id as item_id, b.name, b.icon_name, quantity 
                                from recipe_item_requirements as a
                                inner join item as b
                                on a.item_id = b.id
                                where a.recipe_id = 1
                                
select a.id, a.recipe_name, a.time_to_craft, b.crafting_exp, b.icon_name, a.item_id
                        from recipe as a
                        inner join item as b
                        on a.item_id=b.id

select* from recipe_skill_requirements where recipe_id=2
update recipe_skill_requirements set skill_level_id=33 where id=2
                        
select* from recipe       
select* from item

update item set crafting_exp = 5 where id=2       

select* from item        
select* from inventory_slot where item_id=58
select* from player_item where item_id=39

update inventory_slot set quantity=100 where id=155

create table trader
(
	ID bigint NOT NULL auto_increment,
    NAME varchar(1000) not null,
    X float not null,
    Y float not null,
    Z float not null,
    Y_ROT float not null    ,
    PRIMARY KEY(ID)    
)DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci; 