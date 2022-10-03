using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MechSpiderSpawner : MonoBehaviour
{
    private const int MAX_SPAWNS = 10;

    private static System.Random rng = new System.Random();

    [SerializeField]
    private MechSpider _mechSpiderPrefab;
    [SerializeField]
    private AudioSource _audioSource;

    private Transform[] _spawnPoints;
    // Start is called before the first frame update

    private bool _gameStarted = false;

    private float _minSpeed = 1.5f;
    private float _maxSpeed = 2.0f;
    private float _minTurnSpeed = 45f;
    private float _maxTurnSpeed = 90f;

    public int Waves;
    public float SecondsUntilSpawn = 10;

    void Awake()
    {
        _spawnPoints = new Transform[gameObject.transform.childCount];
        for (int i = 0; i < _spawnPoints.Length; ++i)
        {
            _spawnPoints[i] = gameObject.transform.GetChild(i);
        }
    }

    private void Update()
    {
        if(_gameStarted){
        SecondsUntilSpawn -= Time.deltaTime;
        }
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            Waves++;

            var currentSpawns = Waves;
            currentSpawns = Mathf.Clamp(currentSpawns, 1, MAX_SPAWNS);

            var spawnpoints = _spawnPoints.OrderBy(a => rng.Next()).ToList();
            for (int i = 0; i < currentSpawns; ++i)
            {
                var speed = Random.Range(_minSpeed, _maxSpeed);
                var turningSpeed = Random.Range(_minTurnSpeed, _maxTurnSpeed);

                var spawnPoint = spawnpoints[i];
                var mechSpider = GameObject.Instantiate(_mechSpiderPrefab);
                mechSpider.transform.position = spawnPoint.position;
                StartCoroutine(mechSpider.StartSpawning());
                
                mechSpider.SetStats(speed, turningSpeed);
            }


            SecondsUntilSpawn = 10f;
            yield return new WaitForSeconds(10f);

            _minTurnSpeed += 25f;
            _maxTurnSpeed += 25f;

            _minSpeed += 0.5f;
            _maxSpeed += 0.5f;

            _minSpeed = Mathf.Min(3.5f, _minSpeed);
            _maxSpeed = Mathf.Min(5.0f, _maxSpeed);
            _minTurnSpeed = Mathf.Min(360f, _minTurnSpeed);
            _maxTurnSpeed = Mathf.Min(360f, _maxTurnSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_gameStarted)
        {
            return;
        }
        if (other.CompareTag("Player"))
        {
            _gameStarted = true;
            _audioSource.Play();
            StartCoroutine(SpawnEnemyRoutine());
        }
    }
}
