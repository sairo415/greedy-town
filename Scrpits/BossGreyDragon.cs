using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering;

public class BossGreyDragon : MonoBehaviourPunCallbacks, IPunObservable
{
    // ü��
    public int maxHealth;   // �ִ� ü��
    public int curHealth;   // ���� ü��

    // ��ǥ �÷��̾�
    [SerializeField]
    private BossPlayer targetPlayer;
    private Transform targetPlayerTransform;

    // ��ǥ ���� AI
    NavMeshAgent nav;

    // ���� ������Ʈ
    Rigidbody rigid;

    // �ִϸ��̼�
    Animator anim;

    // �÷��� ����
    bool isDie;         // ���
    bool isSreamEnd;    // ǥȣ ����

    // ����
    AudioSource skillSound;
    public AudioClip dragonSreamSound;

    // Ÿ�� ���� �ð�
    float changeTargetTimeDelta;    // ����
    float changeTargetTime;         // ����ġ

    // ������ �����ϴ� bool ���� �߰�, ������ false ��
    public bool isChase;

    // ���� ������. ������ �ϴ� ���� �ٸ� ������ ���� �ʵ��� ������.
    bool isAttackING;

    // ���� ����
    public Transform iceBallPos;
    public Transform flyAttackPos;

    // ���ƴٴϴ� �ð�
    float flyTimeDelta; // ����
    float flyTime;      // ����ġ

    // ���� ��ų ���� �ε���
    int selectedSkillIdx;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        skillSound = GetComponent<AudioSource>();

        // Ÿ���� �����ϴ� �ð� ����ġ �ʱ�ȭ
        changeTargetTimeDelta = 100.0f;
        changeTargetTime = 10.0f;

        // ���ƴٴϴ� �ð�
        flyTime = 10.0f;

        Invoke("ChaseStart", 2);
    }

    // ���� ����
    void ChaseStart()
    {
        isChase = true;
    }

    void FreezeVelocity()
    {
        if(isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void Start()
    {
        // ������ Ŭ���̾�Ʈ�� �ƴ� Ŭ���̾�Ʈ���� ü�� UI ����ȭ�� ���� ���� �޴����� �������� ������Ʈ�� �Է��Ѵ�.
        if(!photonView.IsMine)
        {
            BossGameManager gameManager = GameObject.FindObjectOfType<BossGameManager>();
            gameManager.bossGrey = gameObject.GetComponent<BossGreyDragon>();
        }

        // ��ȿ
        //StartCoroutine("DoScream");
    }

    void Update()
    {
        // ��ȿ�� ����Ǹ� ���� ���� ���۵�
        //if(!isSreamEnd)
        //    return;

        if(photonView.IsMine)
        {

        }
    }

    // �����Ǹ� ��ȿ
    /*IEnumerator DoScream()
    {
        yield return new WaitForSeconds(1.0f);

        anim.SetTrigger("doScream");
        skillSound.clip = dragonSreamSound;
        skillSound.loop = false;
        skillSound.Play();

        yield return new WaitForSeconds(4.0f);

        isSreamEnd = true;

        yield return null;
    }*/

    // �÷��̾�� ���� �浹�� ���� ����ٴ��� ���ϴ� ���� �ذ�
    void FixedUpdate()
    {
        FreezeVelocity();
    }

    void OnTriggerEnter(Collider other)
    {
        if(isDie)
            return;

        // ���� Ŭ���̾�Ʈ���� ���� ViewID �� ���� Ŭ���� �ѹ��� ������ Ŭ���̾�Ʈ ���� �ߺ��ؼ� ������ ����Ǵ� ���� ������.
        //  1. ���� ��ų�� ������ �ľ�.
        //  2. �ش� ��ų�� �����ڰ� ������ Ŭ���̾�Ʈ�� ������ ��쿡�� ������ ������.
        //  3. ���� �Ϸ� �� ü�� ����ȭ�� ���� RPC (Others) �Լ� ȣ��
        if(other != null && (other.tag == "PlayerAttack" || other.tag == "PlayerAttackOver" || other.tag == "PlayerAttackDot"))
        {
            // PlayerAttack     : ���Ͱ� ������ ����Ʈ�� ������� ����
            // PlayerAttackOver : ���Ͱ� �¾Ƶ� ����Ʈ�� ������� �ʴ� ����
            // PlayerAttackDot  : ��Ʈ ������ ����

            // ���� ��ų�� ������
            int skillOwnerID = other.GetComponent<BossPlayerSkill>().GetID();
            // ���� ��ġ�� Ŭ���̾�Ʈ
            BossPlayer curClient = GameObject.FindObjectOfType<BossGameManager>().player;

            if(curClient.pv == null)
                return;

            int myPlayerID = curClient.pv.ViewID;

            // ���� ����� ��ų�� �ƴ� ��� ���� ���� ����.
            if(skillOwnerID != myPlayerID)
                return;

            if(other.tag == "PlayerAttack" || other.tag == "PlayerAttackOver")
            {
                curHealth -= other.GetComponent<BossPlayerSkill>().damage;
                if(curHealth < 0)
                    curHealth = 0;

                StartCoroutine("OnDamage");

                // �����ڰ� ������ ������ ������ ü���� ȸ����Ų��.
                if(curClient.isVampirism && other.GetComponent<BossPlayerSkill>().isVampirism)
                {
                    int vamHP = curClient.curHealth + 2;
                    if(vamHP > curClient.maxHealth)
                        vamHP = curClient.maxHealth;
                    curClient.curHealth = vamHP;

                    // ����� ü���� �ٸ� Ŭ���̾�Ʈ�� ����ȭ
                    curClient.CallSyncPlayerHPAll(curClient.curHealth);
                }

                int sendRPCBossHP = curHealth;

                // ���� ü���� �ٸ� Ŭ���̾�Ʈ�� ����ȭ
                photonView.RPC("SyncBossHealth", RpcTarget.Others, sendRPCBossHP);

                // ���� ����� �� ����Ʈ �����ǵ��� Destroy() ȣ��
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
        if(isDie)
            return;

        if(other != null && other.tag == "PlayerAttackDot")
        {
            other.GetComponent<BossPlayerSkill>().isInBoss = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(isDie)
            return;

        if(other != null && other.CompareTag("PlayerAttackDot") && other.GetComponent<BossPlayerSkill>().isInBoss)
        {
            other.GetComponent<BossPlayerSkill>().damageTimer += Time.deltaTime;

            if(other.GetComponent<BossPlayerSkill>().damageTimer >= other.GetComponent<BossPlayerSkill>().damageInterval)
            {
                curHealth -= other.GetComponent<BossPlayerSkill>().damage;
                if(curHealth < 0)
                    curHealth = 0;

                StartCoroutine("OnDamage");

                BossPlayer curClient = GameObject.FindObjectOfType<BossGameManager>().player;

                // �����ڰ� ������ ������ ������ ü���� ȸ����Ų��.
                if(curClient.isVampirism && other.GetComponent<BossPlayerSkill>().isVampirism)
                {
                    int vamHP = curClient.curHealth + 10;
                    if(vamHP > curClient.maxHealth)
                        vamHP = curClient.maxHealth;
                    curClient.curHealth = vamHP;

                    // ����� ü���� �ٸ� Ŭ���̾�Ʈ�� ����ȭ
                    curClient.CallSyncPlayerHPAll(curClient.curHealth);
                }

                int sendRPCBossHP = curHealth;

                // ���� ü���� �ٸ� Ŭ���̾�Ʈ�� ����ȭ
                photonView.RPC("SyncBossHealth", RpcTarget.Others, sendRPCBossHP);

                StartCoroutine("OnDamage");

                other.GetComponent<BossPlayerSkill>().damageTimer = 0f;
            }
        }
    }

    IEnumerator OnDamage()
    {
        yield return new WaitForSeconds(0.1f);

        if(curHealth <= 0)
        {
            gameObject.layer = 11;
            isDie = true;

            // ���� �ߴ�
            nav.isStopped = true;
            FreezeVelocity();
            isChase = false;
            anim.SetBool("isRun", false);

            anim.SetTrigger("doDie");

            Debug.Log("���");

            Destroy(gameObject, 20);
        }
    }

    // ���� ü���� �ٸ� Ŭ���̾�Ʈ�� ���� ü�°� ����ȭ
    [PunRPC]
    void SyncBossHealth(int health)
    {
        curHealth = health;

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            // We own this player: send the others our data
        }
        else
        {
            // Network player, receive data
        }
    }
}
