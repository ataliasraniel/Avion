using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class FlightCameraController : MonoBehaviour
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

    [Header("Aim")]
    public bool aim;
    public Transform aimCameraPosition;
    private Vector3 defaultRigPosition;
    public float timeBetweenSwitch = 0.2f;
    public int aimFov = 25;



    private void Start()
    {
        normalFov = flightCamera.fieldOfView;
        defaultRigPosition = flightCamera.transform.localPosition;
    }

    public void StartAimCamera()
    {
        flightCamera.DOFieldOfView(aimFov, timeBetweenSwitch);
        flightCamera.transform.DOMove(aimCameraPosition.position, timeBetweenSwitch);
    }

    public void BoostFOV()
    {
        flightCamera.DOFieldOfView(desiredFov, boostLerpTime);
    }

    public void ResetShotCamera()
    {
        aim = false;
        flightCamera.DOFieldOfView(60, boostLerpTime);
        flightCamera.transform.DOLocalMove(defaultRigPosition, timeBetweenSwitch);
    }
    public void ResetFOV()
    {
        flightCamera.DOFieldOfView(60, boostLerpTime);
    }
}
