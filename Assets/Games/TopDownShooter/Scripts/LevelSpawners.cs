using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Games.TDS
{
    public class LevelSpawners : MonoBehaviour
    {
        [SerializeField, MustBeAssigned] Transform _playerSpawner;
        [SerializeField] Transform[] _enemySpawners;

        [SerializeField] Settings _settings;

        public Transform PlayerSpawner { get => _playerSpawner; set => _playerSpawner =  value ; }

        private void OnDrawGizmos()
        {
            if (_playerSpawner) {
                Gizmos.color = _settings.PlayerSpawnerColor;
                Gizmos.DrawWireCube( _playerSpawner.position, Vector3.one * _settings.GizmosSize );
            }

            Gizmos.color = _settings.EnemySpawnerColor;

            foreach (Transform enemySpawner in _enemySpawners) {
                Gizmos.DrawWireCube( enemySpawner.position, Vector3.one * _settings.GizmosSize );
            }
        }

        [Serializable]
        public class Settings
        {
            public Color PlayerSpawnerColor = Color.green;
            public Color EnemySpawnerColor = Color.red;
            public float GizmosSize = .1f;
        }
    }
}
