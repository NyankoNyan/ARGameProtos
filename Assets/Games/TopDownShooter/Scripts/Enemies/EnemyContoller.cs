using Zenject;

namespace Games.TDS
{
    public class EnemyContoller : ITickable, IInitializable
    {
        private Enemy _enemy;
        private PlayerRegistry _playerRegistry;
        private EnemyMover _enemyMover;
        private EnemyStates _state;

        public EnemyContoller(Enemy enemy, PlayerRegistry playerRegistry, EnemyMover enemyMover)
        {
            _enemy = enemy;
            _playerRegistry = playerRegistry;
            _enemyMover = enemyMover;
        }

        public void Initialize()
        {
            _state = EnemyStates.Wait;
        }

        public void Tick()
        {
            switch (_state) {

                case EnemyStates.Wait:

                    if (_playerRegistry.Player && _playerRegistry.Player.Alive) {
                        ChangeState( EnemyStates.Rush );
                    }
                    break;

                case EnemyStates.Rush:

                    if (_playerRegistry.Player && _playerRegistry.Player.Alive) {
                        _enemyMover.MoveTo( _playerRegistry.Player.Position );
                    } else {
                        ChangeState( EnemyStates.Wait );
                    }
                    break;
            }
        }

        private void ChangeState(EnemyStates newState)
        {
            _state = newState;
        }
    }
}