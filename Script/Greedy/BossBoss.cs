using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering;

public class BossBoss : MonoBehaviour
{
    // 체력
    public int maxHealth;
    public int curHealth;

    // Photon
    private PhotonView pv;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
    }

	private void Start()
	{
        pv = GetComponent<PhotonView>();

        // 씬 이동 시 체력 초기화
        curHealth = maxHealth;
    }

	void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerAttack" || other.tag == "PlayerAttackOver")
        {
            curHealth -= other.GetComponent<BossPlayerSkill>().damage;
            if(curHealth < 0) curHealth = 0;

            // 서버 보스 체력과 동기화
            pv.RPC("SyncBossHealth", RpcTarget.All, curHealth);

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
    }

    // 보스 체력을 서버의 보스 체력과 동기화
    [PunRPC]
    void SyncBossHealth(int health)
    {
        curHealth = health;
    }

    IEnumerator OnDamage()
    {
        Color originMat = mat.color;
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0)
        {
            mat.color = originMat;
        }
        else
        {
            //5초 후 다음 씬으로 이동
            yield return new WaitForSeconds(5.0f);

            int sceneNum = SceneManager.GetActiveScene().buildIndex + 1;

            //sceneNum = 5 : 마지막 스테이지 인덱스
            if(sceneNum != 5)
            {
                int nextSceneNum = sceneNum + 1;
                string nextSceneName = "BossScene" + nextSceneNum.ToString();

                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                //game End
                Debug.Log("End");
            }
        }
    }
}
