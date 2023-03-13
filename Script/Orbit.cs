using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    // ���� ��ǥ, ���� �ӵ�, ��ǥ���� �Ÿ� ���� ����
    public Transform target;
    public float orbitSpeed;
    Vector3 offset;

    void Start()
    {
        // RotateAround �� ��ǥ�� �����̸� �ϱ׷����� ������ �����Ƿ� �������� ���� �ش�.
        offset = transform.position - target.position;
    }

    void Update()
    {
        transform.position = target.position + offset;

        // ����� ������ ȸ��. ���� ��ǥ, ����, �ӵ�
        transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime);

        offset = transform.position - target.position;
    }
}
