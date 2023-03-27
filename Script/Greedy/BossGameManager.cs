using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossGameManager : MonoBehaviour
{
    // stage
    public int stage;
    public Text stageTxt;

    // UI Panel
    public GameObject ingamePanel; // 인게임 화면
    public GameObject selectSkillPanel; // 스킬 선택 화면

    // 보스
    public BossBoss boss;

    public RectTransform bossHealthGroup;
    public RectTransform bossTempHealthBar;
    public RectTransform bossHealthBar;

    int pastBossHealth; // 이전 보스 체력
    

    // 플레이어
    public BossPlayer player;

    public RectTransform playerHealthGroup;
    public RectTransform playerTempHealthBar;
    public RectTransform playerHealthBar;

    int pastPlayerHealth; // 이전 플레이어 체력

    // 스킬 아이콘
    public Image imgSkill_1;
    public Image imgSkill_2;
    public Image imgSkill_3;
    public Image imgSkill_4;
    public Image imgSkill_Dash;

    // 스킬 쿨타임
    float qCool;
    float wCool;
    float eCool;
    float rCool;

    // 스킬 사용 후 경과 시간
    float qDelta;
    float wDelta;
    float eDelta;
    float rDelta;

    // 스킬 재사용 가능 여부
    bool isQReady;
    bool isWReady;
    bool isEReady;
    bool isRReady;

    void Awake()
	{
        pastBossHealth = boss.curHealth;

        isQReady = true;
        isWReady = true;
        isEReady = true;
        isRReady = true;
    }

    void Start()
    {
        //nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        //DontDestroyOnLoad(this.gameObject);
        //SceneManager.sceneLoaded += OnSceneLoaded;


    }

	private void Update()
	{
        //float sCool = GetComponent<BossPlayer>().normalAtkRate;
        qCool = player.GetComponent<BossPlayer>().normalAtkRate;
        wCool = player.GetComponent<BossPlayer>().swordForceRate;
        eCool = player.GetComponent<BossPlayer>().swordDanceRate;
        rCool = player.GetComponent<BossPlayer>().swordFlashRate;

        if(rCool == 0)
            return;

        if(Input.GetKeyDown(KeyCode.Q) && isQReady)
        {
            isQReady = false;
            imgSkill_1.fillAmount = 0.0f;
            StartCoroutine(CoolTimeQ(qCool));
        }
        else if(Input.GetKeyDown(KeyCode.W) && isWReady)
        {
            isWReady = false;
            imgSkill_2.fillAmount = 0.0f;
            StartCoroutine(CoolTimeW(wCool));
        }
        else if(Input.GetKeyDown(KeyCode.E) && isEReady)
        {
            isEReady = false;
            imgSkill_3.fillAmount = 0.0f;
            StartCoroutine(CoolTimeE(eCool));
        }
        else if(Input.GetKeyDown(KeyCode.R) && isQReady)
        {
            isRReady = false;
            imgSkill_4.fillAmount = 0.0f;
            StartCoroutine(CoolTimeR(rCool));
        }
        /*else if(Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(CoolTimeS(sCool));
        }*/
    }

    IEnumerator CoolTimeQ(float cool)
    {
        while(true)
        {
            qDelta += Time.deltaTime;
            imgSkill_1.fillAmount = (qDelta / cool);
            
            yield return new WaitForFixedUpdate();

            if(qDelta >= cool)
                break;
        }

        qDelta = 0;
        isQReady = true;

        yield return null;
    }

    IEnumerator CoolTimeW(float cool)
    {

        Debug.Log(wDelta);
        Debug.Log(wCool);

        while(true)
        {
            wDelta += Time.deltaTime;
            imgSkill_2.fillAmount = (wDelta / cool);

            yield return new WaitForFixedUpdate();

            if(wDelta >= cool)
                break;
        }
        Debug.Log(wDelta);

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

            yield return new WaitForFixedUpdate();

            if(eDelta >= cool)
                break;
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
            imgSkill_4.fillAmount = (rDelta / cool);

            yield return new WaitForFixedUpdate();

            if(rDelta >= cool)
                break;
        }

        rDelta = 0;
        isRReady = true;

        yield return null;
    }

    /*IEnumerator CoolTimeS(float cool)
    {
        while(cool > 0.01f)
        {
            cool -= Time.deltaTime;
            imgSkill_Dash.fillAmount = (1.0f / cool);
            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }*/

    /*void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex == nextSceneIndex)
        {
            bossHealthBar.localScale = new Vector3(700, 1, 1);
            bossTempHealthBar.localScale = new Vector3(700, 1, 1);

            nextSceneIndex++;
        }
    }*/

    void LateUpdate()
	{
        if(pastBossHealth != boss.curHealth)
        {
            StartCoroutine("BossHPGauge");
            pastBossHealth = boss.curHealth;
        }
	}

    IEnumerator BossHPGauge()
    {
        float reteBossHP = (float)boss.curHealth / boss.maxHealth;

        bossHealthBar.localScale = new Vector3(reteBossHP, 1, 1);

        yield return new WaitForSeconds(1.0f);

        bossTempHealthBar.localScale = new Vector3(reteBossHP, 1, 1);

        yield return null;
    }

}
