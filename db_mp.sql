-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema mp
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema mp
-- -----------------------------------------------------
DROP SCHEMA IF EXISTS `mp`;
CREATE SCHEMA IF NOT EXISTS `mp` DEFAULT CHARACTER SET utf8 ;
USE `mp` ;

-- -----------------------------------------------------
-- Table `mp`.`album`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mp`.`album` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(45) NULL,
  `cover` VARCHAR(45) NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mp`.`artist`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mp`.`artist` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(45) NULL,
  `cover` VARCHAR(45) NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mp`.`media`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mp`.`media` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `path` VARCHAR(45) NOT NULL,
  `name` VARCHAR(45) NULL,
  `duration` VARCHAR(45) NULL,
  `favorite` TINYINT NULL DEFAULT 0,
  `heard` TIMESTAMP NULL,
  `extension` VARCHAR(10) NOT NULL,
  `album_id` INT UNSIGNED NOT NULL,
  `artist_id` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`id`),
  CONSTRAINT `fk_media_album1`
    FOREIGN KEY (`album_id`)
    REFERENCES `mp`.`album` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_media_artist1`
    FOREIGN KEY (`artist_id`)
    REFERENCES `mp`.`artist` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mp`.`playlist`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mp`.`playlist` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(45) NOT NULL,
  `cover` VARCHAR(45) NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mp`.`mp`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mp`.`mp` (
  `media_id` INT UNSIGNED NOT NULL,
  `playlist_id` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`media_id`, `playlist_id`),
  CONSTRAINT `fk_table1_media`
    FOREIGN KEY (`media_id`)
    REFERENCES `mp`.`media` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_table1_playlist1`
    FOREIGN KEY (`playlist_id`)
    REFERENCES `mp`.`playlist` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
