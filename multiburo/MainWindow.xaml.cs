using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace multiburo
{
    public partial class MainWindow : Window
    {
        private readonly BaseMySql _base = new BaseMySql();

        public MainWindow()
        {
            InitializeComponent();

            CbBatiment.ItemsSource = new[] { "Batiment_A", "Batiment_B" };
            CbFiltreBat.ItemsSource = new[] { "Tous", "Batiment_A", "Batiment_B" };
            CbFiltreBat.SelectedIndex = 0;

            // Premier lancement : propose de créer le premier gestionnaire
            try
            {
                if (_base.AucunAdmin())
                {
                    BtnPremierAdmin.Visibility = Visibility.Visible;
                    TxtErreurConnexion.Text = "Aucun gestionnaire n'existe encore : saisissez un e-mail et un mot de passe (8 caractères min.), puis créez le compte.";
                }
            }
            catch (Exception ex)
            {
                TxtErreurConnexion.Text = "Base de données inaccessible : " + ex.Message;
            }
        }

        // ───────────── Utilitaires ─────────────

        private void Executer(Action action)
        {
            try { action(); }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Multiburo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool Confirmer(string question) =>
            MessageBox.Show(question, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

        // ───────────── US1 : m'identifier ─────────────

        private void BtnConnexion_Click(object sender, RoutedEventArgs e)
        {
            TxtErreurConnexion.Text = "";
            try
            {
                if (_base.Connexion(IdAdmin.Text.Trim(), MdpAdmin.Password))
                {
                    TxtUtilisateur.Text = $"Connecté : {_base.Connecte.Prenom} {_base.Connecte.Nom}";
                    MdpAdmin.Clear();
                    PageConnexion.Visibility = Visibility.Collapsed;
                    PageGestion.Visibility = Visibility.Visible;
                    ChargerTypes();
                    ChargerRessources();
                }
                else
                {
                    TxtErreurConnexion.Text = "Identifiants incorrects, ou compte non gestionnaire.";
                }
            }
            catch (Exception ex)
            {
                TxtErreurConnexion.Text = "Base de données inaccessible : " + ex.Message;
            }
        }

        private void BtnPremierAdmin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _base.CreerAdmin("Multiburo", "Gestionnaire", IdAdmin.Text.Trim(), MdpAdmin.Password);
                BtnPremierAdmin.Visibility = Visibility.Collapsed;
                TxtErreurConnexion.Text = "Compte créé. Vous pouvez vous connecter.";
            }
            catch (Exception ex)
            {
                TxtErreurConnexion.Text = ex.Message;
            }
        }

        private void BtnDeconnexion_Click(object sender, RoutedEventArgs e)
        {
            _base.Deconnexion();
            PageGestion.Visibility = Visibility.Collapsed;
            PageConnexion.Visibility = Visibility.Visible;
            TxtErreurConnexion.Text = "";
            DgRessources.ItemsSource = null;
            ViderFormulaire();
        }

        // ───────────── US3 : visualiser la liste des ressources ─────────────

        private void ChargerRessources()
        {
            Executer(() =>
            {
                string bat = CbFiltreBat.SelectedItem as string;
                if (bat == "Tous") bat = null;
                DgRessources.ItemsSource = _base.ListerSalles(bat);
            });
        }

        private void CbFiltreBat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_base.Connecte != null) ChargerRessources();
        }

        private void DgRessources_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgRessources.SelectedItem is Salle s)
            {
                TxtIdRessource.Text = s.IdRessources;
                TxtLibelle.Text = s.Libelle;
                CbBatiment.SelectedItem = s.Batiment;
                TxtNbPlace.Text = s.NbPlace.ToString();
                CbType.SelectedValue = s.IdType;
            }
        }

        // ───────────── US2 : créer une ressource ─────────────

        private void BtnCreerRessource_Click(object sender, RoutedEventArgs e)
        {
            if (!LireFormulaire(out var id, out var lib, out var bat, out var nb, out var type)) return;

            Executer(() =>
            {
                _base.AjouterSalle(id, lib, bat, nb, type);
                ChargerRessources();
                ViderFormulaire();
            });
        }

        // ───────────── US4 : gérer les ressources ─────────────

        private void BtnModifierRessource_Click(object sender, RoutedEventArgs e)
        {
            if (!(DgRessources.SelectedItem is Salle s))
            {
                MessageBox.Show("Sélectionnez d'abord une ressource dans la liste.");
                return;
            }
            if (!LireFormulaire(out var id, out var lib, out var bat, out var nb, out var type)) return;

            Executer(() =>
            {
                _base.ModifierSalle(s.IdSalle, id, lib, bat, nb, type);
                ChargerRessources();
            });
        }

        private void BtnSupprimerRessource_Click(object sender, RoutedEventArgs e)
        {
            if (!(DgRessources.SelectedItem is Salle s))
            {
                MessageBox.Show("Sélectionnez d'abord une ressource dans la liste.");
                return;
            }
            if (!Confirmer($"Supprimer « {s.IdRessources} - {s.Libelle} » et toutes ses réservations ?")) return;

            Executer(() =>
            {
                _base.SupprimerSalle(s.IdSalle);
                ChargerRessources();
                ViderFormulaire();
            });
        }

        private void BtnVider_Click(object sender, RoutedEventArgs e)
        {
            DgRessources.SelectedItem = null;
            ViderFormulaire();
        }

        private void ViderFormulaire()
        {
            TxtIdRessource.Clear();
            TxtLibelle.Clear();
            TxtNbPlace.Clear();
            CbBatiment.SelectedIndex = -1;
            CbType.SelectedIndex = -1;
        }

        private bool LireFormulaire(out string id, out string lib, out string bat, out int nb, out int? type)
        {
            id = TxtIdRessource.Text.Trim();
            lib = TxtLibelle.Text.Trim();
            bat = CbBatiment.SelectedItem as string;
            type = CbType.SelectedValue as int?;
            nb = 0;

            if (id == "" || lib == "" || bat == null || !int.TryParse(TxtNbPlace.Text, out nb) || nb < 1)
            {
                MessageBox.Show("Renseignez la référence, le nom, le bâtiment et un nombre de places d'au moins 1.");
                return false;
            }
            return true;
        }

        // ───────────── US5 : gérer les types de ressources ─────────────

        private void ChargerTypes()
        {
            Executer(() =>
            {
                List<TypeRessource> types = _base.ListerTypes();
                LbTypes.ItemsSource = types;
                CbType.ItemsSource = types;
                TxtLibType.Clear();
            });
        }

        private void LbTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LbTypes.SelectedItem is TypeRessource t) TxtLibType.Text = t.Libelle;
        }

        private void BtnAjouterType_Click(object sender, RoutedEventArgs e)
        {
            Executer(() =>
            {
                _base.AjouterType(TxtLibType.Text);
                ChargerTypes();
            });
        }

        private void BtnRenommerType_Click(object sender, RoutedEventArgs e)
        {
            if (!(LbTypes.SelectedItem is TypeRessource t))
            {
                MessageBox.Show("Sélectionnez d'abord un type dans la liste.");
                return;
            }

            Executer(() =>
            {
                _base.ModifierType(t.Id, TxtLibType.Text);
                ChargerTypes();
                ChargerRessources(); // les libellés de type changent dans la liste
            });
        }

        private void BtnSupprimerType_Click(object sender, RoutedEventArgs e)
        {
            if (!(LbTypes.SelectedItem is TypeRessource t))
            {
                MessageBox.Show("Sélectionnez d'abord un type dans la liste.");
                return;
            }
            if (!Confirmer($"Supprimer le type « {t.Libelle} » ? Les ressources concernées resteront, mais sans type.")) return;

            Executer(() =>
            {
                _base.SupprimerType(t.Id);
                ChargerTypes();
                ChargerRessources();
            });
        }
    }
}