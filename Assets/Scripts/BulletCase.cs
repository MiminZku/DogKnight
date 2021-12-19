using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCase : MonoBehaviour
{
    void Start() {
        Invoke("Disapper",5);
    }
    void Disapper(){
        Destroy(gameObject);
    }
    // private void OnCollisionEnter(Collision collision)
    // {
    //     if(collision.gameObject.tag == "Floor")
    //     {
    //         Destroy(gameObject, 5);
    //     }
    // }
}