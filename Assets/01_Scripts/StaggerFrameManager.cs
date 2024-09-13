using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaggerFrameManager : MonoBehaviour
{
    private static StaggerFrameManager instance;

    public static StaggerFrameManager Instance => instance;

    private Dictionary<int, Animator> animatorTable;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        DontDestroyOnLoad(gameObject);

        animatorTable = new();
    }

    private void Start()
    {
        
    }

    public void Delay(int frame)
    {
        StartCoroutine(Coroutine_StartDelay(frame));
    }

    public void AddAnimator(int key, Animator animator)
    {
        if (animatorTable.ContainsKey(key) == false)
        {
            animatorTable.Add(key, animator);
        }
    }

    private IEnumerator Coroutine_StartDelay(int frame)
    {
        foreach (Animator animator in animatorTable.Values)
        {
            if (animator != null)
                animator.speed = 0f;
        }

        for (int i = 0; i < frame; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        foreach (Animator animator in animatorTable.Values)
        {
            if (animator != null)
                animator.speed = 1f;
        }
    }
}
