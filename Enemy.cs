using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float baseSpeed;
    public float health;
    public float maxHealth;
    public float damage;

    public Color[] colors;
    Rigidbody target;

    public bool isLive;//기본값

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
        if (!VamsuGameManager.instance.isLive)
            return;

        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            return;

        if(Vector3.Distance(target.position, rigid.position) > 15)
        {
            speed = baseSpeed * 3;
        }
        else if(speed != baseSpeed)
        {
            speed = baseSpeed;
        }

        Vector3 dirVec = target.position - rigid.position;
        Vector3 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;

        transform.LookAt(target.transform);

        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector3.zero;
    }

    void LateUpdate()
    {
        if (!VamsuGameManager.instance.isLive)
            return;

        if (!isLive)
            return;
    }

    void OnEnable()
    {
        target = VamsuGameManager.instance.player.GetComponent<Rigidbody>();
        isLive = true;
        isAttack = false;
        coll.enabled = true;
        rigid.isKinematic = false;
        health = maxHealth;
        anim.SetBool("Dead", false);
    }

    public void Init(SpawnData data)
    {
        mater.color = colors[data.spriteType];

        baseSpeed = data.speed;
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

        //무기에 공격 받으면
        if (other.CompareTag("Hammer"))
        {
            TakeDamage(other.GetComponent<Hammer>().damage);
        }
        
    }

    void OnCollisionStay(Collision collision)
    {
        //애니메이션이 끝나면 attack을 false로 초기화한다.
        if (collision.gameObject.tag == "Player" && !isAttack)
        {
            isAttack = true;
            VamsuGameManager.instance.GetDamage(damage);
            anim.SetTrigger("Attack");
            StartCoroutine(Attack());
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isLive)
            return;

        health -= (damage * (1 + VamsuGameManager.instance.extraDamage));
        StartCoroutine(KnockBack());

        if (health > 0)
        {
            anim.SetTrigger("Hit");
        }
        else
        {
            isLive = false;
            isAttack = true;
            anim.SetBool("Dead", true);
            StartCoroutine(Dead());

            coll.enabled = false;
            rigid.isKinematic = true;

            VamsuGameManager.instance.kill++;
            VamsuGameManager.instance.GetExp();
        }
    }

    IEnumerator KnockBack()
    {
        yield return wait;//다음 물리 프레임 딜레이

        Vector3 playerPos = VamsuGameManager.instance.player.transform.position;
        Vector3 dir = transform.position - playerPos;

        rigid.AddForce(dir.normalized * 2, ForceMode.Impulse);
    }

    IEnumerator Dead()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(1f);
        isAttack = false;
    }

}
