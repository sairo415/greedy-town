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

        if(other.tag == "PlayerAttack" || other.tag == "PlayerAttackOver")
        {
            // ���� ��ų�� ������
            int skillOwnerID = other.GetComponent<BossPlayerSkill>().GetID();
            // ���� ��ġ�� Ŭ���̾�Ʈ Owner ID
            int myPlayerID = GameObject.FindObjectOfType<BossGameManager>().player.pv.ViewID;

            // ���� ����� ��ų�� �ƴ� ��� ���� ���� ����.
            if(skillOwnerID != myPlayerID)
                return;

            curHealth -= other.GetComponent<BossPlayerSkill>().damage;
            if(curHealth < 0) curHealth = 0;

            // �����ڰ� ������ ������ ������ ü���� ȸ����Ų��.
            if(GameObject.FindObjectOfType<BossGameManager>().player.isVampirism)
            {
                int vamHP = GameObject.FindObjectOfType<BossGameManager>().player.curHealth + 10;
                if(vamHP > GameObject.FindObjectOfType<BossGameManager>().player.maxHealth)
                    vamHP = GameObject.FindObjectOfType<BossGameManager>().player.maxHealth;
                GameObject.FindObjectOfType<BossGameManager>().player.curHealth = vamHP;

                // ȸ����Ų �� ����ȭ �ʿ��� ��...
                // isVampirism == true �� ��, q �� ����� ������, ���⼭ SyncBossHealth �� �� ó��
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
