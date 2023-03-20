using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossBlackDragon : MonoBehaviour
{
    // ������ ����
    public int maxHealth;
    public int currentHealth;
    public bool isDead;
    public bool isFlying = false;

    public GameObject flameStrikePrefab;

    // ������ ���� �ൿ
    private BossState currentState;

    // �÷��̾� Ÿ����
    public Transform target;

    // �̰� ���� ���� ����
    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public NavMeshAgent nav;
    public Animator anim;

    // �Ҳ� �������� ������
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

    // ���� �ൿ ������ ���� ���µ�
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
        // ���� ���� �ʱ� ���� ����
        currentState = BossState.Attack1;
    }

    private void LateUpdate()
    {
        print(currentState);
        print(isAttack);
        // ���� ���� ���� ���¿� ���� �ൿ ó��
        switch (currentState)
        {
            case BossState.Idle:
                // Idle ���¿����� �ൿ ó��
                ChangeState();
                break;
            case BossState.Attack1:
                // Attack1 ���¿����� �ൿ ó��
                if (!isAttack)
                {
                    isAttack = true;
                    AttackFlameStrike();
                }
                break;
            case BossState.Attack2:
                // Attack2 ���¿����� �ൿ ó��
                break;
            case BossState.Attack3:
                // Attack3 ���¿����� �ൿ ó��
                break;
            case BossState.Dead:
                // Dead ���¿����� �ൿ ó��
                break;
        }
    }

    // ���� ������ ���¸� �����ϴ� �Լ�
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
