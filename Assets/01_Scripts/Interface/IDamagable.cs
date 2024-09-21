using System;
using UnityEngine;

[System.Serializable]
public class WeaponActionData
{
    public float Power;
    public float KnockbackDist;

    public int StaggerFrame;
    public float CameraShakeDuration;
    public Vector3 CameraShakeDirection;

    public GameObject Particle;
    public Vector3 ParticleOffset;
    public Vector3 ParticleScale = Vector3.one;
}

public interface IDamagable
{
    void Damage(GameObject attacker, Sword causer, Vector3 hitPoint, WeaponActionData actionData, Action<DamageResult> callback);
    void CriticalDamage(GameObject attacker, Vector3 hitPoint, WeaponActionData actionData, Action<DamageResult> callback);
}
