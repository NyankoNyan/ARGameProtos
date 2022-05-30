using System;
using Zenject;

namespace Games.TDS
{
    public class EnemyMeleeHit : IInitializable, IDisposable
    {
        private Enemy.Settings _settings;
        private HitService _hitService;
        private Enemy _enemy;

        public EnemyMeleeHit(Enemy.Settings settings, HitService hitService, Enemy enemy)
        {
            _settings = settings;
            _hitService = hitService;
            _enemy = enemy;
        }

        public void Dispose()
        {
            _enemy.PlayerHit -= OnPlayerHit;
        }

        public void Initialize()
        {
            _enemy.PlayerHit += OnPlayerHit;
        }

        private void OnPlayerHit(Player player)
        {
            _hitService.MakeHit( _enemy, player, _settings.damage, HitTypes.Melee );
        }
    }
}