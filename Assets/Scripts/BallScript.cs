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
        if (Input.GetKeyDown(KeyCode.Space) == true && ballIsAlive == true && Time.timeScale > 0)
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
            else
            {
                if (flapSound == null) Debug.LogWarning("BallScript: 'Flap Sound' is missing! Assign an AudioClip in the Inspector.");
            }
        }

        // Check if ball goes out of screen
        if (transform.position.y > 50 || transform.position.y < -50 || transform.position.x < -10)
        {
            if (ballIsAlive) // Only trigger game over once
            {
                logic.gameOver();
                ballIsAlive = false;
            }
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