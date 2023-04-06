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
    public GameObject ingamePanel; // 인게임 화면
    public GameObject selectSkillPanel; // 스킬 선택 화면
    public GameObject dangerPanel; // 위험 화면
    public GameObject deathPanel;   // 사망 화면
    public GameObject gameEndPanel; // 게임 종료 판넬
    public GameObject playerCntPanel;   // 0스테이지 포탈앞 인원수 안내 판넬

    // 게임 종료 메시지
    public GameObject gameOverTxt;
    public GameObject gameClearTxt;

    // 보스
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

    int pastBossHealth; // 이전 보스 체력
    bool isBigBossSpawn;

    // 플레이어
    public BossPlayer player;

    public GameObject playerHealthGroup;
    public RectTransform playerHealthBar;

    public GameObject playerSkillIconGroup;

    int pastBossPlayerHealth; // 이전 플레이어 체력

    // 선택된 스킬 아이콘
    Image imgSkill_1;
    Image imgSkill_2;
    Image imgSkill_3;
    Image imgSkill_4;
    public Image imgSkill_Dash;

    // Lv1 스킬 그룹, 아이콘
    public GameObject objStoneSwing;
    public GameObject objSwordSwing;
    public GameObject objBloodSwing;
    public GameObject objPaladinSwing;

    public Image imgStoneSwing;
    public Image imgSwordSwing;
    public Image imgBloodSwing;
    public Image imgPaladinSwing;

    // Lv2 스킬 그룹, 아이콘
    public GameObject objSwordForce;
    public GameObject objVampirism;
    public GameObject objIntent;

    public Image imgSwordForce;
    public Image imgVampirism;
    public Image imgIntent;

    // Lv3 스킬 그룹, 아이콘
    public GameObject objSwordDance;
    public GameObject objBloodExplosion;
    public GameObject objBlessing;

    public Image imgSwordDance;
    public Image imgBloodExplosion;
    public Image imgBlessing;

    // Lv4 스킬 그룹, 아이콘
    public GameObject objSwordBlade;
    public GameObject objBloodField;
    public GameObject objResurrection;

    public Image imgSwordBlade;
    public Image imgBloodField;
    public Image imgResurrection;

    // 스킬 쿨타임
    float qCool = -1;
    float wCool = -1;
    float eCool = -1;
    float rCool = -1;
    float sCool = -1;

    // 스킬 사용 후 경과 시간
    float qDelta;
    float wDelta;
    float eDelta;
    float rDelta;
    float sDelta;

    // 스킬 재사용 가능 여부
    bool isQReady;
    bool isWReady;
    bool isEReady;
    bool isRReady;
    bool isSReady;

    // 씬 이동 관련
    int nextSceneIndex;

    // 스킬 선택 카운트 다운
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

    // 씬 흐름
    // 입장 -> 중간 보스 등장 (isSmallBossDie = false)
    // -> 중간 보스 사망 (isSmallBossDie = true)
    // -> 찐 보스 등장 (isMainBossDie = false)
    // -> 찐 보스 사망 (isMainBossDie = true)
    // -> 스킬 선택 판넬 활성화 (제한 시간 20 초)
    // 시간 경과 후 씬 이동
    bool isSmallBossDie;
    bool isMainBossDie;

    // Update 에서 다시 검사할 필요가 없을 경우 true
    bool isQCheck;
    bool isWCheck;
    bool isECheck;
    bool isRCheck;

    // 이미지 목록
    public Sprite spriteSwordForce;
    public Sprite spriteVampirism;
    public Sprite spriteIntent;
    public Sprite spriteSwordDance;
    public Sprite spriteBloodExplosion;
    public Sprite spriteBlessing;
    public Sprite spriteSwordBlade;
    public Sprite spriteBloodField;
    public Sprite spriteResurrection;

    // 랜덤으로 출력된 스킬 선택창 스킬 목록
    int[] randomSkillIndex;

    // 스킬 선택 완료
    public Image imgSelectLeftButton;
    public Image imgSelectCenterButton;
    public Image imgSelectRightButton;

    bool isPlayerHPDanger;
    bool isPlayerDie;

    bool isGameEnd;

    // 보스 사망 시 플레이어 사망해도, 게임 오버 출력 안하게 함.
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
            // 부활 스킬이 아닐 경우만 쿨타임 로딩

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
        // 중간 보스 클리어 확인
        if(stage != 0 && smallBoss.transform.childCount == 0 && !isBigBossSpawn)
        {
            isBigBossSpawn = true;

            // 능력치 고르는 화면
            // 시간 제한 10초
            // 10초가 지나면 보스 소환
            AbilitySelect();

            // 10초가 지났음을 확인
            // 능력치 고르는 창 비활성화

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

        // 보스 체력 UI 갱신
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

        // 보스 사망
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

        // 플레이어 체력 UI 갱신
        if(player != null && pastBossPlayerHealth != player.curHealth)
        {
            StartCoroutine("PlayerHPGauge");
            pastBossPlayerHealth = player.curHealth;
        }

        // 체력이 10퍼 이하일 경우 경고 on
        if (player != null && ((float)player.curHealth <= (float)player.maxHealth * 0.1) && !isPlayerHPDanger && player.curHealth > 0)
        {
            isPlayerHPDanger = true;
            StartCoroutine("displayDangerPanel");
        }

        // 체력이 없으면 사망처리
        if (player != null && player.curHealth <= 0)
        {
            PlayerDie();
            isPlayerDie = true;
        }

        // 부활 시 다시 띄움
        if (player != null && isPlayerDie && player.curHealth > 0)
        {
            PlayerAlive();
            isPlayerDie = false;
        }

        // 전원 사망 시 나가기 버튼 출력할 것
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
            Debug.Log("전원 사망");
            GameOver();
        }

        if(bossPlayers.Length > 0 && stage == 4 && bossRed.curHealth <= 0 && !isGameEnd)
        {
            isGameEnd = true;
            Debug.Log("게임 클리어");
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
        // 능력치 추가 나중에 구현
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

        string swordForceScript = "검기[액티브]\n전방을 향해 검기를 날립니다.";
        string vampirismScript = "피의 갈증[패시브]\n 기본 공격이 [흡혈] 속성을 가집니다. [흡혈]공격이 적에게 적중하면 체력이 회복됩니다.";
        string intentScript = "의지[패시브]\n방어력이 상승합니다.";

        string swordDanceScript = "검무[액티브]\n전방에 지속적으로 데미지를 주는 영역을 생성합니다.";
        string bloodExplosionScript = "혈폭[액티브]\n체력을 소모하여 캐릭터 주변으로 강한 폭발을 일으킵니다.";
        string blessingScript = "축복[액티브]\n파티원의 체력을 회복시키는 영역을 생성합니다.";

        string swordBladeScript = "폭풍[액티브]\n캐릭터 주변의 넓은 영역을 공격합니다. 잠시동안 무적 상태가 됩니다.";
        string bloodFieldScript = "블러드 필드[액티브]\n[흡혈] 공격. 캐릭터 주변의 넓은 영역을 지속적으로 공격합니다.";
        string resurrectionScript = "부활[액티브]\n단 한번 모든 파티원들을 부활 시킵니다. 본인 사망시 사용 불가합니다.";

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

    // 모든 캐릭터의 스킬 선택을 동기화
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
            skillSelectCountDownTxt.text = displayCount.ToString() + "초 뒤에 다음 스테이지가 시작됩니다.";

            if(displayCount <= 10)
                skillSelectCountDownTxt.color = Color.red;

            if(skillSelectCountDown <= 0)
                break;

            yield return new WaitForFixedUpdate();
        }

        // 다음 씬으로 이동
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
        // 마을로 이동
        SceneManager.LoadScene("BossLobby");
    }

    public void GameOver()
    {
        gameEndPanel.SetActive(true);
        gameOverTxt.SetActive(true);
        Debug.Log("다 죽음");
    }

    public void GameClear()
    {
        gameEndPanel.SetActive(true);
        gameClearTxt.SetActive(true);
    }
}
