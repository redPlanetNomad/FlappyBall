using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogicScript : MonoBehaviour
{
    public int playerScore;
    public Text scoreText;
    public Text scoreLabel;
    public GameObject gameOverScreen;
    public GameObject startScreen;
    public GameObject cowImage;
    public GameObject balloonImage;
    public GameObject startButton; // New: Reference to the Start Button
    public GameObject restartButton; // New: Reference to the Restart Button
    public GameObject gameTitle; // New: Reference to Game Title
    public GameObject gameSubTitle; // New: Reference to Game SubTitle
    public BallScript ballScript; // Reference to the Ball
    public FrikandelSpawnerScript spawner; // Reference to Spawner

    [Header("Animation Settings")]
    public float cowTargetScale = 17.5f;
    public float cowAnimDuration = 0.2f;
    public float balloonTargetScale = 17.5f;
    public float balloonAnimDuration = 1.5f;
    public float startButtonTargetScale = 1.0f;
    public float startButtonAnimDuration = 0.2f;
    public float restartButtonTargetScale = 1.0f;
    public float restartButtonAnimDuration = 0.2f;
    public float gameTitleTargetScale = 1.0f; // Default scale for title
    public float gameTitleAnimDuration = 0.5f; // Default duration for title
    public float gameSubTitleTargetScale = 1.0f; // Default scale for subtitle
    public float gameSubTitleAnimDuration = 0.5f; // Default duration for subtitle

    public bool isGameActive = true;

    public AudioClip startMusic;
    public AudioClip gameMusic;
    public AudioClip gameOverMusic;
    private AudioSource audioSource;

    private System.Collections.Generic.Dictionary<GameObject, Vector3> initialScales = new System.Collections.Generic.Dictionary<GameObject, Vector3>();

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayMusic(startMusic);
        
        // Find Ball if not assigned
        if (ballScript == null)
        {
            GameObject ballObj = GameObject.FindGameObjectWithTag("Player"); 
            if (ballObj != null) 
            {
                ballScript = ballObj.GetComponent<BallScript>();
            }
            else
            {
                 // Try finding by type as fallback
                 ballScript = FindAnyObjectByType<BallScript>();
                 if (ballScript == null) Debug.LogError("LogicScript: Could not find BallScript! Assign it in Inspector or Tag your ball as 'Player'.");
            }
        }

        // Find Spawner if not assigned
        if (spawner == null)
        {
            GameObject spawnerObj = GameObject.FindGameObjectWithTag("Spawner"); 
            if (spawnerObj != null) 
                spawner = spawnerObj.GetComponent<FrikandelSpawnerScript>();
            else
                spawner = FindAnyObjectByType<FrikandelSpawnerScript>();
            
            if (spawner == null) Debug.LogError("LogicScript: Could not find FrikandelSpawnerScript! Assign it in Inspector.");
        }

        // --- Auto-Find UI References if Missing ---
        if (scoreText == null)
        {
            GameObject obj = GameObject.Find("ScoreText"); // Try finding by standard name
            if (obj != null) scoreText = obj.GetComponent<Text>();
        }
        
        if (scoreLabel == null)
        {
            GameObject obj = GameObject.Find("ScoreLabel"); // Try finding by standard name
            if (obj != null) scoreLabel = obj.GetComponent<Text>();
        }

        // User deleted Start Button, so we skip finding it.
        // If you add it back, assign it in the Inspector.

        if (gameOverScreen != null && restartButton == null)
        {
            Transform btn = gameOverScreen.transform.Find("RestartButton");
            if (btn == null) btn = gameOverScreen.transform.Find("Button");
            if (btn != null) restartButton = btn.gameObject;
        }

        if (startScreen != null && gameTitle == null)
        {
            Transform title = startScreen.transform.Find("GameTitle"); // Try to find a child named "GameTitle"
            if (title == null) title = startScreen.transform.Find("Text"); // Fallback to "Text" if renamed
            if (title != null) gameTitle = title.gameObject;
            if (gameTitle == null) Debug.LogWarning("LogicScript: Could not find GameTitle! Assign it in Inspector or name it 'GameTitle'/'Text'.");
        }

        if (startScreen != null && gameSubTitle == null)
        {
            Transform subTitle = startScreen.transform.Find("GameSubTitle"); // Try to find a child named "GameSubTitle"
            if (subTitle != null) gameSubTitle = subTitle.gameObject;
            if (gameSubTitle == null) Debug.LogWarning("LogicScript: Could not find GameSubTitle! Assign it in Inspector or name it 'GameSubTitle'.");
        }
        // ------------------------------------------

        // Capture initial scales before hiding/modifying anything
        if(cowImage != null) initialScales[cowImage] = cowImage.transform.localScale;
        if(balloonImage != null) initialScales[balloonImage] = balloonImage.transform.localScale;
        if(startButton != null) initialScales[startButton] = startButton.transform.localScale;
        if(restartButton != null) initialScales[restartButton] = restartButton.transform.localScale;
        if(gameTitle != null) initialScales[gameTitle] = gameTitle.transform.localScale; // Capture initial scale
        if(gameSubTitle != null) initialScales[gameSubTitle] = gameSubTitle.transform.localScale; // Capture initial scale

        // Attract Mode: Game is "active" visually, but player isn't controlling
        Time.timeScale = 1; // Keep game running for animation
        startScreen.SetActive(true);
        gameOverScreen.SetActive(false);
        
        if (ballScript != null)
        {
            ballScript.SetAutoPilot(true);
        }

        if (spawner != null)
        {
            spawner.isSpawning = false; // Stop pipes
        }
        
        // Hide score initially
        if(scoreText != null) scoreText.gameObject.SetActive(false);
        if(scoreLabel != null) scoreLabel.gameObject.SetActive(false);

        // Animate Start Button (Only if it exists)
        if (startButton != null)
        {
            StartCoroutine(AnimatePopUp(startButton, startButtonTargetScale, startButtonAnimDuration));
        }

        // Animate Game Title
        if (gameTitle != null)
        {
            StartCoroutine(AnimatePopUp(gameTitle, gameTitleTargetScale, gameTitleAnimDuration));
        }

        // Animate Game SubTitle
        if (gameSubTitle != null)
        {
            StartCoroutine(AnimatePopUp(gameSubTitle, gameSubTitleTargetScale, gameSubTitleAnimDuration));
        }
    }

    [ContextMenu("Increase Score")]
    public void addScore(int scoreToAdd)
    {
        if (isGameActive == true)
        {
            playerScore = playerScore + scoreToAdd;
            scoreText.text = playerScore.ToString();
        }
    }

    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void startGame()
    {
        PlayMusic(gameMusic);
        if(scoreText != null) scoreText.gameObject.SetActive(true);
        if(scoreLabel != null) scoreLabel.gameObject.SetActive(true);
        startScreen.SetActive(false);
        Time.timeScale = 1;
        
        if (ballScript != null)
        {
            ballScript.SetAutoPilot(false);
        }

        if (spawner != null)
        {
            spawner.isSpawning = true; // Start pipes
        }
        
        isGameActive = true; // Ensure logic knows game is real now
    }

    public void gameOver()
    {
        if (isGameActive == false) return; // Prevent re-triggering if already game over

        PlayMusic(gameOverMusic);
        gameOverScreen.SetActive(true);
        if(scoreText != null) scoreText.gameObject.SetActive(false);
        if(scoreLabel != null) scoreLabel.gameObject.SetActive(false);
        isGameActive = false;

        // Animate Cow and Balloon
        StartCoroutine(AnimatePopUp(cowImage, cowTargetScale, cowAnimDuration));
        StartCoroutine(AnimatePopUp(balloonImage, balloonTargetScale, balloonAnimDuration));

        // Animate Restart Button
        StartCoroutine(AnimatePopUp(restartButton, restartButtonTargetScale, restartButtonAnimDuration));
        
        // Background Auto Pilot
        if (ballScript != null)
        {
            // Reset position to roughly center so it doesn't stay crashed
            ballScript.transform.position = new Vector3(0, 0, 0); 
            ballScript.SetAutoPilot(true);
        }

        if (spawner != null)
        {
            spawner.isSpawning = false; // Stop pipes
        }
    }

    System.Collections.IEnumerator AnimatePopUp(GameObject target, float targetScaleMultiplier, float duration)
    {
        if (target != null && initialScales.ContainsKey(target))
        {
            target.transform.localScale = Vector3.zero;
            target.SetActive(true);

            float currentTime = 0f;
            Vector3 baseScale = initialScales[target];
            Vector3 finalScale = baseScale * targetScaleMultiplier;

            while (currentTime < duration)
            {
                currentTime += Time.unscaledDeltaTime; 
                float t = currentTime / duration;
                float scaleFactor = Mathf.Lerp(0f, 1f, t); 
                target.transform.localScale = finalScale * scaleFactor;
                yield return null;
            }
            target.transform.localScale = finalScale; 
        }
    }

    void Update()
    {
        // Check for input to start the game ONLY if the startScreen is active
        if (startScreen != null && startScreen.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) // Spacebar or Left Mouse Click (works for screen tap)
            {
                startGame();
            }
        }
    }

    void PlayMusic(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}