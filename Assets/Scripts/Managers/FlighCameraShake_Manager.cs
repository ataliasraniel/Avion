using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlighCameraShake_Manager : MonoBehaviour
{
    static public FlighCameraShake_Manager instance;

    private void Awake()
    {
        instance = this;
    }

    [Header("Shake leve")]
    public float lStrength = 90f;
    public float lDuration = 0.1f;
    public int lVibrato = 10;
    public float lRandomness = 90f;
    public bool lFade;

    [Header("Shake médio")]
    public float mStrength = 90f;
    public float mDuration = 0.1f;
    public int mVibrato = 10;
    public float mRandomness = 90f;
    public bool mFade;

    [Header("Shake hard")]
    public float hStrength = 90f;
    public float hDuration = 0.1f;
    public int hVibrato = 10;
    public float hRandomness = 90f;
    public bool hFade;

    public Camera flightCamera;

    public void ShakeLow()
    {
        flightCamera.DOShakeRotation(lDuration, lStrength, lVibrato,
        lRandomness, lFade);
    }
    public void ShakeMed()
    {
        flightCamera.DOShakeRotation(mDuration, mStrength, mVibrato,
        mRandomness, mFade);
    }
    public void ShakeHard()
    {
        flightCamera.DOShakeRotation(hDuration, hStrength, hVibrato,
                hRandomness, hFade);
    }
    public void CustomShake(float duration, float strength, int vibrato, float randomness, bool fade)
    {
        flightCamera.DOShakeRotation(duration, strength, vibrato, randomness, fade);
    }
}
