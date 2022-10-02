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

    private int _currentSpawns = 1;
    private bool _gameStarted = false;

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
            var spawnpoints = _spawnPoints.OrderBy(a => rng.Next()).ToList();
            for (int i = 0; i < _currentSpawns; ++i)
            {
                var spawnPoint = spawnpoints[i];
                var mechSpider = GameObject.Instantiate(_mechSpiderPrefab);
                mechSpider.transform.position = spawnPoint.position;
                StartCoroutine(mechSpider.StartSpawning());
            }
            _currentSpawns++;
            if(_currentSpawns > MAX_SPAWNS)
            {
                _currentSpawns = MAX_SPAWNS;
            }
            SecondsUntilSpawn = 10f;
            yield return new WaitForSeconds(10f);

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
