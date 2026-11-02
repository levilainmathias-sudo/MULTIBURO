<?php
///=========================SUMMARY=========================///
///===Toutes les fonctions pour afficher les ressources.====///
///=========================================================///

/*Afficher les ressources du batiment A*/
function afficher_bata()
{
    $tmp  = "<table class='table-ressources'>";
    $tmp .= "<thead>";
    $tmp .= "<tr><th colspan='4'>RESSOURCES DISPONIBLES</th></tr>";
    $tmp .= "<tr><th>Type de la ressource</th><th>Nom de la ressource</th><th>Nombre de place(s)</th><th>Réserver</th></tr>";
    $tmp .= "</thead>";
    $tmp .= "<tbody>";

    // Connexion BDD
    $cnx = new PDO('mysql:dbname='.BDD.';host='.HOST.';port='.PORT, LOGIN, PASSW);
    $cnx->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $stmt = $cnx->prepare("SELECT id_ressources, lib_ressources, nb_place, bat_ressources FROM ressources WHERE bat_ressources LIKE 'Batiment_A';");
    $stmt->execute();

    while ($ligne = $stmt->fetch(PDO::FETCH_ASSOC)) {
        $tmp .= "<tr>
                    <td>" . ($ligne['id_ressources']) . "</td>
                    <td>" . ($ligne['lib_ressources']) . "</td>
                    <td>" . ($ligne['nb_place']) . "</td>
                    <td><button type='button' onclick='confirmerReservation()'>Réserver</button></td>
                 </tr>";
    }

    $tmp .= "</tbody></table>";
    return $tmp;
}

/*Afficher les ressources du batiment B*/
function afficher_batb()
{
    $tmp  = "<table class='table-ressources'>";
    $tmp .= "<thead>";
    $tmp .= "<tr><th colspan='4'>RESSOURCES DISPONIBLES</th></tr>";
    $tmp .= "<tr><th>Type de la ressource</th><th>Nom de la ressource</th><th>Nombre de place(s)</th><th>Réserver</th></tr>";
    $tmp .= "</thead>";
    $tmp .= "<tbody>";

    // Connexion BDD
    $cnx = new PDO('mysql:dbname='.BDD.';host='.HOST.';port='.PORT, LOGIN, PASSW);
    $cnx->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $stmt = $cnx->prepare("SELECT id_ressources, lib_ressources, nb_place, bat_ressources FROM ressources WHERE bat_ressources LIKE 'Batiment_B';");
    $stmt->execute();

    while ($ligne = $stmt->fetch(PDO::FETCH_ASSOC)) {
        $tmp .= "<tr>
                    <td>" . ($ligne['id_ressources']) . "</td>
                    <td>" . ($ligne['lib_ressources']) . "</td>
                    <td>" . ($ligne['nb_place']) . "</td>
                    <td><button type='button' onclick='confirmerReservation()'>Réserver</button></td>
                 </tr>";
    }

    $tmp .= "</tbody></table>";
    return $tmp;
}

///=========================SUMMARY=========================///
///====Toutes les fonctions pour gérer les réservations.====///
///=========================================================///

/*Renvoie l'id de l'utilisateur connecté (session), ou lève une exception*/
function id_user_connecte(): int
{
    if (session_status() === PHP_SESSION_NONE) {
        session_start();
    }
    if (empty($_SESSION['id_user'])) {
        throw new RuntimeException("Vous devez être connecté.");
    }
    return (int) $_SESSION['id_user'];
}

/*Vrai si l'utilisateur est admin avec le droit de gérer les réservations*/
function peut_gerer_reservations(int $id_user): bool
{
    $stmt = getConnexionBDD()->prepare(
        "SELECT id_user FROM user WHERE id_user = ? AND est_admin = 1 AND droit_reservations = 1"
    );
    $stmt->execute([$id_user]);
    return (bool) $stmt->fetch();
}

/*Vérifie le format (AAAA-MM-JJ) et que la date n'est pas passée*/
function verifier_date_reservation(string $date): void
{
    $d = DateTime::createFromFormat('Y-m-d', $date);
    if (!$d || $d->format('Y-m-d') !== $date) {
        throw new InvalidArgumentException("Date invalide (format AAAA-MM-JJ).");
    }
    if ($date < date('Y-m-d')) {
        throw new InvalidArgumentException("Impossible de réserver dans le passé.");
    }
}

/*Créer la réservation dans la base de donnée
  Un user réserve pour lui-même ; un admin (droit_reservations) peut réserver pour un autre via $id_user.*/
function creer_reservation(int $id_salle, string $date_res, ?int $id_user = null): int
{
    $connecte = id_user_connecte();
    $id_user  = $id_user ?? $connecte;

    if ($id_user !== $connecte && !peut_gerer_reservations($connecte)) {
        throw new RuntimeException("Vous ne pouvez réserver que pour votre propre compte.");
    }
    verifier_date_reservation($date_res);

    $cnx = getConnexionBDD();

    // La ressource existe ?
    $stmt = $cnx->prepare("SELECT id_salle FROM ressources WHERE id_salle = ?");
    $stmt->execute([$id_salle]);
    if (!$stmt->fetch()) {
        throw new RuntimeException("Ressource introuvable.");
    }

    // L'utilisateur existe ?
    $stmt = $cnx->prepare("SELECT id_user FROM user WHERE id_user = ?");
    $stmt->execute([$id_user]);
    if (!$stmt->fetch()) {
        throw new RuntimeException("Utilisateur introuvable.");
    }

    // Déjà réservée ce jour-là ?
    $stmt = $cnx->prepare("SELECT id FROM reservation WHERE id_salle = ? AND date_res = ?");
    $stmt->execute([$id_salle, $date_res]);
    if ($stmt->fetch()) {
        throw new RuntimeException("Cette ressource est déjà réservée à cette date.");
    }

    try {
        $stmt = $cnx->prepare("INSERT INTO reservation (date_res, id_user, id_salle) VALUES (?, ?, ?)");
        $stmt->execute([$date_res, $id_user, $id_salle]);
    } catch (PDOException $e) {
        // 1062 = doublon (deux personnes réservent en même temps)
        if ((int) ($e->errorInfo[1] ?? 0) === 1062) {
            throw new RuntimeException("Cette ressource est déjà réservée à cette date.");
        }
        throw $e;
    }
    return (int) $cnx->lastInsertId();
}

/*Supprimer une réservation dans la BDD
  Un user supprime uniquement les siennes ; un admin (droit_reservations) peut supprimer toutes les réservations.*/
function supprimer_reservation(int $id_reservation): void
{
    $connecte = id_user_connecte();
    $cnx = getConnexionBDD();

    if (peut_gerer_reservations($connecte)) {
        $stmt = $cnx->prepare("DELETE FROM reservation WHERE id = ?");
        $stmt->execute([$id_reservation]);
    } else {
        $stmt = $cnx->prepare("DELETE FROM reservation WHERE id = ? AND id_user = ?");
        $stmt->execute([$id_reservation, $connecte]);
    }

    if ($stmt->rowCount() === 0) {
        throw new RuntimeException("Réservation introuvable ou non autorisée.");
    }
}

/*Modifier une réservation dans la BDD (null = on ne change pas la valeur)
  Mêmes règles que pour la suppression.*/
function modifier_reservation(int $id_reservation, ?string $nouvelle_date = null, ?int $nouvelle_salle = null): void
{
    $connecte = id_user_connecte();
    $cnx = getConnexionBDD();

    // On retrouve la réservation (restreinte à son propriétaire sauf pour un admin)
    if (peut_gerer_reservations($connecte)) {
        $stmt = $cnx->prepare("SELECT date_res, id_salle FROM reservation WHERE id = ?");
        $stmt->execute([$id_reservation]);
    } else {
        $stmt = $cnx->prepare("SELECT date_res, id_salle FROM reservation WHERE id = ? AND id_user = ?");
        $stmt->execute([$id_reservation, $connecte]);
    }
    $res = $stmt->fetch(PDO::FETCH_ASSOC);
    if (!$res) {
        throw new RuntimeException("Réservation introuvable ou non autorisée.");
    }

    $date  = $nouvelle_date  ?? $res['date_res'];
    $salle = $nouvelle_salle ?? (int) $res['id_salle'];

    if ($nouvelle_date !== null) {
        verifier_date_reservation($nouvelle_date);
    }
    if ($nouvelle_salle !== null) {
        $stmt = $cnx->prepare("SELECT id_salle FROM ressources WHERE id_salle = ?");
        $stmt->execute([$nouvelle_salle]);
        if (!$stmt->fetch()) {
            throw new RuntimeException("Ressource introuvable.");
        }
    }

    // Déjà prise par une autre réservation ?
    $stmt = $cnx->prepare("SELECT id FROM reservation WHERE id_salle = ? AND date_res = ? AND id <> ?");
    $stmt->execute([$salle, $date, $id_reservation]);
    if ($stmt->fetch()) {
        throw new RuntimeException("Cette ressource est déjà réservée à cette date.");
    }

    $stmt = $cnx->prepare("UPDATE reservation SET date_res = ?, id_salle = ? WHERE id = ?");
    $stmt->execute([$date, $salle, $id_reservation]);
}

/*Confirme ou non une réservation validée dans la BDD ou renvoi une erreur*/
function confirmerReservation()
{
    $reserver = confirm("Voulez-vous vraiment réserver cette ressource ?");
   
    if ($reserver = true)
    {
        alert("Réservation confirmée !");
    }
    else
    {
        alert("Réservation annulée.");
    }
}

/*Permet à un user de voir les réservations associés à son compte*/
function consulterReservation()
{
    /*TO DO*/
}

/*Créer un compte user (société) permettant de se connecter sur le site mais sans droit d'ajout et de suppression de salle, d'user et de réservation dans la base de donnée*/
function creer_user()
{
    // Rien à faire tant que le formulaire n'a pas été envoyé
    if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
        return "";
    }

    // Récupération des données du formulaire (valeur vide si absente)
    $nom      = trim($_POST['nom_societe'] ?? '');
    $prenom   = trim($_POST['nom_dirigeant'] ?? '');
    $email    = trim($_POST['email'] ?? '');
    $statut   = $_POST['statut'] ?? '';
    $password = $_POST['password'] ?? '';

    if ($nom === '' || $prenom === '' || $email === '' || $password === '') {
        return "Veuillez remplir tous les champs.";
    }
    if (!filter_var($email, FILTER_VALIDATE_EMAIL)) {
        return "Adresse e-mail invalide.";
    }
    if (!in_array($statut, ['Professionnel', 'Particulier'], true)) {
        return "Statut invalide.";
    }

    // Connexion BDD (fonction de accesBDD.php)
    $cnx = getConnexionBDD();

    try {
        // Requête préparée avec des marqueurs (?)
        $stmt = $cnx->prepare(
            "INSERT INTO user (nom_user, prenom_user, email_user, statut_user, password_user)
             VALUES (?, ?, ?, ?, ?)"
        );
        // Toujours hacher le mot de passe avant insertion !
        $stmt->execute([$nom, $prenom, $email, $statut, password_hash($password, PASSWORD_DEFAULT)]);
    } catch (PDOException $e) {
        if ($e->getCode() === '23000') { // e-mail déjà présent (colonne UNIQUE)
            return "Cette adresse e-mail est déjà utilisée.";
        }
        throw $e;
    }

    return ""; // succès : le pop-up affiche déjà « Compte créé ! »
}

/*Retirer un compte user permettant de se connecter sur le site ainsi que toutes ces données + réservations*/
function retirer_user()
{
    /*TO DO*/
}

/*S'identifier sur le site*/
function identifier_visiteur()
{
    /*TO DO*/
}

?>


