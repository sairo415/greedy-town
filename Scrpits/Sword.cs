using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public int damage;

    // 칼 공격 인식 범위
    public BoxCollider meleeArea;

    // 회전격 이펙트 지정
    public TrailRenderer trailEffect;

    // 기본공격 이펙트
    public GameObject stoneEffect;

    // 궁극기 이펙트
    public GameObject meteor;

    public void BaseAttack()
    {
        StopCoroutine("Swing");
        StartCoroutine("Swing");
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);

        // 공격 범위 활성화
        meleeArea.enabled = true;
        stoneEffect.SetActive(true);


        yield return new WaitForSeconds(0.4f);
        stoneEffect.SetActive(false);


        // 칼을 휘두르는 동안은 활성화 하기 위해 텀을 주자
        yield return new WaitForSeconds(0.3f);
        // 다시 비활성화를 해서 공격이 안되게 막자
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
