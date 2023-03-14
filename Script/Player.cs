using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 인스펙터 창에서 설정 가능하기 위해 public 으로
    public float speed;

    // 플레이어의 무기 관련 배열 함수 2개 선언
    public GameObject[] weapons;
    public bool[] hasWeapons;

    // 탄약, 동전, 체력, 수류탄 변수 생성
    public int ammo;
    public int coin;
    public int health;

    // 수류탄
    public GameObject[] grenades;
    public int hasGrenades;

    // 수류탄 프리펩 저장 변수 추가.
    public GameObject grenadeObj;

    // 마우스로 방향 전환
    public Camera followCamera;

    // 각 수치의 최대 값을 저장할 변수도 생성
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    // 값을 받을 전역변수 선언
    float hAxis;
    float vAxis;

    // shift 키 입력을 받기 위해(걷기)
    bool wDown;

    // space 입력을 받기 위해 (점프)
    bool jDown;

    // 키입력, 공격딜레이, 공격 준비 변수 선언
    bool fDown;

    // 수류탄 투척
    bool gDown;

    // 재장전
    bool rDown;

    // 아이템 상호작용 (아이템 획득)
    bool iDown;

    // 번호 키로 무기 교체
    bool sDown1;
    bool sDown2;
    bool sDown3;   

    // 점프 1회만 가능하도록 바닥에 닿았는지 판단
    bool isJump;
    bool isDodge;

    // 무기 교체 중에는 다른 동작하지 않도록
    bool isSwap;

    // 장전 시
    bool isReload;

    // 딜레이가 끝나고 공격 준비 완료
    bool isFireReady = true;

    // 벽 충돌 플래그 bool 변수를 생성
    bool isBorder;

    // 무적 타임을 위한 플래그
    bool isDamage;

    Vector3 moveVec;

    // 회피 도중 방향 조작 못하도록
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    // 무적 시간을 플레이어에게 보여주기 위함
    // 플레이어는 각 부위별로 MeshRenderer를 가지고 있으므로 모두 가져옴
    MeshRenderer[] meshs;

    // 트리거 된 아잍템을 저장하기 위한 변수 선언
    GameObject nearObject;
    Weapon equipWeapon;

    int equipWeaponIndex = -1;

    // 공격 딜레이
    float fireDelay;

	private void Awake()
	{
        rigid = GetComponent<Rigidbody>();
        // 자식 오브젝트에 있는 컴포넌트를 가져옴
        anim = GetComponentInChildren<Animator>();

        // Component"s"
        meshs = GetComponentsInChildren<MeshRenderer>();
    }

	void Start()
    {
        
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Grenade();
        Attack();
        Reload();
        Dodge();
        Swap();
        Interaction();
    }

    void GetInput()
    {
        // GetAxisRaw() : Axis 값을 정수로 변환
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        // space 를 누르는 그 순간만 뛰도록 GetButtonDown 사용
        jDown = Input.GetButtonDown("Jump");
        // 기본적으로 마우스 왼쪽에 Fire1 이 들어가 있음
        fDown = Input.GetButton("Fire1");
        // 수류탄 투척
        gDown = Input.GetButtonDown("Fire2");
        // 재장전
        rDown = Input.GetButtonDown("Reload");
        // e 키를 누르면 아이템 상호작용
        iDown = Input.GetButtonDown("Interaction");
        // 무기 교체 키
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

	void Move()
	{
        // normalized
        // 대각선이라고 속도 빨라지지 않게.
        // 방향 값이 1로 보정된 벡터
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        // 회피를 하고 있다면
        if(isDodge)
            moveVec = dodgeVec;

        // 무기 교체하는 동안 움직이지 못하게
        // 공격하는 동안 움직이지 못하게
        if(isSwap || isReload || !isFireReady)
            moveVec = Vector3.zero;
        // 위에서 isBorder 를 추가하면 벽에 충돌하면 회전도 못하게됨

        // 벽을 뚫고 못지나가게
        if(!isBorder)
        {
            if(wDown)
                transform.position += moveVec * speed * 0.3f * Time.deltaTime;
            else
                transform.position += moveVec * speed * Time.deltaTime;
        }

        // 애니메이션
        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        // 플레이어 회전 (움직이는 방향으로 바라본다)
        transform.LookAt(transform.position + moveVec);

        if(fDown)
        {
            // 마우스에 의한 회전
            // 스크린에서 월드로 Ray 를 쏘는 함수 ScreenPointToRay();
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            // 무조건 닿도록 크게 줌 100
            // out return 처럼 반환 값을 주어진 변수에 저장하는 키워드
            if(Physics.Raycast(ray, out rayHit, 100))
            {
                // 닿은 지점 - 플레이어의 위치 = 상대 위치
                // 그 위치로 플레이어가 바라봄
                Vector3 nextVec = rayHit.point - transform.position;
                // RayCastHit 의 높이는 무시하도록 y 축 값을 0으로
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
    }

    void Jump()
    {
        //방향키를 누르면서 스페이스바는 점프

        if(jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            // Jump Poser = 15
            rigid.AddForce(Vector3.up * 40, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;

            // Jump Poser = 15
            rigid.AddForce(Vector3.up * 40, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = false;

            // Jump Poser = 15
            rigid.AddForce(Vector3.up * 40, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Grenade()
    {
        if(hasGrenades == 0)
            return;

        if(gDown && !isReload && !isSwap)
        {
            // 마우스 위치로 바로 던질 수 있도록 RayCast 를 사용
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if(Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 10;
                //transform.LookAt(transform.position + nextVec);

                GameObject instantGranade = Instantiate(grenadeObj, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGranade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrenades--;
                grenades[hasGrenades].SetActive(false);
            }
        }
    }

    void Attack()
    {
        // 손에 무기가 없으면 리턴
        if(equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        // 공격을 할 시간이 만족됨
        isFireReady = equipWeapon.rate < fireDelay;

        // 조건이 충족되면 무기 사용 실행
        if(fDown && isFireReady && !isDodge && !isSwap)
        {
            equipWeapon.Use();
            // 근거리, 원거리 에 따라 모션 변경
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            // 공격을 하면 다시 딜레이
            fireDelay = 0;
        }
    }

    void Reload()
    {
        if(equipWeapon == null)
            return;

        if(equipWeapon.type == Weapon.Type.Melee)
            return;

        if(ammo == 0)
            return;

        if(rDown && !isJump && !isDodge && !isSwap && isFireReady)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            // 장전하는데 3초 걸림
            Invoke("ReloadOut", 3f);
        }
    }

    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo; // 플레이어가 소지하고 있는 장전되지 않은 탄은 사라진다.
        isReload = false;
    }

    void Dodge()
    {
        if(jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            // 움직임 벡터 -> 회피방향 벡터로 바뀌도록 구현
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            // 시간차 함수 호출 0.5 초
            Invoke("DodgeOut", 0.5f);
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    void Swap()
    {
        if(sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if(sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if(sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;

        int weaponIndex = -1;

        if(sDown1) weaponIndex = 0;
        if(sDown2) weaponIndex = 1;
        if(sDown3) weaponIndex = 2;

        if(sDown1 || sDown2 || sDown3)
        {
            // 빈손일 경우를 생각하여 조건 추가
            if(equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();

            // 손에 든 무기를 보이도록 활성화
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");

            //무기 교체를 하는 중에는 다른 동작 하지 않도록
            isSwap = true;
            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }

    void Interaction()
    {
        if(iDown && nearObject != null)
        {
            if(nearObject.tag == "Weapon")
            {
                // Item 스크립트
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }

    // 바닥에 닿았을 때 점프 플래그 초기화
    void OnCollisionEnter(Collision collision)
	{
        if(collision.gameObject.tag == "Floor")
        {
            isJump = false;
            anim.SetBool("isJump", false);
        }
    }

    //자동 회전 방지
    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        // Raycast() : Ray 를 쏘아 닿는 오브젝트를 감지하는 함수
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }

	void FixedUpdate()
	{
        FreezeRotation();
        StopToWall();
    }



	void OnTriggerEnter(Collider other)
	{
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            // enum 타입
            switch(item.type)
            {
            case Item.Type.Ammo:
                ammo += item.value; // 총알 추가
                if(ammo > maxAmmo)
                    ammo = maxAmmo;
                break;
            case Item.Type.Coin:
                coin += item.value; // 코인 추가
                if(coin > maxCoin)
                    coin = maxCoin;
                break;
            case Item.Type.Heart:
                health += item.value; // 체력 회복
                if(health > maxHealth)
                    health = maxHealth;
                break;
            case Item.Type.Grenade:
                grenades[hasGrenades].SetActive(true);
                hasGrenades += item.value; // 수류탄 추가
                if(hasGrenades > maxHasGrenades)
                    hasGrenades = maxHasGrenades;
                break;
            }

            // 먹었으니 맵에서 삭제하기
            Destroy(other.gameObject);
        }
        else if(other.tag == "EnemyBullet")
        {
            // 무적 타임
            if(!isDamage)
            {
                // 플레이어 피격 구현
                // Bullet 스크립트 재활용하여 데미지 적용
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;

                // 플레이어가 미사일과 충돌
                // 미사일만이 가지고 있는 특징 -> RigidBody
                // 리지드 바디 유무 조건으로 하여 Destroy() 호출
                if(other.GetComponent<Rigidbody>() != null)
                {
                    Destroy(other.gameObject);
                }

                // 피격 리액션
                StartCoroutine(OnDamage());
            }
        }
	}

    // 리액션을 위한 코루틴 생성 및 호출
    IEnumerator OnDamage()
    {
        // 무적 시간을 위한 플래그
        isDamage = true;

        // 무적 여부를 플레이어에게 확인 시킴. 몸을 노랗게
        // 반복문을 사용하여 모든 재질의 색상을 변경
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }

        // 무적 시간
        yield return new WaitForSeconds(1f);

        isDamage = false;
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }
    }

	void OnTriggerStay(Collider other)
	{
        if(other.tag == "Weapon")
            nearObject = other.gameObject;
	}

	void OnTriggerExit(Collider other)
	{
        if(other.tag == "Weapon")
            nearObject = null;
    }
}
