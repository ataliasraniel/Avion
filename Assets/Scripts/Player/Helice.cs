using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helice : MonoBehaviour
{
    public Transform heliceModel;
    public float rotationSpeed;
    public float degrees;

    void Start()
    {

    }
    private void Update()
    {
        Rotate();
    }
    void Rotate()
    {
        //faz com que a hélice do avião gire        
        //heliceModel.Rotate(new Vector3(0,0, degrees) * rotationSpeed * Time.deltaTime, Space.Self);
        rotationSpeed = Mathf.Clamp(rotationSpeed, 1, 24);
        heliceModel.Rotate(0, 0, degrees * rotationSpeed * Time.deltaTime, Space.Self);
    }
}
