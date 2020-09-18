using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletType1 : MonoBehaviour
{
    public float moveSpeed;
    private void Start()
    {
        Destroy(gameObject, 1);
    }
    private void FixedUpdate()
    {
        transform.position += transform.forward * Time.deltaTime * moveSpeed;
    }
    //colisão do tiro
    private void OnColisionEnter(Collider other)
    {
        print(other.transform.name);
        Destroy(this.gameObject);
    }

}
