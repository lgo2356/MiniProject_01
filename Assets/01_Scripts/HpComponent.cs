using UnityEngine;

public class HpComponent : MonoBehaviour
{
    [SerializeField]
    private float maxHp = 100f;

    private float currentHp;

    public bool IsDead
    {
        get
        {
            return currentHp <= 0f;
        }
    }

    private void Start()
    {
        currentHp = maxHp;
    }

    public void Damage(float amount)
    {
        currentHp += (amount * -1f);
        currentHp = Mathf.Clamp(currentHp, 0f, maxHp);
    }
}
