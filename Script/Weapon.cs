using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // ���� Ÿ��, ������, ����, ����, ȿ�� ���� ����
    public enum Type
    {
        Melee, // ����
        Range // ���Ÿ�
    }
    public Type type;
    public int damage; // ���ݷ�
    public float rate; // ���� �ӵ�
    public int maxAmmo; // �ִ� ź
    public int curAmmo; // �����ִ� ź


    public BoxCollider meleeArea; // ���� ����
    public TrailRenderer trailEffect; // �ֵθ� �� ȿ��

    public Transform bulletPos; // �������� �����ؾ��� ��ġ
    public GameObject bullet; // �������� ������ ����

    public Transform bulletCasePos; // �������� �����ؾ��� ��ġ
    public GameObject bulletCase; // �������� ������ ����

    public void Use()
    {
        if(type == Type.Melee)
        {
            // �ڷ�ƾ �����Լ�
            // ���� �ڷ�ƾ�� ������ �� �������ΰ� �����ؼ� ������ ������ �ʰ� �� �� �ִ�.
            StopCoroutine("Swing");

            // �ڷ�ƾ �����Լ�
            StartCoroutine("Swing");
        }
        else if(type == Type.Range && curAmmo > 0)
        {
            curAmmo--;
            StartCoroutine("Shot");
        }
    }

    // ���ݱ��� �ؿ� ��� -> Invoke �� ����ع����� ��������.
    // Use() ���� ��ƾ -> Swing() ���� ��ƾ -> Use() ���� ��ƾ
    // �ڷ�ƾ �Լ� : ���� ��ƾ + �ڷ�ƾ (���� ����)
    // Use() ���� ��ƾ + Swing() �ڷ�ƾ (Co-Op) Co(�Բ�)

    IEnumerator Swing()
    {
        //1 ���⼭ ������ ����ǰ�

        // ����� �����ϴ� Ű����
        //yield return null; // 1������ ���

        //2 ���⸦ ����

        //yield return null; // 1������ ���

        //3 ���⸦ ����

        //yield return null; // 1������ ���

        //�߰��� �׸��ΰ� ���� ��
        //yield break;

        // �������� ����� �� �ִ�.
        // yield ��� Ű���带 ���� �� ����Ͽ� �ð��� ���� �ۼ� ����

        yield return new WaitForSeconds(0.1f); // 0.1 �� ���

        meleeArea.enabled = true; // collider Ȱ��ȭ
        trailEffect.enabled = true; // effect Ȱ��ȭ

        yield return new WaitForSeconds(1f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }

    IEnumerator Shot()
    {
        //1. �Ѿ� �߻�
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;

        yield return null;

        //2. ź�ǹ���
        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        // �ٱ���, �׸��� ���� �������� ź�� ����
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }
}
