using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager instance;

    public static CameraManager Instance => instance;

    private CinemachineImpulseListener impulseListener;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        DontDestroyOnLoad(gameObject);

        impulseListener = Camera.main.GetComponent<CinemachineImpulseListener>();
    }

    public void Impulse(float gain)
    {
        impulseListener.m_Gain = gain;
    }
}
