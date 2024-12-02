using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    [SerializeField] private float globalShakeForce = 1f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void CameraShaking(CinemachineImpulseSource impulseSource)
    {
        if (instance != null)
        {
            impulseSource.GenerateImpulseWithForce(globalShakeForce);
        }
    }
}
