-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
-- -----------------------------------------------------
-- Schema greedytown
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema greedytown
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `greedytown` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_as_cs ;
USE `greedytown` ;

-- -----------------------------------------------------
-- Table `greedytown`.`achievements`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `greedytown`.`achievements` (
  `achievements_seq` BIGINT NOT NULL,
  `achievements_name` VARCHAR(30) NULL DEFAULT NULL,
  `achievements_content` VARCHAR(200) NULL DEFAULT NULL,
  PRIMARY KEY (`achievements_seq`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_as_cs;


-- -----------------------------------------------------
-- Table `greedytown`.`user`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `greedytown`.`user` (
  `user_seq` BIGINT NOT NULL AUTO_INCREMENT,
  `user_email` VARCHAR(100) NOT NULL,
  `user_password` VARCHAR(100) NOT NULL,
  `user_nickname` VARCHAR(30) NOT NULL,
  `user_money` BIGINT NULL DEFAULT '0',
  `user_join_date` DATE NULL DEFAULT NULL,
  PRIMARY KEY (`user_seq`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_as_cs;


-- -----------------------------------------------------
-- Table `greedytown`.`friend_user_list`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `greedytown`.`friend_user_list` (
  `friend_seq` BIGINT NOT NULL,
  `friend_from` BIGINT NULL,
  `friend_to` BIGINT NULL,
  `friend_accept` TINYINT NULL DEFAULT '0',
  `friend_request_date` DATE NULL DEFAULT NULL,
  PRIMARY KEY (`friend_seq`),
  INDEX `fk_friend_user_list_user2_idx` (`friend_from` ASC) VISIBLE,
  INDEX `fk_friend_user_list_user1_idx` (`friend_to` ASC) VISIBLE,
  CONSTRAINT `fk_friend_user_list_user1`
    FOREIGN KEY (`friend_to`)
    REFERENCES `greedytown`.`user` (`user_seq`),
  CONSTRAINT `fk_friend_user_list_user2`
    FOREIGN KEY (`friend_from`)
    REFERENCES `greedytown`.`user` (`user_seq`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_as_cs;


-- -----------------------------------------------------
-- Table `greedytown`.`item_color`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `greedytown`.`item_color` (
  `item_color_seq` SMALLINT NOT NULL,
  `item_color_name` VARCHAR(30) NULL DEFAULT NULL,
  PRIMARY KEY (`item_color_seq`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_as_cs;


-- -----------------------------------------------------
-- Table `greedytown`.`item_type`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `greedytown`.`item_type` (
  `item_type_seq` SMALLINT NOT NULL,
  `item_type_name` VARCHAR(20) NULL DEFAULT NULL,
  PRIMARY KEY (`item_type_seq`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_as_cs;


-- -----------------------------------------------------
-- Table `greedytown`.`item`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `greedytown`.`item` (
  `item_seq` BIGINT NOT NULL,
  `item_name` VARCHAR(200) NULL DEFAULT NULL,
  `item_price` INT NULL DEFAULT NULL,
  `item_type_seq` SMALLINT NOT NULL,
  `item_color_seq` SMALLINT NOT NULL,
  `achievements_seq` BIGINT NULL DEFAULT NULL,
  PRIMARY KEY (`item_seq`),
  INDEX `fk_item_item_type1_idx` (`item_type_seq` ASC) VISIBLE,
  INDEX `fk_item_item_color1_idx` (`item_color_seq` ASC) VISIBLE,
  INDEX `fk_item_achievements1_idx` (`achievements_seq` ASC) VISIBLE,
  CONSTRAINT `fk_item_achievements1`
    FOREIGN KEY (`achievements_seq`)
    REFERENCES `greedytown`.`achievements` (`achievements_seq`),
  CONSTRAINT `fk_item_item_color1`
    FOREIGN KEY (`item_color_seq`)
    REFERENCES `greedytown`.`item_color` (`item_color_seq`),
  CONSTRAINT `fk_item_item_type1`
    FOREIGN KEY (`item_type_seq`)
    REFERENCES `greedytown`.`item_type` (`item_type_seq`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_as_cs;


-- -----------------------------------------------------
-- Table `greedytown`.`item_user_list`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `greedytown`.`item_user_list` (
  `user_seq` BIGINT NOT NULL,
  `item_seq` BIGINT NOT NULL,
  PRIMARY KEY (`user_seq`, `item_seq`),
  INDEX `fk_item_user_list_user1_idx` (`user_seq` ASC) VISIBLE,
  INDEX `fk_item_user_list_item1` (`item_seq` ASC) VISIBLE,
  CONSTRAINT `fk_item_user_list_item1`
    FOREIGN KEY (`item_seq`)
    REFERENCES `greedytown`.`item` (`item_seq`),
  CONSTRAINT `fk_item_user_list_user1`
    FOREIGN KEY (`user_seq`)
    REFERENCES `greedytown`.`user` (`user_seq`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_as_cs;


-- -----------------------------------------------------
-- Table `greedytown`.`message`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `greedytown`.`message` (
  `message_seq` BIGINT NOT NULL,
  `message_from` BIGINT NOT NULL,
  `message_to` BIGINT NOT NULL,
  `message_content` VARCHAR(400) NULL DEFAULT NULL,
  `message_write_time` DATE NULL DEFAULT NULL,
  `message_check` TINYINT NULL DEFAULT '0',
  PRIMARY KEY (`message_seq`),
  INDEX `fk_message_user1_idx` (`message_from` ASC) VISIBLE,
  INDEX `fk_message_user2_idx` (`message_to` ASC) VISIBLE,
  CONSTRAINT `fk_message_user1`
    FOREIGN KEY (`message_from`)
    REFERENCES `greedytown`.`user` (`user_seq`),
  CONSTRAINT `fk_message_user2`
    FOREIGN KEY (`message_to`)
    REFERENCES `greedytown`.`user` (`user_seq`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_as_cs;


-- -----------------------------------------------------
-- Table `greedytown`.`money_log`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `greedytown`.`money_log` (
  `money_log_seq` BIGINT NOT NULL,
  `money_log_time` DATETIME NULL DEFAULT NULL,
  `money_log_gameinfo` TINYINT NULL DEFAULT NULL,
  `money_log_iteminfo` BIGINT NULL DEFAULT NULL,
  `user_seq` BIGINT NOT NULL,
  PRIMARY KEY (`money_log_seq`),
  INDEX `fk_money_log_user1_idx` (`user_seq` ASC) VISIBLE,
  CONSTRAINT `fk_money_log_user1`
    FOREIGN KEY (`user_seq`)
    REFERENCES `greedytown`.`user` (`user_seq`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_as_cs;


-- -----------------------------------------------------
-- Table `greedytown`.`stat`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `greedytown`.`stat` (
  `user_seq` BIGINT NOT NULL,
  `user_clear_time` TIME NULL DEFAULT NULL,
  PRIMARY KEY (`user_seq`),
  CONSTRAINT `fk_stat_user1`
    FOREIGN KEY (`user_seq`)
    REFERENCES `greedytown`.`user` (`user_seq`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_as_cs;


-- -----------------------------------------------------
-- Table `greedytown`.`success_user_achievements`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `greedytown`.`success_user_achievements` (
  `user_seq` BIGINT NOT NULL,
  `achievements_seq` BIGINT NOT NULL,
  `achievements_date` DATE NULL DEFAULT NULL,
  INDEX `fk_success_user_achievements_user1_idx` (`user_seq` ASC) VISIBLE,
  INDEX `fk_success_user_achievements_achievements1_idx` (`achievements_seq` ASC) VISIBLE,
  CONSTRAINT `fk_success_user_achievements_achievements1`
    FOREIGN KEY (`achievements_seq`)
    REFERENCES `greedytown`.`achievements` (`achievements_seq`),
  CONSTRAINT `fk_success_user_achievements_user1`
    FOREIGN KEY (`user_seq`)
    REFERENCES `greedytown`.`user` (`user_seq`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_as_cs;


-- -----------------------------------------------------
-- Table `greedytown`.`wearing`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `greedytown`.`wearing` (
  `wearing_seq` BIGINT NOT NULL,
  `user_seq` BIGINT NOT NULL,
  `item_seq` BIGINT NOT NULL,
  PRIMARY KEY (`wearing_seq`),
  INDEX `fk_wearing_item1_idx` (`item_seq` ASC) VISIBLE,
  INDEX `fk_wearing_user1_idx` (`user_seq` ASC) VISIBLE,
  CONSTRAINT `fk_wearing_item1`
    FOREIGN KEY (`item_seq`)
    REFERENCES `greedytown`.`item` (`item_seq`),
  CONSTRAINT `fk_wearing_user1`
    FOREIGN KEY (`user_seq`)
    REFERENCES `greedytown`.`user` (`user_seq`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_as_cs;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
