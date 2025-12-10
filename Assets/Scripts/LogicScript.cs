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

    [Header("Animation Settings")]
    public float cowTargetScale = 17.5f;
    public float cowAnimDuration = 0.2f;
    public float balloonTargetScale = 17.5f;
    public float balloonAnimDuration = 1.5f;
    public float startButtonTargetScale = 1.0f;
    public float startButtonAnimDuration = 0.2f;
    public float restartButtonTargetScale = 1.0f;
    public float restartButtonAnimDuration = 0.2f;

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

        // Capture initial scales before hiding/modifying anything
        if(cowImage != null) initialScales[cowImage] = cowImage.transform.localScale;
        if(balloonImage != null) initialScales[balloonImage] = balloonImage.transform.localScale;
        if(startButton != null) initialScales[startButton] = startButton.transform.localScale;
        if(restartButton != null) initialScales[restartButton] = restartButton.transform.localScale;

        Time.timeScale = 0;
        startScreen.SetActive(true);
        gameOverScreen.SetActive(false);
        
        // Hide score initially
        if(scoreText != null) scoreText.gameObject.SetActive(false);
        if(scoreLabel != null) scoreLabel.gameObject.SetActive(false);

        // Animate Start Button
        StartCoroutine(AnimatePopUp(startButton, startButtonTargetScale, startButtonAnimDuration));
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
    }

    public void gameOver()
    {
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