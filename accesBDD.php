<?php
require_once 'dbCodes.php';

function getConnexionBDD()
{
    try {
        $pdo = new PDO(
            SGBDR . ':host=' . HOST . ';port=' . PORT . ';dbname=' . BDD . ';charset=utf8',
            LOGIN,
            PASSW
        );
        $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
        return $pdo;
    } catch (PDOException $e) {
        die("Erreur BDD : " . $e->getMessage());
    }
}
