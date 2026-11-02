CREATE DATABASE IF NOT EXISTS multiburo CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE multiburo;

/* Type de ressource ==================================================== */
CREATE TABLE type_ressources
(
    id_type  INT AUTO_INCREMENT PRIMARY KEY,
    lib_type VARCHAR(50) NOT NULL UNIQUE
);

INSERT INTO type_ressources (lib_type) VALUES
    ('Bureau simple'), ('Bureau double'), ('Bureau ouvert'),
    ('Open Space'), ('Salles de réunions'), ('Table');

/* Ressources (salles, bureaux, tables...) ============================== */
/* id_salle = identifiant technique (les libellés comme 'Bureau simple 10'
   existent dans les deux bâtiments, ils ne peuvent donc pas être clé primaire). */
CREATE TABLE ressources
(
    id_salle       INT AUTO_INCREMENT PRIMARY KEY,
    id_ressources  VARCHAR(50) COLLATE utf8mb4_bin NOT NULL,   -- ex. 'Bureau simple 10'
    lib_ressources VARCHAR(50) NOT NULL,                       -- ex. 'Onyx'
    bat_ressources ENUM('Batiment_A','Batiment_B') NOT NULL,
    nb_place       INT NOT NULL DEFAULT 1,
    id_type        INT NULL,
    UNIQUE (bat_ressources, id_ressources),
    FOREIGN KEY (id_type) REFERENCES type_ressources(id_type) ON DELETE SET NULL
);

CREATE TABLE user
(
    id_user            INT AUTO_INCREMENT PRIMARY KEY,
    nom_user           VARCHAR(50)  NOT NULL,
    prenom_user        VARCHAR(50)  NOT NULL,
    email_user         VARCHAR(100) NOT NULL UNIQUE,
    statut_user        VARCHAR(20)  NOT NULL DEFAULT 'Particulier',
    password_user      VARCHAR(255) NOT NULL,                  -- hash bcrypt (password_hash)
    est_admin          INT          NOT NULL DEFAULT 0,        -- 0 = non, 1 = oui
    droit_salles       INT          NOT NULL DEFAULT 0,        -- ajout / modif / suppression de salle
    droit_users        INT          NOT NULL DEFAULT 0,        -- ajout / suppression d'utilisateur
    droit_reservations INT          NOT NULL DEFAULT 0         -- ajout / suppression de réservation
);

/* Réservations ========================================================= */
/* ON DELETE CASCADE : supprimer une salle ou un user supprime ses réservations. */
CREATE TABLE reservation
(
    id       INT AUTO_INCREMENT PRIMARY KEY,
    date_res DATE NOT NULL,
    id_user  INT  NOT NULL,
    id_salle INT  NOT NULL,
    FOREIGN KEY (id_user)  REFERENCES user(id_user)       ON DELETE CASCADE,
    FOREIGN KEY (id_salle) REFERENCES ressources(id_salle) ON DELETE CASCADE
);

/* Données =============================================================== */
INSERT INTO ressources (id_ressources, lib_ressources, bat_ressources, nb_place) VALUES
    ('Bureau double 1', 'Linkin Park', 'Batiment_B', 2),
    ('Bureau double 2', 'AC/DC', 'Batiment_B', 2),
    ('Bureau ouvert 3', 'Beatles', 'Batiment_B', 1),
    ('Bureau ouvert 4', 'Nirvana', 'Batiment_B', 1),
    ('Bureau ouvert 5', 'Guns n Roses', 'Batiment_B', 1),
    ('Bureau ouvert 6', 'Green Day', 'Batiment_B', 1),
    ('Bureau simple 1', 'Diamant', 'Batiment_A', 1),
    ('Bureau simple 2', 'Rubis', 'Batiment_A', 1),
    ('Bureau simple 3', 'Emeraude', 'Batiment_A', 1),
    ('Bureau simple 4', 'Saphire', 'Batiment_A', 1),
    ('Bureau simple 5', 'Améthyste', 'Batiment_A', 1),
    ('Bureau simple 6', 'Lapiz Lazuli', 'Batiment_A', 1),
    ('Bureau simple 7', 'Jade', 'Batiment_A', 1),
    ('Bureau simple 8', 'Turquoise', 'Batiment_A', 1),
    ('Bureau simple 9', 'Opale', 'Batiment_A', 1),
    ('Bureau simple 10', 'Quartz', 'Batiment_A', 1),
    ('Bureau simple 11', 'Obsidienne', 'Batiment_A', 1),
    ('Bureau simple 12', 'Onyx', 'Batiment_A', 1),
    ('Bureau simple 13', 'Jaspe', 'Batiment_A', 1),
    ('Bureau simple 14', 'Zircon', 'Batiment_A', 1),
    ('Bureau simple 15', 'Ambre', 'Batiment_A', 1),
    ('Bureau simple 16', 'Crystal', 'Batiment_A', 1),
    ('Bureau simple 17', 'Apatite', 'Batiment_A', 1),
    ('Bureau simple 18', 'Malachite', 'Batiment_A', 1),
    ('Bureau simple 19', 'Calcite', 'Batiment_A', 1),
    ('Bureau simple 20', 'Fluorite', 'Batiment_A', 1),
    ('Bureau simple 01', 'Chêne', 'Batiment_B', 1),
    ('Bureau simple 02', 'Erable', 'Batiment_B', 1),
    ('Bureau simple 03', 'Bouleau', 'Batiment_B', 1),
    ('Bureau simple 04', 'Pin', 'Batiment_B', 1),
    ('Bureau simple 05', 'Hêtre', 'Batiment_B', 1),
    ('Bureau simple 06', 'Tilleul', 'Batiment_B', 1),
    ('Bureau simple 07', 'Frêne', 'Batiment_B', 1),
    ('Bureau simple 08', 'Sequoia', 'Batiment_B', 1),
    ('Bureau simple 09', 'Cèdre', 'Batiment_B', 1),
    ('Bureau simple 10', 'Olivier', 'Batiment_B', 1),
    ('Bureau simple 11', 'Bambou', 'Batiment_B', 1),
    ('Bureau simple 12', 'Saule', 'Batiment_B', 1),
    ('Bureau simple 13', 'Acacia', 'Batiment_B', 1),
    ('Bureau simple 14', 'Noyer', 'Batiment_B', 1),
    ('Bureau simple 15', 'Cypress', 'Batiment_B', 1),
    ('Bureau simple 16', 'Peuplier', 'Batiment_B', 1),
    ('Bureau simple 17', 'Méleze', 'Batiment_B', 1),
    ('Bureau simple 18', 'Ginkgo', 'Batiment_B', 1),
    ('Open Space 1', 'Zeus', 'Batiment_A', 10),
    ('Open Space 2', 'Hera', 'Batiment_A', 10),
    ('Open Space 3', 'Aphrodite', 'Batiment_A', 10),
    ('Open Space 4', 'Athena', 'Batiment_A', 10),
    ('Open Space 5', 'Hermes', 'Batiment_A', 10),
    ('Open Space 6', 'Poséidon', 'Batiment_A', 10),
    ('Open Space 7', 'Artemis', 'Batiment_A', 10),
    ('Open Space 8', 'Hadès', 'Batiment_A', 10),
    ('Open Space 9', 'Apollon', 'Batiment_A', 10),
    ('Open Space 10', 'Ares', 'Batiment_A', 10),
    ('Bureau Ouvert 1', 'Lyon', 'Batiment_B', 1),
    ('Bureau Ouvert 2', 'Bordeaux', 'Batiment_B', 1),
    ('Bureau Ouvert 3', 'Nantes', 'Batiment_B', 1),
    ('Bureau Ouvert 4', 'Strasbourg', 'Batiment_B', 1),
    ('Bureau Ouvert 5', 'Toulouse', 'Batiment_B', 1),
    ('Bureau Ouvert 6', 'Lille', 'Batiment_B', 1),
    ('Bureau Ouvert 7', 'Rennes', 'Batiment_B', 1),
    ('Bureau Ouvert 8', 'Montpellier', 'Batiment_B', 1),
    ('Bureau Ouvert 9', 'Grenoble', 'Batiment_B', 1),
    ('Bureau Ouvert 10', 'Dijon', 'Batiment_B', 1),
    ('Bureau Ouvert 11', 'Angers', 'Batiment_B', 1),
    ('Bureau Ouvert 12', 'Reims', 'Batiment_B', 1),
    ('Bureau Ouvert 13', 'Clermont', 'Batiment_B', 1),
    ('Bureau Ouvert 14', 'Vienne', 'Batiment_B', 1),
    ('Bureau Ouvert 15', 'Rouen', 'Batiment_B', 1),
    ('Salles de réunions 1', 'Mars', 'Batiment_A', 20),
    ('Salles de réunions 2', 'Jupiter', 'Batiment_A', 20),
    ('Salles de réunions 3', 'Terre', 'Batiment_A', 20),
    ('Salles de réunions 4', 'Netune', 'Batiment_A', 20),
    ('Salles de réunions 5', 'Venus', 'Batiment_A', 20),
    ('Salles de réunions 6', 'Uranus', 'Batiment_A', 20),
    ('Salles de réunions 7', 'Saturne', 'Batiment_A', 20),
    ('Salles de réunions 8', 'Mercure', 'Batiment_A', 20),
    ('Salles de réunions 9', 'Lune', 'Batiment_A', 20),
    ('Salles de réunions 10', 'Soleil', 'Batiment_A', 20),
    ('Table 1', 'As de Coeur', 'Batiment_B', 6),
    ('Table 2', 'Roi de Coeur', 'Batiment_B', 6),
    ('Table 3', 'Dame de Coeur', 'Batiment_B', 6),
    ('Table 4', 'As de Pique', 'Batiment_B', 6),
    ('Table 5', 'Roi de Pique', 'Batiment_B', 6),
    ('Table 6', 'Dame de Pique', 'Batiment_B', 6),
    ('Table 7', 'As de carreaux', 'Batiment_B', 6),
    ('Table 8', 'Roi de carreaux', 'Batiment_B', 6),
    ('Table 9', 'Dame de carreaux', 'Batiment_B', 6),
    ('Table 10', 'As de Trèfle', 'Batiment_B', 6),
    ('Table 11', 'Roi de Trèfle', 'Batiment_B', 6),
    ('Table 12', 'Dame de Trèfle', 'Batiment_B', 6);

/* Rattache chaque ressource à son type d'après le début de sa référence */
UPDATE ressources r
JOIN type_ressources t ON LOWER(r.id_ressources) LIKE CONCAT(LOWER(t.lib_type), '%')
SET r.id_type = t.id_type
WHERE r.id_type IS NULL;