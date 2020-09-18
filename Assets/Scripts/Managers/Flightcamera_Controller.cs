using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Flightcamera_Controller : MonoBehaviour
{
    ///<summary>
    //este script dará conta de todos os movimentos e alterações
    //na câmera de voo
    ///</summary>

    [Header("Cameras")]
    public Camera flightCamera;

    [Header("Boost Camera")]
    private float normalFov;
    public float desiredFov;
    public float boostLerpTime;

    [Header("Mirar camera")]
    public bool mirar;
    public float mirarLerpTime;
    public int mirarFov;
    public float xTweenTime;
    public bool snapCamera = false;
    public Transform rigPos;
    public Vector3 startRigPos;



    private void Start()
    {
        normalFov = flightCamera.fieldOfView;
    }

    public void Mirar()
    {
        flightCamera.DOFieldOfView(mirarFov, mirarLerpTime);
    }

    public void BoostFOV()
    {
        flightCamera.DOFieldOfView(desiredFov, boostLerpTime);
    }

    public void ResetShotCamera()
    {
        mirar = false;
        flightCamera.DOFieldOfView(60, boostLerpTime);
    }
    public void ResetFOV()
    {
        flightCamera.DOFieldOfView(60, boostLerpTime);
    }
}
