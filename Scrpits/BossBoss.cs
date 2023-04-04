using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering;

public class BossBoss : MonoBehaviour
{
    // ü��
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
        //���� ��ų�� ������ �ľ�.
        //�ش� �����ְ� �� ������ ��ġ�� ���� �÷��̾���̵�� ������ Ʈ���� ����
        //���� �Ϸ��� RPC Other

        if(other != null && (other.tag == "PlayerAttack" || other.tag == "PlayerAttackOver" || other.tag == "PlayerAttackDot"))
        {
            // ���� ��ų�� ������
            int skillOwnerID = other.GetComponent<BossPlayerSkill>().GetID();
            // ���� ��ġ�� Ŭ���̾�Ʈ
            BossPlayer curClient = GameObject.FindObjectOfType<BossGameManager>().player;

            // Owner ID
            int myPlayerID = curClient.pv.ViewID;

            // ���� ����� ��ų�� �ƴ� ��� ���� ���� ����.
            if(skillOwnerID != myPlayerID)
                return;

            if(other.tag == "PlayerAttack" || other.tag == "PlayerAttackOver")
            {
                curHealth -= other.GetComponent<BossPlayerSkill>().damage;
                if(curHealth < 0)
                    curHealth = 0;

                // �����ڰ� ������ ������ ������ ü���� ȸ����Ų��.
                if(curClient.isVampirism && other.GetComponent<BossPlayerSkill>().isVampirism)
                {
                    int vamHP = curClient.curHealth + 10;
                    if(vamHP > curClient.maxHealth)
                        vamHP = curClient.maxHealth;
                    curClient.curHealth = vamHP;

                    // ȸ����Ų �� ����ȭ �ʿ��� ��...
                    // isVampirism == true �� ��, q �� ����� ������, ���⼭ SyncBossHealth �� �� ó��
                    curClient.CallSyncPlayerHPAll(curClient.curHealth);
                }

                int sendRPCBossHP = curHealth;

                // ���� ���� ü�°� ����ȭ
                pv.RPC("SyncBossHealth", RpcTarget.Others, sendRPCBossHP);

                // ���� ����� �� ����Ʈ �����ǵ��� Destroy() ȣ��
                // tag PlayerAttack => ������ �����Ǵ� ����Ʈ
                // tag PlayerAttackOver => ������ �������� �ʴ� ����Ʈ
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

                // �����ڰ� ������ ������ ������ ü���� ȸ����Ų��.
                if(curClient.isVampirism && other.GetComponent<BossPlayerSkill>().isVampirism)
                {
                    int vamHP = curClient.curHealth + 10;
                    if(vamHP > curClient.maxHealth)
                        vamHP = curClient.maxHealth;
                    curClient.curHealth = vamHP;

                    // ȸ����Ų �� ����ȭ �ʿ��� ��...
                    // isVampirism == true �� ��, q �� ����� ������, ���⼭ SyncBossHealth �� �� ó��
                    curClient.CallSyncPlayerHPAll(curClient.curHealth);
                }

                int sendRPCBossHP = curHealth;

                // ���� ���� ü�°� ����ȭ
                pv.RPC("SyncBossHealth", RpcTarget.Others, sendRPCBossHP);

                StartCoroutine("OnDamage");

                other.GetComponent<BossPlayerSkill>().damageTimer = 0f;
            }
        }
    }

    // ���� ü���� �ٸ� Ŭ���̾�Ʈ�� ���� ü�°� ����ȭ
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
            // ���
        }
    }
}
