-- MySQL dump 10.13  Distrib 8.0.22, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: ship
-- ------------------------------------------------------
-- Server version	8.0.22

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `category`
--

DROP TABLE IF EXISTS `category`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `category` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `NAME` varchar(1000) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `ICON` varchar(1000) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `category`
--

LOCK TABLES `category` WRITE;
/*!40000 ALTER TABLE `category` DISABLE KEYS */;
INSERT INTO `category` VALUES (1,'Ingredient','Ingredient'),(2,'Weapon','Weapon'),(3,'Armor','Armoring'),(4,'Ship equipment','WeaponArmor'),(5,'Food','Apple'),(8,'Cooking','Cooking'),(9,'Smithing','Smithing'),(10,'All categories','ListOfItems'),(11,'Consumables','Consumables');
/*!40000 ALTER TABLE `category` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `crafting_spot_spawn`
--

DROP TABLE IF EXISTS `crafting_spot_spawn`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `crafting_spot_spawn` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `SKILL_TYPE` int NOT NULL,
  `X` float NOT NULL,
  `Y` float NOT NULL,
  `Z` float NOT NULL,
  `Y_ROT` float NOT NULL,
  `GAME_OBJECT_TYPE` int NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `crafting_spot_spawn`
--

LOCK TABLES `crafting_spot_spawn` WRITE;
/*!40000 ALTER TABLE `crafting_spot_spawn` DISABLE KEYS */;
INSERT INTO `crafting_spot_spawn` VALUES (2,6,32,1,54,0,10);
/*!40000 ALTER TABLE `crafting_spot_spawn` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `experience`
--

DROP TABLE IF EXISTS `experience`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `experience` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `LEVEL` int NOT NULL,
  `FROM_` int NOT NULL,
  `TO_` int NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `experience`
--

LOCK TABLES `experience` WRITE;
/*!40000 ALTER TABLE `experience` DISABLE KEYS */;
/*!40000 ALTER TABLE `experience` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `inventory`
--

DROP TABLE IF EXISTS `inventory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inventory` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `PLAYER_ID` int NOT NULL,
  `SLOT_ID` int NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `PLAYER_ID` (`PLAYER_ID`),
  KEY `SLOT_ID` (`SLOT_ID`),
  CONSTRAINT `inventory_ibfk_1` FOREIGN KEY (`PLAYER_ID`) REFERENCES `player` (`ID`),
  CONSTRAINT `inventory_ibfk_2` FOREIGN KEY (`SLOT_ID`) REFERENCES `inventory_slot` (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=336 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `inventory`
--

LOCK TABLES `inventory` WRITE;
/*!40000 ALTER TABLE `inventory` DISABLE KEYS */;
INSERT INTO `inventory` VALUES (171,2,173),(175,2,177),(184,2,186),(185,2,187),(186,2,188),(187,2,189),(188,2,190),(189,2,191),(190,2,192),(191,2,193),(192,2,194),(193,3,195),(194,3,196),(195,3,197),(196,3,198),(197,3,199),(198,3,200),(199,3,201),(200,3,202),(201,3,203),(203,2,205),(204,2,206),(205,1,207),(206,1,208),(207,1,209),(208,1,210),(209,1,211),(210,1,212),(211,1,213),(212,1,214),(213,1,215),(214,1,216),(215,1,217),(216,1,218),(217,1,219),(218,1,220),(219,1,221),(220,1,222),(221,1,223),(222,1,224),(223,1,225),(224,1,226),(225,1,227),(226,1,228),(227,1,229),(228,1,230),(229,1,231),(230,1,232),(231,1,233),(232,1,234),(233,1,235),(234,1,236),(235,1,237),(236,1,238),(237,1,239),(238,1,240),(239,1,241),(240,1,242),(241,1,243),(242,1,244),(243,1,245),(244,1,246),(245,1,247),(246,1,248),(247,1,249),(248,1,250),(249,1,251),(250,1,252),(251,1,253),(252,1,254),(253,1,255),(254,1,256),(255,1,257),(256,1,258),(257,1,259),(258,1,260),(259,1,261),(260,1,262),(261,1,263),(262,1,264),(263,1,265),(264,1,266),(265,1,267),(266,1,268),(267,1,269),(268,1,270),(269,1,271),(270,1,272),(271,1,273),(272,1,274),(273,1,275),(274,1,276),(275,1,277),(276,1,278),(277,1,279),(278,1,280),(279,1,281),(280,1,282),(281,1,283),(282,1,284),(283,1,285),(284,1,286),(332,2,287),(333,2,288),(334,2,289),(335,2,290);
/*!40000 ALTER TABLE `inventory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `inventory_slot`
--

DROP TABLE IF EXISTS `inventory_slot`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inventory_slot` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `ITEM_ID` int DEFAULT NULL,
  `SLOT_ID` int NOT NULL,
  `QUANTITY` int DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `ITEM_ID` (`ITEM_ID`),
  CONSTRAINT `inventory_slot_ibfk_1` FOREIGN KEY (`ITEM_ID`) REFERENCES `player_item` (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=291 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `inventory_slot`
--

LOCK TABLES `inventory_slot` WRITE;
/*!40000 ALTER TABLE `inventory_slot` DISABLE KEYS */;
INSERT INTO `inventory_slot` VALUES (143,34,11,1554),(146,9,26,3),(147,NULL,41,0),(148,18,9,1),(149,54,8,100),(150,12,12,1),(151,61,21,48),(152,41,15,1),(153,20,3,5),(154,NULL,39,0),(155,14,27,1),(156,15,6,1),(157,57,22,129),(158,NULL,13,0),(159,59,10,100),(160,25,4,1),(161,17,5,1),(162,60,31,231),(163,11,2,2),(164,19,1,1),(167,NULL,17,0),(168,53,19,32),(169,16,7,1),(170,85,12,1),(171,NULL,14,0),(172,52,20,192),(173,63,1,11),(177,51,2,257),(178,NULL,24,0),(179,17,28,1),(180,10,25,1),(181,NULL,30,0),(182,15,29,1),(183,58,18,87),(184,NULL,38,0),(185,62,23,52),(186,108,3,1),(187,64,4,200),(188,65,5,398),(189,66,6,55),(190,67,7,158),(191,68,8,248),(192,69,9,150),(193,70,10,446),(194,71,11,62),(195,80,1,1),(196,75,2,10),(197,NULL,3,0),(198,NULL,4,0),(199,NULL,5,0),(200,NULL,6,0),(201,NULL,7,0),(202,NULL,8,0),(203,NULL,9,0),(204,10,16,1),(205,106,12,1),(206,87,13,1),(207,59,63,20),(208,36,79,1),(209,116,23,47),(210,36,77,1),(211,36,68,1),(212,36,75,1),(213,36,74,1),(214,36,73,1),(215,56,11,30),(216,61,69,1),(217,36,70,1),(218,36,72,1),(219,36,76,1),(220,36,67,1),(221,36,66,1),(222,36,65,1),(223,36,64,1),(224,NULL,80,0),(225,36,53,1),(226,36,61,1),(227,57,60,101),(228,54,16,40),(229,36,58,1),(230,36,57,1),(231,36,56,1),(232,36,55,1),(233,36,54,1),(234,36,62,1),(235,36,52,1),(236,36,59,1),(237,36,50,1),(238,36,49,1),(239,36,48,1),(240,36,47,1),(241,36,46,1),(242,36,45,1),(243,36,44,1),(244,36,43,1),(245,36,42,1),(246,36,41,1),(247,36,40,1),(248,36,39,1),(249,36,38,1),(250,36,37,1),(251,36,36,1),(252,36,35,1),(253,36,34,1),(254,36,33,1),(255,NULL,32,0),(256,NULL,30,0),(257,36,3,1),(258,36,15,1),(259,36,10,1),(260,36,27,1),(261,36,26,1),(262,36,25,1),(263,36,24,1),(264,36,78,1),(265,36,13,1),(266,36,8,1),(267,36,7,1),(268,36,6,1),(269,36,18,1),(270,36,71,1),(271,36,17,1),(272,36,51,1),(273,36,14,1),(274,115,22,44),(275,114,29,8),(276,36,9,1),(277,113,28,50),(278,112,31,49),(279,111,21,46),(280,110,20,42),(281,109,19,37),(282,60,5,4),(283,58,4,88),(284,52,12,17),(285,53,2,107),(286,99,1,382),(287,NULL,30,0),(288,123,14,20),(289,124,15,27),(290,125,16,7);
/*!40000 ALTER TABLE `inventory_slot` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `item`
--

DROP TABLE IF EXISTS `item`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `item` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `NAME` varchar(1000) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `ICON_NAME` varchar(1000) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `IS_DEFAULT_ITEM` tinyint NOT NULL,
  `item_type` varchar(1000) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `ATTACK` int DEFAULT NULL,
  `HEALTH` int DEFAULT NULL,
  `DEFENCE` int DEFAULT NULL,
  `ROTATION` int DEFAULT NULL,
  `SPEED` int DEFAULT NULL,
  `VISIBILITY` int DEFAULT NULL,
  `CANNON_RELOAD_SPEED` int DEFAULT NULL,
  `CRIT_CHANCE` int NOT NULL DEFAULT '0',
  `CANNON_FORCE` int NOT NULL DEFAULT '0',
  `DROP_CHANCE` int DEFAULT NULL,
  `MAX_LOOT_QUANTITY` float DEFAULT NULL,
  `crafting_exp` int DEFAULT NULL,
  `STACKABLE` tinyint(1) DEFAULT NULL,
  `ENERGY` int NOT NULL DEFAULT (0),
  `MAX_HEALTH` int NOT NULL DEFAULT '0',
  `MAX_ENERGY` int NOT NULL DEFAULT '0',
  `OVERTIME` tinyint(1) NOT NULL DEFAULT '0',
  `BUFF_DURATION` float NOT NULL DEFAULT '0',
  `COOLDOWN` float NOT NULL DEFAULT '0',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=52 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `item`
--

LOCK TABLES `item` WRITE;
/*!40000 ALTER TABLE `item` DISABLE KEYS */;
INSERT INTO `item` VALUES (1,'Wood log','wood.png',0,'resource',0,0,0,0,0,0,0,0,0,200,50,NULL,1,0,0,0,0,0,0),(2,'Wood plank','WoodPlank.png',0,'general',0,0,0,0,0,0,0,0,0,10,20,5,1,0,0,0,0,0,0),(3,'Gold crows nest','crows_nest_gold.png',0,'crows_nest',0,0,0,0,0,0,0,0,0,10,NULL,NULL,0,0,0,0,0,0,0),(4,'Bow sprite','bow_sprite.png',0,'bow_sprite',0,0,0,0,0,0,0,0,0,10,NULL,NULL,0,0,0,0,0,0,0),(5,'Jib sail','jib_sail.png',0,'jib_sail',0,0,0,0,0,0,0,0,0,10,NULL,NULL,0,0,0,0,0,0,0),(6,'Fore sail','fore_sail.png',0,'fore_sail',0,0,0,0,0,0,0,0,0,10,NULL,NULL,0,0,0,0,0,0,0),(7,'Rudder','rudder.png',0,'rudder',0,0,0,0,0,0,0,0,0,10,NULL,NULL,0,0,0,0,0,0,0),(8,'Keel','keel.png',0,'keel',0,0,0,0,0,0,0,0,0,10,NULL,NULL,0,0,0,0,0,0,0),(9,'Hull','hull.png',0,'hull',0,0,0,0,0,0,0,0,0,10,NULL,NULL,0,0,0,0,0,0,0),(10,'Main mast','main_mast.png',0,'main_mast',0,0,0,0,0,0,0,0,0,10,NULL,NULL,0,0,0,0,0,0,0),(11,'Rigging','rigging.png',0,'rigging',0,0,0,0,1,0,0,0,0,10,NULL,NULL,0,0,0,0,0,0,0),(12,'Captains cabin','captains_cabin.png',0,'captains_cabin',0,0,0,0,0,0,0,0,0,10,NULL,NULL,0,0,0,0,0,0,0),(13,'Main sail','main_sail.png',0,'main_sail',0,0,0,0,0,0,0,0,0,10,NULL,NULL,0,0,0,0,0,0,0),(14,'Crows nest','crows_nest.png',0,'crows_nest',0,0,0,0,0,0,0,0,0,10,NULL,NULL,0,0,0,0,0,0,0),(15,'Legs','legs.png',0,'legs',0,0,0,0,0,0,0,0,0,10,NULL,NULL,0,0,0,0,0,0,0),(16,'Helmet','helmet.png',0,'helmet',0,0,0,0,0,0,0,0,0,10,NULL,NULL,0,0,0,0,0,0,0),(17,'Top','top.png',0,'top',0,0,0,0,0,0,0,0,0,10,NULL,NULL,0,0,0,0,0,0,0),(18,'Hands','hands.png',0,'hands',0,0,0,0,0,0,0,0,0,10,NULL,NULL,0,0,0,0,0,0,0),(19,'Boots','boots.png',0,'boots',0,0,0,0,0,0,0,0,0,10,NULL,NULL,0,0,0,0,0,0,0),(20,'Wooden Sword','Wooden Sword',0,'hands',10,0,0,0,0,0,0,2,0,10,NULL,NULL,0,0,0,0,0,0,0),(21,'Iron Sword','Iron Sword',0,'hands',15,0,0,0,0,0,0,2,0,40,NULL,NULL,0,0,0,0,0,0,0),(22,'Steel Sword','Steel Sword',0,'hands',20,0,0,0,0,0,0,3,0,30,NULL,NULL,0,0,0,0,0,0,0),(23,'Edged Sword','Edged Sword',0,'hands',25,0,0,0,0,0,0,5,0,30,NULL,NULL,0,0,0,0,0,0,0),(24,'Pirate Sword','Pirate Sword',0,'hands',30,0,0,0,0,0,0,7,0,15,NULL,NULL,0,0,0,0,0,0,0),(25,'Small Dagger','Small Dagger',0,'hands',10,0,0,0,0,0,0,10,0,35,NULL,NULL,0,0,0,0,0,0,0),(26,'Axe','Axe',0,'hands',30,0,0,0,0,0,0,2,0,25,NULL,NULL,0,0,0,0,0,0,0),(27,'Battleaxe','Battleaxe',0,'hands',35,0,0,0,0,0,0,4,0,20,NULL,NULL,0,0,0,0,0,0,0),(28,'Iron Mace','Iron Mace',0,'hands',30,0,0,0,0,0,0,2,0,30,NULL,NULL,0,0,0,0,0,0,0),(29,'Mace','Mace',0,'hands',15,0,0,0,0,0,0,2,0,45,NULL,NULL,0,0,0,0,0,0,0),(30,'Iron ore','Iron Ore',0,'resource',0,0,0,0,0,0,0,0,0,NULL,NULL,NULL,1,0,0,0,0,0,0),(31,'Gold ore','Gold Ore',0,'resource',0,0,0,0,0,0,0,0,0,NULL,NULL,NULL,1,0,0,0,0,0,0),(35,'Coal','Coal',0,'resource',0,0,0,0,0,0,0,0,0,NULL,NULL,NULL,1,0,0,0,0,0,0),(36,'Silver ore','Silver Ore',0,'resource',0,0,0,0,0,0,0,0,0,NULL,NULL,NULL,1,0,0,0,0,0,0),(37,'Tin ore','Tin Ore',0,'resource',0,0,0,0,0,0,0,0,0,NULL,NULL,NULL,1,0,0,0,0,0,0),(38,'Water','Water',0,'cooking',0,0,0,0,0,0,0,0,0,NULL,NULL,NULL,1,0,0,0,0,0,0),(39,'Flour','Flour',0,'cooking',0,0,0,0,0,0,0,0,0,NULL,NULL,NULL,1,0,0,0,0,0,0),(40,'Egg','Egg',0,'cooking',0,0,0,0,0,0,0,0,0,NULL,NULL,NULL,1,0,0,0,0,0,0),(41,'Bread','Bread',0,'cooking',0,0,0,0,0,0,0,0,0,NULL,NULL,10,1,0,0,0,0,0,0),(42,'Milk','Milk',0,'cooking',0,0,0,0,0,0,0,0,0,NULL,NULL,NULL,1,0,0,0,0,0,0),(43,'Cheese','Cheese',0,'cooking',0,0,0,0,0,0,0,0,0,NULL,NULL,10,1,0,0,0,0,0,0),(44,'Health Scroll','scroll_health',0,'scroll',0,0,0,0,0,0,0,0,0,NULL,NULL,NULL,1,0,50,0,0,1,0),(45,'Crit scroll','scroll_crit',0,'scroll',0,0,0,0,0,0,0,20,0,NULL,NULL,NULL,1,0,0,0,0,1,0),(46,'Attack scroll','scroll_att',0,'scroll',10,0,0,0,0,0,0,0,0,NULL,NULL,NULL,1,0,0,0,0,1,0),(47,'Defence scroll','scroll_defence',0,'scroll',0,0,20,0,0,0,0,0,0,NULL,NULL,NULL,1,0,0,0,0,1,0),(48,'Speed scroll','scroll_speed',0,'scroll',0,0,0,0,1,0,0,0,0,NULL,NULL,NULL,1,0,0,0,0,1,0),(49,'Health potion','potion_health',0,'potion',0,100,0,0,0,0,0,0,0,NULL,NULL,NULL,1,0,0,0,0,0,0.1),(50,'Energy overtime potion','potion_energy_overtime',0,'potion',0,0,0,0,0,0,0,0,0,NULL,NULL,NULL,1,5,0,0,1,1,0),(51,'Energy potion','potion_energy',0,'potion',0,0,0,0,0,0,0,0,0,NULL,NULL,NULL,1,50,0,0,0,0,0.1);
/*!40000 ALTER TABLE `item` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `item_category`
--

DROP TABLE IF EXISTS `item_category`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `item_category` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `CATEGORY_ID` bigint NOT NULL,
  `ITEM_ID` int NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `CATEGORY_ID` (`CATEGORY_ID`),
  KEY `ITEM_ID` (`ITEM_ID`),
  CONSTRAINT `item_category_ibfk_1` FOREIGN KEY (`CATEGORY_ID`) REFERENCES `category` (`ID`),
  CONSTRAINT `item_category_ibfk_2` FOREIGN KEY (`ITEM_ID`) REFERENCES `item` (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=138 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `item_category`
--

LOCK TABLES `item_category` WRITE;
/*!40000 ALTER TABLE `item_category` DISABLE KEYS */;
INSERT INTO `item_category` VALUES (1,1,1),(2,1,2),(3,4,3),(4,4,4),(5,4,5),(6,4,6),(7,4,7),(8,4,8),(9,4,9),(10,4,10),(11,4,11),(12,4,12),(13,4,13),(14,4,14),(15,3,15),(16,3,16),(17,3,17),(18,3,18),(19,3,19),(20,2,20),(21,2,21),(22,2,22),(23,2,23),(24,2,24),(25,2,25),(26,2,26),(27,2,27),(28,2,28),(29,2,29),(30,2,30),(31,5,42),(32,5,43),(64,1,31),(65,9,31),(66,1,35),(67,9,35),(68,1,36),(69,9,36),(70,1,37),(71,9,37),(79,1,42),(80,1,43),(82,1,38),(83,5,38),(84,1,39),(85,5,39),(86,1,40),(87,5,40),(88,1,41),(89,5,42),(90,10,1),(91,10,2),(92,10,3),(93,10,4),(94,10,5),(95,10,6),(96,10,7),(97,10,8),(98,10,9),(99,10,10),(100,10,11),(101,10,12),(102,10,13),(103,10,14),(104,10,15),(105,10,16),(106,10,17),(107,10,18),(108,10,19),(109,10,20),(110,10,21),(111,10,22),(112,10,23),(113,10,24),(114,10,25),(115,10,26),(116,10,27),(117,10,28),(118,10,29),(119,10,30),(120,10,31),(121,10,35),(122,10,36),(123,10,37),(124,10,38),(125,10,39),(126,10,40),(127,10,41),(128,10,42),(129,10,43),(130,11,44),(131,11,45),(132,11,46),(133,11,47),(134,11,48),(135,11,49),(136,11,50),(137,11,51);
/*!40000 ALTER TABLE `item_category` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `npc_base_stats`
--

DROP TABLE IF EXISTS `npc_base_stats`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `npc_base_stats` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `LEVEL` int NOT NULL,
  `ATTACK` int NOT NULL DEFAULT '0',
  `HEALTH` int NOT NULL DEFAULT '0',
  `DEFENCE` int NOT NULL DEFAULT '0',
  `rotation` float NOT NULL DEFAULT '0',
  `SPEED` int NOT NULL DEFAULT '0',
  `VISIBILITY` int NOT NULL DEFAULT '0',
  `CANNON_RELOAD_SPEED` int NOT NULL DEFAULT '0',
  `CRIT_CHANCE` int NOT NULL DEFAULT '0',
  `cannon_force` int NOT NULL DEFAULT '0',
  `NPC_TYPE_ID` bigint DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `NPC_TYPE_ID` (`NPC_TYPE_ID`),
  CONSTRAINT `npc_base_stats_ibfk_1` FOREIGN KEY (`NPC_TYPE_ID`) REFERENCES `npc_type` (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `npc_base_stats`
--

LOCK TABLES `npc_base_stats` WRITE;
/*!40000 ALTER TABLE `npc_base_stats` DISABLE KEYS */;
INSERT INTO `npc_base_stats` VALUES (1,1,50,300,10,1,3,20,2,10,800,1),(2,1,50,1000,20,0,4,0,0,10,0,2);
/*!40000 ALTER TABLE `npc_base_stats` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `npc_spawn`
--

DROP TABLE IF EXISTS `npc_spawn`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `npc_spawn` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `NPC_TYPE` bigint NOT NULL,
  `X` float NOT NULL,
  `Y` float NOT NULL,
  `Z` float NOT NULL,
  `Y_ROT` float NOT NULL,
  `GAME_OBJECT_TYPE` int NOT NULL,
  `AGGRO_RANGE` float NOT NULL DEFAULT '0',
  `ENABLED` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`ID`),
  KEY `NPC_TYPE` (`NPC_TYPE`),
  CONSTRAINT `npc_spawn_ibfk_1` FOREIGN KEY (`NPC_TYPE`) REFERENCES `npc_type` (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `npc_spawn`
--

LOCK TABLES `npc_spawn` WRITE;
/*!40000 ALTER TABLE `npc_spawn` DISABLE KEYS */;
INSERT INTO `npc_spawn` VALUES (3,2,57,1,42,0,11,10,1),(4,1,0,0,0,0,1,40,0);
/*!40000 ALTER TABLE `npc_spawn` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `npc_type`
--

DROP TABLE IF EXISTS `npc_type`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `npc_type` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `NPC_TYPE` varchar(1000) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `npc_type`
--

LOCK TABLES `npc_type` WRITE;
/*!40000 ALTER TABLE `npc_type` DISABLE KEYS */;
INSERT INTO `npc_type` VALUES (1,'ship'),(2,'dragon');
/*!40000 ALTER TABLE `npc_type` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `player`
--

DROP TABLE IF EXISTS `player`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `player` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `USERNAME` varchar(1000) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `LEVEL` int DEFAULT NULL,
  `EXPERIENCE` int DEFAULT NULL,
  `X_SHIP` float DEFAULT NULL,
  `Z_SHIP` float DEFAULT NULL,
  `Y_SHIP` float DEFAULT NULL,
  `Y_ROT_SHIP` float DEFAULT NULL,
  `PASSWORD` varchar(100) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `X_PLAYER` float DEFAULT NULL,
  `Y_PLAYER` float DEFAULT NULL,
  `Z_PLAYER` float DEFAULT NULL,
  `IS_ON_SHIP` tinyint(1) DEFAULT NULL,
  `Y_ROT_PLAYER` float DEFAULT NULL,
  `GOLD` float DEFAULT '0',
  `DEAD` tinyint(1) DEFAULT (false),
  `SUNK` tinyint(1) NOT NULL DEFAULT '0',
  `Y_ROT_CHILD` float DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `player`
--

LOCK TABLES `player` WRITE;
/*!40000 ALTER TABLE `player` DISABLE KEYS */;
INSERT INTO `player` VALUES (1,'player1',1,0,36.9863,79.9994,0.2871,262.588,'',37.277,0.323864,63.8291,0,354.181,31697,0,0,24.9117),(2,'player2',1,0,73.8614,46.3795,0.334343,180.428,'',34.9106,0.171446,66.5311,0,185.8,40720,0,0,185.8),(3,'player3',1,0,47.9244,80.9973,0.272637,91.0605,'',34.6222,0.971226,49.1339,1,0.0449949,50000,0,0,NULL);
/*!40000 ALTER TABLE `player` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `player_base_stats`
--

DROP TABLE IF EXISTS `player_base_stats`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `player_base_stats` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `LEVEL` int NOT NULL,
  `ATTACK` int NOT NULL DEFAULT '0',
  `HEALTH` int NOT NULL DEFAULT '0',
  `DEFENCE` int NOT NULL DEFAULT '0',
  `SPEED` int NOT NULL DEFAULT '0',
  `CRIT_CHANCE` int NOT NULL DEFAULT '0',
  `ENERGY` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `player_base_stats`
--

LOCK TABLES `player_base_stats` WRITE;
/*!40000 ALTER TABLE `player_base_stats` DISABLE KEYS */;
INSERT INTO `player_base_stats` VALUES (2,1,100,200,50,4,20,100);
/*!40000 ALTER TABLE `player_base_stats` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `player_equipment`
--

DROP TABLE IF EXISTS `player_equipment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `player_equipment` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `ITEM_ID` int DEFAULT NULL,
  `PLAYER_ID` int NOT NULL,
  `ITEM_TYPE` varchar(1000) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `ITEM_ID` (`ITEM_ID`),
  KEY `PLAYER_ID` (`PLAYER_ID`),
  CONSTRAINT `player_equipment_ibfk_1` FOREIGN KEY (`ITEM_ID`) REFERENCES `player_item` (`ID`),
  CONSTRAINT `player_equipment_ibfk_2` FOREIGN KEY (`PLAYER_ID`) REFERENCES `player` (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `player_equipment`
--

LOCK TABLES `player_equipment` WRITE;
/*!40000 ALTER TABLE `player_equipment` DISABLE KEYS */;
INSERT INTO `player_equipment` VALUES (1,NULL,1,'hands'),(2,105,2,'hands'),(3,NULL,1,'legs'),(4,63,2,'resource');
/*!40000 ALTER TABLE `player_equipment` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `player_item`
--

DROP TABLE IF EXISTS `player_item`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `player_item` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `ITEM_ID` int NOT NULL,
  `PLAYER_ID` int NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `ITEM_ID` (`ITEM_ID`),
  KEY `PLAYER_ID` (`PLAYER_ID`),
  CONSTRAINT `player_item_ibfk_1` FOREIGN KEY (`ITEM_ID`) REFERENCES `item` (`ID`),
  CONSTRAINT `player_item_ibfk_2` FOREIGN KEY (`PLAYER_ID`) REFERENCES `player` (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=126 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `player_item`
--

LOCK TABLES `player_item` WRITE;
/*!40000 ALTER TABLE `player_item` DISABLE KEYS */;
INSERT INTO `player_item` VALUES (5,1,1),(9,2,1),(10,3,1),(11,4,1),(12,5,1),(13,6,1),(14,7,1),(15,8,1),(16,9,1),(17,10,1),(18,11,1),(19,12,1),(20,13,1),(21,14,1),(22,20,1),(23,29,1),(24,15,1),(25,5,1),(26,1,1),(27,1,1),(28,1,1),(29,1,1),(30,1,1),(31,1,1),(32,1,1),(33,1,1),(34,1,1),(35,29,1),(36,21,1),(37,18,1),(38,21,1),(39,28,1),(40,25,1),(41,6,1),(42,29,1),(43,25,1),(44,28,1),(45,7,2),(46,29,2),(51,1,2),(52,30,1),(53,31,1),(54,35,1),(55,36,1),(56,37,1),(57,38,1),(58,39,1),(59,40,1),(60,41,1),(61,42,1),(62,43,1),(63,35,2),(64,40,2),(65,39,2),(66,31,2),(67,30,2),(68,42,2),(69,36,2),(70,38,2),(71,41,2),(72,35,3),(73,40,3),(74,39,3),(75,31,3),(76,30,3),(77,42,3),(78,36,3),(79,37,3),(80,38,3),(81,15,2),(82,2,2),(85,15,1),(86,25,2),(87,18,2),(88,25,1),(89,21,1),(90,1,1),(91,1,1),(92,1,1),(93,1,1),(94,1,1),(95,1,1),(96,1,1),(97,1,1),(98,1,1),(99,1,1),(100,29,1),(101,29,1),(102,29,2),(103,29,1),(104,29,1),(105,21,2),(106,21,2),(107,21,2),(108,37,2),(109,46,1),(110,45,1),(111,47,1),(112,50,1),(113,51,1),(114,49,1),(115,44,1),(116,48,1),(123,49,2),(124,44,2),(125,48,2);
/*!40000 ALTER TABLE `player_item` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `player_skill_level`
--

DROP TABLE IF EXISTS `player_skill_level`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `player_skill_level` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `PLAYER_ID` int NOT NULL,
  `SKILL_LEVEL_ID` bigint NOT NULL,
  `EXPERIENCE` int NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `PLAYER_ID` (`PLAYER_ID`),
  KEY `SKILL_LEVEL_ID` (`SKILL_LEVEL_ID`),
  CONSTRAINT `player_skill_level_ibfk_1` FOREIGN KEY (`PLAYER_ID`) REFERENCES `player` (`ID`),
  CONSTRAINT `player_skill_level_ibfk_2` FOREIGN KEY (`SKILL_LEVEL_ID`) REFERENCES `skill_level` (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=107 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `player_skill_level`
--

LOCK TABLES `player_skill_level` WRITE;
/*!40000 ALTER TABLE `player_skill_level` DISABLE KEYS */;
INSERT INTO `player_skill_level` VALUES (2,2,1,759),(3,3,1,0),(5,2,6,70),(6,3,6,0),(84,1,11,0),(85,1,18,0),(86,1,25,0),(88,1,39,0),(89,1,46,0),(91,2,11,0),(92,2,18,0),(93,2,25,0),(94,2,32,740),(95,2,39,0),(96,2,46,0),(98,3,11,0),(99,3,18,0),(100,3,25,0),(101,3,32,0),(102,3,39,0),(103,3,46,0),(104,1,33,1440),(105,1,4,13105),(106,1,7,2149);
/*!40000 ALTER TABLE `player_skill_level` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `recipe`
--

DROP TABLE IF EXISTS `recipe`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `recipe` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `ITEM_ID` int NOT NULL,
  `RECIPE_NAME` varchar(1000) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `TIME_TO_CRAFT` float NOT NULL,
  `SKILL_ID` bigint DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `ITEM_ID` (`ITEM_ID`),
  KEY `SKILL_ID` (`SKILL_ID`),
  CONSTRAINT `recipe_ibfk_1` FOREIGN KEY (`ITEM_ID`) REFERENCES `item` (`ID`),
  CONSTRAINT `recipe_ibfk_2` FOREIGN KEY (`SKILL_ID`) REFERENCES `skill` (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `recipe`
--

LOCK TABLES `recipe` WRITE;
/*!40000 ALTER TABLE `recipe` DISABLE KEYS */;
INSERT INTO `recipe` VALUES (1,41,'Bread',5,6),(2,43,'Cheese',5,6),(3,2,'Wood plank',5,5);
/*!40000 ALTER TABLE `recipe` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `recipe_item_requirements`
--

DROP TABLE IF EXISTS `recipe_item_requirements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `recipe_item_requirements` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `RECIPE_ID` bigint NOT NULL,
  `ITEM_ID` int NOT NULL,
  `QUANTITY` int NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `ITEM_ID` (`ITEM_ID`),
  KEY `RECIPE_ID` (`RECIPE_ID`),
  CONSTRAINT `recipe_item_requirements_ibfk_1` FOREIGN KEY (`ITEM_ID`) REFERENCES `item` (`ID`),
  CONSTRAINT `recipe_item_requirements_ibfk_2` FOREIGN KEY (`RECIPE_ID`) REFERENCES `recipe` (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `recipe_item_requirements`
--

LOCK TABLES `recipe_item_requirements` WRITE;
/*!40000 ALTER TABLE `recipe_item_requirements` DISABLE KEYS */;
INSERT INTO `recipe_item_requirements` VALUES (1,1,38,1),(2,1,39,3),(4,2,42,2),(5,3,1,1);
/*!40000 ALTER TABLE `recipe_item_requirements` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `recipe_skill_requirements`
--

DROP TABLE IF EXISTS `recipe_skill_requirements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `recipe_skill_requirements` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `RECIPE_ID` bigint NOT NULL,
  `SKILL_LEVEL_ID` bigint NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `RECIPE_ID` (`RECIPE_ID`),
  KEY `SKILL_LEVEL_ID` (`SKILL_LEVEL_ID`),
  CONSTRAINT `recipe_skill_requirements_ibfk_1` FOREIGN KEY (`RECIPE_ID`) REFERENCES `recipe` (`ID`),
  CONSTRAINT `recipe_skill_requirements_ibfk_2` FOREIGN KEY (`SKILL_LEVEL_ID`) REFERENCES `skill_level` (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `recipe_skill_requirements`
--

LOCK TABLES `recipe_skill_requirements` WRITE;
/*!40000 ALTER TABLE `recipe_skill_requirements` DISABLE KEYS */;
INSERT INTO `recipe_skill_requirements` VALUES (1,1,32),(2,2,33),(3,3,25);
/*!40000 ALTER TABLE `recipe_skill_requirements` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `resource`
--

DROP TABLE IF EXISTS `resource`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `resource` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `ITEM_ID` int DEFAULT NULL,
  `RESOURCE_TYPE` int NOT NULL,
  `RESOURCE_HP` float NOT NULL,
  `RESOURCE_COUNT` int NOT NULL,
  `EXPERIENCE` float NOT NULL,
  `SKILL_TYPE` int DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `ITEM_ID` (`ITEM_ID`),
  CONSTRAINT `resource_ibfk_1` FOREIGN KEY (`ITEM_ID`) REFERENCES `item` (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `resource`
--

LOCK TABLES `resource` WRITE;
/*!40000 ALTER TABLE `resource` DISABLE KEYS */;
INSERT INTO `resource` VALUES (1,1,2,1000,10,50,1),(2,31,4,500,5,50,2),(3,30,3,1500,20,50,2),(4,35,5,3000,30,50,2),(5,36,6,1000,10,50,2),(6,37,7,1500,15,50,2);
/*!40000 ALTER TABLE `resource` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `resource_spawn`
--

DROP TABLE IF EXISTS `resource_spawn`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `resource_spawn` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `RESOURCE_ID` bigint NOT NULL,
  `RESPAWN_TIME` float NOT NULL,
  `X` float NOT NULL,
  `Y` float NOT NULL,
  `Z` float NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `RESOURCE_ID` (`RESOURCE_ID`),
  CONSTRAINT `resource_spawn_ibfk_1` FOREIGN KEY (`RESOURCE_ID`) REFERENCES `resource` (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `resource_spawn`
--

LOCK TABLES `resource_spawn` WRITE;
/*!40000 ALTER TABLE `resource_spawn` DISABLE KEYS */;
INSERT INTO `resource_spawn` VALUES (1,1,15,32,0.9,46),(2,2,10,28,0.8,51),(3,3,10,29,0.6,48),(4,4,15,22,0.9,54),(5,5,15,21,0.7,50),(6,6,15,18,1,53);
/*!40000 ALTER TABLE `resource_spawn` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ship_base_stats`
--

DROP TABLE IF EXISTS `ship_base_stats`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ship_base_stats` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `LEVEL` int NOT NULL,
  `ATTACK` int NOT NULL DEFAULT '0',
  `HEALTH` int NOT NULL DEFAULT '0',
  `DEFENCE` int NOT NULL DEFAULT '0',
  `rotation` float DEFAULT NULL,
  `SPEED` int NOT NULL DEFAULT '0',
  `VISIBILITY` int NOT NULL DEFAULT '0',
  `CANNON_RELOAD_SPEED` int NOT NULL DEFAULT '0',
  `CRIT_CHANCE` int NOT NULL DEFAULT '0',
  `CANNON_FORCE` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ship_base_stats`
--

LOCK TABLES `ship_base_stats` WRITE;
/*!40000 ALTER TABLE `ship_base_stats` DISABLE KEYS */;
INSERT INTO `ship_base_stats` VALUES (1,1,100,500,20,0.5,5,50,1,20,600);
/*!40000 ALTER TABLE `ship_base_stats` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ship_equipment`
--

DROP TABLE IF EXISTS `ship_equipment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ship_equipment` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `ITEM_ID` int DEFAULT NULL,
  `PLAYER_ID` int NOT NULL,
  `ITEM_TYPE` varchar(1000) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `ITEM_ID` (`ITEM_ID`),
  KEY `PLAYER_ID` (`PLAYER_ID`),
  CONSTRAINT `ship_equipment_ibfk_1` FOREIGN KEY (`ITEM_ID`) REFERENCES `player_item` (`ID`),
  CONSTRAINT `ship_equipment_ibfk_2` FOREIGN KEY (`PLAYER_ID`) REFERENCES `player` (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ship_equipment`
--

LOCK TABLES `ship_equipment` WRITE;
/*!40000 ALTER TABLE `ship_equipment` DISABLE KEYS */;
INSERT INTO `ship_equipment` VALUES (1,NULL,1,'crows_nest'),(2,NULL,1,'bow_sprite'),(3,NULL,1,'keel'),(4,NULL,1,'rudder'),(5,NULL,1,'fore_sail'),(6,NULL,1,'main_mast'),(7,NULL,1,'rigging'),(8,19,1,'captains_cabin'),(9,20,1,'main_sail'),(10,12,1,'jib_sail'),(11,NULL,1,'hull'),(12,NULL,2,'rudder'),(13,53,1,'resource');
/*!40000 ALTER TABLE `ship_equipment` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `skill`
--

DROP TABLE IF EXISTS `skill`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `skill` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `SKILL_NAME` varchar(1000) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `ICON` varchar(1000) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `skill`
--

LOCK TABLES `skill` WRITE;
/*!40000 ALTER TABLE `skill` DISABLE KEYS */;
INSERT INTO `skill` VALUES (1,'Woodcutting','Woodcutting'),(2,'Mining','Mining'),(3,'Alchemy','Alchemy'),(4,'Armoring','Armoring'),(5,'Carpenting','Carpenting'),(6,'Cooking','Cooking'),(7,'Gathering','Gathering'),(8,'Smithing','Smithing');
/*!40000 ALTER TABLE `skill` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `skill_level`
--

DROP TABLE IF EXISTS `skill_level`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `skill_level` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `SKILL_ID` bigint DEFAULT NULL,
  `LVL` int NOT NULL,
  `modifier` int NOT NULL,
  `EXP_START` int NOT NULL,
  `EXP_END` int NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `SKILL_ID` (`SKILL_ID`),
  CONSTRAINT `skill_level_ibfk_1` FOREIGN KEY (`SKILL_ID`) REFERENCES `skill` (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=51 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `skill_level`
--

LOCK TABLES `skill_level` WRITE;
/*!40000 ALTER TABLE `skill_level` DISABLE KEYS */;
INSERT INTO `skill_level` VALUES (1,1,1,10,0,999),(2,1,2,12,1000,3999),(3,1,3,16,4000,9999),(4,1,4,22,10000,29999),(5,1,5,28,30000,99999),(6,2,1,5,0,999),(7,2,2,8,1000,3999),(8,2,3,12,4000,9999),(9,2,4,15,10000,29999),(10,2,5,18,30000,99999),(11,3,1,10,0,999),(12,3,2,12,1000,3999),(13,3,3,16,4000,9999),(14,3,4,22,10000,29999),(15,3,5,29,30000,99999),(18,4,1,10,0,999),(19,4,2,12,1000,3999),(20,4,3,16,4000,9999),(21,4,4,22,10000,29999),(22,4,5,29,30000,99999),(25,5,1,10,0,999),(26,5,2,12,1000,3999),(27,5,3,16,4000,9999),(28,5,4,22,10000,29999),(29,5,5,29,30000,99999),(32,6,1,10,0,999),(33,6,2,12,1000,3999),(34,6,3,16,4000,9999),(35,6,4,22,10000,29999),(36,6,5,29,30000,99999),(39,7,1,10,0,999),(40,7,2,12,1000,3999),(41,7,3,16,4000,9999),(42,7,4,22,10000,29999),(43,7,5,29,30000,99999),(46,8,1,10,0,999),(47,8,2,12,1000,3999),(48,8,3,16,4000,9999),(49,8,4,22,10000,29999),(50,8,5,29,30000,99999);
/*!40000 ALTER TABLE `skill_level` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `temporary_storage`
--

DROP TABLE IF EXISTS `temporary_storage`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `temporary_storage` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `ITEM_ID` int DEFAULT NULL,
  `QUANTITY` int DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `ITEM_ID` (`ITEM_ID`),
  CONSTRAINT `temporary_storage_ibfk_1` FOREIGN KEY (`ITEM_ID`) REFERENCES `player_item` (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `temporary_storage`
--

LOCK TABLES `temporary_storage` WRITE;
/*!40000 ALTER TABLE `temporary_storage` DISABLE KEYS */;
/*!40000 ALTER TABLE `temporary_storage` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `trade_broker`
--

DROP TABLE IF EXISTS `trade_broker`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `trade_broker` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `X` float NOT NULL,
  `Y` float NOT NULL,
  `Z` float NOT NULL,
  `Y_ROT` float NOT NULL,
  `GAME_OBJECT_TYPE` int DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `trade_broker`
--

LOCK TABLES `trade_broker` WRITE;
/*!40000 ALTER TABLE `trade_broker` DISABLE KEYS */;
INSERT INTO `trade_broker` VALUES (1,34,1,54,145,9);
/*!40000 ALTER TABLE `trade_broker` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `trade_broker_items`
--

DROP TABLE IF EXISTS `trade_broker_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `trade_broker_items` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `QUANTITY` int NOT NULL,
  `PRICE` float NOT NULL,
  `PLAYER_ID` int DEFAULT NULL,
  `ITEM_ID` int DEFAULT NULL,
  `PARENT_ID` bigint DEFAULT NULL,
  `SOLD` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`ID`),
  KEY `PLAYER_ID` (`PLAYER_ID`),
  KEY `ITEM_ID` (`ITEM_ID`),
  KEY `PARENT_ID` (`PARENT_ID`),
  CONSTRAINT `trade_broker_items_ibfk_2` FOREIGN KEY (`PLAYER_ID`) REFERENCES `player` (`ID`),
  CONSTRAINT `trade_broker_items_ibfk_3` FOREIGN KEY (`ITEM_ID`) REFERENCES `item` (`ID`),
  CONSTRAINT `trade_broker_items_ibfk_4` FOREIGN KEY (`PARENT_ID`) REFERENCES `trade_broker_items` (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=42 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `trade_broker_items`
--

LOCK TABLES `trade_broker_items` WRITE;
/*!40000 ALTER TABLE `trade_broker_items` DISABLE KEYS */;
INSERT INTO `trade_broker_items` VALUES (21,0,1000,2,30,NULL,1),(22,1,1000,2,30,21,1),(25,1,100000,2,2,NULL,0),(26,0,1,2,30,NULL,1),(27,3,1,2,30,26,1),(28,0,100,2,29,NULL,1),(29,1,100,2,29,28,1),(30,0,100,2,29,NULL,1),(31,1,100,2,29,30,1),(32,0,2500,1,21,NULL,1),(33,0,2500,1,21,NULL,1),(34,0,2500,1,21,NULL,1),(35,1,2500,1,21,32,1),(36,1,2500,1,21,34,1),(37,1,2500,1,21,33,1),(38,20,1,1,31,NULL,0),(39,2,50,1,43,NULL,0),(40,1,50,1,21,NULL,0),(41,1,2500,1,21,NULL,0);
/*!40000 ALTER TABLE `trade_broker_items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `trader`
--

DROP TABLE IF EXISTS `trader`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `trader` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `NAME` varchar(1000) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `X` float NOT NULL,
  `Y` float NOT NULL,
  `Z` float NOT NULL,
  `Y_ROT` float NOT NULL,
  `ITEM_RESPAWN_TIME` float DEFAULT NULL,
  `GAME_OBJECT_TYPE` int DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `trader`
--

LOCK TABLES `trader` WRITE;
/*!40000 ALTER TABLE `trader` DISABLE KEYS */;
INSERT INTO `trader` VALUES (1,'Joe',30,0.9,54,145,1,8);
/*!40000 ALTER TABLE `trader` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `trader_inventory`
--

DROP TABLE IF EXISTS `trader_inventory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `trader_inventory` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `TRADER_ID` bigint NOT NULL,
  `ITEM_ID` int NOT NULL,
  `QUANTITY` int NOT NULL,
  `SELL_PRICE` float NOT NULL,
  `BUY_PRICE` float NOT NULL DEFAULT '1',
  PRIMARY KEY (`ID`),
  KEY `TRADER_ID` (`TRADER_ID`),
  KEY `ITEM_ID` (`ITEM_ID`),
  CONSTRAINT `trader_inventory_ibfk_1` FOREIGN KEY (`TRADER_ID`) REFERENCES `trader` (`ID`),
  CONSTRAINT `trader_inventory_ibfk_2` FOREIGN KEY (`ITEM_ID`) REFERENCES `item` (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `trader_inventory`
--

LOCK TABLES `trader_inventory` WRITE;
/*!40000 ALTER TABLE `trader_inventory` DISABLE KEYS */;
INSERT INTO `trader_inventory` VALUES (1,1,30,100,5,1),(2,1,31,40,20,1),(3,1,35,100,2,1),(4,1,36,60,15,1),(5,1,37,100,5,1),(6,1,38,500,1,1),(7,1,39,300,3,1),(8,1,40,100,2,1),(9,1,42,100,4,1),(10,1,44,100,30,1),(11,1,45,100,30,1),(12,1,46,100,30,1),(13,1,47,100,30,1),(14,1,48,100,30,1),(15,1,49,100,30,1),(16,1,50,100,30,1),(17,1,51,100,30,1);
/*!40000 ALTER TABLE `trader_inventory` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2021-05-15 23:21:45
