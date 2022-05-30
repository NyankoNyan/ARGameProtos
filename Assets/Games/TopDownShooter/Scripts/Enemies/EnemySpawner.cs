using System;
using UnityEngine;
using Zenject;

namespace Games.TDS
{
    public class EnemySpawner : ITickable, IInitializable
    {
        private Settings _settings;
        private Enemy.Factory _enemyFactory;
        private PlayerRegistry _playerRegistry;
        private float _spawnTimer;

        public EnemySpawner(Settings settings, Enemy.Factory enemyFactory, PlayerRegistry playerRegistry)
        {
            _settings = settings;
            _enemyFactory = enemyFactory;
            _playerRegistry = playerRegistry;
        }

        public void Initialize()
        {
            _spawnTimer = _settings.TimeBeforeFirstSpawn;
        }

        public void Tick()
        {
            if (!_playerRegistry.Player) {
                return;
            }
            _spawnTimer -= Time.deltaTime;

            if (_spawnTimer <= 0) {
                Enemy newEnemy = _enemyFactory.Create();

                float angle = UnityEngine.Random.Range( 0f, 360f );
                Vector3 distance = Quaternion.AngleAxis( angle, Vector3.up ) * Vector3.forward * _settings.SpawnDistance;
                newEnemy.transform.position = _playerRegistry.Player.Position + distance;

                _spawnTimer = _settings.TimeBetweenSpawns;
            }
        }

        [Serializable]
        public class Settings
        {
            public float TimeBeforeFirstSpawn = 3;
            public float TimeBetweenSpawns = 2;
            public float SpawnDistance = 10;
        }
    }
}