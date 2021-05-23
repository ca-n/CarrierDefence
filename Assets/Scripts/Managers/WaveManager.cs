using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using Types;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class WaveManager : MonoSingleton<WaveManager>
    {
        public EnemyDownedEvent onEnemyDowned;
        public WaveChangedEvent onWaveChanged;
        public WaveClearedEvent onWaveCleared;
        
        public int EnemiesRemaining { get; private set; }
        public int Wave { get; private set; }
        
        [SerializeField] private GameObject[] enemyPrefabs;
        private List<GameObject> _spawnQueue;

        private void Start()
        {
            Wave = 0;
            _spawnQueue = new List<GameObject>();
            onEnemyDowned.AddListener(EnemyDownedHandler);
            onEnemyDowned.AddListener(ScoreManager.Instance.EnemyDownedHandler);
            onEnemyDowned.AddListener(UIManager.Instance.UpdateEnemiesRemainingText);
            onWaveChanged.AddListener(UIManager.Instance.UpdateWaveText);
            onWaveCleared.AddListener(UIManager.Instance.ShowWaveClearedText);
            NextWave();
        }

        public void NextWave()
        {
            ++Wave;
            UpdateSpawnQueue();
            EnemiesRemaining = _spawnQueue.Count;
            onWaveChanged.Invoke(Wave, EnemiesRemaining);
            StartCoroutine(SpawnWave());
        }

        private void UpdateSpawnQueue()
        {
            // Add basic enemy on every wave
            _spawnQueue.Add(enemyPrefabs[0]);
            
            // On multiples of 5, add a harder enemy as well
            /*
            if (Wave % 5 == 0)
            {
                _spawnQueue.Add(enemyPrefabs[1]);
            }
            
            // On multiples of 10, add an even harder enemy
            if (Wave % 10 == 0)
            {
                _spawnQueue.Add(enemyPrefabs[2]);
            }
            */
        }

        private IEnumerator SpawnWave()
        {
            foreach (var enemy in _spawnQueue)
            {
                Instantiate(enemy, GetRandomSpawnPosition(enemy.tag), Quaternion.identity);
                // A bit of math to make planes spawn faster as the game progresses.
                // Planes spawn 10 seconds apart until wave 6, then the time between plane spawns decreases,
                // scaling with progression, until a minimum of 0.5 seconds between spawns starting at level 100
                yield return new WaitForSeconds(Mathf.Clamp(10.0f * (1.0f/(Wave/5.0f)), 0.5f, 10f));
            }
        }

        private Vector3 GetRandomSpawnPosition(string enemyTag)
        {
            Vector2 edge;
            // Loop to cover extremely rare case where we get 0,0
            do
            {
                edge = Random.insideUnitCircle.normalized * 8000;
            } while (edge.magnitude == 0);
            
            return new Vector3(edge.x, GetHeightByTag(enemyTag), edge.y);
        }

        private float GetHeightByTag(string enemyTag)
        {
            switch (enemyTag)
            {
                case "EnemyAwacs":
                    return 1000;
                case "EnemyHawkeye":
                    return 500;
                case "EnemySeahawk":
                    return 250;
                default:
                    Debug.LogError("Invalid enemy prefab tag!");
                    return 0;
            }
        }

        private void EnemyDownedHandler(string enemyTag)
        {
            --EnemiesRemaining;
            if (EnemiesRemaining == 0)
            {
                onWaveCleared.Invoke();
            }
        }
    }
}