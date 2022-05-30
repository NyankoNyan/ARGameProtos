using UnityEngine;
using Zenject;

namespace Games.TDS
{
    public class EnemyMover
    {
        private Enemy _enemy;
        private Enemy.Settings _settings;

        [Inject]
        public EnemyMover(Enemy enemy, Enemy.Settings settings)
        {
            _enemy = enemy;
            _settings = settings;
        }

        public void MoveTo(Vector3 point)
        {
            _enemy.Position += _settings.speed * Time.deltaTime * ( point - _enemy.Position ).normalized;
        }
    }
}