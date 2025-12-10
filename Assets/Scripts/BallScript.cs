using UnityEngine;
using System.Collections; // Added for IEnumerator

public class BallScript : MonoBehaviour
{

    public Rigidbody2D myRigidbody;
    public float flapStrength;
    public LogicScript logic;
    public bool ballIsAlive = true;

    public GameObject Flag_0;
    public GameObject Flag_1;
    private Coroutine flapCoroutine;

    public AudioClip flapSound;
    private AudioSource audioSource;

    [Header("Auto Pilot Settings")]
    public bool isAutoPilot = false;
    public float autoPilotMinHeight = -1.0f; // Height to trigger flap

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.name = "Mr Bitter Ball";
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
        
        if (audioSource == null)
        {
            Debug.LogError("BallScript: No 'Audio Source' component found on this GameObject! Please add one.");
        }
        
        // Set default state
        if (Flag_0 != null) Flag_0.SetActive(true);
        if (Flag_1 != null) Flag_1.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // User Input
        if (Input.GetKeyDown(KeyCode.Space) == true && ballIsAlive == true && !isAutoPilot && Time.timeScale > 0)
        {
            Flap();
        }

        // Auto Pilot Logic
        if (isAutoPilot && ballIsAlive && Time.timeScale > 0)
        {
            // Only flap if we are below height AND falling (prevents spamming flap every frame)
            if (transform.position.y < autoPilotMinHeight && myRigidbody.linearVelocity.y <= 0)
            {
                Flap();
            }
        }

        // Check if ball goes out of screen
        if (transform.position.y > 50 || transform.position.y < -50 || transform.position.x < -10)
        {
            if (ballIsAlive && !isAutoPilot) // Don't die in auto pilot if slightly out of bounds (though usually we keep it in)
            {
                logic.gameOver();
                ballIsAlive = false;
            }
        }
    }

    public void Flap()
    {
        myRigidbody.linearVelocity = Vector2.up * flapStrength;
            
        // Trigger animation
        if (flapCoroutine != null) StopCoroutine(flapCoroutine);
        flapCoroutine = StartCoroutine(FlapSequence());

        // Play flap sound
        if (flapSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(flapSound);
        }
    }

    public void SetAutoPilot(bool active)
    {
        isAutoPilot = active;
        // Collision remains enabled as requested
        
        if (active)
        {
            ballIsAlive = true; // Resurrect if needed
            myRigidbody.linearVelocity = Vector2.zero; // Reset momentum
        }
    }

    IEnumerator FlapSequence()
    {
        Flag_0.SetActive(false);
        Flag_1.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        Flag_1.SetActive(false);
        Flag_0.SetActive(true);
        flapCoroutine = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        logic.gameOver();
        ballIsAlive = false;
    }
}