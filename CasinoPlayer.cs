using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasinoPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;
    public Vector3 inputVec;
    public bool moveOn;

    Rigidbody rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        moveOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!moveOn)
            return;

        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.z = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        if (!moveOn)
            return;


        Vector3 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;

        rigid.MovePosition(rigid.position + nextVec);
        transform.LookAt(transform.position + nextVec);

        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;

       
    }
}
