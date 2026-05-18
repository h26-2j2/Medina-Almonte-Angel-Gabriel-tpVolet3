using UnityEngine;
using UnityEngine.InputSystem;

public class Personnage : MonoBehaviour
{
    private Vector2 positionDepart;

    //configurations des contrôles et des entrées joueur
    [Header("Gestion bouton")]
    public InputAction actionDeplacement;
    public InputAction actionSaut;
    public InputAction actionAppuyer;
    public InputAction actionTirer;

    private GameObject boutonActuel;

    //composants internes physiques et visuels
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;

    //paramètres pour la course et la direction
    [Header("Deplacement Horizontale")]
    public float vitesseDeplacement;
    private float directionInput;

    //force appliquée lors d'un saut
    [Header("ForceSaut")]
    public float forceSaut;

    //variables pour la gestion des tirs de shurikens
    [Header("Tire")]
    public float delaiTirMin = 1f;
    public float tempsEntreTir = 0f;
    public GameObject prefabProjectile;
    public Transform positionProjectile;
    public float directionProjectile;
    public float vitesseProjectile;

    //variables pour la détection du sol et l'état de saut
    [Header("Saut & Détection Sol")]
    private bool souhaiteSauter;
    public bool estAuSol;

    public Transform detecteurSol;
    public float rayonDetection = 0.2f;
    public LayerMask layerSol;

    //clips et sources pour les effets sonores
    [Header("Audio d'interaction")]
    public AudioSource sourceAudio;
    public AudioClip sonBouton;
    public AudioClip sonLanceShuriken;


    void OnEnable()
    {
        actionDeplacement.Enable();
        actionSaut.Enable();
        actionAppuyer.Enable();
        actionTirer.Enable();
    }


    void OnDisable()
    {
        actionDeplacement.Disable();
        actionSaut.Disable();
        actionAppuyer.Disable();
        actionTirer.Disable();
    }


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();

        positionDepart = transform.position;
    }


    void Update()
    {
        //mise à jour de la détection du sol
        if (detecteurSol != null)
        {
            estAuSol = Physics2D.OverlapCircle(detecteurSol.position, rayonDetection, layerSol);
        }

        directionInput = actionDeplacement.ReadValue<float>();

        //vérification de sauter
        if (actionSaut.WasPressedThisFrame() && estAuSol)
        {
            souhaiteSauter = true;
            estAuSol = false; // bloque immédiatement le double saut avant la mise à jour physique
        }

        //action d'appuyer sur un bouton interactif
        if (actionAppuyer.WasPressedThisFrame() && boutonActuel != null)
        {
            ValiderReponse();
        }

        //ajustements visuels et retournement du sprite selon la direction
        if (directionInput < 0)
        {
            sr.flipX = true;
            directionProjectile = -1.5f;
            Vector2 nouvellePositionProjectile = positionProjectile.localPosition;
            nouvellePositionProjectile.x = nouvellePositionProjectile.x * -1;
            positionProjectile.localPosition = nouvellePositionProjectile;
        }
        else if (directionInput > 0)
        {
            sr.flipX = false;
            directionProjectile = 1.5f;
            Vector2 nouvellePositionProjectile = positionProjectile.localPosition;
            nouvellePositionProjectile.x = Mathf.Abs(nouvellePositionProjectile.x);
            positionProjectile.localPosition = nouvellePositionProjectile;
        }

        //mise à jour des animations de course
        anim.SetFloat("vitesse", Mathf.Abs(rb.linearVelocityX));

        //gestion du temps de recharge du tir
        if (tempsEntreTir > 0)
        {
            tempsEntreTir -= Time.deltaTime;
        }

        //création du projectile et activation du son de tir
        if (actionTirer.WasPressedThisFrame() == true && tempsEntreTir <= 0)
        {
            GameObject clone = Instantiate(prefabProjectile, positionProjectile.position, positionProjectile.rotation);
            clone.GetComponent<Projectile>().direction = directionProjectile;
            tempsEntreTir = delaiTirMin;

            if (sourceAudio != null && sonLanceShuriken != null)
            {
                sourceAudio.PlayOneShot(sonLanceShuriken);
            }

            Vector2 nouvellePositionDepart = positionProjectile.position;
            nouvellePositionDepart.x = 1.5f;

            if (sr.flipX == true)
            {
                directionInput = -1;
                nouvellePositionDepart.x = -1.5f;
            }
        }
    }

    //réinitialisation du personnage en cas de défaite ou chute
    public void Respawn()
    {
        rb.linearVelocity = Vector2.zero;
        transform.position = positionDepart;
        Debug.Log("Ninja réinitialisé à sa position de départ !");
    }

    //détection d'entrée dans la zone d'un bouton de réponse
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("bouton"))
        {
            boutonActuel = other.gameObject;
        }
    }

    //détection de sortie de la zone d'un bouton de réponse
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("bouton"))
        {
            boutonActuel = null;
        }
    }

    //envoi de l'ordre de validation au script de la boîte touchée
    void ValiderReponse()
    {
        if (sourceAudio != null && sonBouton != null)
            sourceAudio.PlayOneShot(sonBouton);

        reponsePossible scriptBulle = boutonActuel.GetComponent<reponsePossible>();
        if (scriptBulle == null) scriptBulle = boutonActuel.GetComponentInChildren<reponsePossible>();

        if (scriptBulle != null)
        {
            scriptBulle.PrendreCetteReponse();
        }
        else
        {
            Debug.LogError("Le bouton touché n'a pas de script reponsePossible !");
        }
    }

    //gestion des déplacements physiques et des forces de saut
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(directionInput * vitesseDeplacement, rb.linearVelocity.y);

        if (souhaiteSauter)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            Debug.Log("Saut exécuté !");
            rb.AddForce(Vector2.up * forceSaut, ForceMode2D.Impulse);
            souhaiteSauter = false;
        }
    }
}