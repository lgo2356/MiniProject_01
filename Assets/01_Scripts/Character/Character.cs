using UnityEngine;

public abstract class Character : MonoBehaviour
{
    #region Components
    protected Animator animator;
    protected new Rigidbody rigidbody;
    #endregion

    private void Test()
    {
        print("Git Test");
    }
}
