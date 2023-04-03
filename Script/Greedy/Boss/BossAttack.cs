using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    // 데미지에 랜덤 값 적용 범위
    public int minDamage;
    public int maxDamage;

    // 스킬 데미지
    public int damage;

    public float damageTimer = 0.0f;    // 도트 데미지를 위한 시간 측정
    public float damageInterval;        // 도트 데미지 주기

    public bool isInBoss;				// 플레이어가 해당 영역 안으로 들어옴

    private void Awake()
    {
        damage = Random.Range(minDamage, maxDamage);
    }
}
