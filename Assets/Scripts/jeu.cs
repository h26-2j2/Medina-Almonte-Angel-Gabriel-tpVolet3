using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class jeu : MonoBehaviour
{
    //composants de l'interface utilisateur pour la question et les flashs de couleur
    [Header("Element UI")]
    public TextMeshProUGUI texteQuestion;
    public Image feedbackOverlay;
    public TextMeshProUGUI texteCompteur; // ajout de la case pour glisser le compteur de score (0/5)

    //tableau contenant les bulles de réponses disponibles
    [Header("Bulles de Réponses (Array)")]
    public reponsePossible[] boitesReponses; 

    //liste des effets sonores et voix pour les retours audio
    [Header("Audio")]
    public AudioSource sourceAudio;
    public AudioClip sonCorrect;
    public AudioClip sonErreur;
    public AudioClip sonDebutJeu;
    public AudioClip SonCorrecteVoix;
    public AudioClip sonErreurVoix;
    public AudioClip SonFinJeu;

    //variables pour suivre la progression et l'objectif du niveau
    [Header("Score")]
    public int scoreReussi = 0;
    public int scoreObjectif = 5;

    //variables internes pour la gestion des chronomètres et des transitions
    private float tempsRestantFlash = 0f;
    private bool niveauTermine = false;
    private float tempsAvantMenu = 0f;

    public int reponseCorrecte;

    //initialisation de la première question et du son de départ
    void Start()
    {
        GenererQuestion();
        MettreAJourCompteur(); // affiche 0/5 dès le départ du jeu
        if (sourceAudio != null && sonDebutJeu != null)
        {
            sourceAudio.PlayOneShot(sonDebutJeu);
        }
    }

    //boucle principale pour mettre à jour les chronomètres de flash et de fin de niveau
    void Update()
    {
        // gestion de la durée d'affichage du flash de couleur (vert ou rouge)
        if (tempsRestantFlash > 0)
        {
            tempsRestantFlash -= Time.deltaTime; 

            if (tempsRestantFlash <= 0)
            {
                if (feedbackOverlay != null) feedbackOverlay.color = new Color(0, 0, 0, 0); 
                GenererQuestion();
            }
        }

        //gestion du délai d'attente avant de charger le niveau suivant
        if (niveauTermine)
        {
            tempsAvantMenu -= Time.deltaTime;
            if (tempsAvantMenu <= 0)
            {
                SceneManager.LoadScene("niveau2");
            }
        }
    }

    //fonction simple pour modifier l'affichage du texte à l'écran
    void MettreAJourCompteur()
    {
        if (texteCompteur != null)
        {
            texteCompteur.text = scoreReussi + " / " + scoreObjectif;
        }
    }

    //création d'une addition aléatoire, mélange et répartition des réponses sur le tableau de bulles
    public void GenererQuestion()
    {
        //calcul des nombres et de la bonne réponse
        int a = Random.Range(1, 10);
        int b = Random.Range(1, 10);
        reponseCorrecte = a + b;
        if (texteQuestion != null) texteQuestion.text = a + " + " + b + " = ?";

        //création d'une liste contenant la bonne réponse et deux fausses options
        List<int> valeursReponses = new List<int>();
        valeursReponses.Add(reponseCorrecte);
        valeursReponses.Add(reponseCorrecte + Random.Range(1, 4));
        valeursReponses.Add(reponseCorrecte - Random.Range(1, 3));

        //algorithme pour mélanger l'ordre des réponses de manière aléatoire
        for (int i = 0; i < valeursReponses.Count; i++)
        {
            int temp = valeursReponses[i];
            int randomIndex = Random.Range(i, valeursReponses.Count);
            valeursReponses[i] = valeursReponses[randomIndex];
            valeursReponses[randomIndex] = temp;
        }

        //attribution finale des valeurs mélangées aux composants de nos bulles
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

    //vérification de la réponse choisie par le joueur et application des conséquences
    public void Valider(int valeurChoisie)
    {
        if (tempsRestantFlash > 0 || niveauTermine) return;

        // comportement si la réponse est bonne
        if (valeurChoisie == reponseCorrecte)
        {
            if (sourceAudio != null && sonCorrect != null) sourceAudio.PlayOneShot(sonCorrect);
            if (sourceAudio != null && SonCorrecteVoix != null) sourceAudio.PlayOneShot(SonCorrecteVoix);
            
            scoreReussi++;
            MettreAJourCompteur(); 

            //vérification si l'objectif du niveau est atteint
            if (scoreReussi >= scoreObjectif)
            {
                niveauTermine = true;
                if (sourceAudio != null && SonFinJeu != null) sourceAudio.PlayOneShot(SonFinJeu);
                tempsAvantMenu = 5.0f;
            }
            else
            {
                if (feedbackOverlay != null) feedbackOverlay.color = new Color(0, 1, 0, 0.5f);
                tempsRestantFlash = 0.6f;
            }
        }
        //comportement si la réponse est mauvaise
        else
        {
            if (sourceAudio != null && sonErreur != null) sourceAudio.PlayOneShot(sonErreur);
            if (sourceAudio != null && sonErreurVoix != null) sourceAudio.PlayOneShot(sonErreurVoix);
            
            if (feedbackOverlay != null) feedbackOverlay.color = new Color(1, 0, 0, 0.5f);
            tempsRestantFlash = 0.6f;
        }
    }
}