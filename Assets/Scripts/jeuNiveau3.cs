using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class jeuNiveau3 : MonoBehaviour
{
    //composants graphiques de l'interface pour les textes et les retours visuels
    [Header("Element UI")]
    public TextMeshProUGUI texteQuestion;
    public Image feedbackOverlay;

    //tableau stockant l'ensemble des boîtes et boutons de réponses du niveau final
    [Header("Boites de Réponses (Boutons + Caisses)")]
    public reponsePossible[] boitesReponses; 

    //pistes audio pour les alertes sonores principales du jeu
    [Header("Audio (Sons fixes)")]
    public AudioSource sourceAudio;
    public AudioClip sonDebutJeu;
    public AudioClip sonCorrect;  
    public AudioClip sonErreur;    
    public AudioClip SonFinJeu;

    //banques de sons pour jouer des messages vocaux aléatoires
    [Header("Audio Aléatoire (Voix)")]
    public AudioClip[] sonsCorrectsVoix; 
    public AudioClip[] sonsErreursVoix;   

    //suivi du score de l'élève et définition de l'objectif de fin de partie
    [Header("Score")]
    public int scoreReussi = 0;
    public int scoreObjectif = 6; 

    //nom de la scène vers laquelle basculer une fois le jeu complètement terminé
    [Header("Fin de partie")]
    public string nomSceneAccueil = "EcranTitre"; 

    //compteurs de temps pour réguler les animations visuelles et les transitions
    private float tempsRestantFlash = 0f;
    private bool niveauTermine = false;
    private float tempsAvantMenu = 0f;

    public int reponseCorrecte;

    //initialisation de la question de départ et activation de l'ambiance sonore
    void Start()
    {
        GenererQuestion(); 
        if (sourceAudio != null && sonDebutJeu != null)
        {
            sourceAudio.PlayOneShot(sonDebutJeu);
        }
    }

    //actualisation des différents chronomètres de transition de scènes et de couleurs
    void Update()
    {
        //gestion du retour à l'écran d'accueil après la victoire finale
        if (niveauTermine)
        {
            tempsAvantMenu -= Time.deltaTime;
            if (tempsAvantMenu <= 0)
            {
                SceneManager.LoadScene(nomSceneAccueil);
            }
            return;
        }

        //gestion de l'extinction du panneau de couleur après une validation
        if (tempsRestantFlash > 0)
        {
            tempsRestantFlash -= Time.deltaTime;
            if (tempsRestantFlash <= 0)
            {
                if (feedbackOverlay != null) feedbackOverlay.color = new Color(0, 0, 0, 0); 
                GenererQuestion(); 
            }
        }
    }

    //génération d'une nouvelle addition avec sélection et mélange des options numériques
    public void GenererQuestion()
    {
        //réactivation globale des boîtes masquées lors de la question précédente
        foreach (reponsePossible boite in boitesReponses)
        {
            if (boite != null) boite.RéinitialiserBoite(); 
        }

        //création des nombres de l'équation mathématique
        int a = Random.Range(1, 10);
        int b = Random.Range(1, 10);
        reponseCorrecte = a + b;
        if (texteQuestion != null) texteQuestion.text = a + " + " + b + " = ?";

        //intégration du résultat légitime et calcul d'options alternatives erronées
        List<int> valeursReponses = new List<int>();
        valeursReponses.Add(reponseCorrecte);
        valeursReponses.Add(reponseCorrecte + Random.Range(1, 4));
        valeursReponses.Add(reponseCorrecte - Random.Range(1, 3));
        valeursReponses.Add(reponseCorrecte + Random.Range(5, 8));

        //mélange aléatoire de l'emplacement des nombres dans la liste
        for (int i = 0; i < valeursReponses.Count; i++)
        {
            int temp = valeursReponses[i];
            int randomIndex = Random.Range(i, valeursReponses.Count);
            valeursReponses[i] = valeursReponses[randomIndex];
            valeursReponses[randomIndex] = temp;
        }

        //distribution sécurisée des chiffres mélangés sur l'affichage de nos boîtes
        for (int i = 0; i < boitesReponses.Length; i++)
        {
            if (i < valeursReponses.Count && boitesReponses[i] != null)
            {
                boitesReponses[i].valeurAssignee = valeursReponses[i]; 
                if (boitesReponses[i].texteAffichage != null)
                {
                    boitesReponses[i].texteAffichage.text = valeursReponses[i].ToString(); 
                }
            }
        }
    }

    //réception du choix utilisateur, traitement du score et diffusion des récompenses audio
    public void Valider(int valeurChoisie)
    {
        if (tempsRestantFlash > 0 || niveauTermine) return;

        //scénario en cas de succès à la question
        if (valeurChoisie == reponseCorrecte)
        {
            if (sourceAudio != null && sonCorrect != null) sourceAudio.PlayOneShot(sonCorrect);
            
            //tirage au sort d'un enregistrement vocal d'encouragement
            if (sonsCorrectsVoix != null && sonsCorrectsVoix.Length > 0 && sourceAudio != null)
            {
                int indexAleatoire = Random.Range(0, sonsCorrectsVoix.Length);
                sourceAudio.PlayOneShot(sonsCorrectsVoix[indexAleatoire]);
            }

            scoreReussi++;

            //évaluation de la condition de réussite finale du jeu complet
            if (scoreReussi >= scoreObjectif)
            {
                niveauTermine = true;
                if (sourceAudio != null && SonFinJeu != null) sourceAudio.PlayOneShot(SonFinJeu);
                tempsAvantMenu = 4.0f;
                if (feedbackOverlay != null) feedbackOverlay.color = new Color(0, 1, 0, 0.8f);
            }
            else
            {
                if (feedbackOverlay != null) feedbackOverlay.color = new Color(0, 1, 0, 0.5f);
                tempsRestantFlash = 1.0f;
            }
        }
        //scénario en cas d'erreur de calcul
        else
        {
            if (sourceAudio != null && sonErreur != null) sourceAudio.PlayOneShot(sonErreur);
            
            //enregistrement vocal random indiquant une mauvaise réponse
            if (sonsErreursVoix != null && sonsErreursVoix.Length > 0 && sourceAudio != null)
            {
                int indexAleatoire = Random.Range(0, sonsErreursVoix.Length);
                sourceAudio.PlayOneShot(sonsErreursVoix[indexAleatoire]);
            }

            if (feedbackOverlay != null) feedbackOverlay.color = new Color(1, 0, 0, 0.5f);
            tempsRestantFlash = 1.0f;
        }
    }
}