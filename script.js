function confirmerCreation() {
  const popup = document.getElementById('custom-popup');
  const box = document.getElementById('popup-box');
  const message = document.getElementById('popup-message');
  const btnCancel = document.getElementById('popup-btn-cancel');
  const btnConfirm = document.getElementById('popup-btn-confirm');
  const btnOk = document.getElementById('popup-btn-ok');
  const form = document.querySelector('#Inscription form');

  // Réinitialisation des styles
  box.className = 'popup-box';
  message.textContent = "Avant d'accepter les CGV et le traitement de vos données : renseignez-vous dans la rubrique en savoir plus.";
  message.style.color = '#333333';

  btnCancel.style.display = 'inline-block';
  btnConfirm.style.display = 'inline-block';
  btnOk.style.display = 'none';

  popup.classList.add('active');

  // Annuler
  btnCancel.onclick = function() {
    box.classList.add('error');
    message.textContent = "Erreur lors de la création du compte";
    btnCancel.style.display = 'none';
    btnConfirm.style.display = 'none';
    btnOk.style.display = 'inline-block';

    btnOk.onclick = function() {
      popup.classList.remove('active');
    };
  };

  // Continuer (Succès & Envoi du formulaire)
  btnConfirm.onclick = function() {
    box.classList.add('success');
    message.textContent = "Compte créé !";
    btnCancel.style.display = 'none';
    btnConfirm.style.display = 'none';
    btnOk.style.display = 'inline-block';

    btnOk.onclick = function() {
      popup.classList.remove('active');
      form.submit(); // Soumission effective du formulaire
    };
  };

  // Bloque la soumission automatique pour attendre le clic utilisateur
  return false;
}