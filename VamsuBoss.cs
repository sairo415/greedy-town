using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VamsuBoss : MonoBehaviour
{
    public float speed;
    public float baseSpeed;
    public float health;
    public float maxHealth;
    public float damage;
    public GameObject webObject;
    public GameObject eggObject;

    Rigidbody target;

    public bool isLive;//기본값

    bool isAttack;

    Rigidbody rigid;
    Animator anim;
    WaitForFixedUpdate wait;
    Collider coll;
    GameObject web;
    GameObject[] eggs;
    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();

        web = Instantiate(webObject);
        web.SetActive(false);
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!VamsuGameManager.instance.isLive || !isLive)
            return;


        if (Vector3.Distance(target.position, rigid.position) <= 13)
        {
            speed = 0;
            anim.SetFloat("Speed", 0);
        }
        else if (speed != baseSpeed)
        {
            speed = baseSpeed;
            anim.SetFloat("Speed", baseSpeed);
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
        StartCoroutine(WebMake());
        StartCoroutine(Fire());
        isLive = true;
        isAttack = false;
        coll.enabled = true;
        rigid.isKinematic = false;
        health = maxHealth;
        anim.SetBool("Dead", false);
    }

    public void Init(BossData data)
    {
        baseSpeed = data.speed;
        speed = data.speed;
        damage = data.damage;
        anim.SetFloat("Speed", data.speed);
        maxHealth = data.health;
        health = data.health;
        anim.SetBool("Dead", false);

        eggs = new GameObject[data.eggCount];
        for (int i = 0; i < eggs.Length; i++)
        {
            GameObject egg = Instantiate(eggObject);
            egg.SetActive(false);
            egg.GetComponent<EggController>().damage = damage;
            eggs[i] = egg;
        }
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

    //보스 근접 공격이 필요한가?
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

        if(health <= 0)
        {
            isLive = false;
            isAttack = true;
            web.GetComponent<SpiderWeb>().WebRemove();
            anim.SetBool("Dead", true);
            StopCoroutine(WebMake());
            StopCoroutine(Fire());
            StartCoroutine(Dead());

            coll.enabled = false;
            rigid.isKinematic = true;

            VamsuGameManager.instance.kill++;
            VamsuSoundManager.instance.PlayBasicBGM();
            //경험치 어케 할지 고민
            //VamsuGameManager.instance.GetExp();
        }
    }

    void CreateWeb()
    {
        web.SetActive(true);
        web.transform.position = target.position + (VamsuGameManager.instance.player.inputVec * 6 + new Vector3(Random.Range(-1.5f, 1.5f), 0, Random.Range(-1.5f, 1.5f)));
        StartCoroutine(WebRemove());
    }

    IEnumerator WebMake()
    {
        yield return new WaitForSeconds(7f);
        CreateWeb();
        if (isLive)
        {
            StartCoroutine(WebMake());
        }
    }

    IEnumerator WebRemove()
    {
        yield return new WaitForSeconds(3f);
        web.GetComponent<SpiderWeb>().WebRemove();
    }

    IEnumerator Fire()
    {
        yield return new WaitForSeconds(5f);
        anim.SetTrigger("Attack");
        for (int i=0; i<eggs.Length; i++)
        {
            eggs[i].SetActive(false);
            eggs[i].transform.position = transform.position + new Vector3(0, 1, 0);
            eggs[i].SetActive(true);
        }

        if (isLive)
        {
            StartCoroutine(Fire());
        }
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
