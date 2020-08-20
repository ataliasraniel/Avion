using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aircraft_Controller : MonoBehaviour
{
    [Header("Look Objeto")]
    public Transform lookObj;
    public float whireRadius;
    public float distance;

    [Header("Rotação")]
    float angle;

   private void Update() 
   {
       transform.LookAt(lookObj);
       transform.position = Vector3.MoveTowards(transform.position, lookObj.position
       , distance * Time.deltaTime);
   }



   private void OnDrawGizmos() 
   {
       Gizmos.color = Color.red;
       Gizmos.DrawWireSphere(lookObj.position, whireRadius);
   }
}
