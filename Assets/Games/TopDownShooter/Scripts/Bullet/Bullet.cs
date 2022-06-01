using MyBox;
using System;
using UnityEngine;
using Zenject;

namespace Games.TDS
{
    public class Bullet : MonoBehaviour, IPoolable<IMemoryPool>, IHitable
    {
        private IMemoryPool _pool;
        private Settings _settings;
        private HitService _hitService;
        private float _lifetime;

        public Vector3 Position => transform.position;

        [Inject]
        public void Construct(Settings settings, HitService hitService)
        {
            _settings = settings;
            _hitService = hitService;
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

        private void OnCollisionEnter(Collision collision)
        {
            if (!this) {
                return;
            }
            var hitable = collision.transform.GetComponent<IHitable>();
            if (hitable != null) {
                _hitService.MakeHit( this, hitable, _settings.Damage, HitTypes.Range );
                _pool.Despawn( this );
            }
        }

        private void Update()
        {
            transform.position += transform.forward * _settings.Speed * Time.deltaTime;

            _lifetime += Time.deltaTime;

            if (_lifetime >= _settings.Lifetime) {
                _pool.Despawn( this );
            }
        }

        public void GetDamage(float damage)
        {

        }

        public void GetImpact(Vector3 impactForce)
        {

        }

        [Serializable]
        public class Settings
        {
            public GameObject SpawnEffect;
            public GameObject HitEffect;
            public float Lifetime = 5f;
            public float Speed = 1f;
            public float Damage = 10f;
        }

        public class Factory : PlaceholderFactory<Bullet> { }

        public class Pool : MonoPoolableMemoryPool<IMemoryPool, Bullet> { }
    }
}
