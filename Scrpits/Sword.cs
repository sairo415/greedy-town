using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public int damage;

    // 칼 공격 인식 범위
    public BoxCollider meleeArea;

    public void Use()
    {
        StopCoroutine("Swing");
        StartCoroutine("Swing");
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);

        // 공격 범위 활성화
        meleeArea.enabled = true;

        // 칼을 휘두르는 동안은 활성화 하기 위해 텀을 주자
        yield return new WaitForSeconds(0.7f);
        // 다시 비활성화를 해서 공격이 안되게 막자
        meleeArea.enabled = false;

        yield break;
    }
}
