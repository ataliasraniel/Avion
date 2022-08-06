using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class HeliceRotation : MonoBehaviour
{
    public Transform helice;
    public float speed;

    void Update()
    {
        helice.Rotate(new Vector3(speed, this.transform.rotation.y, 0) * Time.deltaTime, Space.Self);
    }
}
