using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable_Object : MonoBehaviour
{
    int lives = 10;
    public GameObject explosion;

    private void OnTriggerEnter(Collider other)
    {
        if (lives <= 0)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
        lives--;
    }
}
