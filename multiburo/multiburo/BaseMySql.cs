using System;
using System.Collections.Generic;
using MySqlConnector;

namespace multiburo
{
    public class Salle
    {
        public int IdSalle { get; set; }
        public string IdRessources { get; set; }
        public string Libelle { get; set; }
        public string Batiment { get; set; }
        public int NbPlace { get; set; }
        public int? IdType { get; set; }
        public string LibType { get; set; }
    }

    public class TypeRessource
    {
        public int Id { get; set; }
        public string Libelle { get; set; }
    }

    public class Utilisateur
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public bool EstAdmin { get; set; }
    }

    public class BaseMySql
    {
        private readonly string _chaine;

        public Utilisateur Connecte { get; private set; }

        public BaseMySql(string chaineConnexion =
            "Server=localhost;Port=3306;Database=multiburo;User ID=root;Password=;CharSet=utf8mb4")
        {
            _chaine = chaineConnexion;
        }

        private MySqlConnection Ouvrir()
        {
            var cnx = new MySqlConnection(_chaine);
            cnx.Open();
            return cnx;
        }

        // ───────────── Connexion ─────────────

        public bool Connexion(string email, string motDePasse)
        {
            using var cnx = Ouvrir();
            using var cmd = new MySqlCommand(
                "SELECT id_user, nom_user, prenom_user, email_user, password_user, est_admin FROM user WHERE email_user = @e", cnx);
            cmd.Parameters.AddWithValue("@e", email);

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return false;
            if (!r.GetBoolean("est_admin")) return false;
            if (!BCrypt.Net.BCrypt.Verify(motDePasse, r.GetString("password_user"))) return false;

            Connecte = new Utilisateur
            {
                Id = r.GetInt32("id_user"),
                Nom = r.GetString("nom_user"),
                Prenom = r.GetString("prenom_user"),
                Email = r.GetString("email_user"),
                EstAdmin = true
            };
            return true;
        }

        public void Deconnexion() => Connecte = null;

        // ───────────── Salles ─────────────

        public int AjouterSalle(string idRessources, string libelle, string batiment, int nbPlace, int? idType = null)
        {
            ExigerDroit("droit_salles");
            VerifierBatiment(batiment);
            if (nbPlace < 1) throw new ArgumentException("Le nombre de places doit être d'au moins 1.");

            try
            {
                using var cnx = Ouvrir();
                using var cmd = new MySqlCommand(
                    @"INSERT INTO ressources (id_ressources, lib_ressources, bat_ressources, nb_place, id_type)
                      VALUES (@id, @lib, @bat, @nb, @type)", cnx);
                cmd.Parameters.AddWithValue("@id", idRessources);
                cmd.Parameters.AddWithValue("@lib", libelle);
                cmd.Parameters.AddWithValue("@bat", batiment);
                cmd.Parameters.AddWithValue("@nb", nbPlace);
                cmd.Parameters.AddWithValue("@type", (object)idType ?? DBNull.Value);
                cmd.ExecuteNonQuery();
                return (int)cmd.LastInsertedId;
            }
            catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
            {
                throw new InvalidOperationException($"'{idRessources}' existe déjà dans {batiment}.");
            }
        }

        public void SupprimerSalle(int idSalle)
        {
            ExigerDroit("droit_salles");

            using var cnx = Ouvrir();
            using var cmd = new MySqlCommand("DELETE FROM ressources WHERE id_salle = @id", cnx);
            cmd.Parameters.AddWithValue("@id", idSalle);
            if (cmd.ExecuteNonQuery() == 0) throw new InvalidOperationException("Salle introuvable.");
        }

        public void ModifierSalle(int idSalle, string idRessources = null, string libelle = null,
                                  string batiment = null, int? nbPlace = null, int? idType = null)
        {
            ExigerDroit("droit_salles");
            if (batiment != null) VerifierBatiment(batiment);
            if (nbPlace.HasValue && nbPlace < 1) throw new ArgumentException("Le nombre de places doit être d'au moins 1.");

            try
            {
                using var cnx = Ouvrir();
                using var cmd = new MySqlCommand(
                    @"UPDATE ressources SET
                        id_ressources  = COALESCE(@id, id_ressources),
                        lib_ressources = COALESCE(@lib, lib_ressources),
                        bat_ressources = COALESCE(@bat, bat_ressources),
                        nb_place       = COALESCE(@nb, nb_place),
                        id_type        = COALESCE(@type, id_type)
                      WHERE id_salle = @salle", cnx);
                cmd.Parameters.AddWithValue("@id", (object)idRessources ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@lib", (object)libelle ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@bat", (object)batiment ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@nb", (object)nbPlace ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@type", (object)idType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@salle", idSalle);
                if (cmd.ExecuteNonQuery() == 0) throw new InvalidOperationException("Salle introuvable.");
            }
            catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
            {
                throw new InvalidOperationException("Cette ressource existe déjà dans ce bâtiment.");
            }
        }

        public List<Salle> ListerSalles(string batiment = null)
        {
            var liste = new List<Salle>();
            using var cnx = Ouvrir();
            using var cmd = new MySqlCommand(
                @"SELECT r.id_salle, r.id_ressources, r.lib_ressources, r.bat_ressources, r.nb_place, r.id_type, t.lib_type
                  FROM ressources r LEFT JOIN type_ressources t ON t.id_type = r.id_type
                  WHERE @bat IS NULL OR r.bat_ressources = @bat
                  ORDER BY r.bat_ressources, r.id_ressources", cnx);
            cmd.Parameters.AddWithValue("@bat", (object)batiment ?? DBNull.Value);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                liste.Add(new Salle
                {
                    IdSalle = r.GetInt32("id_salle"),
                    IdRessources = r.GetString("id_ressources"),
                    Libelle = r.GetString("lib_ressources"),
                    Batiment = r.GetString("bat_ressources"),
                    NbPlace = r.GetInt32("nb_place"),
                    IdType = r.IsDBNull(r.GetOrdinal("id_type")) ? (int?)null : r.GetInt32("id_type"),
                    LibType = r.IsDBNull(r.GetOrdinal("lib_type")) ? "" : r.GetString("lib_type")
                });
            }
            return liste;
        }

        // ───────────── Types de ressources ─────────────

        public List<TypeRessource> ListerTypes()
        {
            var liste = new List<TypeRessource>();
            using var cnx = Ouvrir();
            using var cmd = new MySqlCommand("SELECT id_type, lib_type FROM type_ressources ORDER BY lib_type", cnx);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                liste.Add(new TypeRessource { Id = r.GetInt32("id_type"), Libelle = r.GetString("lib_type") });
            return liste;
        }

        public int AjouterType(string libelle)
        {
            ExigerDroit("droit_salles");
            if (string.IsNullOrWhiteSpace(libelle)) throw new ArgumentException("Le libellé est obligatoire.");
            try
            {
                using var cnx = Ouvrir();
                using var cmd = new MySqlCommand("INSERT INTO type_ressources (lib_type) VALUES (@l)", cnx);
                cmd.Parameters.AddWithValue("@l", libelle.Trim());
                cmd.ExecuteNonQuery();
                return (int)cmd.LastInsertedId;
            }
            catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
            {
                throw new InvalidOperationException("Ce type existe déjà.");
            }
        }

        public void ModifierType(int idType, string libelle)
        {
            ExigerDroit("droit_salles");
            if (string.IsNullOrWhiteSpace(libelle)) throw new ArgumentException("Le libellé est obligatoire.");
            try
            {
                using var cnx = Ouvrir();
                using var cmd = new MySqlCommand("UPDATE type_ressources SET lib_type = @l WHERE id_type = @id", cnx);
                cmd.Parameters.AddWithValue("@l", libelle.Trim());
                cmd.Parameters.AddWithValue("@id", idType);
                if (cmd.ExecuteNonQuery() == 0) throw new InvalidOperationException("Type introuvable.");
            }
            catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
            {
                throw new InvalidOperationException("Ce type existe déjà.");
            }
        }

        public void SupprimerType(int idType)
        {
            ExigerDroit("droit_salles");
            using var cnx = Ouvrir();
            using var cmd = new MySqlCommand("DELETE FROM type_ressources WHERE id_type = @id", cnx);
            cmd.Parameters.AddWithValue("@id", idType);
            if (cmd.ExecuteNonQuery() == 0) throw new InvalidOperationException("Type introuvable.");
        }

        // ───────────── Admins ─────────────

        public int CreerAdmin(string nom, string prenom, string email, string motDePasse,
                              bool dSalles = true, bool dUsers = true, bool dReservations = true)
        {
            if (CompterAdmins() > 0) ExigerDroit("droit_users");

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                throw new ArgumentException("Adresse e-mail invalide.");
            if (string.IsNullOrEmpty(motDePasse) || motDePasse.Length < 8)
                throw new ArgumentException("Mot de passe trop court (8 caractères minimum).");

            try
            {
                using var cnx = Ouvrir();
                using var cmd = new MySqlCommand(
                    @"INSERT INTO user (nom_user, prenom_user, email_user, statut_user, password_user,
                                        est_admin, droit_salles, droit_users, droit_reservations)
                      VALUES (@nom, @prenom, @email, 'Administrateur', @hash, 1, @ds, @du, @dr)", cnx);
                cmd.Parameters.AddWithValue("@nom", nom);
                cmd.Parameters.AddWithValue("@prenom", prenom);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@hash", BCrypt.Net.BCrypt.HashPassword(motDePasse));
                cmd.Parameters.AddWithValue("@ds", dSalles);
                cmd.Parameters.AddWithValue("@du", dUsers);
                cmd.Parameters.AddWithValue("@dr", dReservations);
                cmd.ExecuteNonQuery();
                return (int)cmd.LastInsertedId;
            }
            catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
            {
                throw new InvalidOperationException("Cette adresse e-mail est déjà utilisée.");
            }
        }

        public void RetirerAdmin(int idUser, bool supprimerCompte = false)
        {
            ExigerDroit("droit_users");

            using var cnx = Ouvrir();

            using (var verif = new MySqlCommand("SELECT est_admin FROM user WHERE id_user = @id", cnx))
            {
                verif.Parameters.AddWithValue("@id", idUser);
                var estAdmin = verif.ExecuteScalar();
                if (estAdmin == null) throw new InvalidOperationException("Utilisateur introuvable.");
                if (!Convert.ToBoolean(estAdmin)) throw new InvalidOperationException("Cet utilisateur n'est pas administrateur.");
            }

            if (CompterAdmins() <= 1)
                throw new InvalidOperationException("Impossible de retirer le dernier administrateur.");

            string sql = supprimerCompte
                ? "DELETE FROM user WHERE id_user = @id"
                : @"UPDATE user SET est_admin = 0, statut_user = 'Particulier',
                                    droit_salles = 0, droit_users = 0, droit_reservations = 0
                    WHERE id_user = @id";

            using var cmd = new MySqlCommand(sql, cnx);
            cmd.Parameters.AddWithValue("@id", idUser);
            cmd.ExecuteNonQuery();

            if (Connecte != null && Connecte.Id == idUser) Connecte = null;
        }

        // ───────────── Outils internes ─────────────

        private void ExigerDroit(string colonne)
        {
            if (Connecte == null) throw new UnauthorizedAccessException("Vous devez être connecté.");
            if (colonne != "droit_salles" && colonne != "droit_users" && colonne != "droit_reservations")
                throw new ArgumentException("Droit inconnu.");

            using var cnx = Ouvrir();
            using var cmd = new MySqlCommand($"SELECT est_admin, {colonne} FROM user WHERE id_user = @id", cnx);
            cmd.Parameters.AddWithValue("@id", Connecte.Id);
            using var r = cmd.ExecuteReader();
            if (!r.Read() || !r.GetBoolean(0) || !r.GetBoolean(1))
                throw new UnauthorizedAccessException("Droits insuffisants.");
        }

        public bool AucunAdmin() => CompterAdmins() == 0;

        private int CompterAdmins()
        {
            using var cnx = Ouvrir();
            using var cmd = new MySqlCommand("SELECT COUNT(*) FROM user WHERE est_admin = 1", cnx);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        private static void VerifierBatiment(string b)
        {
            if (b != "Batiment_A" && b != "Batiment_B")
                throw new ArgumentException("Bâtiment invalide (Batiment_A ou Batiment_B).");
        }
    }
}