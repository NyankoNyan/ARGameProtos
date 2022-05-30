using System;
using UnityEngine;

namespace Games.TDS
{
    public class HitService
    {
        private Settings _settings;

        public HitService(Settings settings)
        {
            _settings = settings;
        }

        public void MakeHit(IHitable hitSource, IHitable hitTarget, float value, HitTypes hitType)
        {
            Vector3 normal = ( hitTarget.Position - hitSource.Position ).normalized;
            hitTarget.GetDamage( value );
            hitTarget.GetImpact( value * _settings.DamageToHitForceMult * normal );
            if (hitType == HitTypes.Melee) {
                hitSource.GetImpact( value * _settings.DamageToHitForceMult * -normal );
            }
        }

        [Serializable]
        public class Settings
        {
            public float DamageToHitForceMult = .1f;
        }
    }
}