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
    public bool isGameActive = true;

    public AudioClip startMusic;
    public AudioClip gameMusic;
    public AudioClip gameOverMusic;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayMusic(startMusic);

        Time.timeScale = 0;
        startScreen.SetActive(true);
        gameOverScreen.SetActive(false);
        if(scoreText != null) scoreText.gameObject.SetActive(false);
        if(scoreLabel != null) scoreLabel.gameObject.SetActive(false);
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