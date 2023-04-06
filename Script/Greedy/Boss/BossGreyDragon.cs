using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering;
using System.Linq;

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

    bool startFlying;
    bool isFlyAttackING;

    // ���� ����
    public Transform iceBallPos;
    public Transform flyAttackPos;
    public Transform windPos1;
    public Transform windPos2;
    public Transform windPos3;
    public Transform windPos4;

    // ���ƴٴϴ� �ð�
    // float flyTimeDelta; // ����
    // float flyTime;      // ����ġ

    // �� ��ֹ� �ð�
    float mapIceTimeDelta; // ����
    float mapIceTime;      // ����ġ

    // ���� ��ų ���� �ε���
    int selectedSkillIdx;

    bool isBossHPLow;

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
        mapIceTimeDelta = 100.0f;
        mapIceTime = 5.0f;

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

            if(targetPlayerTransform != null)
            {
                if(isFlyAttackING)
                {
                    // ����
                    nav.isStopped = false;
                    isChase = true;
                    anim.SetBool("isFlyMove", true);
                    nav.SetDestination(targetPlayerTransform.position);
                }

                mapIceTimeDelta += Time.deltaTime;

                if(((float)curHealth <= (float)maxHealth * 0.3) && (mapIceTimeDelta >= mapIceTime))
                {
                    StartCoroutine("MakeIceField");
                    mapIceTimeDelta = 0.0f;
                }

                if(isAttackING)
                    return;

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
                    StartCoroutine("DoWindAttack");
                }
                else if(selectedSkillIdx == 2)
                {
                    isAttackING = true;
                    StartCoroutine("DoFlyingAttack");
                }
            }
        }
    }

    IEnumerator MakeIceField()
	{
		IceField iceFieldInfo = GameObject.FindObjectOfType<IceField>();
        GameObject[] iceFieldPosInfo = iceFieldInfo.iceFieldPos;
        
        int selectCount = Random.Range(30, 40);
        GameObject[] selectedPos = new GameObject[selectCount];

        for(int i = 0; i < selectCount; i++)
        {
            int index = Random.Range(0, iceFieldPosInfo.Length);
            selectedPos[i] = iceFieldPosInfo[index];
            iceFieldPosInfo = iceFieldPosInfo.Where((val, idx) => idx != index).ToArray();
        }

        GameObject[] iceEffectObj = new GameObject[selectedPos.Length];

        for(int i = 0; i < selectedPos.Length; i++)
        {
            iceEffectObj[i] = PhotonNetwork.Instantiate("IceField", selectedPos[i].transform.position, selectedPos[i].transform.rotation);
        }

        yield return new WaitForSeconds(1.5f);

        GameObject[] iceAttackObj = new GameObject[selectedPos.Length];

        for(int i = 0; i < selectedPos.Length; i++)
        {
            iceAttackObj[i] = PhotonNetwork.Instantiate("IceBlockFissure", selectedPos[i].transform.position, Quaternion.Euler(-90, 0, 0));
        }

        yield return new WaitForSeconds(1.5f);

        for(int i = 0; i < selectedPos.Length; i++)
        {
            PhotonNetwork.Destroy(iceEffectObj[i]);
        }

        yield return new WaitForSeconds(1.5f);

        for(int i = 0; i < selectedPos.Length; i++)
        {
            PhotonNetwork.Destroy(iceAttackObj[i]);
        }

        yield return null;
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

        PhotonNetwork.Destroy(iceBallObj1);
        PhotonNetwork.Destroy(iceBallObj2);
        PhotonNetwork.Destroy(iceBallObj3);
        PhotonNetwork.Destroy(iceBallObj4);
        PhotonNetwork.Destroy(iceBallObj5);

        yield return null;
    }

    IEnumerator DoWindAttack()
    {
        yield return new WaitForSeconds(2.0f);

        anim.SetTrigger("doTakeOff");

        yield return new WaitForSeconds(2.0f);

        anim.SetTrigger("doFly");

        yield return new WaitForSeconds(1.0f);

        transform.LookAt(targetPlayerTransform);

        GameObject windObj1 = PhotonNetwork.Instantiate("SteelStorm", windPos1.position, windPos1.rotation);
        GameObject windObj2 = PhotonNetwork.Instantiate("SteelStorm", windPos2.position, windPos2.rotation);
        GameObject windObj3 = PhotonNetwork.Instantiate("SteelStorm", windPos3.position, windPos3.rotation);
        GameObject windObj4 = PhotonNetwork.Instantiate("SteelStorm", windPos4.position, windPos4.rotation);

        yield return new WaitForSeconds(1.0f);

        transform.LookAt(targetPlayerTransform);

        GameObject windObj5 = PhotonNetwork.Instantiate("SteelStorm", windPos1.position, windPos1.rotation);
        GameObject windObj6 = PhotonNetwork.Instantiate("SteelStorm", windPos2.position, windPos2.rotation);
        GameObject windObj7 = PhotonNetwork.Instantiate("SteelStorm", windPos3.position, windPos3.rotation);
        GameObject windObj8 = PhotonNetwork.Instantiate("SteelStorm", windPos4.position, windPos4.rotation);

        yield return new WaitForSeconds(1.0f);

        transform.LookAt(targetPlayerTransform);

        GameObject windObj9 = PhotonNetwork.Instantiate("SteelStorm", windPos1.position, windPos1.rotation);
        GameObject windObj10 = PhotonNetwork.Instantiate("SteelStorm", windPos2.position, windPos2.rotation);
        GameObject windObj11 = PhotonNetwork.Instantiate("SteelStorm", windPos3.position, windPos3.rotation);
        GameObject windObj12 = PhotonNetwork.Instantiate("SteelStorm", windPos4.position, windPos4.rotation);

        yield return new WaitForSeconds(4.0f);

        // Null ����ó��
        PhotonNetwork.Destroy(windObj1);
        PhotonNetwork.Destroy(windObj1);
        PhotonNetwork.Destroy(windObj2);
        PhotonNetwork.Destroy(windObj3);
        PhotonNetwork.Destroy(windObj4);
        PhotonNetwork.Destroy(windObj5);
        PhotonNetwork.Destroy(windObj6);
        PhotonNetwork.Destroy(windObj7);
        PhotonNetwork.Destroy(windObj8);
        PhotonNetwork.Destroy(windObj9);
        PhotonNetwork.Destroy(windObj10);
        PhotonNetwork.Destroy(windObj11);
        PhotonNetwork.Destroy(windObj12);

        anim.SetTrigger("doIdle");
        isAttackING = false;

        yield return null;
    }

    IEnumerator DoFlyingAttack()
    {
        yield return new WaitForSeconds(2.0f);

        anim.SetTrigger("doTakeOff");

        yield return new WaitForSeconds(2.0f);

        anim.SetTrigger("doFly");

        yield return new WaitForSeconds(1.0f);

        anim.SetBool("isFlyMove", true);

        isFlyAttackING = true;

        //InvokeRepeating("CallIceField", 1.0f, 0.5f);
        GameObject[] iceFieldArray = new GameObject[25];
        GameObject[] iceAttackArray = new GameObject[25];
        for(int i = 0; i < 20; i++)
        {
            iceFieldArray[i] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
            //iceAttackArray[i] = PhotonNetwork.Instantiate("IceBlockCrash", flyAttackPos.position, flyAttackPos.rotation);
            StartCoroutine("CallIceAttack");

            yield return new WaitForSeconds(0.5f);
        }

        for(int i = 0; i < 20; i++)
        {
            PhotonNetwork.Destroy(iceFieldArray[i]);
        }
        
        yield return new WaitForSeconds(2.0f);

        // ���� ����
        isFlyAttackING = false;
        nav.isStopped = true;
        FreezeVelocity();
        isChase = false;
        anim.SetBool("isFlyMove", false);

        yield return new WaitForSeconds(2.0f);

        anim.SetTrigger("doIdle");

        yield return new WaitForSeconds(5.0f);

        isAttackING = false;

        yield return null;
    }

    /*IEnumerator CallIceField()
    {
        GameObject[] iceFieldArray = new GameObject[20];

        iceFieldArray[0] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[1] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[2] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[3] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[4] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[5] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[6] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[0]);

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[7] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[1]);

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[8] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[2]);

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[9] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[3]);

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[10] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[4]);

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[11] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[5]);

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[12] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[6]);

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[13] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[7]);

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[14] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[8]);

        yield return new WaitForSeconds(0.5f);
        
        iceFieldArray[15] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[9]);

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[16] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[10]);

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[17] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[11]);

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[18] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[12]);

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[19] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[13]);

        yield return new WaitForSeconds(0.5f);

        PhotonNetwork.Destroy(iceFieldArray[14]);

        yield return new WaitForSeconds(0.5f);

        PhotonNetwork.Destroy(iceFieldArray[15]);

        yield return new WaitForSeconds(0.5f);

        PhotonNetwork.Destroy(iceFieldArray[16]);

        yield return new WaitForSeconds(0.5f);

        PhotonNetwork.Destroy(iceFieldArray[17]);

        yield return new WaitForSeconds(0.5f);

        PhotonNetwork.Destroy(iceFieldArray[18]);

        yield return new WaitForSeconds(0.5f);

        PhotonNetwork.Destroy(iceFieldArray[19]);

        yield return null;
    }*/

    IEnumerator CallIceAttack()
    {
        GameObject iceAttack = PhotonNetwork.Instantiate("IceBlockCrash", flyAttackPos.position, flyAttackPos.rotation);

        yield return new WaitForSeconds(3.0f);

        PhotonNetwork.Destroy(iceAttack);

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
