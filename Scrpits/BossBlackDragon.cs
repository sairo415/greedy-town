using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossBlackDragon : MonoBehaviour
{
    // 보스의 상태
    public int maxHealth;
    public int currentHealth;
    public bool isDead;
    public bool isFlying = false;

    public GameObject flameStrikePrefab;

    // 보스의 현재 행동
    private BossState currentState;

    // 플레이어 타게팅
    public Transform target;

    // 이것 저것 시작 설정
    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public NavMeshAgent nav;
    public Animator anim;

    // 불꽃 떨어지는 지점들
    public GameObject[] fallingSpots;
    public GameObject[] flameStrikes;

    public bool isAttack;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();

        nav.isStopped = true;

    }

    // 보스 행동 패턴을 위한 상태들
   private enum BossState
    {
        Idle,
        Attack1,
        Attack2,
        Attack3,
        Dead
    }


    private void Start()
    {
        // 보스 몬스터 초기 상태 설정
        currentState = BossState.Attack1;
    }

    private void LateUpdate()
    {
        print(currentState);
        print(isAttack);
        // 현재 보스 몬스터 상태에 따른 행동 처리
        switch (currentState)
        {
            case BossState.Idle:
                // Idle 상태에서의 행동 처리
                ChangeState();
                break;
            case BossState.Attack1:
                // Attack1 상태에서의 행동 처리
                if (!isAttack)
                {
                    isAttack = true;
                    AttackFlameStrike();
                }
                break;
            case BossState.Attack2:
                // Attack2 상태에서의 행동 처리
                break;
            case BossState.Attack3:
                // Attack3 상태에서의 행동 처리
                break;
            case BossState.Dead:
                // Dead 상태에서의 행동 처리
                break;
        }
    }

    // 보스 몬스터의 상태를 변경하는 함수
    private void ChangeState()
    {
        if (currentHealth <= 0)
        {
            currentState = BossState.Dead;
        }
        else if (isAttack)
        {
            return;
        }
        else
        {
            switch (currentState)
            {
                case BossState.Idle:
                    currentState = BossState.Attack1;
                    break;

            }
        }
    }

    void AttackFlameStrike()
    {
        List<int> ranNums = new List<int>();

        while (ranNums.Count < 5)
        {
            int num = Random.Range(0, 9);
            if (!ranNums.Contains(num))
            {
                ranNums.Add(num);
            }
        }

        foreach (int num in ranNums)
        {
            StartCoroutine(Strike(num));
        }

        StartCoroutine(EndAttack());
    }

    IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(4f);
        isAttack = false;
        currentState = BossState.Idle;
    }

    IEnumerator Strike(int idx)
    {
        yield return new WaitForSeconds(0.1f);
        fallingSpots[idx].SetActive(true);

        yield return new WaitForSeconds(1f);
        fallingSpots[idx].SetActive(false);
        Vector3 fitVector = new Vector3(0f, 10f, 16f);
        GameObject flameStrike = Instantiate(flameStrikePrefab, fallingSpots[idx].transform.position + fitVector, fallingSpots[idx].transform.rotation);
        flameStrike.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        Destroy(flameStrike);
    }
}
