using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // 무기 타입, 데미지, 공속, 범위, 효과 변수 생성
    public enum Type
    {
        Melee, // 근접
        Range // 원거리
    }
    public Type type;
    public int damage; // 공격력
    public float rate; // 공격 속도
    public int maxAmmo; // 최대 탄
    public int curAmmo; // 남아있는 탄


    public BoxCollider meleeArea; // 공격 범위
    public TrailRenderer trailEffect; // 휘두를 때 효과

    public Transform bulletPos; // 프리펩을 생성해야할 위치
    public GameObject bullet; // 프리펩을 저장할 변수

    public Transform bulletCasePos; // 프리펩을 생성해야할 위치
    public GameObject bulletCase; // 프리펩을 저장할 변수

    public void Use()
    {
        if(type == Type.Melee)
        {
            // 코루틴 정지함수
            // 같은 코루틴을 실행할 때 진행중인걸 정지해서 로직이 꼬이지 않게 할 수 있다.
            StopCoroutine("Swing");

            // 코루틴 실행함수
            StartCoroutine("Swing");
        }
        else if(type == Type.Range && curAmmo > 0)
        {
            curAmmo--;
            StartCoroutine("Shot");
        }
    }

    // 지금까지 해온 방식 -> Invoke 를 사용해버리면 복잡해짐.
    // Use() 메인 루틴 -> Swing() 서버 루틴 -> Use() 메인 루틴
    // 코루틴 함수 : 메인 루틴 + 코루틴 (동시 실행)
    // Use() 메인 루틴 + Swing() 코루틴 (Co-Op) Co(함께)

    IEnumerator Swing()
    {
        //1 여기서 로직이 실행되고

        // 결과를 전달하는 키워드
        //yield return null; // 1프레임 대기

        //2 여기를 실행

        //yield return null; // 1프레임 대기

        //3 여기를 실행

        //yield return null; // 1프레임 대기

        //중간에 그만두고 싶을 때
        //yield break;

        // 여러개를 사용할 수 있다.
        // yield 라는 키워드를 여러 개 사용하여 시간차 로직 작성 가능

        yield return new WaitForSeconds(0.1f); // 0.1 초 대기

        meleeArea.enabled = true; // collider 활성화
        trailEffect.enabled = true; // effect 활성화

        yield return new WaitForSeconds(1f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }

    IEnumerator Shot()
    {
        //1. 총알 발사
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;

        yield return null;

        //2. 탄피배출
        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        // 바깥쪽, 그리고 조금 위쪽으로 탄피 배출
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }
}
