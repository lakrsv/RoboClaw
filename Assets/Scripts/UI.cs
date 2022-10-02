using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;

public class UI : MonoSingleton<UI>
{
    [SerializeField]
    private TextMeshProUGUI _healthText;
    [SerializeField]
    private TextMeshProUGUI _nextWaveText;
    [SerializeField]
    private TextMeshProUGUI _scoreText;
    [SerializeField]
    private TextMeshProUGUI _highScoreText;
    [SerializeField]
    private MechSpiderSpawner _spawner;
    private float _playerHealth = 100f;
    private int _currentScore = 0;
    [SerializeField]
    private Image _damageOverlay;
    [SerializeField]
    private Transform _playerCameraRoot;
    private bool _dying = false;

    private void Start()
    {
        var highScore = PlayerPrefs.GetInt("Highscore", 0);
        _highScoreText.text = $"High score: {highScore}";
    }

    private void Update()
    {
        Debug.Log($"Player Health: {_playerHealth}");
        _healthText.text = $"Health: {(int)_playerHealth}";
        _nextWaveText.text = $"Next wave: {(int)_spawner.SecondsUntilSpawn}";

        _scoreText.text = $"Score: {_currentScore}";

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }

    public void DecrementPlayerHealth(float delta)
    {
        if (_playerHealth == 0f)
        {
            if (!_dying)
            {
                _dying = true;
                StartCoroutine(Die());
            }
            return;
        }
        _playerHealth -= delta;
        if (_playerHealth < 0f)
        {
            _playerHealth = 0f;
        }
        StartCoroutine(AnimateDamageOverlay());
    }

    private IEnumerator Die()
    {
        var targetColor = new Color(255f / 255f, 40f / 255f, 0f, 1f);
        var lerp = 0f;
        while (lerp < 1f)
        {
            _damageOverlay.color = Color.Lerp(_damageOverlay.color, targetColor, lerp);
            lerp += Time.deltaTime * 5f;
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(0);
    }

    private IEnumerator AnimateDamageOverlay()
    {
        _damageOverlay.color = new Color(255f / 255f, 40f / 255f, 0f, 0.125f);
        yield return new WaitForSeconds(0.1f);
        _damageOverlay.color = new Color(255f / 255f, 40f / 255f, 0f, 0f);
    }

    public void AddPlayerScore(int score)
    {
        _currentScore += score;
        var highscore = PlayerPrefs.GetInt("Highscore", 0);
        if (_currentScore > highscore)
        {
            _highScoreText.text = $"High score: {highscore}";

            PlayerPrefs.SetInt("Highscore", _currentScore);
            PlayerPrefs.Save();
        }
    }
}
