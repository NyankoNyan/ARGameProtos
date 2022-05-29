using MyBox;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Games.TDS
{
    public class BulletSpawnService
    {
        public delegate void BulletSpawned(Bullet bullet);
        public event BulletSpawned BulletSpawnedEvent;

        private readonly Bullet.Factory _bulletFactory;

        public BulletSpawnService(
            Bullet.Factory bulletFactory
            )
        {
            _bulletFactory = bulletFactory;
        }

        public void Spawn(GameObject shooter, Vector3 spawnPoint, Quaternion spawnRotation)
        {
            Bullet newBullet = _bulletFactory.Create();
            newBullet.transform.position = spawnPoint;
            newBullet.transform.rotation = spawnRotation;
        }
    }
}
