using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MenuCamera_Animation : MonoBehaviour
{
    //a câmera gira no cenário para dar uma profundidade
    public Transform mainCameraTrans;
    public Vector3 endValue;
    public float duration;
    private void Start()
    {
        AnimateCamera();
    }
    void AnimateCamera()
    {
        mainCameraTrans.DOLocalRotate(endValue, duration, RotateMode.LocalAxisAdd);
    }
}
