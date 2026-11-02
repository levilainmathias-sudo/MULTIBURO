CREATE DATABASE lvln_multiburo COLLATE 'utf8mb4_roman_ci';
USE lvln_multiburo;
/*Creation table user========================================================================*/
CREATE TABLE user
(
    id_user INT,
    nom_user VARCHAR(50),
    prenom_user VARCHAR(50),
    CONSTRAINT pk_user PRIMARY KEY (id_user)
);
/*Creation table type_ressources========================================================================*/
CREATE TABLE type_ressources
(
    id_ts VARCHAR (10),
    lib_type VARCHAR(50),
    CONSTRAINT pk_type_ressources PRIMARY KEY (id_ts)
);
/*Creation table ressources========================================================================*/
CREATE TABLE ressources
(
    tri_res VARCHAR(2),
    id_ressources VARCHAR(20),
    lib_ressources VARCHAR(50),
    nb_place INT,
    etage_res VARCHAR(10),
    id_ts VARCHAR (10),
    CONSTRAINT pk_ressources PRIMARY KEY (id_ressources),
    CONSTRAINT fk_ressources FOREIGN KEY(id_ts) REFERENCES type_ressources(id_ts)
);
/*Creation table reservation========================================================================*/
CREATE TABLE reservation
(
 id INT NOT NULL,
 id_ressources VARCHAR(10),
 id_user INT,
 date_res DATE,
 CONSTRAINT pk_reservation PRIMARY KEY (id),
 CONSTRAINT fk_reservation_ressource FOREIGN KEY (id_ressources) REFERENCES ressources(id_ressources),
 CONSTRAINT fk_reservation_user FOREIGN KEY (id_user) REFERENCES user(id_user)
);
/*Ajout de ligne dans la table ressources (bâtiment B)============================================*/
USE lvln_multiburo;
/*===========================1er étage======================================================*/
USE lvln_multiburo;
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BS101', 'OPTIMUS', '1', 'Etage 1','1A');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BS102', 'BUMBLEBEE', '1', 'Etage 1','1B');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BS103', 'IRONHIDE', '1', 'Etage 1','1C');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BS104', 'MEGATRON', '1', 'Etage 1','1D');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BS105', 'SENTINEL PRIME', '1', 'Etage 1','1E');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BS106', 'RATCHET', '1', 'Etage 1','1F');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BS107', 'STARSCREAM', '1', 'Etage 1','1G');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BD108', 'CYBERTRON', '2', 'Etage 1','1H');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BD109', 'UNICRON', '2', 'Etage 1','1I');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BO110', 'ARCEE', '1', 'Etage 1','1J');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BO111', 'DRIFT', '1', 'Etage 1','1K');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BO112', 'ALLSPARK', '1', 'Etage 1','1L');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BO113', 'SHOCKWAVE', '1', 'Etage 1','1M');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BO114', 'CROSSHAIRS', '1', 'Etage 1','1N');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BO115', 'HOT ROD', '1', 'Etage 1','1O');
/*===========================2eme étage======================================================*/
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BS201', 'CLARK', '1', 'Etage 2','1P');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BS202', 'OCTAVIA', '1', 'Etage 2','1Q');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BS203', 'BELLAMY', '1', 'Etage 2','1R');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BS204', 'RAVEN', '1', 'Etage 2','1S');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BS205', 'MURPHY', '1', 'Etage 2','1T');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BS206', 'DIOZA', '1', 'Etage 2','1U');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BS207', 'ECHO', '1', 'Etage 2','1V');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BD208', 'LEXA', '2', 'Etage 2','1W');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BD209', 'LINCOLN', '2', 'Etage 2','1X');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BO210', 'MARCUS', '1', 'Etage 2','1Y');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BO211', 'INDRA', '1', 'Etage 2','1Z');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BO212', 'HARPER', '1', 'Etage 2','2A');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BO213', 'WELLS', '1', 'Etage 2','2B');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BO214', 'SINCLAIR', '1', 'Etage 2','2C');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BO215', 'ONTARI', '1', 'Etage 2','2D');
/*===========================3eme étage======================================================*/
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BS301', 'SATISFACTORY', '1', 'Etage 3','2E');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BS302', 'STRANDED DEEP', '1', 'Etage 3','2F');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BS303', 'STATE OF DECAY', '1', 'Etage 3','2G');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BS304', 'THE FOREST', '1', 'Etage 3','2H');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BS305', 'FARMING SIMULATOR', '1', 'Etage 3','2I');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BS306', 'ROCKET LEAGUE', '1', 'Etage 3','2J');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BS307', 'DYING LIGHT', '1', 'Etage 3','2K');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BD308', 'CALL OF DUTY', '2', 'Etage 3','2L');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BD309', 'RESIDENT EVIL', '2', 'Etage 3','2M');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BO310', 'SPIDERMAN', '1', 'Etage 3','2N');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BO311', 'PLANET CRAFTER', '1', 'Etage 3','2O');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BO312', 'ASTRONEER', '1', 'Etage 3','2P');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BO313', 'SPACE ENGINEERS', '1', 'Etage 3','2Q');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BO314', '911 OPERATOR', '1', 'Etage 3','2R');
INSERT INTO ressources(id_ressources, lib_ressources, nb_place, etage_res,tri_res)
VALUES ('BO315', 'SPACEWAR', '1', 'Etage 3','2S');
/*-----Création user----------------------------------------------------------------------*/
CREATE user levilain_multiburo IDENTIFIED BY 'Soleil@Lune';
GRANT ALL ON lvln_multiburo TO 'levilain_multiburo';