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
-- Table `greedytown`.`user`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `greedytown`.`user` ;

CREATE TABLE IF NOT EXISTS `greedytown`.`user` (
  `user_index` BIGINT NOT NULL AUTO_INCREMENT,
  `user_email` VARCHAR(255) NULL,
  `user_password` VARCHAR(255) NULL,
  `user_nickname` VARCHAR(255) NULL,
  `user_money` BIGINT NULL,
  `user_clear_time` TIME NULL,
  PRIMARY KEY (`user_index`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `greedytown`.`friend_user_list`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `greedytown`.`friend_user_list` ;

CREATE TABLE IF NOT EXISTS `greedytown`.`friend_user_list` (
  `user_index_a` BIGINT NOT NULL,
  `user_index_b` BIGINT NOT NULL,
  PRIMARY KEY (`user_index_a`, `user_index_b`),
  INDEX `fk_friend_user_list_user1_idx` (`user_index_b` ASC) VISIBLE,
  CONSTRAINT `fk_friend_user_list_user`
    FOREIGN KEY (`user_index_a`)
    REFERENCES `greedytown`.`user` (`user_index`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_friend_user_list_user1`
    FOREIGN KEY (`user_index_b`)
    REFERENCES `greedytown`.`user` (`user_index`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `greedytown`.`message`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `greedytown`.`message` ;

CREATE TABLE IF NOT EXISTS `greedytown`.`message` (
  `message_index` BIGINT NOT NULL AUTO_INCREMENT,
  `message_from` BIGINT NOT NULL,
  `message_to` BIGINT NOT NULL,
  `message_content` VARCHAR(255) NULL,
  `message_write_time` DATE NULL,
  `message_check` TINYINT NULL,
  PRIMARY KEY (`message_index`),
  INDEX `fk_message_user1_idx` (`message_from` ASC) VISIBLE,
  INDEX `fk_message_user2_idx` (`message_to` ASC) VISIBLE,
  CONSTRAINT `fk_message_user1`
    FOREIGN KEY (`message_from`)
    REFERENCES `greedytown`.`user` (`user_index`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_message_user2`
    FOREIGN KEY (`message_to`)
    REFERENCES `greedytown`.`user` (`user_index`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `greedytown`.`achivements`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `greedytown`.`achivements` ;

CREATE TABLE IF NOT EXISTS `greedytown`.`achivements` (
  `achivements_index` BIGINT NOT NULL AUTO_INCREMENT,
  `achivements_content` VARCHAR(255) NULL,
  PRIMARY KEY (`achivements_index`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `greedytown`.`item`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `greedytown`.`item` ;

CREATE TABLE IF NOT EXISTS `greedytown`.`item` (
  `item_index` BIGINT NOT NULL AUTO_INCREMENT,
  `item_name` VARCHAR(255) NULL,
  `item_code` VARCHAR(255) NULL,
  `item_price` BIGINT NULL,
  `achivements_index` BIGINT NOT NULL,
  PRIMARY KEY (`item_index`),
  INDEX `fk_item_achivements1_idx` (`achivements_index` ASC) VISIBLE,
  CONSTRAINT `fk_item_achivements1`
    FOREIGN KEY (`achivements_index`)
    REFERENCES `greedytown`.`achivements` (`achivements_index`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `greedytown`.`item_user_list`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `greedytown`.`item_user_list` ;

CREATE TABLE IF NOT EXISTS `greedytown`.`item_user_list` (
  `user_index` BIGINT NOT NULL,
  `item_index` BIGINT NOT NULL,
  PRIMARY KEY (`user_index`, `item_index`),
  INDEX `fk_item_user_list_item1_idx` (`item_index` ASC) VISIBLE,
  CONSTRAINT `fk_item_user_list_user1`
    FOREIGN KEY (`user_index`)
    REFERENCES `greedytown`.`user` (`user_index`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_item_user_list_item1`
    FOREIGN KEY (`item_index`)
    REFERENCES `greedytown`.`item` (`item_index`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `greedytown`.`success_user_achievements`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `greedytown`.`success_user_achievements` ;

CREATE TABLE IF NOT EXISTS `greedytown`.`success_user_achievements` (
  `user_index` BIGINT NOT NULL,
  `achivements_index` BIGINT NOT NULL,
  PRIMARY KEY (`user_index`, `achivements_index`),
  INDEX `fk_success_user_achievements_achivements1_idx` (`achivements_index` ASC) VISIBLE,
  CONSTRAINT `fk_success_user_achievements_user1`
    FOREIGN KEY (`user_index`)
    REFERENCES `greedytown`.`user` (`user_index`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_success_user_achievements_achivements1`
    FOREIGN KEY (`achivements_index`)
    REFERENCES `greedytown`.`achivements` (`achivements_index`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `greedytown`.`wearing`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `greedytown`.`wearing` ;

CREATE TABLE IF NOT EXISTS `greedytown`.`wearing` (
  `wearing_index` BIGINT NOT NULL AUTO_INCREMENT,
  `user_index` BIGINT NOT NULL,
  `wearing_head` BIGINT NOT NULL,
  PRIMARY KEY (`wearing_index`),
  INDEX `fk_wearing_user1_idx` (`user_index` ASC) VISIBLE,
  INDEX `fk_wearing_item1_idx` (`wearing_head` ASC) VISIBLE,
  CONSTRAINT `fk_wearing_user1`
    FOREIGN KEY (`user_index`)
    REFERENCES `greedytown`.`user` (`user_index`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_wearing_item1`
    FOREIGN KEY (`wearing_head`)
    REFERENCES `greedytown`.`item` (`item_index`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
