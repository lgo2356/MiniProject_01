using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Enemy : Weapon
{
    [SerializeField]
    private WeaponActionData[] actionDatas;

    public Action<DamageResult> DamageResultCallback;

    /**
     * Compoent
     */
    private new Collider collider;
    private CinemachineImpulseSource impulseSource;

    /**
     * Valuable
     */
    private GameObject ownerObject;
    private List<GameObject> hittedList;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        impulseSource = GetComponent<CinemachineImpulseSource>();

        ownerObject = transform.root.gameObject;
        hittedList = new();
    }

    private void Start()
    {
        DisableCollision();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == ownerObject)
            return;

        foreach (GameObject go in hittedList)
        {
            if (go == other.gameObject)
                return;
        }

        hittedList.Add(other.gameObject);

        IDamagable damagable = other.gameObject.GetComponent<IDamagable>();
        Enemy enemy = ownerObject.GetComponent<Enemy>();

        if (enemy != null && damagable != null)
        {
            WeaponActionData actionData = actionDatas[enemy.ComboIndex];

            if (impulseSource != null && actionData.CameraShakeDuration > 0f && actionData.CameraShakeDirection.magnitude > 0f)
            {
                impulseSource.m_ImpulseDefinition.m_ImpulseDuration = actionData.CameraShakeDuration;
                impulseSource.m_DefaultVelocity = actionData.CameraShakeDirection;

                impulseSource.GenerateImpulse(actionDatas[enemy.ComboIndex].CameraShakeDuration);
            }

            Vector3 hitPoint = collider.ClosestPoint(other.transform.position);  // World (로컬 * 월드 * 뷰 * 프로젝션)
            hitPoint = other.transform.InverseTransformPoint(hitPoint);  // 로컬 * 월드 * 월드 역행렬 => 로컬

            damagable.Damage(ownerObject, null, hitPoint, actionData, DamageResultCallback);
        }
    }

    public void EnableCollision()
    {
        collider.enabled = true;
    }

    public void DisableCollision()
    {
        collider.enabled = false;

        hittedList.Clear();
    }
}
