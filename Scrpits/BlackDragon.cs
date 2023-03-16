using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BlackDragon : MonoBehaviour
{
    // ����â
    public int maxHealth;
    public int curHealth;
    public bool isDead;
    public bool isFly = false;

    // �÷��̾� ���
    public Transform target;
    public bool isDetected;

    // �⺻ ���� ����
    public BoxCollider baseAttackArea;

    // �ҵ��� �����
    // public GameObject fireBall;
    [SerializeField] ParticleSystem fireBall;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public NavMeshAgent nav;
    public Animator anim;

    // �÷��̾ �ٶ󺸴� �÷��� ����
    public bool isLook;

    // �÷��̾� �̵����� ��ġ�� ���� �̸� Ȯ���ϴ� ����
    Vector3 lookVector;

    // �� �� ��ġ(��)
    public Transform mouth;

    float extraRotateSpeed = 5f;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();

        // �켱 �������� ���ϰ� �����ֱ�
        nav.isStopped = true;
        // ���� ����
        isLook = true;
        // ����
        StartCoroutine(Think());
    }

    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }

        if (!nav.isStopped)
        {

            nav.SetDestination(target.position);
            Vector3 lookRotate = target.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                Quaternion.LookRotation(lookRotate),
                                                extraRotateSpeed * Time.deltaTime);
        }

        if (isLook)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            lookVector = new Vector3(horizontal, 0, vertical) * 2f;
            transform.LookAt(target.position + lookVector);
        }
    }

    void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }

    void FreezeVelocity()
    {
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }

    void Targeting()
    {
        if (!isFly)
        {
            isLook = false;
            float targetRadius = 3f;
            float targetRange = 6f;

            RaycastHit[] rayHits =
                    Physics.SphereCastAll(transform.position + transform.forward * 5f,
                                            targetRadius,
                                            transform.forward,
                                            targetRange,
                                            LayerMask.GetMask("Player"));

            if (rayHits.Length > 0)
            {
                nav.isStopped = true;
                StartCoroutine(DoAttack());
                isLook = true;
            }
        }
    }


    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.3f);

        // ���� ���� Ȯ�� �����ֱ�
        int ranAction = Random.Range(0, 10);

        switch (ranAction)
        {
            case 0:
            case 1:
            case 2:
                //StartCoroutine(BaseAttack());
                //break;
            case 3:
            case 4:
            case 5:
            case 6:
                //StartCoroutine(FireShot());
                //break;
            case 7:
            case 8:
            case 9:
                StartCoroutine(TakeOff());
                break;
        }

    }

    IEnumerator Flying()
    {
        yield return new WaitForSeconds(0.1f);

        int ranAction = Random.Range(0, 6);
    }

    IEnumerator BaseAttack()
    {
        yield return new WaitForSeconds(0.1f);
        nav.isStopped = false;
        anim.SetBool("isRun", true);

        // �÷��̾� ��ġ�� �׺���̼� �ý������� �����ϰ� �̵��ϵ��� ��
        nav.SetDestination(target.position);

        // �÷��̾ �ٶ󺸵��� ȸ��
        Vector3 lookRotate = target.position - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation,
                                              Quaternion.LookRotation(lookRotate),
                                              extraRotateSpeed * Time.deltaTime);

        // ���� �Ÿ� �ȿ� �����ϸ� ���� ����
        while (Vector3.Distance(transform.position, target.position) > 2f)
        {
            yield return null;
        }

        nav.isStopped = true;
        StartCoroutine(DoAttack());
    }



    IEnumerator DoAttack()
    {
        anim.SetBool("isRun", false);
        yield return new WaitForSeconds(0.1f);
        anim.SetTrigger("doBaseAttack");
        rigid.velocity = Vector3.zero;


        yield return new WaitForSeconds(0.5f);
        baseAttackArea.enabled = true;

        yield return new WaitForSeconds(1f);
        baseAttackArea.enabled = false;

        yield return new WaitForSeconds(0.5f);
        isLook = true;
        StopAllCoroutines();

        StartCoroutine(Think());
    }

    IEnumerator FireShot()
    {
        isLook = false;
        anim.SetTrigger("doFire");
        yield return new WaitForSeconds(0.5f);
        Instantiate(fireBall, mouth.position, mouth.rotation).Play();

        yield return new WaitForSeconds(1f);
        isLook = true;
        StartCoroutine(Think());
    }

    IEnumerator TakeOff()
    {
        nav.isStopped = true;
        boxCollider.enabled = false;

        yield return new WaitForSeconds(0.1f);
        rigid.useGravity = false;
        anim.SetTrigger("doFly");

        transform.position += Vector3.up * 20f;

        yield return new WaitForSeconds(2.5f);
        anim.SetBool("isFly", true);

        yield return new WaitForSeconds(5f);
        anim.SetBool("isFly", false);

        rigid.useGravity = true;

        yield return new WaitForSeconds(3f);
        StartCoroutine(DoDie());
    }

    IEnumerator DoDie()
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetTrigger("doDie");
        StopAllCoroutines();
    }

}
