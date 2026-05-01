using UnityEngine;
using UnityEngine.InputSystem;

public class Personnage : MonoBehaviour
{
    [Header("Gestion bouton")]
    public InputAction actionDeplacement;
    public InputAction actionSaut;
    public InputAction actionAppuyer;

    private GameObject boutonActuel;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;

    [Header("Deplacement Horizontale")]
    public float vitesseDeplacement;
    private float directionInput;

    [Header("ForceSaut")]
    public float forceSaut;

    [Header("Saut")]
    private bool souhaiteSauter;
    public bool estAuSol;

    [Header("Audio d'interaction")]
    public AudioSource sourceAudio;
    public AudioClip sonBouton;

    void OnEnable()
    {
        actionDeplacement.Enable();
        actionSaut.Enable();
        actionAppuyer.Enable();
    }

    void OnDisable()
    {
        actionDeplacement.Disable();
        actionSaut.Disable();
        actionAppuyer.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        directionInput = actionDeplacement.ReadValue<float>();

        if (actionSaut.WasPressedThisFrame())
        {
            souhaiteSauter = true;
        }


        if (actionAppuyer.WasPressedThisFrame() && boutonActuel != null)
        {
            ValiderReponse();
        }

        // Visuals
        if (directionInput < 0)
        { sr.flipX = true; }

        else if (directionInput > 0)
        { sr.flipX = false; }

        anim.SetFloat("vitesse", Mathf.Abs(rb.linearVelocityX));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("bouton"))
        {
            boutonActuel = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("bouton"))
        {
            boutonActuel = null;
        }
    }

    void ValiderReponse()
    {

        if (sourceAudio != null) sourceAudio.PlayOneShot(sonBouton);


        reponsePossible scriptBulle = boutonActuel.GetComponentInChildren<reponsePossible>();


        if (scriptBulle != null)
        {
            int reponseJoueur = int.Parse(scriptBulle.texteAffichage.text);


            FindAnyObjectByType<jeu>().Valider(reponseJoueur);
        }
    }

    void FixedUpdate()
    {

        rb.linearVelocity = new Vector2(directionInput * vitesseDeplacement, rb.linearVelocity.y);

        if (souhaiteSauter)
        {

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            Debug.Log("allo");
            rb.AddForce(Vector2.up * forceSaut, ForceMode2D.Impulse);
            souhaiteSauter = false;
        }
    }
}