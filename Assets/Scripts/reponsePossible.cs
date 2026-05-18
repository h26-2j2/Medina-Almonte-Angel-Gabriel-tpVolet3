using UnityEngine;
using TMPro;

public class reponsePossible : MonoBehaviour
{
    //Variable et composant
    [Header("UI Component")]
    public TextMeshProUGUI texteAffichage;

    [HideInInspector] 
    public int valeurAssignee; 

    //Références pour lier la boîte au bon niveau actif
    private jeu gestionnaireNiveau1;
    private jeuNiveau2 gestionnaireNiveau2;
    private jeuNiveau3 gestionnaireNiveau3; 
    
    private SpriteRenderer spriteRenderer;
    private Collider2D monCollider;

    //Initialisation
    void Start()
    {
        //Détecte automatiquement quel gestionnaire de niveau est présent dans la scène
        gestionnaireNiveau1 = Object.FindFirstObjectByType<jeu>();
        gestionnaireNiveau2 = Object.FindFirstObjectByType<jeuNiveau2>(); 
        gestionnaireNiveau3 = Object.FindFirstObjectByType<jeuNiveau3>(); 
        
        //Récupère les composants visuels et physiques de l'objet
        spriteRenderer = GetComponent<SpriteRenderer>();
        monCollider = GetComponent<Collider2D>();
    }

    //Detection de shuriken
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Si un shuriken touche la boîte, on le détruit et on valide la réponse
        if (collision.CompareTag("Shuriken"))
        {
            Destroy(collision.gameObject);
            PrendreCetteReponse();
        }
    }

    //Traitement de la reponse
    public void PrendreCetteReponse()
    {
        //Récupère le texte de la bulle et le transforme en nombre si la valeur numérique est vide
        int valeurAValider = valeurAssignee;
        if (valeurAValider == 0 && texteAffichage != null && !string.IsNullOrEmpty(texteAffichage.text))
        {
            int.TryParse(texteAffichage.text, out valeurAValider);
        }

        //Envoie le résultat au bon script selon le niveau actuel
        if (typeof(jeuNiveau3) != null && gestionnaireNiveau3 != null)
        {
            gestionnaireNiveau3.Valider(valeurAValider);
            CasserBoite(); 
        }
        else if (typeof(jeuNiveau2) != null && gestionnaireNiveau2 != null)
        {
            gestionnaireNiveau2.Valider(valeurAValider);
            CasserBoite(); 
        }
        else if (typeof(jeu) != null && gestionnaireNiveau1 != null)
        {
            gestionnaireNiveau1.Valider(valeurAValider);
        }
        else
        {
            Debug.LogError("Aucun gestionnaire de niveau trouvé pour cette boîte !");
        }
    }

    //Gestion affichage boite
    //Désactive les composants pour faire disparaître la boîte (Niveau 2 et 3)
    public void CasserBoite()
    {
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (monCollider != null) monCollider.enabled = false;
        if (texteAffichage != null) texteAffichage.gameObject.SetActive(false);
    }

    //Réactive les composants pour faire réapparaître la boîte au changement de question
    public void RéinitialiserBoite()
    {
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (monCollider != null) monCollider.enabled = true;
        if (texteAffichage != null) texteAffichage.gameObject.SetActive(true);
    }
}