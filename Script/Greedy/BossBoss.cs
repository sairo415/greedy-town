using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossBoss : MonoBehaviour
{
    // ü�°� ������Ʈ�� ���� ���� ����
    public int maxHealth;
    public int curHealth;

    Rigidbody rigid;
    BoxCollider boxCollider;

    Material mat;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
    }

    // �÷��̾ �ֵθ��� ��ġ Ȥ�� ���ƿ��� �Ѿ�
    // Ʈ���ŷ� ó��
    // OnTriggerEnter() �Լ��� �±� �� ������ �ۼ�
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerAttack" || other.tag == "PlayerAttackOver")
        {
            curHealth -= other.GetComponent<BossPlayerSkill>().damage;
            if(curHealth < 0)
                curHealth = 0;

            // ���� ����� �� �����ǵ��� Destroy() ȣ��
            if(other.tag == "PlayerAttack")
            {
                Destroy(other.gameObject);
                other.gameObject.SetActive(false);
            }

            StartCoroutine("OnDamage");
        }

        /*if(other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            // ���� ��ġ�� �ǰ� ��ġ�� ���� ���ۿ� ���ϱ�
            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVec));

            //Debug.Log("Melee : " + curHealth);
        }
        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            // ���� ��ġ�� �ǰ� ��ġ�� ���� ���ۿ� ���ϱ�
            Vector3 reactVec = transform.position - other.transform.position;

            // �Ѿ��� ���, ���� ����� �� �����ǵ��� Destroy() ȣ��
            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec));
            //Debug.Log("Range : " + curHealth);
        }*/
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
            yield return new WaitForSeconds(10.0f);

            int sceneNum = SceneManager.GetActiveScene().buildIndex + 1;

            if(sceneNum < 4)
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
