using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering;

public class AlbinoDragon : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public bool isDead;
    public bool isAttack;
    public bool isLook;
    public bool isRun;
    public bool wasRun;

    public GameObject hornSpot;
    public GameObject hornEffectPrefab;
    public BoxCollider hornDamageArea;
    public GameObject clawEffectPrefab;
    public GameObject tornadoPrefab;
    public GameObject[] tornadoSpots;

    private PhotonView pv;

    private enum BossState { Idle, Attack1, Attack2, Run, Dead };
    private BossState currentState;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public Transform target;
    public NavMeshAgent nav;
    public Animator anim;

    Vector3 lookVector;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();
        pv = GetComponent<PhotonView>();

        isLook = true;
        isAttack = false;
        nav.isStopped = true;
    }

    private void Start()
    {
        currentState = BossState.Idle;
    }


    private void Update()
    {
        Vector3 direction = target.position - transform.position;

        float distance = direction.magnitude;

        if(!isRun)
        {
            switch(currentState)
            {
            case BossState.Idle:
                if(!isAttack)
                    ChangeState();
                break;
            case BossState.Attack1:
                if(!isAttack)
                {
                    isAttack = true;
                    Attack1();
                    wasRun = false;
                }
                break;
            case BossState.Attack2:
                if(!isAttack)
                {
                    isAttack = true;
                    Attack2();
                    wasRun = false;
                }
                break;
            case BossState.Run:
                if(!isRun)
                {
                    isRun = true;
                    Chase();
                }
                else if(isRun && (distance < 10f))
                {
                    StopCoroutine(MakeRun());
                    isRun = false;
                    currentState = BossState.Idle;
                }
                break;
            case BossState.Dead:
                if(!isDead)
                {
                    isDead = true;
                    DoDie();
                }
                break;
            }
        }

        if(isLook)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            lookVector = new Vector3(horizontal, 0, vertical);
            transform.LookAt(target.position + lookVector);
        }
    }

    private void FixedUpdate()
    {
        FreezeVelocity();
        if(isDead)
            StopAllCoroutines();
    }

    void FreezeVelocity()
    {
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }

    void ChangeState()
    {
        if(currentHealth <= 0)
        {
            currentState = BossState.Dead;
        }
        else if(isAttack || isRun)
        {
            return;
        }
        else
        {
            int ranAction = Random.Range(0, 100);
            if(ranAction < 40)
            {
                currentState = BossState.Attack1;
            }
            else if(ranAction < 80)
            {
                currentState = BossState.Attack2;
            }
            else if(ranAction < 100 && !wasRun)
            {
                currentState = BossState.Run;
            }
        }
    }

    void Attack1()
    {
        StartCoroutine(HornAttack());
    }

    IEnumerator HornAttack()
    {
        isLook = false;
        yield return new WaitForSeconds(0.1f);
        anim.SetTrigger("doHornAttack");

        yield return new WaitForSeconds(1f);
        GameObject instantHornEffect = Instantiate(hornEffectPrefab, hornSpot.transform.position, hornSpot.transform.rotation);
        hornDamageArea.enabled = true;

        yield return new WaitForSeconds(1f);
        hornDamageArea.enabled = false;
        Destroy(instantHornEffect);

        yield return new WaitForSeconds(1f);
        isLook = true;
        currentState = BossState.Idle;
        isAttack = false;
    }

    void Attack2()
    {
        StartCoroutine(ClawAttack());
    }

    IEnumerator ClawAttack()
    {
        isLook = false;
        yield return new WaitForSeconds(0.1f);
        anim.SetTrigger("doClawAttack");

        yield return new WaitForSeconds(1.6f);
        GameObject instantEffect = Instantiate(clawEffectPrefab, transform.position + Vector3.forward * 3f, Quaternion.Euler(0f, 0f, 180f));
        GameObject instantTornado1 = Instantiate(tornadoPrefab, tornadoSpots[0].transform.position, transform.rotation);
        GameObject instantTornado2 = Instantiate(tornadoPrefab, tornadoSpots[1].transform.position, transform.rotation);
        GameObject instantTornado3 = Instantiate(tornadoPrefab, tornadoSpots[2].transform.position, transform.rotation);

        yield return new WaitForSeconds(2f);
        Destroy(instantEffect);
        isLook = true;
        currentState = BossState.Idle;
        isAttack = false;

        yield return new WaitForSeconds(2f);
        Destroy(instantTornado1);
        Destroy(instantTornado2);
        Destroy(instantTornado3);
    }

    void Chase()
    {
        wasRun = true;
        StartCoroutine(MakeRun());
    }

    IEnumerator MakeRun()
    {
        anim.SetBool("isRun", true);
        yield return new WaitForSeconds(0.1f);
        nav.isStopped = false;
        nav.SetDestination(target.transform.position);

        yield return new WaitForSeconds(2f);
        nav.isStopped = true;
        anim.SetBool("isRun", false);
        isRun = false;
        currentState = BossState.Idle;
    }

    void DoDie()
    {
        StartCoroutine(MakeDead());
    }

    IEnumerator MakeDead()
    {
        anim.SetTrigger("doDie");
        isLook = false;
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        //맞은 스킬의 소유주 파악.
        //해당 소유주가 이 보스가 위치한 곳의 플레이어아이디와 같으면 트리거 적용
        //적용 완료후 RPC Other

        if(other != null && (other.tag == "PlayerAttack" || other.tag == "PlayerAttackOver" || other.tag == "PlayerAttackDot"))
        {
            // 맞은 스킬의 시전자
            int skillOwnerID = other.GetComponent<BossPlayerSkill>().GetID();
            // 현재 위치한 클라이언트
            BossPlayer curClient = GameObject.FindObjectOfType<BossGameManager>().player;

            // Owner ID
            int myPlayerID = curClient.pv.ViewID;

            // 내가 사용한 스킬이 아닐 경우 로직 실행 안함.
            if(skillOwnerID != myPlayerID)
                return;

            if(other.tag == "PlayerAttack" || other.tag == "PlayerAttackOver")
            {
                currentHealth -= other.GetComponent<BossPlayerSkill>().damage;
                if(currentHealth < 0)
                    currentHealth = 0;

                // 시전자가 피흡을 가지고 있으면 체력을 회복시킨다.
                if(curClient.isVampirism && other.GetComponent<BossPlayerSkill>().isVampirism)
                {
                    int vamHP = curClient.curHealth + 10;
                    if(vamHP > curClient.maxHealth)
                        vamHP = curClient.maxHealth;
                    curClient.curHealth = vamHP;

                    // 회복시킨 후 동기화 필요할 듯...
                    // isVampirism == true 일 때, q 를 사용할 때마다, 여기서 SyncBossHealth 한 것 처럼
                    curClient.CallSyncPlayerHPAll(curClient.curHealth);
                }

                int sendRPCBossHP = currentHealth;

                // 서버 보스 체력과 동기화
                pv.RPC("SyncBossHealth", RpcTarget.Others, sendRPCBossHP);

                // 적과 닿았을 때 이펙트 삭제되도록 Destroy() 호출
                // tag PlayerAttack => 닿으면 삭제되는 이펙트
                // tag PlayerAttackOver => 닿으면 삭제되지 않는 이펙트
                if(other.tag == "PlayerAttack")
                {
                    Destroy(other.gameObject);
                    other.gameObject.SetActive(false);
                }

                StartCoroutine("OnDamage");
            }
            else if(other.tag == "PlayerAttackDot")
            {
                other.GetComponent<BossPlayerSkill>().isInBoss = true;
            }

        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other != null && other.tag == "PlayerAttackDot")
        {
            other.GetComponent<BossPlayerSkill>().isInBoss = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other != null && other.CompareTag("PlayerAttackDot") && other.GetComponent<BossPlayerSkill>().isInBoss)
        {

            Debug.Log("DotDam1");

            other.GetComponent<BossPlayerSkill>().damageTimer += Time.deltaTime;

            if(other.GetComponent<BossPlayerSkill>().damageTimer >= other.GetComponent<BossPlayerSkill>().damageInterval)
            {
                currentHealth -= other.GetComponent<BossPlayerSkill>().damage;
                if(currentHealth < 0)
                    currentHealth = 0;

                BossPlayer curClient = GameObject.FindObjectOfType<BossGameManager>().player;

                // 시전자가 피흡을 가지고 있으면 체력을 회복시킨다.
                if(curClient.isVampirism && other.GetComponent<BossPlayerSkill>().isVampirism)
                {
                    int vamHP = curClient.curHealth + 10;
                    if(vamHP > curClient.maxHealth)
                        vamHP = curClient.maxHealth;
                    curClient.curHealth = vamHP;

                    // 회복시킨 후 동기화 필요할 듯...
                    // isVampirism == true 일 때, q 를 사용할 때마다, 여기서 SyncBossHealth 한 것 처럼
                    curClient.CallSyncPlayerHPAll(curClient.curHealth);
                }

                int sendRPCBossHP = currentHealth;

                // 서버 보스 체력과 동기화
                pv.RPC("SyncBossHealth", RpcTarget.Others, sendRPCBossHP);

                StartCoroutine("OnDamage");

                other.GetComponent<BossPlayerSkill>().damageTimer = 0f;
            }
        }
    }

    [PunRPC]
    void SyncBossHealth(int health)
    {
        currentHealth = health;
    }
}
