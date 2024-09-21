using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    [SerializeField]
    private WeaponActionData[] actionDatas;

    #region Components
    private new Collider collider;
    private CinemachineImpulseSource impulseSource;
    #endregion

    #region Valuables
    private GameObject ownerObject;
    private List<GameObject> hittedList;
    #endregion

    #region Properties
    public bool IsCritical { get; set; } = false;
    public bool IsRiposte { get; set; } = false;
    #endregion

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
        Player player = ownerObject.GetComponent<Player>();

        if (player != null && damagable != null)
        {
            WeaponActionData actionData = actionDatas[player.ComboIndex];

            if (impulseSource != null && actionData.CameraShakeDuration > 0f && actionData.CameraShakeDirection.magnitude > 0f)
            {
                impulseSource.m_ImpulseDefinition.m_ImpulseDuration = actionData.CameraShakeDuration;
                impulseSource.m_DefaultVelocity = actionData.CameraShakeDirection;
                impulseSource.GenerateImpulse(actionDatas[player.ComboIndex].CameraShakeDuration);
            }

            Vector3 hitPoint = collider.ClosestPoint(other.transform.position);  // World (로컬 * 월드 * 뷰 * 프로젝션)
            hitPoint = other.transform.InverseTransformPoint(hitPoint);  // 로컬 * 월드 * 월드 역행렬 => 로컬

            if (IsCritical)
            {
                IsCritical = false;

                damagable.CriticalDamage(ownerObject, hitPoint, actionData, null);
            }
            else
            {
                damagable.Damage(ownerObject, this, hitPoint, actionData, null);
            }
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
