using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // ī�޶� ����ٴϴ� ��ǥ
    public Transform target;

    // ���� ��ǥ�� ī�޶� ������ �Ÿ�
    public Vector3 offset;


    void Update()
    {
        transform.position = target.position + offset;    
    }

}
