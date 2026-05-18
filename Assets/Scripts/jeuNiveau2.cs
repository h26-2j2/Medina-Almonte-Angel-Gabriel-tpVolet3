using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class jeuNiveau2 : MonoBehaviour
{
    //composants de l'interface utilisateur pour afficher les textes et les flashs
    [Header("Element UI")]
    public TextMeshProUGUI texteQuestion;
    public Image feedbackOverlay;

    //tableau contenant toutes les boîtes de réponses du niveau
    [Header("Boites de Réponses (Array)")]
    public reponsePossible[] boitesReponses;

    //fichiers audio fixes pour les événements principaux
    [Header("Audio (Sons fixes)")]
    public AudioSource sourceAudio;
    public AudioClip sonDebutJeu;
    public AudioClip sonCorrect;
    public AudioClip sonErreur;
    public AudioClip SonFinJeu;

    //listes de fichiers audio pour jouer des voix aléatoires
    [Header("Audio Aléatoire (Voix/Messages)")]
    public AudioClip[] sonsCorrectsVoix;
    public AudioClip[] sonsErreursVoix;

    //variables pour suivre le score actuel et le but à atteindre
    [Header("Score")]
    public int scoreReussi = 0;
    public int scoreObjectif = 5;

    //chronomètres pour gérer les transitions visuelles et le changement de scène
    private float tempsRestantFlash = 0f;
    private bool niveauTermine = false;
    private float tempsAvantMenu = 0f;

    public int reponseCorrecte;

    //lancement de la première équation et du son d'ambiance au départ
    void Start()
    {
        GenererQuestion();
        sourceAudio.PlayOneShot(sonDebutJeu);
    }

    //mise à jour des chronomètres et chargement du niveau suivant
    void Update()
    {
        //vérification de la fin du niveau pour charger la scène 3
        if (niveauTermine)
        {
            tempsAvantMenu -= Time.deltaTime;
            if (tempsAvantMenu <= 0)
            {
                SceneManager.LoadScene("niveau3");
            }
            return;
        }

        //gestion de la disparition du flash de couleur
        if (tempsRestantFlash > 0)
        {
            tempsRestantFlash -= Time.deltaTime;
            if (tempsRestantFlash <= 0)
            {
                feedbackOverlay.color = new Color(0, 0, 0, 0);
                GenererQuestion();
            }
        }
    }

    //création d'un nouveau calcul et mélange des réponses dans le tableau de boîtes
    public void GenererQuestion()
    {
        //réinitialisation visuelle et physique de chaque boîte de réponse
        foreach (reponsePossible boite in boitesReponses)
        {
            if (boite != null)
            {
                boite.RéinitialiserBoite();
            }
        }

        // calcul des nombres et de la bonne réponse
        int a = Random.Range(1, 10);
        int b = Random.Range(1, 10);
        reponseCorrecte = a + b;
        texteQuestion.text = a + " + " + b + " = ?";

        //création d'une liste contenant la bonne réponse et trois fausses options
        List<int> valeursReponses = new List<int>();
        valeursReponses.Add(reponseCorrecte);
        valeursReponses.Add(reponseCorrecte + Random.Range(1, 4));
        valeursReponses.Add(reponseCorrecte - Random.Range(1, 3));
        valeursReponses.Add(reponseCorrecte + Random.Range(5, 8));

        //algorithme pour mélanger l'ordre des réponses de manière aléatoire
        for (int i = 0; i < valeursReponses.Count; i++)
        {
            int temp = valeursReponses[i];
            int randomIndex = Random.Range(i, valeursReponses.Count);
            valeursReponses[i] = valeursReponses[randomIndex];
            valeursReponses[randomIndex] = temp;
        }

        //attribution finale des valeurs mélangées aux composants de nos boîtes
        for (int i = 0; i < boitesReponses.Length; i++)
        {
            if (i < valeursReponses.Count)
            {
                boitesReponses[i].valeurAssignee = valeursReponses[i];
                boitesReponses[i].texteAffichage.text = valeursReponses[i].ToString();
            }
        }
    }

    //validation de la réponse reçue, gestion des scores et choix des voix audio
    public void Valider(int valeurChoisie)
    {
        Debug.Log("Validation reçue ! Reçu : " + valeurChoisie + " | Attendu (Correct) : " + reponseCorrecte);

        if (tempsRestantFlash > 0 || niveauTermine) return;

        //actions effectuées si le joueur donne le bon résultat
        if (valeurChoisie == reponseCorrecte)
        {
            sourceAudio.PlayOneShot(sonCorrect);

            //sélection et lecture d'une voix de félicitation aléatoire
            if (sonsCorrectsVoix != null && sonsCorrectsVoix.Length > 0)
            {
                int indexAleatoire = Random.Range(0, sonsCorrectsVoix.Length);
                sourceAudio.PlayOneShot(sonsCorrectsVoix[indexAleatoire]);
            }

            scoreReussi++;

            //validation de la réussite totale du niveau
            if (scoreReussi >= scoreObjectif)
            {
                niveauTermine = true;
                sourceAudio.PlayOneShot(SonFinJeu);
                tempsAvantMenu = 5.0f;
                
            }
            else
            {
                feedbackOverlay.color = new Color(0, 1, 0, 0.5f);
                tempsRestantFlash = 1.0f;
            }
        }
        //actions effectuées si la réponse est incorrecte
        else
        {
            sourceAudio.PlayOneShot(sonErreur);

            //sélection et lecture d'une voix d'erreur aléatoire
            if (sonsErreursVoix != null && sonsErreursVoix.Length > 0)
            {
                int indexAleatoire = Random.Range(0, sonsErreursVoix.Length);
                sourceAudio.PlayOneShot(sonsErreursVoix[indexAleatoire]);
            }

            feedbackOverlay.color = new Color(1, 0, 0, 0.5f);
            tempsRestantFlash = 1.0f;
        }
    }
}