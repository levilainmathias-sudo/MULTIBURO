<?php
require_once 'codesAccesBDD.php';

try {
    $cnx = "mysql:host=" . DB_HOST . ";port=" . DB_PORT . ";dbname=" . DB_NAME . ";charset=utf8";

    $pdo = new PDO($cnx, DB_USER, DB_PASS);

    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

} catch (PDOException $e) 
{
    die("Erreur de connexion : " . $e->getMessage());
}
?>
<?php
/*-----Recupération des données-------*/
$stmt = $cnx->prepare
("SELECT id_ressources, lib_ressources, nb_place
FROM ressources");
$stmt->bindParam(':param_login', $login, PDO::PARAM_STR, 20);
$stmt->bindParam(':param_passw', $passw, PDO::PARAM_STR, 250);
// Execution
$stmt->execute();
// Fetch Monoligne Requête
if($result = $stmt->fetch())
{
echo $result['Locaux'];
}
?>