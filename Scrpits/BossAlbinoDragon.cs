using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossAlbinoDragon : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public bool isDead;
    public bool isAttack;
    public bool isLook;


    private enum BossState { Idle, Attack1, Attack2, Run, Dead };
    private BossState currentState;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public Transform target;
    public NavMeshAgent nav;
    public Animator anim;

    Vector3 lookVector;
}