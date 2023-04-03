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

    // ��ġ�� ���� ������. ������ �ϴ� ���� �ٸ� ������ ���� �ʵ��� ������.
    bool isFlyAttackING;

    // �����ִ� ���� ���� ����
    bool isFly;

    // ���� ����
    public Transform iceBallPos;
    public Transform flyAttackPos;
    public Transform windPos1;
    public Transform windPos2;
    public Transform windPos3;
    public Transform windPos4;

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
        flyTime = 15.0f;

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
        StartCoroutine("DoScream");
    }

    void Update()
    {
        // ��ȿ�� ����Ǹ� ���� ���� ���۵�
        if(!isSreamEnd)
            return;

        if(photonView.IsMine)
        {
            // ����
            if(!isDie && curHealth <= 0)
            {
                // ��� ������Ʈ�� ����� �� �ִ� ���̾�
                gameObject.layer = 11;
                isDie = true;

                // ���� �ߴ�
                nav.isStopped = true;
                FreezeVelocity();
                isChase = false;
                anim.SetBool("isRun", false);

                // �ִϸ��̼�
                anim.SetTrigger("doDie");

                // ���� �ð� �� �ı�
                Destroy(gameObject, 4);
            }

            // �׾����� ���� ������ ������ �ʿ� ����
            if(isDie)
                return;

            // Ÿ�� ���� �ð� ����
            changeTargetTimeDelta += Time.deltaTime;

            // ����ִ� �÷��̾� �� Ÿ���� ���� ����
            if(changeTargetTimeDelta >= changeTargetTime && !isAttackING)
            {
                BossPlayer[] bossPlayers = FindObjectsOfType<BossPlayer>();
                List<BossPlayer> bossPlayersAlive = new List<BossPlayer>();

                foreach(BossPlayer bossPlayer in bossPlayers)
                {
                    if(!bossPlayer.isDie)
                    {
                        bossPlayersAlive.Add(bossPlayer);
                    }
                }

                if(bossPlayersAlive.Count == 0)
                    return;

                int index = Random.Range(0, bossPlayersAlive.Count);
                targetPlayer = bossPlayersAlive[index];
                targetPlayerTransform = targetPlayer.transform;

                changeTargetTimeDelta = 0.0f;
            }


            // ���� ���� ���� ����
            // 0 : ���̽� ��
            // 1 : ȸ���� ��ȯ
            // 2 : ���ƴٴϸ� ���� => ����
            selectedSkillIdx = Random.Range(0, 3);

            //Test
            selectedSkillIdx = 1;

            if(targetPlayerTransform != null)
            {
                // ���ƴٴϴ� �ð� ����
                flyTimeDelta += Time.deltaTime;

                if(isAttackING)
                    return;

                if(isFlyAttackING)
                    selectedSkillIdx = 2;

                if(selectedSkillIdx == 0)
                {
                    // ����
                    isAttackING = true;
                    StartCoroutine("DoIceBall");
                }
                else if(selectedSkillIdx == 1)
                {
                    // ����
                    isAttackING = true;
                    anim.SetTrigger("doTakeOff");
                    isFly = true;
                    StartCoroutine("DoWindAttack");
                }
                else if(selectedSkillIdx == 2)
                {

                }
            }
        }
    }

	// �����Ǹ� ��ȿ
	IEnumerator DoScream()
	{
		yield return new WaitForSeconds(1.0f);

		anim.SetTrigger("doScream");
		skillSound.clip = dragonSreamSound;
		skillSound.loop = false;
		skillSound.Play();

		yield return new WaitForSeconds(4.0f);

		isSreamEnd = true;

		yield return null;
	}

    // ���̽� �극��
    IEnumerator DoIceBall()
    {
        transform.LookAt(targetPlayerTransform);

        anim.SetTrigger("doIceBall");

        yield return new WaitForSeconds(0.5f);

        GameObject iceBallObj1 = PhotonNetwork.Instantiate("IceFatalWheel", iceBallPos.position, iceBallPos.rotation);
        Rigidbody iceBallObjRigid1 = iceBallObj1.GetComponentInChildren<Rigidbody>();
        iceBallObjRigid1.velocity = iceBallPos.forward * 50;

        yield return new WaitForSeconds(0.3f);

        transform.LookAt(targetPlayerTransform);

        anim.SetTrigger("doIceBall");

        yield return new WaitForSeconds(0.5f);

        GameObject iceBallObj2 = PhotonNetwork.Instantiate("IceFatalWheel", iceBallPos.position, iceBallPos.rotation);
        Rigidbody iceBallObjRigid2 = iceBallObj2.GetComponentInChildren<Rigidbody>();
        iceBallObjRigid2.velocity = iceBallPos.forward * 50;

        yield return new WaitForSeconds(0.3f);

        transform.LookAt(targetPlayerTransform);

        anim.SetTrigger("doIceBall");

        yield return new WaitForSeconds(0.5f);

        GameObject iceBallObj3 = PhotonNetwork.Instantiate("IceFatalWheel", iceBallPos.position, iceBallPos.rotation);
        Rigidbody iceBallObjRigid3 = iceBallObj3.GetComponentInChildren<Rigidbody>();
        iceBallObjRigid3.velocity = iceBallPos.forward * 50;

        yield return new WaitForSeconds(0.3f);

        transform.LookAt(targetPlayerTransform);

        anim.SetTrigger("doIceBall");

        yield return new WaitForSeconds(0.5f);

        GameObject iceBallObj4 = PhotonNetwork.Instantiate("IceFatalWheel", iceBallPos.position, iceBallPos.rotation);
        Rigidbody iceBallObjRigid4 = iceBallObj4.GetComponentInChildren<Rigidbody>();
        iceBallObjRigid4.velocity = iceBallPos.forward * 50;

        yield return new WaitForSeconds(0.3f);

        transform.LookAt(targetPlayerTransform);

        anim.SetTrigger("doIceBall");

        yield return new WaitForSeconds(0.5f);

        GameObject iceBallObj5 = PhotonNetwork.Instantiate("IceFatalWheel", iceBallPos.position, iceBallPos.rotation);
        Rigidbody iceBallObjRigid5 = iceBallObj5.GetComponentInChildren<Rigidbody>();
        iceBallObjRigid5.velocity = iceBallPos.forward * 50;

        yield return new WaitForSeconds(5.0f);

        isAttackING = false;

        Destroy(iceBallObj1);
        Destroy(iceBallObj2);
        Destroy(iceBallObj3);
        Destroy(iceBallObj4);
        Destroy(iceBallObj5);

        yield return null;
    }

    IEnumerator DoWindAttack()
    {
        //anim.SetTrigger("doFly");

        yield return new WaitForSeconds(2.0f);

        anim.SetTrigger("doFly");

        GameObject windObj1 = PhotonNetwork.Instantiate("SteelStorm", windPos1.position, windPos1.rotation);
        GameObject windObj2 = PhotonNetwork.Instantiate("SteelStorm", windPos2.position, windPos2.rotation);
        GameObject windObj3 = PhotonNetwork.Instantiate("SteelStorm", windPos3.position, windPos3.rotation);
        GameObject windObj4 = PhotonNetwork.Instantiate("SteelStorm", windPos4.position, windPos4.rotation);

        Rigidbody windObjRigid1 = windObj1.GetComponentInChildren<Rigidbody>();
        Rigidbody windObjRigid2 = windObj2.GetComponentInChildren<Rigidbody>();
        Rigidbody windObjRigid3 = windObj3.GetComponentInChildren<Rigidbody>();
        Rigidbody windObjRigid4 = windObj4.GetComponentInChildren<Rigidbody>();

        windObjRigid1.velocity = windPos1.forward * 50;
        windObjRigid2.velocity = windPos2.forward * 50;
        windObjRigid3.velocity = windPos3.forward * 50;
        windObjRigid4.velocity = windPos4.forward * 50;

        yield return new WaitForSeconds(1.0f);

        GameObject windObj5 = PhotonNetwork.Instantiate("SteelStorm", windPos1.position, windPos1.rotation);
        GameObject windObj6 = PhotonNetwork.Instantiate("SteelStorm", windPos2.position, windPos2.rotation);
        GameObject windObj7 = PhotonNetwork.Instantiate("SteelStorm", windPos3.position, windPos3.rotation);
        GameObject windObj8 = PhotonNetwork.Instantiate("SteelStorm", windPos4.position, windPos4.rotation);

        Rigidbody windObjRigid5 = windObj5.GetComponentInChildren<Rigidbody>();
        Rigidbody windObjRigid6 = windObj6.GetComponentInChildren<Rigidbody>();
        Rigidbody windObjRigid7 = windObj7.GetComponentInChildren<Rigidbody>();
        Rigidbody windObjRigid8 = windObj8.GetComponentInChildren<Rigidbody>();

        windObjRigid5.velocity = windPos1.forward * 50;
        windObjRigid6.velocity = windPos2.forward * 50;
        windObjRigid7.velocity = windPos3.forward * 50;
        windObjRigid8.velocity = windPos4.forward * 50;

        yield return new WaitForSeconds(1.0f);

        GameObject windObj9 = PhotonNetwork.Instantiate("SteelStorm", windPos1.position, windPos1.rotation);
        GameObject windObj10 = PhotonNetwork.Instantiate("SteelStorm", windPos2.position, windPos2.rotation);
        GameObject windObj11 = PhotonNetwork.Instantiate("SteelStorm", windPos3.position, windPos3.rotation);
        GameObject windObj12 = PhotonNetwork.Instantiate("SteelStorm", windPos4.position, windPos4.rotation);

        Rigidbody windObjRigid9 = windObj9.GetComponentInChildren<Rigidbody>();
        Rigidbody windObjRigid10 = windObj10.GetComponentInChildren<Rigidbody>();
        Rigidbody windObjRigid11 = windObj11.GetComponentInChildren<Rigidbody>();
        Rigidbody windObjRigid12 = windObj12.GetComponentInChildren<Rigidbody>();

        windObjRigid9.velocity = windPos1.forward * 50;
        windObjRigid10.velocity = windPos2.forward * 50;
        windObjRigid11.velocity = windPos3.forward * 50;
        windObjRigid12.velocity = windPos4.forward * 50;

        yield return new WaitForSeconds(3.0f);

        Destroy(windObj1);
        Destroy(windObj1);
        Destroy(windObj2);
        Destroy(windObj3);
        Destroy(windObj4);
        Destroy(windObj5);
        Destroy(windObj6);
        Destroy(windObj7);
        Destroy(windObj8);
        Destroy(windObj9);
        Destroy(windObj10);
        Destroy(windObj11);
        Destroy(windObj12);

        anim.SetTrigger("doLand");
        isAttackING = false;
        isFly = false;

        yield return null;
    }

    // �÷��̾�� ���� �浹�� ���� ����ٴ��� ���ϴ� ���� �ذ�
    void FixedUpdate()
    {
        FreezeVelocity();
    }

    void OnTriggerEnter(Collider other)
    {
        if(isDie)
            return;

        if(isFly)
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

        if(isFly)
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

        if(isFly)
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
