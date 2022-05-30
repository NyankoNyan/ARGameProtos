using ModestTree;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Games.TDS
{
    public class Enemy : MonoBehaviour, IPoolable<IMemoryPool>, IHitable
    {
        private IMemoryPool _pool;
        private Rigidbody _rigidbody;

        public delegate void PlayerHitDelegate(Player player);
        public event PlayerHitDelegate PlayerHit;

        [Inject]
        void Construct()
        {
            _rigidbody = GetComponent<Rigidbody>();
            Assert.IsNotNull( _rigidbody );
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.tag == "Player") {
                Player player = collision.transform.GetComponent<Player>();
                Assert.IsNotNull( player );
                PlayerHit?.Invoke( player );
            }
        }

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public void OnDespawned()
        {
            _pool = null;
        }

        public void OnSpawned(IMemoryPool pool)
        {
            _pool = pool;
        }

        public void GetDamage(float damage)
        {

        }

        public void GetImpact(Vector3 impactForce)
        {
            transform.position += Vector3.ProjectOnPlane( impactForce, transform.up );
        }

        public class Factory : PlaceholderFactory<Enemy> { }
        public class Pool : MonoPoolableMemoryPool<IMemoryPool, Enemy> { }
        [Serializable]
        public class Settings
        {
            public float speed = 3;
            public float damage = 20;
        }
    }
}