<?php require_once "accesBDD.php"; ?>
<?php include_once "functions.php"; ?>
<!DOCTYPE html>
<html lang="fr">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Multiburo – Location de bureaux & openspaces</title>
  <link rel="stylesheet" href="multiburo.css">
  <script src="script.js" defer></script>
</head>
<body>

  <!-- PAGE D'ACCUEIL -->
  <main id="accueil">
    <a class="login-link" href="#Inscription">Inscription</a>

    <header class="brand">
      <h1>Multiburo</h1>
      <div class="underline"></div>
    </header>

    <div class="buildings">
      <article class="card">
        <div class="photo">
          <img src="image/batiment-a.jpg" alt="Façade du bâtiment A" loading="lazy">
        </div>
        <div class="card-content">
          <h2>Bâtiment A</h2>
          <p>Bureaux & openspaces</p>
          <a class="btn" href="#batimentA">Accéder</a>
        </div>
      </article>

      <article class="card">
        <div class="photo">
          <img src="image/batiment-b.jpg" alt="Façade du bâtiment B" loading="lazy">
        </div>
        <div class="card-content">
          <h2>Bâtiment B</h2>
          <p>Bureaux & openspaces</p>
          <a class="btn" href="#batimentB">Accéder</a>
        </div>
      </article>
    </div>
  </main>

  <!-- PAGE BÂTIMENT A -->
  <section class="page" id="batimentA">
    <div class="page-card">
      <h2>Bâtiment A</h2>
      <div class="underline"></div>
      <section><?php echo afficher_bata(); ?></section>
      <a class="btn btn-back" href="#">← Retour</a>
    </div>
  </section>

  <!-- PAGE BÂTIMENT B -->
  <section class="page" id="batimentB">
    <div class="page-card">
      <h2>Bâtiment B</h2>
      <div class="underline"></div>
      <section><?php echo afficher_batb(); ?></section>
      <a class="btn btn-back" href="#">← Retour</a>
    </div>
  </section>

  <!-- PAGE INSCRIPTION -->
  <section class="page" id="Inscription">
    <div class="divinscri">
        <h2>Inscrivez-vous</h2>
        <div class="underline"></div>

        <form method="POST" action="" onsubmit="return confirmerCreation()">
            <table>
                <tr>
                    <td>Votre nom ou nom de l'entreprise</td>
                    <td><input type="text" name="nom_societe" required></td>
                </tr>
                <tr>
                    <td>Prénom du dirigeant</td>
                    <td><input type="text" name="nom_dirigeant" required></td>
                </tr>
                <tr>
                    <td>Statut</td>
                    <td>
                        <label><input type="radio" name="statut" value="Professionnel" checked> Professionnel</label><br>
                        <label><input type="radio" name="statut" value="Particulier"> Particulier</label>
                    </td>
                </tr>
                <tr>
                    <td>Adresse E-Mail</td>
                    <td><input type="email" name="email" required></td>
                </tr>
                <tr>
                    <td>Mot de passe</td>
                    <td><input type="password" name="password" required></td>
                </tr>
            </table>
    <section><?php echo creer_user(); ?></section>
        <div class="form-buttons">
            <a class="btn-back-inscri" href="#">← Retour</a>
            <button type="submit" class="btn-submit">Valider l'inscription</button>
        </div>
        </form>
    </div>
  </section>
  <!-- Pop-up sur mesure faite par Gémini -->
  <div id="custom-popup" class="popup-overlay">
    <div class="popup-box" id="popup-box">
      <p id="popup-message"></p>
      <div class="popup-buttons">
        <button id="popup-btn-cancel" class="btn-popup btn-cancel">Annuler</button>
        <button id="popup-btn-confirm" class="btn-popup btn-confirm">Continuer</button>
        <button id="popup-btn-ok" class="btn-popup btn-confirm" style="display: none;">OK</button>
      </div>
    </div>
  </div>
</body>
</html>