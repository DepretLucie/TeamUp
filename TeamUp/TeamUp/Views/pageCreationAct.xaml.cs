﻿using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;
using System.Collections.ObjectModel;
using TeamUp.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace TeamUp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class pageCreationAct : ContentPage
    {
        private string urlAct = "http://10.3.229.19/Api/activites";
        //private string urlAct = "http://192.168.1.38/Api/activites";

        private string urlUser = "http://10.3.229.19/Api/utilisateur.php?identifiant=";
        //private string urlUser = "http://192.168.1.38/Api/utilisateur.php?identifiant=";

        private HttpClient client = new HttpClient();
        public pageCreationAct()
        {
            InitializeComponent();

            var ListeNiveau = new List<string>();
            ListeNiveau.Add("Débutant");
            ListeNiveau.Add("Amateur");
            ListeNiveau.Add("Avancé");

            Picker.ItemsSource = ListeNiveau;
        }

        public void OnDateSelected(object sender, EventArgs e)
        {

        }

        private async void OnClickCreationAct(object sender, EventArgs e)
        {
            var content = await client.GetStringAsync(urlUser);
            var utilisateur = JsonConvert.DeserializeObject<List<Utilisateur>>(content);

            foreach (Utilisateur user in utilisateur)
            {
                App.utilisateur = user;
                Console.WriteLine("La liste est " + user.identifiant);
                Console.WriteLine("L'utilisateur est " + user.identifiant);
            }

            var dateAct = startDatePicker.Date.ToString().Substring(0, 10);
            var nom = Nom.Text;
            var lieu = Lieu.Text;
            var tarif = Tarif.Text;
            var nbTeam = NbTeam.Text;

            if (string.IsNullOrEmpty(dateAct) || string.IsNullOrEmpty(nom) || string.IsNullOrEmpty(lieu) || string.IsNullOrEmpty(Horaires.Text) || string.IsNullOrEmpty(Picker.SelectedItem.ToString()) || string.IsNullOrEmpty(nbTeam))
            {
                DisplayAlert("Un ou plusieurs champs obligatoires vides", "Veuillez saisir les informations manquantes", "Ok");
                return;
            }

            string horaires = Horaires.Text.ToString();
            int taille = horaires.Length;

            if (taille != 13)
            {
                DisplayAlert("Problème de format", "Veuillez saisir de nouveaux horaires respectant la nomenclature 00h00 - 00h00", "Ok");
                return;
            }

            var heureDeb = Horaires.Text.Substring(0, 5);
            var duree = Horaires.Text.Substring(8, 5);
            var niveau = Picker.SelectedItem.ToString();

            // création de l'activité et ajout dans la BD
            // ne push pas dans la BD

            string contentType = "application/json";
            var actTerminee = false;

            JObject json = new JObject
                {
                    // récupérer l'id de la personne connectée
                    { "titre", nom },
                    { "date", dateAct },
                    { "heure_debut", heureDeb },
                    { "duree", duree },
                    { "lieu", lieu },
                    { "tarif", tarif },
                    { "niveau", niveau },
                    { "nbr_participant", nbTeam },
                    { "activite_terminee", actTerminee }
                    // récupérer l'id de l'activité
                };

            
            var content2 = new StringContent(json.ToString(), Encoding.UTF8, contentType);
            await client.PostAsync(urlAct, content2);

            await Navigation.PushAsync(new pageAccueil()); // doit renvoyer sur la page de consultation de l'activité pageActiviteGratuite(id)
        }
    }
}