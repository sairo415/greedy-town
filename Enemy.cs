using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public int damage;

    public Color[] colors;
    Rigidbody target;

    bool isLive;//�⺻��

    bool isAttack;

    Rigidbody rigid;
    Material mater;
    Animator anim;
    WaitForFixedUpdate wait;
    Collider coll;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        mater = GetComponent<Renderer>().material;
        coll = GetComponent<Collider>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            return;

        Vector3 dirVec = target.position - rigid.position;
        Vector3 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;

        transform.LookAt(target.transform);

        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector3.zero;
    }

    void LateUpdate()
    {
        if (!isLive)
            return;
    }

    void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody>();
        isLive = true;
        coll.enabled = true;
        rigid.isKinematic = false;
        health = maxHealth;
        anim.SetBool("Dead", false);
    }

    public void Init(SpawnData data)
    {
        mater.color = colors[data.spriteType];

        speed = data.speed;
        damage = data.damage;
        anim.SetFloat("Speed", data.speed);
        maxHealth = data.health;
        health = data.health;
        anim.SetBool("Dead", false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isLive)
            return;

        //���⿡ ���� ������
        if (other.CompareTag("Hammer"))
        {
            TakeDamage(other.GetComponent<Hammer>().damage);
        }
        
    }

    void OnCollisionStay(Collision collision)
    {
        //�ִϸ��̼��� ������ attack�� false�� �ʱ�ȭ�Ѵ�.
        if (collision.gameObject.tag == "Player" && !isAttack)
        {
            isAttack = true;
            GameManager.instance.GetDamage(damage);
            anim.SetTrigger("Attack");
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        StartCoroutine(KnockBack());

        if (health > 0)
        {
            anim.SetTrigger("Hit");
        }
        else
        {
            anim.SetBool("Dead", true);
            isLive = false;
            coll.enabled = false;
            rigid.isKinematic = true;

            GameManager.instance.kill++;
            GameManager.instance.GetExp();
        }
    }

    IEnumerator KnockBack()
    {
        yield return wait;//���� ���� ������ ������

        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dir = transform.position - playerPos;

        rigid.AddForce(dir.normalized * 5, ForceMode.Impulse);
    }

    //�ؿ��� �ִϸ��̼� ������ ����Ǵ� �Լ�
    void Attack()
    {
        isAttack = false;
    }

    void Dead()
    {
        gameObject.SetActive(false);
    }
}
