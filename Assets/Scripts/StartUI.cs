using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _highScoreText;

    private void Start()
    {
        var highScore = PlayerPrefs.GetInt("Highscore", 0);
        _highScoreText.text = $"High score: {highScore}";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopPlaying();
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void StopPlaying()
    {
        Application.Quit();
    }
}
