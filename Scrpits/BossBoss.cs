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
        pv = GetComponent<PhotonView>();
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
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
                curHealth -= other.GetComponent<BossPlayerSkill>().damage;
                if(curHealth < 0)
                    curHealth = 0;

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

                int sendRPCBossHP = curHealth;

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
                curHealth -= other.GetComponent<BossPlayerSkill>().damage;
                if(curHealth < 0)
                    curHealth = 0;

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

                int sendRPCBossHP = curHealth;

                // 서버 보스 체력과 동기화
                pv.RPC("SyncBossHealth", RpcTarget.Others, sendRPCBossHP);

                StartCoroutine("OnDamage");

                other.GetComponent<BossPlayerSkill>().damageTimer = 0f;
            }
        }
    }

    // 보스 체력을 다른 클라이언트의 보스 체력과 동기화
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
            // 사망
        }
    }
}
