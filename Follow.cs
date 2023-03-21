using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;


    // Update is called once per frame
    void Update()
    {
        //transform.position = new Vector3(target.position.x, 1.0f, target.position.z);
        transform.position = target.position;
    }
}
