using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;//몇번째인가
    public int prefabId;
    public float damage;
    public int count;//몇개를 배치할거냐 -> 근접 or 관통 수?
    public float speed;//회전속도 -> 근접 or  연사속도 -> 원거리

    float timer;
    Player player;

    void Awake()
    {
        player = GameManager.instance.player.GetComponent<Player>();
    }

    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.down * speed * Time.deltaTime);
                break;
            case 1:
                timer += Time.deltaTime;
                if(timer > speed)
                {
                    timer = 0f;
                    Fire();
                }
                break;
            default:
                break;
        }

        //level up test
        if (Input.GetButtonDown("Jump")){
            LevelUp(10, 1);
        }
    }

    public void LevelUp(float damage, int count)
    {
        this.damage = damage;
        this.count += count;

        if (id == 0)
            Batch();
    }

    public void Init()
    {
        switch (id)
        {
            case 0:
                speed = 150;
                Batch();
                break;
            case 1:
                speed = 0.3f;//연사속도
                break;
            default:
                break;
        }
    }

    void Batch()
    {
        for(int i=0; i<count; i++)
        {
            Transform hammer; 
                
            if(i < transform.childCount)
            {
                hammer = transform.GetChild(i);
            }
            else
            {
                hammer = GameManager.instance.pool.Get(prefabId, false).transform;
                hammer.parent = transform;//부모가 자기 자신
            }
            

            hammer.localPosition = Vector3.zero;
            hammer.localRotation = Quaternion.identity;


            Vector3 rotVec = Vector3.up * 360 * i / count;// + Vector3.right * 90;
            hammer.Rotate(rotVec);
            hammer.Translate(hammer.forward * 1.3f, Space.World);
            hammer.GetComponent<Hammer>().Init(damage, -1);//근접무기 -> 무한관통
        }
    }

    void Fire()
    {
        if (!player.scanner.nearestTarget)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = (targetPos - transform.position).normalized;

        Transform shoot = GameManager.instance.pool.Get(prefabId, false).transform;
        shoot.position = transform.position;
        shoot.rotation = Quaternion.FromToRotation(Vector3.right, dir);
        shoot.GetComponent<Hammer>().Init(damage, count, dir);
    }
}
