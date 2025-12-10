using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class LogicScriptTests
{
    private GameObject logicGO;
    private LogicScript logic;

    [SetUp]
    public void Setup()
    {
        // Create a GameObject with the LogicScript attached before each test
        logicGO = new GameObject("LogicManager");
        logic = logicGO.AddComponent<LogicScript>();
        
        // Mock dependencies
        logic.scoreText = new GameObject("ScoreText").AddComponent<Text>();
        logic.scoreLabel = new GameObject("ScoreLabel").AddComponent<Text>();
        logic.gameOverScreen = new GameObject("GameOverScreen");
        logic.startScreen = new GameObject("StartScreen");
        logicGO.AddComponent<AudioSource>(); // Add AudioSource as it's required by LogicScript
    }

    [TearDown]
    public void Teardown()
    {
        // Clean up after each test
        Object.DestroyImmediate(logicGO);
    }

    // This test verifies that the player's score is initialized to zero when the LogicScript starts.
    [Test]
    public void Score_Starts_At_Zero()
    {
        Assert.AreEqual(0, logic.playerScore);
    }

    // This test checks if calling the AddScore method correctly increases the playerScore
    // and updates the scoreText UI element.
    [Test]
    public void AddScore_Increases_Score()
    {
        // Act
        logic.addScore(1);

        // Assert
        Assert.AreEqual(1, logic.playerScore);
        Assert.AreEqual("1", logic.scoreText.text);
    }

    // This test ensures that the score does not increase when the game is marked as inactive (e.g., game over).
    [Test]
    public void AddScore_Does_Not_Increase_When_Game_Inactive()
    {
        // Arrange
        logic.isGameActive = false;

        // Act
        logic.addScore(1);

        // Assert
        Assert.AreEqual(0, logic.playerScore);
    }

    // This test verifies that the game is initially set to an active state.
    [Test]
    public void Game_Is_Active_By_Default()
    {
        Assert.IsTrue(logic.isGameActive);
    }

    // This test checks if the GameOver method correctly sets the game to an inactive state
    // and activates the game over screen UI.
    [Test]
    public void GameOver_Sets_GameInactive()
    {
        // Act
        logic.gameOver();

        // Assert
        Assert.IsFalse(logic.isGameActive);
        Assert.IsTrue(logic.gameOverScreen.activeSelf);
    }
}
