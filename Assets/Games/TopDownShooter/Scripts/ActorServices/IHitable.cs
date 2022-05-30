using UnityEngine;

namespace Games.TDS
{
    public interface IHitable
    {
        void GetDamage(float damage);
        Vector3 Position { get; }
        void GetImpact(Vector3 impactForce);
    }
}