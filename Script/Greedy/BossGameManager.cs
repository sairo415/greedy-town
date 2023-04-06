using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering;

public class BossGameManager : MonoBehaviour
{
    // Photon
    private PhotonView pv;

    // stage
    public int stage;
    public TextMeshProUGUI txtStageNumber;

    // UI Panel
    public GameObject ingamePanel; // �ΰ��� ȭ��
    public GameObject selectSkillPanel; // ��ų ���� ȭ��
    public GameObject dangerPanel; // ���� ȭ��
    public GameObject deathPanel;   // ��� ȭ��
    public GameObject gameEndPanel; // ���� ���� �ǳ�
    public GameObject playerCntPanel;   // 0�������� ��Ż�� �ο��� �ȳ� �ǳ�

    // ���� ���� �޽���
    public GameObject gameOverTxt;
    public GameObject gameClearTxt;

    // ����
    public GameObject smallBoss;
    //public GameObject bigBoss;

    //1
    public BossAlbinoDragon bossAlbino;
    public Transform bossAlbinoSpawnPoint;

    //2
    public BossPurple bossPurple;
    public Transform bossPurpleSpawnPoint;

    //3
    public BossGreyDragon bossGrey;
    public Transform bossGreySpawnPoint;

    //4
    public BossRed bossRed;
    public Transform bossRedSpawnPoint;

    public GameObject bossHealthGroup;
    public RectTransform bossTempHealthBar;
    public RectTransform bossHealthBar;

    int pastBossHealth; // ���� ���� ü��
    bool isBigBossSpawn;

    // �÷��̾�
    public BossPlayer player;

    public GameObject playerHealthGroup;
    public RectTransform playerHealthBar;

    public GameObject playerSkillIconGroup;

    int pastBossPlayerHealth; // ���� �÷��̾� ü��

    // ���õ� ��ų ������
    Image imgSkill_1;
    Image imgSkill_2;
    Image imgSkill_3;
    Image imgSkill_4;
    public Image imgSkill_Dash;

    // Lv1 ��ų �׷�, ������
    public GameObject objStoneSwing;
    public GameObject objSwordSwing;
    public GameObject objBloodSwing;
    public GameObject objPaladinSwing;

    public Image imgStoneSwing;
    public Image imgSwordSwing;
    public Image imgBloodSwing;
    public Image imgPaladinSwing;

    // Lv2 ��ų �׷�, ������
    public GameObject objSwordForce;
    public GameObject objVampirism;
    public GameObject objIntent;

    public Image imgSwordForce;
    public Image imgVampirism;
    public Image imgIntent;

    // Lv3 ��ų �׷�, ������
    public GameObject objSwordDance;
    public GameObject objBloodExplosion;
    public GameObject objBlessing;

    public Image imgSwordDance;
    public Image imgBloodExplosion;
    public Image imgBlessing;

    // Lv4 ��ų �׷�, ������
    public GameObject objSwordBlade;
    public GameObject objBloodField;
    public GameObject objResurrection;

    public Image imgSwordBlade;
    public Image imgBloodField;
    public Image imgResurrection;

    // ��ų ��Ÿ��
    float qCool = -1;
    float wCool = -1;
    float eCool = -1;
    float rCool = -1;
    float sCool = -1;

    // ��ų ��� �� ��� �ð�
    float qDelta;
    float wDelta;
    float eDelta;
    float rDelta;
    float sDelta;

    // ��ų ���� ���� ����
    bool isQReady;
    bool isWReady;
    bool isEReady;
    bool isRReady;
    bool isSReady;

    // �� �̵� ����
    int nextSceneIndex;

    // ��ų ���� ī��Ʈ �ٿ�
    float skillSelectCountDown;

    bool isSkillCountStart;

    public TextMeshProUGUI skillSelectCountDownTxt;

    public Button selectStartGame;

    public Button selectLeftButton;
    public Button selectCenterButton;
    public Button selectRightButton;

    public TextMeshProUGUI selectLeftButtonTxt;
    public TextMeshProUGUI selectCenterButtonTxt;
    public TextMeshProUGUI selectRightButtonTxt;

    public Image imgSelectLeftSkillImage;
    public Image imgSelectCenterSkillImage;
    public Image imgSelectRightSkillImage;

    // �� �帧
    // ���� -> �߰� ���� ���� (isSmallBossDie = false)
    // -> �߰� ���� ��� (isSmallBossDie = true)
    // -> �� ���� ���� (isMainBossDie = false)
    // -> �� ���� ��� (isMainBossDie = true)
    // -> ��ų ���� �ǳ� Ȱ��ȭ (���� �ð� 20 ��)
    // �ð� ��� �� �� �̵�
    bool isSmallBossDie;
    bool isMainBossDie;

    // Update ���� �ٽ� �˻��� �ʿ䰡 ���� ��� true
    bool isQCheck;
    bool isWCheck;
    bool isECheck;
    bool isRCheck;

    // �̹��� ���
    public Sprite spriteSwordForce;
    public Sprite spriteVampirism;
    public Sprite spriteIntent;
    public Sprite spriteSwordDance;
    public Sprite spriteBloodExplosion;
    public Sprite spriteBlessing;
    public Sprite spriteSwordBlade;
    public Sprite spriteBloodField;
    public Sprite spriteResurrection;

    // �������� ��µ� ��ų ����â ��ų ���
    int[] randomSkillIndex;

    // ��ų ���� �Ϸ�
    public Image imgSelectLeftButton;
    public Image imgSelectCenterButton;
    public Image imgSelectRightButton;

    bool isPlayerHPDanger;
    bool isPlayerDie;

    bool isGameEnd;

    // ���� ��� �� �÷��̾� ����ص�, ���� ���� ��� ���ϰ� ��.
    bool isStageClear;

    void Awake()
    {
        pv = GetComponent<PhotonView>();

        if(player != null)
            pastBossPlayerHealth = player.maxHealth;

        isQReady = true;
        isWReady = true;
        isEReady = true;
        isRReady = true;
        isSReady = true;

        skillSelectCountDown = 10.0f;
    }

    void Start()
    {
        txtStageNumber.text = "stage : " + stage;
        //nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        //DontDestroyOnLoad(this.gameObject);
        //SceneManager.sceneLoaded += OnSceneLoaded;

        //stage = 0;
    }

    private void Update()
    {
        if(player == null)
            return;

        qCool = player.GetComponent<BossPlayer>().qSkillRate;
        wCool = player.GetComponent<BossPlayer>().wSkillRate;
        eCool = player.GetComponent<BossPlayer>().eSkillRate;
        rCool = player.GetComponent<BossPlayer>().rSkillRate;
        sCool = player.GetComponent<BossPlayer>().dodgeRate;

        if(!isQCheck)
        {
            if(player.GetComponent<BossPlayer>().lvOneSkill == BossPlayer.LvOneSkill.NULL)
            {
                imgSkill_1 = imgStoneSwing;
            }
            else if(player.GetComponent<BossPlayer>().lvOneSkill == BossPlayer.LvOneSkill.SwordForce)
            {
                objStoneSwing.SetActive(false);
                objSwordSwing.SetActive(true);
                isQCheck = true;
                imgSkill_1 = imgSwordSwing;
            }
            else if(player.GetComponent<BossPlayer>().lvOneSkill == BossPlayer.LvOneSkill.Vampirism)
            {
                objStoneSwing.SetActive(false);
                objBloodSwing.SetActive(true);
                isQCheck = true;
                imgSkill_1 = imgBloodSwing;
            }
            else if(player.GetComponent<BossPlayer>().lvOneSkill == BossPlayer.LvOneSkill.Intent)
            {
                objStoneSwing.SetActive(false);
                objPaladinSwing.SetActive(true);
                isQCheck = true;
                imgSkill_1 = imgPaladinSwing;
            }
        }

        if(!isWCheck)
        {
            if(player.GetComponent<BossPlayer>().lvOneSkill == BossPlayer.LvOneSkill.SwordForce)
            {
                objSwordForce.SetActive(true);
                isWCheck = true;
                imgSkill_2 = imgSwordForce;
            }
            else if(player.GetComponent<BossPlayer>().lvOneSkill == BossPlayer.LvOneSkill.Vampirism)
            {
                objVampirism.SetActive(true);
                isWCheck = true;
                imgSkill_2 = imgVampirism;
            }
            else if(player.GetComponent<BossPlayer>().lvOneSkill == BossPlayer.LvOneSkill.Intent)
            {
                objIntent.SetActive(true);
                isWCheck = true;
                imgSkill_2 = imgIntent;
            }
        }

        if(!isECheck)
        {
            if(player.GetComponent<BossPlayer>().lvTwoSkill == BossPlayer.LvTwoSkill.SwordDance)
            {
                objSwordDance.SetActive(true);
                isECheck = true;
                imgSkill_3 = imgSwordDance;
            }
            else if(player.GetComponent<BossPlayer>().lvTwoSkill == BossPlayer.LvTwoSkill.BloodExplosion)
            {
                objBloodExplosion.SetActive(true);
                isECheck = true;
                imgSkill_3 = imgBloodExplosion;
            }
            else if(player.GetComponent<BossPlayer>().lvTwoSkill == BossPlayer.LvTwoSkill.Blessing)
            {
                objBlessing.SetActive(true);
                isECheck = true;
                imgSkill_3 = imgBlessing;
            }
        }

        if(!isRCheck)
        {
            if(player.GetComponent<BossPlayer>().lvThreeSkill == BossPlayer.LvThreeSkill.SwordBlade)
            {
                objSwordBlade.SetActive(true);
                isRCheck = true;
                imgSkill_4 = imgSwordBlade;
            }
            else if(player.GetComponent<BossPlayer>().lvThreeSkill == BossPlayer.LvThreeSkill.BloodField)
            {
                objBloodField.SetActive(true);
                isRCheck = true;
                imgSkill_4 = imgBloodField;
            }
            else if(player.GetComponent<BossPlayer>().lvThreeSkill == BossPlayer.LvThreeSkill.Resurrection)
            {
                objResurrection.SetActive(true);
                isRCheck = true;
                imgSkill_4 = imgResurrection;
            }
        }


        if(!player.isSkillReady)
            return;
/*
        if(player.isDie)
            return;*/

        if(Input.GetKeyDown(KeyCode.Q) && isQReady)
        {
            isQReady = false;
            imgSkill_1.fillAmount = 0.0f;
            StartCoroutine(CoolTimeQ(qCool));
        }
        else if(Input.GetKeyDown(KeyCode.W) && isWReady && isWCheck)
        {
            isWReady = false;
            imgSkill_2.fillAmount = 0.0f;
            StartCoroutine(CoolTimeW(wCool));
        }
        else if(Input.GetKeyDown(KeyCode.E) && isEReady && isECheck)
        {
            isEReady = false;
            imgSkill_3.fillAmount = 0.0f;
            StartCoroutine(CoolTimeE(eCool));
        }
        else if(Input.GetKeyDown(KeyCode.R) && isRReady && isRCheck)
        {
            isRReady = false;
            imgSkill_4.fillAmount = 0.0f;

            StartCoroutine(CoolTimeR(rCool));
            // ��Ȱ ��ų�� �ƴ� ��츸 ��Ÿ�� �ε�

            StartCoroutine(CoolTimeR(rCool));
        }
        else if(Input.GetKeyDown(KeyCode.Space) && isSReady && player.GetComponent<BossPlayer>().moveVec != Vector3.zero)
        {
            isSReady = false;
            imgSkill_Dash.fillAmount = 0.0f;
            StartCoroutine(CoolTimeS(sCool));
        }
    }

    IEnumerator CoolTimeQ(float cool)
    {
        while(true)
        {
            qDelta += Time.deltaTime;
            imgSkill_1.fillAmount = (qDelta / cool);

            if(qDelta >= cool)
                break;

            yield return new WaitForFixedUpdate();
        }

        qDelta = 0;
        isQReady = true;

        yield return null;
    }

    IEnumerator CoolTimeW(float cool)
    {
        while(true)
        {
            wDelta += Time.deltaTime;
            imgSkill_2.fillAmount = (wDelta / cool);

            if(wDelta >= cool)
                break;

            yield return new WaitForFixedUpdate();
        }

        wDelta = 0;
        isWReady = true;

        yield return null;
    }

    IEnumerator CoolTimeE(float cool)
    {
        while(true)
        {
            eDelta += Time.deltaTime;
            imgSkill_3.fillAmount = (eDelta / cool);

            if(eDelta >= cool)
                break;

            yield return new WaitForFixedUpdate();
        }

        eDelta = 0;
        isEReady = true;

        yield return null;
    }

    IEnumerator CoolTimeR(float cool)
    {
        while(true)
        {
            rDelta += Time.deltaTime;

            if(player.GetComponent<BossPlayer>().lvThreeSkill != BossPlayer.LvThreeSkill.Resurrection)
                imgSkill_4.fillAmount = (rDelta / cool);

            //Debug.Log(rDelta);

            if(rDelta >= cool)
                break;

            yield return new WaitForFixedUpdate();
        }

        rDelta = 0;
        isRReady = true;

        yield return null;
    }

    IEnumerator CoolTimeS(float cool)
    {
        while(true)
        {
            sDelta += Time.deltaTime;
            imgSkill_Dash.fillAmount = (sDelta / cool);

            if(sDelta >= cool)
                break;

            yield return new WaitForFixedUpdate();
        }

        sDelta = 0;
        isSReady = true;

        yield return null;
    }

    void LateUpdate()
    {
        // �߰� ���� Ŭ���� Ȯ��
        if(stage != 0 && smallBoss.transform.childCount == 0 && !isBigBossSpawn)
        {
            isBigBossSpawn = true;

            // �ɷ�ġ ���� ȭ��
            // �ð� ���� 10��
            // 10�ʰ� ������ ���� ��ȯ
            AbilitySelect();

            // 10�ʰ� �������� Ȯ��
            // �ɷ�ġ ���� â ��Ȱ��ȭ

            //Transform stage1BigBossPosition = 

            //bigBoss.gameObject.SetActive(true);
            if(PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                if (stage == 1)
                {
                    bossAlbino = PhotonNetwork.Instantiate("BossAlbino", bossAlbinoSpawnPoint.position, bossAlbinoSpawnPoint.rotation, 0).GetComponent<BossAlbinoDragon>();
                    pastBossHealth = bossAlbino.GetComponent<BossAlbinoDragon>().maxHealth;
                }
                else if (stage == 2)
                {
                    bossPurple = PhotonNetwork.Instantiate("BossPurple", bossPurpleSpawnPoint.position, bossPurpleSpawnPoint.rotation, 0).GetComponent<BossPurple>();
                    pastBossHealth = bossPurple.GetComponent<BossPurple>().maxHealth;
                }
                else if(stage == 3)
                {
                    bossGrey = PhotonNetwork.Instantiate("BossGrey", bossGreySpawnPoint.position, bossGreySpawnPoint.rotation, 0).GetComponent<BossGreyDragon>();
                    pastBossHealth = bossGrey.GetComponent<BossGreyDragon>().maxHealth;
                }
                else if (stage == 4)
                {
                    bossRed = PhotonNetwork.Instantiate("BossRed", bossRedSpawnPoint.position, bossRedSpawnPoint.rotation, 0).GetComponent<BossRed>();
                    pastBossHealth = bossRed.GetComponent<BossRed>().maxHealth;
                }
            }

            bossHealthGroup.SetActive(true);
        }

        // ���� ü�� UI ����
        if (stage != 0)
        {
            if(bossAlbino != null && stage == 1 && pastBossHealth != bossAlbino.curHealth)
            {
                StartCoroutine("BossHPGauge");
                pastBossHealth = bossAlbino.curHealth;
            }
            else if (bossPurple != null && stage == 2 && pastBossHealth != bossPurple.curHealth)
            {
                StartCoroutine("BossHPGauge");
                pastBossHealth = bossPurple.curHealth;
            }
            else if(bossGrey != null && stage == 3 && pastBossHealth != bossGrey.curHealth)
            {
                StartCoroutine("BossHPGauge");
                pastBossHealth = bossGrey.curHealth;
            }
            else if (bossRed != null && stage == 4 && pastBossHealth != bossRed.curHealth)
            {
                StartCoroutine("BossHPGauge");
                pastBossHealth = bossRed.curHealth;
            }
        }

        // ���� ���
        if (stage != 0 && !isSkillCountStart)
        {
            if(bossAlbino != null && stage == 1 && bossAlbino.curHealth <= 0)
            {
                isSkillCountStart = true;
                isStageClear = true;
                SkillSelect();
            }
            else if (bossPurple != null && stage == 2 && bossPurple.curHealth <= 0)
            {
                isSkillCountStart = true;
                isStageClear = true;
                SkillSelect();
            }
            else if(bossGrey != null && stage == 3 && bossGrey.curHealth <= 0)
            {
                isSkillCountStart = true;
                isStageClear = true;
                SkillSelect();
            }
            else if (bossRed != null && stage == 4 && bossRed.curHealth <= 0)
            {
                isStageClear = true;
            }
        }

        // �÷��̾� ü�� UI ����
        if(player != null && pastBossPlayerHealth != player.curHealth)
        {
            StartCoroutine("PlayerHPGauge");
            pastBossPlayerHealth = player.curHealth;
        }

        // ü���� 10�� ������ ��� ��� on
        if (player != null && ((float)player.curHealth <= (float)player.maxHealth * 0.1) && !isPlayerHPDanger && player.curHealth > 0)
        {
            isPlayerHPDanger = true;
            StartCoroutine("displayDangerPanel");
        }

        // ü���� ������ ���ó��
        if (player != null && player.curHealth <= 0)
        {
            PlayerDie();
            isPlayerDie = true;
        }

        // ��Ȱ �� �ٽ� ���
        if (player != null && isPlayerDie && player.curHealth > 0)
        {
            PlayerAlive();
            isPlayerDie = false;
        }

        // ���� ��� �� ������ ��ư ����� ��
        bool allDie = true;
        BossPlayer[] bossPlayers = GameObject.FindObjectsOfType<BossPlayer>();
        foreach(BossPlayer bossPlayer in bossPlayers)
        {
            if(!bossPlayer.isDie)
            {
                allDie = false;
                break;
            }
        }

        if(bossPlayers.Length >0 && allDie && !isGameEnd && !isStageClear)
        {
            isGameEnd = true;
            Debug.Log("���� ���");
            GameOver();
        }

        if(bossPlayers.Length > 0 && stage == 4 && bossRed.curHealth <= 0 && !isGameEnd)
        {
            isGameEnd = true;
            Debug.Log("���� Ŭ����");
            GameClear();
        }
    }

    IEnumerator displayDangerPanel()
    {
        dangerPanel.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        dangerPanel.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        isPlayerHPDanger = false;
        yield return null;
    }

    IEnumerator PlayerHPGauge()
    {
        float reteBossPlayerHP = (float)player.curHealth / player.maxHealth;
        playerHealthBar.localScale = new Vector3(reteBossPlayerHP, 1, 1);
        yield return null;
    }


    IEnumerator BossHPGauge()
    {
        int curHP = 0;
        int maxHP = 0;

        if(stage == 1)
        {
            curHP = bossAlbino.curHealth;
            maxHP = bossAlbino.maxHealth;
        }
        else if (stage == 2)
        {
            curHP = bossPurple.curHealth;
            maxHP = bossPurple.maxHealth;
        }
        else if(stage == 3)
        {
            curHP = bossGrey.curHealth;
            maxHP = bossGrey.maxHealth;
        }
        else if (stage == 4)
        {
            curHP = bossRed.curHealth;
            maxHP = bossRed.maxHealth;
        }

        float reteBossHP = (float)curHP / maxHP;

        bossHealthBar.localScale = new Vector3(reteBossHP, 1, 1);

        yield return new WaitForSeconds(0.5f);

        bossTempHealthBar.localScale = new Vector3(reteBossHP, 1, 1);

        yield return null;
    }

    void AbilitySelect()
    {
        // �ɷ�ġ �߰� ���߿� ����
        return;
    }

    void SkillSelect()
    {
        int[] indexArray;
        randomSkillIndex = new int[3];
        int lengthSkills = 3;

        switch(stage)
        {
        case 1:
            lengthSkills = System.Enum.GetValues(typeof(BossPlayer.LvOneSkill)).Length;
            break;
        case 2:
            lengthSkills = System.Enum.GetValues(typeof(BossPlayer.LvTwoSkill)).Length;
            break;
        case 3:
            lengthSkills = System.Enum.GetValues(typeof(BossPlayer.LvThreeSkill)).Length;
            break;
        }

        indexArray = new int[lengthSkills];

        for(int i = 0; i < lengthSkills; i++)
        {
            indexArray[i] = i;
        }

        for(int i = 0; i < 3; i++)
        {
            Debug.Log("indexArray.Length : " + indexArray.Length);
            int index = Random.Range(1, indexArray.Length);
            randomSkillIndex[i] = indexArray[index];
            indexArray = indexArray.Where((val, idx) => idx != index).ToArray();
        }

        string swordForceScript = "�˱�[��Ƽ��]\n������ ���� �˱⸦ �����ϴ�.";
        string vampirismScript = "���� ����[�нú�]\n �⺻ ������ [����] �Ӽ��� �����ϴ�. [����]������ ������ �����ϸ� ü���� ȸ���˴ϴ�.";
        string intentScript = "����[�нú�]\n������ ����մϴ�.";

        string swordDanceScript = "�˹�[��Ƽ��]\n���濡 ���������� �������� �ִ� ������ �����մϴ�.";
        string bloodExplosionScript = "����[��Ƽ��]\nü���� �Ҹ��Ͽ� ĳ���� �ֺ����� ���� ������ ����ŵ�ϴ�.";
        string blessingScript = "�ູ[��Ƽ��]\n��Ƽ���� ü���� ȸ����Ű�� ������ �����մϴ�.";

        string swordBladeScript = "��ǳ[��Ƽ��]\nĳ���� �ֺ��� ���� ������ �����մϴ�. ��õ��� ���� ���°� �˴ϴ�.";
        string bloodFieldScript = "���� �ʵ�[��Ƽ��]\n[����] ����. ĳ���� �ֺ��� ���� ������ ���������� �����մϴ�.";
        string resurrectionScript = "��Ȱ[��Ƽ��]\n�� �ѹ� ��� ��Ƽ������ ��Ȱ ��ŵ�ϴ�. ���� ����� ��� �Ұ��մϴ�.";

        switch(stage)
        {
        case 1:
            for(int i = 0; i < 3; i++)
            {
                if(randomSkillIndex[i] == 1)
                {
                    if(i == 0)
                    {
                        imgSelectLeftSkillImage.sprite = spriteSwordForce;
                        selectLeftButtonTxt.text = swordForceScript;
                    }
                    else if(i == 1)
                    {
                        imgSelectCenterSkillImage.sprite = spriteSwordForce;
                        selectCenterButtonTxt.text = swordForceScript;
                    }
                    else if(i == 2)
                    {
                        imgSelectRightSkillImage.sprite = spriteSwordForce;
                        selectRightButtonTxt.text = swordForceScript;
                    }
                }
                else if(randomSkillIndex[i] == 2)
                {
                    if(i == 0)
                    {
                        imgSelectLeftSkillImage.sprite = spriteVampirism;
                        selectLeftButtonTxt.text = vampirismScript;
                    }
                    else if(i == 1)
                    {
                        imgSelectCenterSkillImage.sprite = spriteVampirism;
                        selectCenterButtonTxt.text = vampirismScript;
                    }
                    else if(i == 2)
                    {
                        imgSelectRightSkillImage.sprite = spriteVampirism;
                        selectRightButtonTxt.text = vampirismScript;
                    }
                }
                else if(randomSkillIndex[i] == 3)
                {
                    if(i == 0)
                    {
                        imgSelectLeftSkillImage.sprite = spriteIntent;
                        selectLeftButtonTxt.text = intentScript;
                    }
                    else if(i == 1)
                    {
                        imgSelectCenterSkillImage.sprite = spriteIntent;
                        selectCenterButtonTxt.text = intentScript;
                    }
                    else if(i == 2)
                    {
                        imgSelectRightSkillImage.sprite = spriteIntent;
                        selectRightButtonTxt.text = intentScript;
                    }
                }
            }
            break;
        case 2:
            for(int i = 0; i < 3; i++)
            {
                if(randomSkillIndex[i] == 1)
                {
                    if(i == 0)
                    {
                        imgSelectLeftSkillImage.sprite = spriteSwordDance;
                        selectLeftButtonTxt.text = swordDanceScript;
                    }
                    else if(i == 1)
                    {
                        imgSelectCenterSkillImage.sprite = spriteSwordDance;
                        selectCenterButtonTxt.text = swordDanceScript;
                    }
                    else if(i == 2)
                    {
                        imgSelectRightSkillImage.sprite = spriteSwordDance;
                        selectRightButtonTxt.text = swordDanceScript;
                    }
                }
                else if(randomSkillIndex[i] == 2)
                {
                    if(i == 0)
                    {
                        imgSelectLeftSkillImage.sprite = spriteBloodExplosion;
                        selectLeftButtonTxt.text = bloodExplosionScript;
                    }
                    else if(i == 1)
                    {
                        imgSelectCenterSkillImage.sprite = spriteBloodExplosion;
                        selectCenterButtonTxt.text = bloodExplosionScript;
                    }
                    else if(i == 2)
                    {
                        imgSelectRightSkillImage.sprite = spriteBloodExplosion;
                        selectRightButtonTxt.text = bloodExplosionScript;
                    }
                }
                else if(randomSkillIndex[i] == 3)
                {
                    if(i == 0)
                    {
                        imgSelectLeftSkillImage.sprite = spriteBlessing;
                        selectLeftButtonTxt.text = blessingScript;
                    }
                    else if(i == 1)
                    {
                        imgSelectCenterSkillImage.sprite = spriteBlessing;
                        selectCenterButtonTxt.text = blessingScript;
                    }
                    else if(i == 2)
                    {
                        imgSelectRightSkillImage.sprite = spriteBlessing;
                        selectRightButtonTxt.text = blessingScript;
                    }
                }
            }
            break;
        case 3:
            for(int i = 0; i < 3; i++)
            {
                if(randomSkillIndex[i] == 1)
                {
                    if(i == 0)
                    {
                        imgSelectLeftSkillImage.sprite = spriteSwordBlade;
                        selectLeftButtonTxt.text = swordBladeScript;
                    }
                    else if(i == 1)
                    {
                        imgSelectCenterSkillImage.sprite = spriteSwordBlade;
                        selectCenterButtonTxt.text = swordBladeScript;
                    }
                    else if(i == 2)
                    {
                        imgSelectRightSkillImage.sprite = spriteSwordBlade;
                        selectRightButtonTxt.text = swordBladeScript;
                    }
                }
                else if(randomSkillIndex[i] == 2)
                {
                    if(i == 0)
                    {
                        imgSelectLeftSkillImage.sprite = spriteBloodField;
                        selectLeftButtonTxt.text = bloodFieldScript;
                    }
                    else if(i == 1)
                    {
                        imgSelectCenterSkillImage.sprite = spriteBloodField;
                        selectCenterButtonTxt.text = bloodFieldScript;
                    }
                    else if(i == 2)
                    {
                        imgSelectRightSkillImage.sprite = spriteBloodField;
                        selectRightButtonTxt.text = bloodFieldScript;
                    }
                }
                else if(randomSkillIndex[i] == 3)
                {
                    if(i == 0)
                    {
                        imgSelectLeftSkillImage.sprite = spriteResurrection;
                        selectLeftButtonTxt.text = resurrectionScript;
                    }
                    else if(i == 1)
                    {
                        imgSelectCenterSkillImage.sprite = spriteResurrection;
                        selectCenterButtonTxt.text = resurrectionScript;
                    }
                    else if(i == 2)
                    {
                        imgSelectRightSkillImage.sprite = spriteResurrection;
                        selectRightButtonTxt.text = resurrectionScript;
                    }
                }
            }
            break;
        }

        ingamePanel.SetActive(false);
        selectSkillPanel.SetActive(true);
        StartCoroutine("MoveNextScene");
    }

    public void PressStartGameButton()
    {
        pv.RPC("MoveFirstScene", RpcTarget.All);
    }

    [PunRPC]
    public void MoveFirstScene()
    {
        SceneManager.LoadScene("BossScene1");
    }

    // ��� ĳ������ ��ų ������ ����ȭ
    [PunRPC]
    void SyncSelectLvOneSkill(int bossPlayerViewID, BossPlayer.LvOneSkill selected)
    {
        GameObject bossPlayerObj = PhotonView.Find(bossPlayerViewID).gameObject;
        bossPlayerObj.GetComponent<BossPlayer>().lvOneSkill = selected;
    }

    [PunRPC]
    void SyncSelectLvTwoSkill(int bossPlayerViewID, BossPlayer.LvTwoSkill selected)
    {
        GameObject bossPlayerObj = PhotonView.Find(bossPlayerViewID).gameObject;
        bossPlayerObj.GetComponent<BossPlayer>().lvTwoSkill = selected;
    }

    [PunRPC]
    void SyncSelectLvThreeSkill(int bossPlayerViewID, BossPlayer.LvThreeSkill selected)
    {
        GameObject bossPlayerObj = PhotonView.Find(bossPlayerViewID).gameObject;
        bossPlayerObj.GetComponent<BossPlayer>().lvThreeSkill = selected;
    }

    public void PressButton1()
    {
        PressButtonFunc(0);
    }

    public void PressButton2()
    {
        PressButtonFunc(1);
    }

    public void PressButton3()
    {
        PressButtonFunc(2);
    }

    void PressButtonFunc(int pressedIdx)
    {
        int bossPlayerViewID = player.pv.ViewID;

        imgSelectLeftButton.gameObject.SetActive(false);
        imgSelectCenterButton.gameObject.SetActive(false);
        imgSelectRightButton.gameObject.SetActive(false);

        if(pressedIdx == 0)
            imgSelectLeftButton.gameObject.SetActive(true);
        else if(pressedIdx == 1)
            imgSelectCenterButton.gameObject.SetActive(true);
        else if(pressedIdx == 2)
            imgSelectRightButton.gameObject.SetActive(true);

        switch(stage)
        {
        case 1:
            if(randomSkillIndex[pressedIdx] == 1)
            {
                player.GetComponent<BossPlayer>().lvOneSkill = BossPlayer.LvOneSkill.SwordForce;
            }
            else if(randomSkillIndex[pressedIdx] == 2)
            {
                player.GetComponent<BossPlayer>().lvOneSkill = BossPlayer.LvOneSkill.Vampirism;
            }
            else if(randomSkillIndex[pressedIdx] == 3)
            {
                player.GetComponent<BossPlayer>().lvOneSkill = BossPlayer.LvOneSkill.Intent;
            }

            pv.RPC("SyncSelectLvOneSkill", RpcTarget.AllBuffered, bossPlayerViewID, player.GetComponent<BossPlayer>().lvOneSkill);

            break;
        case 2:
            if(randomSkillIndex[pressedIdx] == 1)
            {
                player.GetComponent<BossPlayer>().lvTwoSkill = BossPlayer.LvTwoSkill.SwordDance;
            }
            else if(randomSkillIndex[pressedIdx] == 2)
            {
                player.GetComponent<BossPlayer>().lvTwoSkill = BossPlayer.LvTwoSkill.BloodExplosion;
            }
            else if(randomSkillIndex[pressedIdx] == 3)
            {
                player.GetComponent<BossPlayer>().lvTwoSkill = BossPlayer.LvTwoSkill.Blessing;
            }

            pv.RPC("SyncSelectLvTwoSkill", RpcTarget.AllBuffered, bossPlayerViewID, player.GetComponent<BossPlayer>().lvTwoSkill);

            break;
        case 3:
            if(randomSkillIndex[pressedIdx] == 1)
            {
                player.GetComponent<BossPlayer>().lvThreeSkill = BossPlayer.LvThreeSkill.SwordBlade;
            }
            else if(randomSkillIndex[pressedIdx] == 2)
            {
                player.GetComponent<BossPlayer>().lvThreeSkill = BossPlayer.LvThreeSkill.BloodField;
            }
            else if(randomSkillIndex[pressedIdx] == 3)
            {
                player.GetComponent<BossPlayer>().lvThreeSkill = BossPlayer.LvThreeSkill.Resurrection;
            }

            pv.RPC("SyncSelectLvThreeSkill", RpcTarget.AllBuffered, bossPlayerViewID, player.GetComponent<BossPlayer>().lvThreeSkill);

            break;
        }
    }

    IEnumerator MoveNextScene()
    {
        while(true)
        {
            skillSelectCountDown -= Time.deltaTime;

            int displayCount = (int)skillSelectCountDown;
            skillSelectCountDownTxt.text = displayCount.ToString() + "�� �ڿ� ���� ���������� ���۵˴ϴ�.";

            if(displayCount <= 10)
                skillSelectCountDownTxt.color = Color.red;

            if(skillSelectCountDown <= 0)
                break;

            yield return new WaitForFixedUpdate();
        }

        // ���� ������ �̵�
        SceneManager.LoadScene("BossScene" + (stage + 1));

        yield return null;
    }

    void PlayerDie()
    {
        dangerPanel.SetActive(false);
        deathPanel.SetActive(true);
        playerHealthGroup.SetActive(false);
        playerSkillIconGroup.SetActive(false);
    }

    void PlayerAlive()
    {
        deathPanel.SetActive(false);
        playerHealthGroup.SetActive(true);
        playerSkillIconGroup.SetActive(true);
    }

    public void GoToGreadyTown()
    {
        // ������ �̵�
        SceneManager.LoadScene("BossLobby");
    }

    public void GameOver()
    {
        gameEndPanel.SetActive(true);
        gameOverTxt.SetActive(true);
        Debug.Log("�� ����");
    }

    public void GameClear()
    {
        gameEndPanel.SetActive(true);
        gameClearTxt.SetActive(true);
    }
}
