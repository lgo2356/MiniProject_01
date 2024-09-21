using UnityEngine;

public partial class Enemy
{
    private bool isComboEnabled = false;
    private bool isComboExist = false;

    private int comboIndex = 0;

    public int ComboIndex { get => comboIndex; }

    private void Begin_Combo()
    {
        isComboEnabled = true;
    }

    private void End_Combo()
    {
        isComboEnabled = false;
    }

    private void Begin_Collision()
    {
        sword.EnableCollision();
    }

    private void End_Collision()
    {
        sword.DisableCollision();
    }

    private void Riposte_Hit()
    {
        
    }
}
