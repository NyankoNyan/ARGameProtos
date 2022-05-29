using MyBox;
using System;
using UnityEngine;
using Zenject;

namespace Games.TDS
{
    public class Bullet : MonoBehaviour, IPoolable<IMemoryPool>
    {
        private IMemoryPool _pool;
        private Settings _settings;
        private float _lifetime;

        [Inject]
        public void Construct(Settings settings)
        {
            _settings = settings;
        }

        public void OnDespawned()
        {
            _pool = null;
            CreateEffect( _settings.HitEffect );
        }

        private void CreateEffect(GameObject effectPrefab)
        {
            if (effectPrefab) {
                GameObject effect = GameObject.Instantiate( effectPrefab );
                effect.transform.position = transform.position;
                effect.transform.rotation = transform.rotation;
            }
        }

        public void OnSpawned(IMemoryPool pool)
        {
            _pool = pool;
            _lifetime = 0;

            CreateEffect( _settings.SpawnEffect );
        }

        private void OnTriggerEnter(Collider other)
        {
        }

        private void Update()
        {
            transform.position += transform.forward * _settings.Speed * Time.deltaTime;

            _lifetime += Time.deltaTime;

            if (_lifetime >= _settings.Lifetime) {
                _pool.Despawn( this );
            }
        }

        [Serializable]
        public class Settings
        {
            public GameObject SpawnEffect;
            public GameObject HitEffect;
            public float Lifetime = 5f;
            public float Speed = 1f;
        }

        public class Factory : PlaceholderFactory<Bullet> { }

        public class Pool : MonoPoolableMemoryPool<IMemoryPool, Bullet> { }
    }
}
