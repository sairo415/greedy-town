using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // �� Ÿ��
    public enum Type{ A, B, C};
    public Type enemyType;
    // ü�°� ������Ʈ�� ���� ���� ����
    public int maxHealth;
    public int curHealth;
    // ��ǥ��
    public Transform target;
    // �ݶ��̴��� ���� ���� �߰�
    public BoxCollider meleeArea;
    // �̻��� �������� ��Ƶ� ����
    public GameObject bullet;
    // ������ �����ϴ� bool ���� �߰�
    public bool isChase;
    // ���� ������ �ϰ� �ִ��� �÷��� ����
    public bool isAttack;
    

    Rigidbody rigid;
    BoxCollider boxCollider;

    Material mat;

    // Nav ���� Ŭ������ UnityEngine.AI ���ӽ����̽� ��� -> ���� �߰�
    // NavMesh : NavAgent �� ��θ� �׸��� ���� ����.
    // AI �� ��ǥ���� ����ٴϱ� ���� ���� �����ߵǴµ� �� ���� ���� ����.
    // Windoew - AI - Navigation

    NavMeshAgent nav;

    // �ִϸ��̼�
    Animator anim;

    void Awake()
	{
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        // MeshRender �� ���� ������Ʈ�� �����Ƿ� ����
        // mat = GetComponent<MeshRenderer>().material;
        mat = GetComponentInChildren<MeshRenderer>().material;
        // ��ǥ�� ���� nav �ʱ�ȭ
        nav = GetComponent<NavMeshAgent>();
        // �ִϸ��̼� �ڽĿ�����Ʈ�� ����
        anim = GetComponentInChildren<Animator>();

        Invoke("ChaseStart", 2);
    }

    // ���� ����
    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    void FreezeVelocity()
    {
        // �ǰ� �ÿ� ���� ������ �����ϱ� ���ؼ�
        // ���� ���� ���� ���� �ɱ�
        if(isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void Targeting()
    {
        // SphereCast() �� ������, ���̸� ������ ���� ����
        float targetRadius = 0;
        float targetRange = 0;

        // switch ������ �� Ÿ���� ��ġ�� ���ϱ�
        switch(enemyType)
        {
        case Type.A:
            targetRadius = 1.5f;
            targetRange = 3f;
            break;
        case Type.B:
            targetRadius = 1f; // ������ ��Ȯ�ϰ� �ؾߵ����� �۰�
            targetRange = 12f; // ���� Ÿ���� ���� �ø���
            break;
        case Type.C:
            // ���Ÿ��̱� ������ �ٰ��� ���� �ø��� ��Ȯ�ؾߵ�
            targetRadius = 0.5f;
            targetRange = 25f;
            break;
        }

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

        // rayHit ������ �����Ͱ� ������ ���� �ڷ�ƾ ����
        // �̹� ���� ���̸� �����ϸ� �ȵ�
        if(rayHits.Length > 0 && !isAttack) 
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        // ������ �ϰ� ������ �ϰ� ������ ����
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        switch(enemyType)
        {
        case Type.A:
            // ���� ����� �����̿� ���� ���ݵ��� ��ũ�� ����
            yield return new WaitForSeconds(0.2f);
            meleeArea.enabled = true; // ���� ���� Ȱ��ȭ

            // 1�� �� ��Ȱ��ȭ
            yield return new WaitForSeconds(1f);
            meleeArea.enabled = false;

            yield return new WaitForSeconds(1f);
            break;
        case Type.B:
            // ���� ������ �ؾߵ�. 0.1 �� ��������
            yield return new WaitForSeconds(0.1f);
            rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
            meleeArea.enabled = true; // ���� ���� Ȱ��ȭ

            // ���� �߱� ������ ������ ����
            yield return new WaitForSeconds(0.5f);
            rigid.velocity = Vector3.zero; // velocity �� Vector3.zero �� �ӵ� ����
            meleeArea.enabled = false; // ���� ���� ��Ȱ��ȭ

            // ������ ���� ��� ������
            yield return new WaitForSeconds(2f);


            break;
        case Type.C:
            // �߻� �غ� ���� �ð�
            yield return new WaitForSeconds(0.5f);

            // Instantiate() �Լ��� �̻��� �ν��Ͻ�ȭ
            GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
            // �̻����� �����ڸ��� �浹�ϴ� ���� �ڱ� �ڽ��̹Ƿ� �±׿� ���̾ Enemy �� ����

            Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
            rigidBullet.velocity = transform.forward * 20;

            // ������ ���� ��� ������
            yield return new WaitForSeconds(2f);

            break;
        }

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }

    void Update()
	{
        //if(isChase)
        // ������ ��ǥ ��ġ ���� �Լ�
        //    nav.SetDestination(target.position); // ���� ����

        // ���� ������ ��ǥ�� �Ҿ�����°Ŷ� �̵��� ������
        // ��ǥ�� Ȱ��ȭ �Ǿ����� ���� ��ǥ�� ����
        if(nav.enabled)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase; //isStopped �� ����Ͽ� �Ϻ��ϰ� ���ߵ��� �ۼ�
        }
            
    }
    
    // �÷��̾�� ���� �浹�� ���� ����ٴ��� ���ϴ� ���� �ذ�
    void FixedUpdate()
    {
        // ������ ������ �����ϰ� ���� ������
        // Ÿ������ ���� �Լ� ����

        Targeting();
        FreezeVelocity();
    }

    // �÷��̾ �ֵθ��� ��ġ Ȥ�� ���ƿ��� �Ѿ�
    // Ʈ���ŷ� ó��
    // OnTriggerEnter() �Լ��� �±� �� ������ �ۼ�
    void OnTriggerEnter(Collider other)
	{
        if(other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            // ���� ��ġ�� �ǰ� ��ġ�� ���� ���ۿ� ���ϱ�
            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVec, false));

            //Debug.Log("Melee : " + curHealth);
        }
        else if(other.tag == "Bullet") 
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            // ���� ��ġ�� �ǰ� ��ġ�� ���� ���ۿ� ���ϱ�
            Vector3 reactVec = transform.position - other.transform.position;

            // �Ѿ��� ���, ���� ����� �� �����ǵ��� Destroy() ȣ��
            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec, false));
            //Debug.Log("Range : " + curHealth);
        }
	}

    // �ǰ��Լ� ������ ���� �ð��� ������ �Ͱ� ����
    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, true));
    }

    // �ǰ� ó��
    // ����ź���� ���׼��� ���� bool �Ű����� �߰�
    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        // ������ ������
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0)
        {
            mat.color = Color.white;
        }
        else
        {
            mat.color = Color.gray;
            gameObject.layer = 12; // 12 : Layer Dead
            isChase = false;
            // ��� ���׼�, ���� ���󰡱� �� ���� NavAgent ��Ȱ��
            nav.enabled = false;
            anim.SetTrigger("doDie");

            if(isGrenade)
            {
                // ����ź�� ���� ��� ���׼��� ū ���� ȸ���� �߰�
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;

                //Freeze rotation �� �ɷ������Ƿ�
                rigid.freezeRotation = false;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }
            

            Destroy(gameObject, 4); // 4�� �Ŀ� �����
        }
    }
}
