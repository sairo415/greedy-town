using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public int damage;

    // Į ���� �ν� ����
    public BoxCollider meleeArea;

    // ȸ���� ����Ʈ ����
    public TrailRenderer trailEffect;

    // �⺻���� ����Ʈ
    public GameObject stoneEffect;

    // �ñر� ����Ʈ
    public GameObject meteor;

    public void BaseAttack()
    {
        StopCoroutine("Swing");
        StartCoroutine("Swing");
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);

        // ���� ���� Ȱ��ȭ
        meleeArea.enabled = true;
        stoneEffect.SetActive(true);


        yield return new WaitForSeconds(0.4f);
        stoneEffect.SetActive(false);


        // Į�� �ֵθ��� ������ Ȱ��ȭ �ϱ� ���� ���� ����
        yield return new WaitForSeconds(0.3f);
        // �ٽ� ��Ȱ��ȭ�� �ؼ� ������ �ȵǰ� ����
        meleeArea.enabled = false;

        yield break;
    }

    public void SpinAttack()
    {
        StopCoroutine("Spin");
        StartCoroutine("Spin");
    }

    IEnumerator Spin()
    {
        yield return new WaitForSeconds(0.1f);

        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.1f);
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.7f);

        meleeArea.enabled = false;
        trailEffect.enabled = false;

        yield break;
    }

    public void Ultimate()
    {
        StopCoroutine("Summon");
        StartCoroutine("Summon");
    }

    IEnumerator Summon()
    {
        yield return new WaitForSeconds(0.1f);

        meteor.SetActive(true);

        yield return new WaitForSeconds(1.7f);
        meteor.SetActive(false);

        yield break;
    }
}
