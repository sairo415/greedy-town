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
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
    }

	private void Start()
	{
        pv = GetComponent<PhotonView>();

        // �� �̵� �� ü�� �ʱ�ȭ
        curHealth = maxHealth;
    }

	void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerAttack" || other.tag == "PlayerAttackOver")
        {
            curHealth -= other.GetComponent<BossPlayerSkill>().damage;
            if(curHealth < 0) curHealth = 0;

            // ���� ���� ü�°� ����ȭ
            pv.RPC("SyncBossHealth", RpcTarget.All, curHealth);

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

    // ���� ü���� ������ ���� ü�°� ����ȭ
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
            //5�� �� ���� ������ �̵�
            yield return new WaitForSeconds(5.0f);

            int sceneNum = SceneManager.GetActiveScene().buildIndex + 1;

            //sceneNum = 5 : ������ �������� �ε���
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
