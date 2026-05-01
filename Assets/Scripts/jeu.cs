using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class jeu : MonoBehaviour
{
    [Header("Element UI")]
    public TextMeshProUGUI texteQuestion;
    public Image feedbackOverlay;

    [Header("Bulle-reponse")]
    public reponsePossible bulleGauche;
    public reponsePossible bulleCentre;
    public reponsePossible bulleDroite;

    [Header("Audio")]
    public AudioSource sourceAudio;
    public AudioClip sonCorrect;
    public AudioClip sonErreur;
    public AudioClip sonDebutJeu;
    public AudioClip SonCorrecteVoix;
    public AudioClip sonErreurVoix;
    public AudioClip SonFinJeu;

    [Header("Score")]
    public int scoreReussi = 0;
    public int scoreObjectif = 5;

   
    private float tempsRestantFlash = 0f;
    private bool niveauTermine = false;
    private float tempsAvantMenu = 0f;

    public int reponseCorrecte;

    void Start()
    {
        GenererQuestion();
        sourceAudio.PlayOneShot(sonDebutJeu);
    }

    void Update()
    {
      
        if (tempsRestantFlash > 0)
        {
            tempsRestantFlash -= Time.deltaTime; // Count down

            if (tempsRestantFlash <= 0)
            {
                feedbackOverlay.color = new Color(0, 0, 0, 0); 
                GenererQuestion();
            }
        }

        
        if (niveauTermine)
        {
            tempsAvantMenu -= Time.deltaTime;
            if (tempsAvantMenu <= 0)
            {
                SceneManager.LoadScene("EcranTitre");
            }
        }
    }

    public void GenererQuestion()
    {
        int a = Random.Range(1, 10);
        int b = Random.Range(1, 10);
        reponseCorrecte = a + b;
        texteQuestion.text = a + " + " + b + " = ?";

        bulleGauche.texteAffichage.text = reponseCorrecte.ToString();
        bulleCentre.texteAffichage.text = (reponseCorrecte + 3).ToString();
        bulleDroite.texteAffichage.text = (reponseCorrecte - 2).ToString();
    }

    public void Valider(int valeurChoisie)
    {
        
        if (tempsRestantFlash > 0 || niveauTermine) return;

        if (valeurChoisie == reponseCorrecte)
        {
            
            sourceAudio.PlayOneShot(sonCorrect);
            sourceAudio.PlayOneShot(SonCorrecteVoix);
            scoreReussi++;

            if (scoreReussi >= scoreObjectif)
            {
                // temp pour changement de scene
                niveauTermine = true;
                sourceAudio.PlayOneShot(SonFinJeu);
                tempsAvantMenu = 6.0f;
                feedbackOverlay.color = new Color(0, 1, 0, 0.8f);
            }
            else
            {
                //temp pour flash
                feedbackOverlay.color = new Color(0, 1, 0, 0.5f);
                tempsRestantFlash = 0.6f;
            }
        }
        else
        {
            sourceAudio.PlayOneShot(sonErreur);
            sourceAudio.PlayOneShot(sonErreurVoix);
            feedbackOverlay.color = new Color(1, 0, 0, 0.5f);
            tempsRestantFlash = 0.6f;
        }
    }
}