using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    // 공전 목표, 공전 속도, 목표와의 거리 변수 생성
    public Transform target;
    public float orbitSpeed;
    Vector3 offset;

    void Start()
    {
        // RotateAround 는 목표가 움직이면 일그러지는 단점이 있으므로 오프셋을 직접 준다.
        offset = transform.position - target.position;
    }

    void Update()
    {
        transform.position = target.position + offset;

        // 대상을 주위로 회전. 공전 목표, 방향, 속도
        transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime);

        offset = transform.position - target.position;
    }
}
