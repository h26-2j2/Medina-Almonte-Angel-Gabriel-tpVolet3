using UnityEngine;

public class zoneMort : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // On vérifie si c'est le joueur qui est tombé dans le vide
        // Note : Assure-toi que ton personnage a bien le Tag "Player" dans l'Inspecteur !
        if (collision.CompareTag("Player"))
        {
            // On récupère le script Personnage sur l'objet qui a recu la collision
            Personnage joueur = collision.GetComponent<Personnage>();

            if (joueur != null)
            {
                // On appelle la fonction de téléportation
                joueur.Respawn();
            }
        }
    }
}

