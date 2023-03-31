using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VamsuPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    public float baseSpeed;
    public float speed;
    public Vector3 inputVec;
    public Scanner scanner;
    Rigidbody rigid;
    float degree;
    public float attack;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        scanner = GetComponent<Scanner>();
        degree = 180f;
        RenderSettings.skybox.SetFloat("_Rotation", degree);
    }

    // Update is called once per frame
    void Update()
    {
        if(!VamsuGameManager.instance.isLive)
            return;

        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.z = Input.GetAxisRaw("Vertical");

        degree += Time.deltaTime * 3f * inputVec.x;
        if (degree > 360)
            degree = 0;
        else if (degree < 0)
            degree = 360;

        RenderSettings.skybox.SetFloat("_Rotation", degree);
    }

    void FixedUpdate()
    {
        if (!VamsuGameManager.instance.isLive)
            return;

        Vector3 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;

        rigid.MovePosition(rigid.position + nextVec);
        transform.LookAt(transform.position + nextVec);

        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;

       
    }
}
