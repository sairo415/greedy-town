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
  `achievements_seq` BIGINT NOT NULL AUTO_INCREMENT,
  `achievements_content` VARCHAR(255) NULL DEFAULT NULL,
  PRIMARY KEY (`achievements_seq`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_as_cs;


-- -----------------------------------------------------
-- Table `greedytown`.`user`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `greedytown`.`user` (
  `user_seq` BIGINT NOT NULL AUTO_INCREMENT,
  `user_email` VARCHAR(100) NULL DEFAULT NULL,
  `user_join_date` DATE NULL DEFAULT NULL,
  `user_money` BIGINT NULL DEFAULT NULL,
  `user_nickname` VARCHAR(30) NULL DEFAULT NULL,
  `user_password` VARCHAR(100) NULL DEFAULT NULL,
  PRIMARY KEY (`user_seq`),
  UNIQUE INDEX `UK_j09k2v8lxofv2vecxu2hde9so` (`user_email` ASC) VISIBLE,
  UNIQUE INDEX `UK_cr59axqya8utby3j37qi341rm` (`user_nickname` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_as_cs;


-- -----------------------------------------------------
-- Table `greedytown`.`friend_user_list`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `greedytown`.`friend_user_list` (
  `friend_seq` BIGINT NOT NULL AUTO_INCREMENT,
  `friend_accept` BIT(1) NULL DEFAULT NULL,
  `friend_request_date` DATE NULL DEFAULT NULL,
  `friend_from` BIGINT NULL DEFAULT NULL,
  `friend_to` BIGINT NULL DEFAULT NULL,
  PRIMARY KEY (`friend_seq`),
  INDEX `FKrmeebf6sgr901o2jfjja3c4v7` (`friend_from` ASC) VISIBLE,
  INDEX `FK9am8xmouw5nm2wegeeb3olt3b` (`friend_to` ASC) VISIBLE,
  CONSTRAINT `FK9am8xmouw5nm2wegeeb3olt3b`
    FOREIGN KEY (`friend_to`)
    REFERENCES `greedytown`.`user` (`user_seq`),
  CONSTRAINT `FKrmeebf6sgr901o2jfjja3c4v7`
    FOREIGN KEY (`friend_from`)
    REFERENCES `greedytown`.`user` (`user_seq`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_as_cs;


-- -----------------------------------------------------
-- Table `greedytown`.`item_type`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `greedytown`.`item_type` (
  `item_type_seq` SMALLINT NOT NULL AUTO_INCREMENT,
  `item_type_name` VARCHAR(255) NULL DEFAULT NULL,
  PRIMARY KEY (`item_type_seq`))
ENGINE = InnoDB
AUTO_INCREMENT = 9
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_as_cs;


-- -----------------------------------------------------
-- Table `greedytown`.`item_color`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `greedytown`.`item_color` (
  `item_color_seq` SMALLINT NOT NULL AUTO_INCREMENT,
  `item_color_name` VARCHAR(255) NULL DEFAULT NULL,
  PRIMARY KEY (`item_color_seq`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_as_cs;


-- -----------------------------------------------------
-- Table `greedytown`.`item`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `greedytown`.`item` (
  `item_seq` BIGINT NOT NULL AUTO_INCREMENT,
  `item_image` VARCHAR(255) NULL DEFAULT NULL,
  `item_name` VARCHAR(255) NULL DEFAULT NULL,
  `item_price` INT NULL DEFAULT NULL,
  `ahichievements_seq` BIGINT NULL DEFAULT NULL,
  `item_color_seq` SMALLINT NULL DEFAULT NULL,
  `item_type_seq` SMALLINT NULL DEFAULT NULL,
  PRIMARY KEY (`item_seq`),
  INDEX `FKms3h1r2k65fmhsst1orgq0xyr` (`ahichievements_seq` ASC) VISIBLE,
  INDEX `FKreq8q4fka3egdv5i54bybui0d` (`item_color_seq` ASC) VISIBLE,
  INDEX `FKnl7evhv8wuds3unlihmd5t22b` (`item_type_seq` ASC) VISIBLE,
  CONSTRAINT `FKms3h1r2k65fmhsst1orgq0xyr`
    FOREIGN KEY (`ahichievements_seq`)
    REFERENCES `greedytown`.`achievements` (`achievements_seq`),
  CONSTRAINT `FKnl7evhv8wuds3unlihmd5t22b`
    FOREIGN KEY (`item_type_seq`)
    REFERENCES `greedytown`.`item_type` (`item_type_seq`),
  CONSTRAINT `FKreq8q4fka3egdv5i54bybui0d`
    FOREIGN KEY (`item_color_seq`)
    REFERENCES `greedytown`.`item_color` (`item_color_seq`))
ENGINE = InnoDB
AUTO_INCREMENT = 163
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_as_cs;


-- -----------------------------------------------------
-- Table `greedytown`.`item_user_list`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `greedytown`.`item_user_list` (
  `item_seq` BIGINT NOT NULL,
  `user_seq` BIGINT NOT NULL,
  PRIMARY KEY (`item_seq`, `user_seq`),
  INDEX `FKt3chj8omxu9gjkibq62aqpeeh` (`user_seq` ASC) VISIBLE,
  CONSTRAINT `FK7sliecbir12pmnacarxle5qc2`
    FOREIGN KEY (`item_seq`)
    REFERENCES `greedytown`.`item` (`item_seq`),
  CONSTRAINT `FKt3chj8omxu9gjkibq62aqpeeh`
    FOREIGN KEY (`user_seq`)
    REFERENCES `greedytown`.`user` (`user_seq`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_as_cs;


-- -----------------------------------------------------
-- Table `greedytown`.`message`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `greedytown`.`message` (
  `message_seq` BIGINT NOT NULL AUTO_INCREMENT,
  `message_check` BIT(1) NULL DEFAULT NULL,
  `message_content` VARCHAR(255) NULL DEFAULT NULL,
  `message_write_time` DATETIME(6) NULL DEFAULT NULL,
  `message_from` BIGINT NULL DEFAULT NULL,
  `message_to` BIGINT NULL DEFAULT NULL,
  PRIMARY KEY (`message_seq`),
  INDEX `FKoj95ydu8kwhsi8pn0bm33aqqs` (`message_from` ASC) VISIBLE,
  INDEX `FK5pe33n1lieyd5ochn3i8jm1sy` (`message_to` ASC) VISIBLE,
  CONSTRAINT `FK5pe33n1lieyd5ochn3i8jm1sy`
    FOREIGN KEY (`message_to`)
    REFERENCES `greedytown`.`user` (`user_seq`),
  CONSTRAINT `FKoj95ydu8kwhsi8pn0bm33aqqs`
    FOREIGN KEY (`message_from`)
    REFERENCES `greedytown`.`user` (`user_seq`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_as_cs;


-- -----------------------------------------------------
-- Table `greedytown`.`money_log`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `greedytown`.`money_log` (
  `money_log_seq` BIGINT NOT NULL AUTO_INCREMENT,
  `money_log_gameinfo` TINYINT NULL DEFAULT NULL,
  `money_log_money` BIGINT NULL DEFAULT NULL,
  `money_log_time` DATETIME NULL DEFAULT NULL,
  `money_log_iteminfo` BIGINT NULL DEFAULT NULL,
  `user_seq` BIGINT NULL DEFAULT NULL,
  PRIMARY KEY (`money_log_seq`),
  INDEX `FK121l641hl53q1hncw4xmi572e` (`money_log_iteminfo` ASC) VISIBLE,
  INDEX `FKtb8ql1rv4rjke6obus7aemcf7` (`user_seq` ASC) VISIBLE,
  CONSTRAINT `FK121l641hl53q1hncw4xmi572e`
    FOREIGN KEY (`money_log_iteminfo`)
    REFERENCES `greedytown`.`item` (`item_seq`),
  CONSTRAINT `FKtb8ql1rv4rjke6obus7aemcf7`
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
  CONSTRAINT `FK8uetfh238j40c27yqlne2vfn9`
    FOREIGN KEY (`user_seq`)
    REFERENCES `greedytown`.`user` (`user_seq`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_as_cs;


-- -----------------------------------------------------
-- Table `greedytown`.`success_user_achievements`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `greedytown`.`success_user_achievements` (
  `achievements_seq` BIGINT NOT NULL,
  `user_seq` BIGINT NOT NULL,
  PRIMARY KEY (`achievements_seq`, `user_seq`),
  INDEX `FKae08mefqkkuisj06w20oe1mfg` (`user_seq` ASC) VISIBLE,
  CONSTRAINT `FK1add1jmcys4a3k2299xxoxbhl`
    FOREIGN KEY (`achievements_seq`)
    REFERENCES `greedytown`.`achievements` (`achievements_seq`),
  CONSTRAINT `FKae08mefqkkuisj06w20oe1mfg`
    FOREIGN KEY (`user_seq`)
    REFERENCES `greedytown`.`user` (`user_seq`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_as_cs;


-- -----------------------------------------------------
-- Table `greedytown`.`wearing`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `greedytown`.`wearing` (
  `wearing_seq` BIGINT NOT NULL AUTO_INCREMENT,
  `item_seq` BIGINT NULL DEFAULT NULL,
  `user_seq` BIGINT NULL DEFAULT NULL,
  PRIMARY KEY (`wearing_seq`),
  INDEX `FKgp89etsvd13tx3s1rpje0h8k1` (`item_seq` ASC) VISIBLE,
  INDEX `FK6ywcx6u45pmnsookltug7nfqs` (`user_seq` ASC) VISIBLE,
  CONSTRAINT `FK6ywcx6u45pmnsookltug7nfqs`
    FOREIGN KEY (`user_seq`)
    REFERENCES `greedytown`.`user` (`user_seq`),
  CONSTRAINT `FKgp89etsvd13tx3s1rpje0h8k1`
    FOREIGN KEY (`item_seq`)
    REFERENCES `greedytown`.`item` (`item_seq`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_as_cs;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;


# 타입 인서트
insert into item_type (item_type_name) values("Hat");
insert into item_type (item_type_name) values("Head");
insert into item_type (item_type_name) values("Acs");
insert into item_type (item_type_name) values("Hair");
insert into item_type (item_type_name) values("Weapon");
insert into item_type (item_type_name) values("Sheild");
insert into item_type (item_type_name) values("Body");
insert into item_type (item_type_name) values("Back");

# 아이템 넣기
INSERT INTO item (item_seq, item_name, item_price, item_type_seq)
VALUES
(1, '궁수모자', 1000, 1),
(2, '험악모자', 2000, 1),
(3, '방울모자', 3000, 1),
(4, '가죽모자', 3000, 1),
(5, '초록모자', 3500, 1),
(6, '귀도리모자', 3500, 1),
(7, '뾰족모자', 4200, 1),
(8, '마법사모자', 4200, 1),
(9, '토끼토끼', 4500, 1),
(10, '해녀왕관', 5000, 1),
(11, '해녀투구', 5000, 1),
(12, '사무라이투구', 5000, 1),
(13, '아가미투구', 7000, 1),
(14, '날개툭', 10000, 1),
(15, '남자 두상', 0, 2),
(16, '여자 두상', 0, 2),
(17, '노란 수염 두상', 1500, 2),
(18, '갈색 수염 두상', 1000, 2),
(19, '빨간 수염 두상', 1500, 2),
(20, '파란 수염 두상', 1000, 2),
(21, '보라 수염 두상', 1000, 2),
(22, '검은 수염 두상', 1000, 2),
(23, '주황 긴수염 두상', 3200, 2),
(24, '갈색 긴수염 두상', 3000, 2),
(25, '빨간 긴수염 두상', 3200, 2),
(26, '파란 긴수염 두상', 2000, 2),
(27, '보라 긴수염 두상', 3000, 2),
(28, '검은 긴수염 두상', 3000, 2),
(29, '산타클로즈 수염 두상', 8000, 2),
(30, '로봇 투구', 4000, 2),
(31, '메카닉 사무라이 투구', 4200, 2),
(32, '파리 투구', 4600, 2),
(33, '종이접기 투구', 4700, 2),
(34, '코끼리 투구', 4800, 2),
(35, '하트머리', 1000, 3),
(36, '새싹머리', 1000, 3),
(37, '뿔테안경', 1500, 3),
(38, '뺑글안경', 2000, 3),
(39, '헤비메탈안경', 2000, 3),
(40, '야간경', 2000, 3),
(41, '토끼의 귀마개', 2000, 3),
(42, '요들의 귀마개', 2000, 3),
(43, '안테나 귀마개', 2000, 3),
(44, '천사의 귀마개', 2000, 3),
(45, '뿔', 2300, 3),
(46, '코뿔소의 뿔', 2300, 3),
(47, '도깨비 뿔', 2300, 3),
(48, '악마의 뿔', 2500, 3),
(49, '왕관', 3000, 3),
(50, '양동이 머리', 3000, 3),
(51, '마녀의 왕관', 3000, 3),
(52, '외눈박이 해적안대', 4500, 3),
(53, '보라 마스크', 3000, 3),
(54, '초록 마스크', 3000, 3),
(55, '빨간 마스크', 3000, 3),
(56, '파란 마스크', 3000, 3),
(57, '검은 마스크', 3000, 3),
(58, '노란 마스크', 3000, 3),
(59, '분홍 마스크', 3000, 3),
(60, '갈색 마스크', 3000, 3),
(61, '파란 리본', 4500, 3),
(62, '노란 수염', 3500, 3),
(63, '갈색 수염', 3500, 3),
(64, '빨간 수염', 3500, 3),
(65, '파란 수염', 3500, 3),
(66, '보라 수염', 3500, 3),
(67, '검은 수염', 3500, 3),
(68, '고급 노란 수염', 4000, 3),
(69, '고급 갈색 수염', 4000, 3),
(70, '고급 빨간 수염', 4000, 3),
(71, '고급 파란 수염', 4000, 3),
(72, '고급 보라 수염', 4000, 3),
(73, '최고급 검은 수염', 5000, 3),
(74, '외국도치컷', 1000, 4),
(75, '식빵컷', 2000, 4),
(76, '닭벼슬컷', 3000, 4),
(77, '민수컷', 3000, 4),
(78, '구름컷', 3500, 4),
(79, '핑크버섯컷', 3500, 4),
(80, '상모컷', 4200, 4),
(81, '양갈래컷', 4200, 4),
(82, '헤어밴드컷', 4500, 4),
(83, '잉어컷', 5000, 4),
(84, '영도치컷', 5000, 4),
(85, '검정버섯컷', 5000, 4),
(86, '올드도치컷', 7000, 4),
(87, '나뭇가지', 0, 5),
(88, '바늘', 1000, 5),
(89, '노멀 소드', 1200, 5),
(90, '그레이트 소드', 3000, 5),
(91, '골드 소드', 3500, 5),
(92, '수퍼 소드', 4500, 5),
(93, '카무사리 소드', 5000, 5),
(94, '레드 소드', 5200, 5),
(95, '블러드 소드', 6000, 5),
(96, '노멀 엑스', 7000, 5),
(97, '옐로우 윙 해머', 7000, 5),
(98, '그레이트 퍼플 소드', 8000, 5),
(99, '레드 머신 소드', 9000, 5),
(100, '라이트닝 해머', 12000, 5),
(101, '엑스 칼리버 소드', 13000, 5),
(102, 'DEM 소드', 15000, 5),
(103, '거인의 대검', 8000, 5),
(104, '대지의 힘', 9000, 5),
(105, '다인슬라이프', 15000, 5),
(106, '아론다이트', 20000, 5),
(107, '목마른 자의 검', 23000, 5),
(108, '드래곤 스피어스', 24000, 5),
(109, '투명 드래곤의 유산', 50000, 5),
(110, '그린완드', 3000, 5),
(111, '올드완드', 3200, 5),
(112, '페어리완드', 3500, 5),
(113, '미네랄 완드', 4000, 5),
(114, 'B.K데몬완드', 4500, 5),
(115, 'PM디토완드', 5000, 5),
(116, 'DEM완드', 10000, 5),
(117, '나무토막 방패', 1000, 6),
(118, '엑스 방패', 1500, 6),
(119, '판자 방패', 1500, 6),
(120, 'BMW 방패', 2000, 6),
(121, '타마마 방패', 2300, 6),
(122, '고급 판자 방패', 2500, 6),
(123, '삼성 방패', 3000, 6),
(124, '황금 방패', 3500, 6),
(125, '강철 방패', 3500, 6),
(126, '체르니 방패', 3800, 6),
(127, '나뭇잎 방패', 4000, 6),
(128, '풍뎅이 방패', 4200, 6),
(129, '프로모 방패', 4500, 6),
(130, '드래곤 방패', 5000, 6),
(131, '드래곤 아이 쉴드', 6000, 6),
(132, '엑스 드레이크 쉴드', 7000, 6),
(133, '레드 드래곤 블러드 쉴드', 7500, 6),
(134, '거북이 등딱지 방패', 8500, 6),
(135, '황금 무당벌레 방패', 13000, 6),
(136, 'DEM 쉴드', 15000, 6),
(137, '수영복(남)', 0, 7),
(138, '수영복(여)', 0, 7),
(139, '피라미드 노동자', 1000, 7),
(140, '완판 신화룩', 2000, 7),
(141, '승리의 V', 2500, 7),
(142, '면접합격룩', 3000, 7),
(143, '옐로부엉룩', 3500, 7),
(144, '심장절대보호', 4800, 7),
(145, '마법사 옷', 5000, 7),
(146, '전설의 레전드', 5200, 7),
(147, '케첩 앤 머스타드', 6200, 7),
(148, '동창회 추천룩', 6700, 7),
(149, '반반무많이', 7200, 7),
(150, '소방공무원', 7500, 7),
(151, '정글의 법칙', 7800, 7),
(152, '퍼플찍찍', 8000, 7),
(153, '그레이트솔져', 8500, 7),
(154, '패완블랙', 9000, 7),
(155, '레드댕댕', 10000, 7),
(156, '산타흑화갑옷', 15000, 7),
(157, '마법망토', 10000, 8),
(158, '플레임망토', 20000, 8),
(159, '바람망토', 25000, 8),
(160, '여행가방', 28000, 8),
(161, '초록가방', 29000, 8),
(162, '고서', 40000, 8);


