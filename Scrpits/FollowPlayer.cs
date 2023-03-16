using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // 카메라가 따라다니는 목표
    public Transform target;

    // 따라갈 목표와 카메라 사이의 거리
    public Vector3 offset;


    void Update()
    {
        transform.position = target.position + offset;    
    }

}
