using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VamsuPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;
    public Vector3 inputVec;
    public Scanner scanner;

    Rigidbody rigid;

    public float attack;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        scanner = GetComponent<Scanner>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.instance.isLive)
            return;

        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.z = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        Vector3 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;

        rigid.MovePosition(rigid.position + nextVec);
        transform.LookAt(transform.position + nextVec);

        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }
}
