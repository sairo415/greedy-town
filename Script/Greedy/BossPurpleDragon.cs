using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering;
using System.Transactions;


public class BossPurpleDragon : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public bool isDead;
    public bool isAttack;
    public bool isLook;
    public bool isFlying = false;
    public bool isLanding = false;
    public bool isStartFlying = false;
    public bool isChase;


    public GameObject flameBreathPrefab;
    public GameObject flameSpot;
    public GameObject[] meteorSpots;
    public GameObject meteorPrefab;
    public GameObject explosionPrefab;
    public GameObject clawSpot;
    public GameObject clawPrefab;
    public GameObject[] bossDomains;
    private Transform ToGo;

    private PhotonView pv;

    private enum BossState { Idle, Attack1, Attack2, Attack3, Dead, Fly, Flying };
    private BossState currentState;


    public Transform target;
    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public NavMeshAgent nav;
    public Animator anim;

    Vector3 lookVector;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();
        pv = GetComponent<PhotonView>();

        nav.isStopped = true;
        isLook = true;
        isAttack = false;
    }

    private void Start()
    {
        currentState = BossState.Idle;
    }

    private void Update()
    {
        if (!isFlying && !isLanding && !isStartFlying && !isDead)
        {
            switch (currentState)
            {
                case BossState.Idle:
                    if (!isAttack)
                        ChangeState();
                    break;
                case BossState.Attack1:
                    if (!isAttack)
                    {
                        isAttack = true;
                        Attack1();
                    }
                    break;
                case BossState.Attack2:
                    if (!isAttack)
                    {
                        isAttack = true;
                        Attack2();
                    }
                    break;
                case BossState.Attack3:
                    if (!isAttack)
                    {
                        isAttack = true;
                        Attack3();
                    }
                    break;
                case BossState.Fly:
                    TakeOff();
                    break;
                case BossState.Flying:
                    NowFlying();
                    break;
                case BossState.Dead:
                    if (!isDead)
                        DoDie();
                    break;
            }
        }

        else if (isFlying && !isLanding && !isStartFlying && !isDead)
        {
            isLook = false;

            nav.isStopped = false;
            nav.SetDestination(ToGo.position);

            Vector3 direction = ToGo.position - transform.position;

            float distance = direction.magnitude;

            if (distance < 10f)
            {
                StartCoroutine(Land());
            }
        }

        if (isLook && !isLanding && !isStartFlying && !isDead)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            lookVector = new Vector3(horizontal, 0, vertical);
            transform.LookAt(target.position + lookVector);
        }
    }


    private void ChangeState()
    {
        if (currentHealth <= 0)
        {
            currentState = BossState.Dead;
        }
        else if (isAttack)
        {
            return;
        }
        else
        {
            int ranAction = Random.Range(0, 100);

            if (ranAction < 20)
            {
                currentState = BossState.Attack1;
            }
            else if (ranAction < 40)
            {
                currentState = BossState.Attack2;
            }
            else if (ranAction < 80)
            {
                currentState = BossState.Attack3;
            }
            else if (ranAction < 100)
            {
                currentState = BossState.Fly;
            }
        }
    }

    void Attack1()
    {
        StartCoroutine(FlameBreath());
    }

    IEnumerator FlameBreath()
    {
        yield return new WaitForSeconds(0.1f);
        isLook = false;
        anim.SetTrigger("doFlameAttack");

        yield return new WaitForSeconds(0.5f);
        // GameObject instantFlame = Instantiate(flameBreathPrefab, flameSpot.transform.position, flameSpot.transform.rotation);
        flameBreathPrefab.SetActive(true);

        yield return new WaitForSeconds(2f);
        flameBreathPrefab.SetActive(false);
        isLook = true;

        yield return new WaitForSeconds(3f);
        isAttack = false;
        currentState = BossState.Idle;
    }

    void Attack2()
    {
        anim.SetTrigger("doScream");
        for (int i = 1; i < 7; i++)
        {
            float delay = 0.6f * i;
            StartCoroutine(Meteor(delay, i-1));
        }

        StartCoroutine(EndAttack2());
    }

    IEnumerator Meteor(float delay, int idx)
    {
        yield return new WaitForSeconds(delay);
        GameObject instantMeteor = Instantiate(meteorPrefab, meteorSpots[idx].transform.position + Vector3.up * 25f, meteorSpots[idx].transform.rotation);

        yield return new WaitForSeconds(0.5f);
        GameObject instantExplosion = Instantiate(explosionPrefab, meteorSpots[idx].transform.position, meteorSpots[idx].transform.rotation);
        meteorSpots[idx].SetActive(true);

        yield return new WaitForSeconds(3f);
        Destroy(instantMeteor);
        Destroy(instantExplosion);
        meteorSpots[idx].SetActive(false);
    }

    IEnumerator EndAttack2()
    {
        yield return new WaitForSeconds(6f);
        isLook = true;
        isAttack = false;
        currentState = BossState.Idle;
    }


    void Attack3()
    {
        StartCoroutine(ClawSlash());
    }


    IEnumerator ClawSlash()
    {
        yield return new WaitForSeconds(0.1f);
        isLook = false;
        anim.SetTrigger("doClawAttack");

        yield return new WaitForSeconds(1.5f);
        GameObject instantClaw = Instantiate(clawPrefab, clawSpot.transform.position, Quaternion.Euler(0f, 0f, 0f));


        yield return new WaitForSeconds(0.5f);
        Destroy(instantClaw);

        yield return new WaitForSeconds(2f);
        isLook = true;
        isAttack = false;
        currentState = BossState.Idle;
    }

    void TakeOff()
    {
        isStartFlying = true;
        boxCollider.enabled = false;
        anim.SetTrigger("doTakeOff");

        StartCoroutine(MakeFlying());
    }


    IEnumerator MakeFlying()
    {
        yield return new WaitForSeconds(3f);
        isStartFlying = false;
        currentState= BossState.Flying;
    }


    void NowFlying()
    {
        anim.SetBool("isFly", true);

        int ranIdx = Random.Range(0, 4);

        ToGo = bossDomains[ranIdx].transform;

        Vector3 direction = ToGo.position - transform.position;

        float distance = direction.magnitude;

        while (distance < 10f)
        {
            int newRanIdx = Random.Range(0, 4);

            ToGo = bossDomains[newRanIdx].transform;
        }

        isFlying = true;
        anim.SetBool("isFlying", true);
    }


    IEnumerator Land()
    {
        nav.isStopped = true;
        anim.SetBool("isFlying", false);
        isLook = true;
        yield return new WaitForSeconds(0.1f);
        isLook = false;
        anim.SetBool("isFlyFlameAttack", true);
        isAttack = true;

        yield return new WaitForSeconds(1f);
        flameBreathPrefab.SetActive(true);

        yield return new WaitForSeconds(2f);
        flameBreathPrefab.SetActive(false);
        anim.SetBool("isFlyFlameAttack", false);

        isAttack = false;
        isLanding = true;
        anim.SetBool("isFly", false);
        isFlying = false;
        isLook = true;
        yield return new WaitForSeconds(3f);

        yield return new WaitForSeconds(1.5f);
        currentState = BossState.Idle;
        boxCollider.enabled = true;
        isLanding = false;
    }


    void DoDie()
    {
        StartCoroutine(MakeDead());
    }

    IEnumerator MakeDead()
    {
        anim.SetTrigger("doDie");
        isDead = true;
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
                if(pv != null)
                    pv.RPC("SyncBossHealth", RpcTarget.Others, sendRPCBossHP);

                // 적과 닿았을 때 이펙트 삭제되도록 Destroy() 호출
                // tag PlayerAttack => 닿으면 삭제되는 이펙트
                // tag PlayerAttackOver => 닿으면 삭제되지 않는 이펙트
                if(other.tag == "PlayerAttack")
                {
                    Destroy(other.gameObject);
                    other.gameObject.SetActive(false);
                }
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
