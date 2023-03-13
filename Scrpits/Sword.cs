using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public int damage;

    // Į ���� �ν� ����
    public BoxCollider meleeArea;

    public void Use()
    {
        StopCoroutine("Swing");
        StartCoroutine("Swing");
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);

        // ���� ���� Ȱ��ȭ
        meleeArea.enabled = true;

        // Į�� �ֵθ��� ������ Ȱ��ȭ �ϱ� ���� ���� ����
        yield return new WaitForSeconds(0.7f);
        // �ٽ� ��Ȱ��ȭ�� �ؼ� ������ �ȵǰ� ����
        meleeArea.enabled = false;

        yield break;
    }
}
