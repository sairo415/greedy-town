using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    // ���� ��ǥ�� ��ġ �������� public ������ ����
    public Transform target;
    public Vector3 offset;

    void Update()
    {
        transform.position = target.position + offset;
    }
}
