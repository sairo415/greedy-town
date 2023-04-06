using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering;

public class BossAlbinoDragon : MonoBehaviourPunCallbacks, IPunObservable
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
    bool isHornAttackING;

    // ���� ��� �ν� ����
    public GameObject targetOnSpot;     // ��ġ�� ������ ����
    public GameObject jumpTargetOnSpot; // ���� ���� ������ ����

    // ���� ����
    public Transform hornPos;
    public Transform jumpPos;
    public Transform windPos1;
    public Transform windPos2;
    public Transform windPos3;
    public Transform windPos4;
    public Transform windPos5;

    // �޸��� �ð�
    float runTimeDelta; // ����
    float runTime;      // ����ġ

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

        // �޸��� �ð�
        runTime = 10.0f;

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
            gameManager.bossAlbino = gameObject.GetComponent<BossAlbinoDragon>();
        }

        // ��ȿ
        StartCoroutine("DoScream");
    }

    void Update()
    {
        // ��ȿ�� ����Ǹ� ���� ���� ���۵�
        if(!isSreamEnd)
            return;

        // ������ Ŭ���̾�Ʈ �ȿ� ������ ��� �ൿ ���� ������ ����
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
            selectedSkillIdx = Random.Range(0, 3);

            // �÷��̾ �νĵȴٸ�
            if(targetPlayerTransform != null)
            {
                // �޸��� �ð� ����
                runTimeDelta += Time.deltaTime;

                if(isAttackING)
                    return;

                if(isHornAttackING)
                    selectedSkillIdx = 0;

                // ��ġ�� ����
                if(selectedSkillIdx == 0)
                {
                    // �� ������ ���۵Ǹ� �ٸ� �������� ��ȯ���� �ʵ��� ������.
                    isHornAttackING = true;

                    // �޸���
                    nav.isStopped = false;
                    isChase = true;
                    anim.SetBool("isRun", true);
                    nav.SetDestination(targetPlayerTransform.position);

                    // ���� �ð����� �޸� �� ��ġ��, �Ǵ� ���߿� �÷��̾ ������ ��ġ��
                    runTimeDelta += Time.deltaTime;
                    if(runTimeDelta >= runTime || targetOnSpot.GetComponent<BossTarget>().isTargetOn)
                    {
                        // ���� �ߴ�
                        nav.isStopped = true;
                        FreezeVelocity();
                        isChase = false;
                        anim.SetBool("isRun", false);

                        // ����
                        isAttackING = true;
                        anim.SetTrigger("doAttack2");
                        StartCoroutine("DoHornAttack");
                    }
                }
                // ȸ���� ����
                else if(selectedSkillIdx == 1)
                {
                    // ���� �ߴ�
                    nav.isStopped = true;
                    FreezeVelocity();
                    isChase = false;
                    anim.SetBool("isRun", false);

                    transform.LookAt(targetPlayerTransform);

                    // ����
                    isAttackING = true;
                    anim.SetTrigger("doAttack");
                    StartCoroutine("DoWindAttack");
                }
                // ���� ����
                else if(selectedSkillIdx == 2)
                {
                    // ���� �ǰ� ���� ���� �÷��̾� ���� ��� ���� ����
                    if(jumpTargetOnSpot.GetComponent<BossAlbinoJumpTarget>().isTargetOn)
                    {
                        // ���� �ߴ�
                        nav.isStopped = true;
                        FreezeVelocity();
                        isChase = false;
                        anim.SetBool("isRun", false);

                        // ����
                        isAttackING = true;
                        anim.SetTrigger("doJump");
                        StartCoroutine("DoJumpAttack");
                    }
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

    // ��ġ�� ����
    IEnumerator DoHornAttack()
    {
        // ���� ���� �ߵ� �� �ִϸ��̼� �ð� ���� ���
        yield return new WaitForSeconds(1.0f);

        // ����Ʈ
        GameObject skillAreaObj = PhotonNetwork.Instantiate("OneHandSmash", hornPos.position, hornPos.rotation);

		// ���� ���� �ð�
		yield return new WaitForSeconds(1.0f);

        PhotonNetwork.Destroy(skillAreaObj);

        // ���� �� ���
        yield return new WaitForSeconds(2.0f);

        isAttackING = false;
        isHornAttackING = false;
        runTimeDelta = 0.0f;

        yield return null;
    }

    // �ٶ� ����
    IEnumerator DoWindAttack()
    {
        yield return new WaitForSeconds(1.0f);

        GameObject skillAreaObj1 = PhotonNetwork.Instantiate("StormTornado", windPos1.position, windPos1.rotation);
        GameObject skillAreaObj2 = PhotonNetwork.Instantiate("StormTornado", windPos2.position, windPos2.rotation);
        GameObject skillAreaObj3 = PhotonNetwork.Instantiate("StormTornado", windPos3.position, windPos3.rotation);
        GameObject skillAreaObj4 = PhotonNetwork.Instantiate("StormTornado", windPos4.position, windPos4.rotation);
        GameObject skillAreaObj5 = PhotonNetwork.Instantiate("StormTornado", windPos5.position, windPos5.rotation);

        skillAreaObj1.SetActive(true);
        skillAreaObj2.SetActive(true);
        skillAreaObj3.SetActive(true);
        skillAreaObj4.SetActive(true);
        skillAreaObj5.SetActive(true);

        yield return new WaitForSeconds(0.6f);

        Rigidbody skillAreaRigid1 = skillAreaObj1.GetComponentInChildren<Rigidbody>();
        Rigidbody skillAreaRigid2 = skillAreaObj2.GetComponentInChildren<Rigidbody>();
        Rigidbody skillAreaRigid3 = skillAreaObj3.GetComponentInChildren<Rigidbody>();
        Rigidbody skillAreaRigid4 = skillAreaObj4.GetComponentInChildren<Rigidbody>();
        Rigidbody skillAreaRigid5 = skillAreaObj5.GetComponentInChildren<Rigidbody>();

        skillAreaRigid1.velocity = windPos1.forward * 50;
        skillAreaRigid2.velocity = windPos2.forward * 50;
        skillAreaRigid3.velocity = windPos3.forward * 50;
        skillAreaRigid4.velocity = windPos4.forward * 50;
        skillAreaRigid5.velocity = windPos5.forward * 50;

        yield return new WaitForSeconds(2.0f);

        isAttackING = false;

        yield return new WaitForSeconds(3.0f);

        skillAreaObj1.SetActive(false);
        skillAreaObj2.SetActive(false);
        skillAreaObj3.SetActive(false);
        skillAreaObj4.SetActive(false);
        skillAreaObj5.SetActive(false);

        PhotonNetwork.Destroy(skillAreaObj1);
        PhotonNetwork.Destroy(skillAreaObj2);
        PhotonNetwork.Destroy(skillAreaObj3);
        PhotonNetwork.Destroy(skillAreaObj4);
        PhotonNetwork.Destroy(skillAreaObj5);

        yield return null;
    }

    // ���� ����
    IEnumerator DoJumpAttack()
	{
        yield return new WaitForSeconds(1.6f);

        GameObject skillAreaObj = PhotonNetwork.Instantiate("LumenJudgement", jumpPos.position, jumpPos.rotation);

        yield return new WaitForSeconds(1.0f);

        PhotonNetwork.Destroy(skillAreaObj);

        yield return new WaitForSeconds(2.0f);

        isAttackING = false;

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
                    PhotonNetwork.Destroy(other.gameObject);
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
