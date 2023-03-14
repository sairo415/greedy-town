using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // �ν����� â���� ���� �����ϱ� ���� public ����
    public float speed;

    // �÷��̾��� ���� ���� �迭 �Լ� 2�� ����
    public GameObject[] weapons;
    public bool[] hasWeapons;

    // ź��, ����, ü��, ����ź ���� ����
    public int ammo;
    public int coin;
    public int health;

    // ����ź
    public GameObject[] grenades;
    public int hasGrenades;

    // ����ź ������ ���� ���� �߰�.
    public GameObject grenadeObj;

    // ���콺�� ���� ��ȯ
    public Camera followCamera;

    // �� ��ġ�� �ִ� ���� ������ ������ ����
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    // ���� ���� �������� ����
    float hAxis;
    float vAxis;

    // shift Ű �Է��� �ޱ� ����(�ȱ�)
    bool wDown;

    // space �Է��� �ޱ� ���� (����)
    bool jDown;

    // Ű�Է�, ���ݵ�����, ���� �غ� ���� ����
    bool fDown;

    // ����ź ��ô
    bool gDown;

    // ������
    bool rDown;

    // ������ ��ȣ�ۿ� (������ ȹ��)
    bool iDown;

    // ��ȣ Ű�� ���� ��ü
    bool sDown1;
    bool sDown2;
    bool sDown3;   

    // ���� 1ȸ�� �����ϵ��� �ٴڿ� ��Ҵ��� �Ǵ�
    bool isJump;
    bool isDodge;

    // ���� ��ü �߿��� �ٸ� �������� �ʵ���
    bool isSwap;

    // ���� ��
    bool isReload;

    // �����̰� ������ ���� �غ� �Ϸ�
    bool isFireReady = true;

    // �� �浹 �÷��� bool ������ ����
    bool isBorder;

    // ���� Ÿ���� ���� �÷���
    bool isDamage;

    Vector3 moveVec;

    // ȸ�� ���� ���� ���� ���ϵ���
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    // ���� �ð��� �÷��̾�� �����ֱ� ����
    // �÷��̾�� �� �������� MeshRenderer�� ������ �����Ƿ� ��� ������
    MeshRenderer[] meshs;

    // Ʈ���� �� �Ɵ����� �����ϱ� ���� ���� ����
    GameObject nearObject;
    Weapon equipWeapon;

    int equipWeaponIndex = -1;

    // ���� ������
    float fireDelay;

	private void Awake()
	{
        rigid = GetComponent<Rigidbody>();
        // �ڽ� ������Ʈ�� �ִ� ������Ʈ�� ������
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
        // GetAxisRaw() : Axis ���� ������ ��ȯ
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        // space �� ������ �� ������ �ٵ��� GetButtonDown ���
        jDown = Input.GetButtonDown("Jump");
        // �⺻������ ���콺 ���ʿ� Fire1 �� �� ����
        fDown = Input.GetButton("Fire1");
        // ����ź ��ô
        gDown = Input.GetButtonDown("Fire2");
        // ������
        rDown = Input.GetButtonDown("Reload");
        // e Ű�� ������ ������ ��ȣ�ۿ�
        iDown = Input.GetButtonDown("Interaction");
        // ���� ��ü Ű
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

	void Move()
	{
        // normalized
        // �밢���̶�� �ӵ� �������� �ʰ�.
        // ���� ���� 1�� ������ ����
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        // ȸ�Ǹ� �ϰ� �ִٸ�
        if(isDodge)
            moveVec = dodgeVec;

        // ���� ��ü�ϴ� ���� �������� ���ϰ�
        // �����ϴ� ���� �������� ���ϰ�
        if(isSwap || isReload || !isFireReady)
            moveVec = Vector3.zero;
        // ������ isBorder �� �߰��ϸ� ���� �浹�ϸ� ȸ���� ���ϰԵ�

        // ���� �հ� ����������
        if(!isBorder)
        {
            if(wDown)
                transform.position += moveVec * speed * 0.3f * Time.deltaTime;
            else
                transform.position += moveVec * speed * Time.deltaTime;
        }

        // �ִϸ��̼�
        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        // �÷��̾� ȸ�� (�����̴� �������� �ٶ󺻴�)
        transform.LookAt(transform.position + moveVec);

        if(fDown)
        {
            // ���콺�� ���� ȸ��
            // ��ũ������ ����� Ray �� ��� �Լ� ScreenPointToRay();
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            // ������ �굵�� ũ�� �� 100
            // out return ó�� ��ȯ ���� �־��� ������ �����ϴ� Ű����
            if(Physics.Raycast(ray, out rayHit, 100))
            {
                // ���� ���� - �÷��̾��� ��ġ = ��� ��ġ
                // �� ��ġ�� �÷��̾ �ٶ�
                Vector3 nextVec = rayHit.point - transform.position;
                // RayCastHit �� ���̴� �����ϵ��� y �� ���� 0����
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
    }

    void Jump()
    {
        //����Ű�� �����鼭 �����̽��ٴ� ����

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
            // ���콺 ��ġ�� �ٷ� ���� �� �ֵ��� RayCast �� ���
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
        // �տ� ���Ⱑ ������ ����
        if(equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        // ������ �� �ð��� ������
        isFireReady = equipWeapon.rate < fireDelay;

        // ������ �����Ǹ� ���� ��� ����
        if(fDown && isFireReady && !isDodge && !isSwap)
        {
            equipWeapon.Use();
            // �ٰŸ�, ���Ÿ� �� ���� ��� ����
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            // ������ �ϸ� �ٽ� ������
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

            // �����ϴµ� 3�� �ɸ�
            Invoke("ReloadOut", 3f);
        }
    }

    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo; // �÷��̾ �����ϰ� �ִ� �������� ���� ź�� �������.
        isReload = false;
    }

    void Dodge()
    {
        if(jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            // ������ ���� -> ȸ�ǹ��� ���ͷ� �ٲ�� ����
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            // �ð��� �Լ� ȣ�� 0.5 ��
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
            // ����� ��츦 �����Ͽ� ���� �߰�
            if(equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();

            // �տ� �� ���⸦ ���̵��� Ȱ��ȭ
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");

            //���� ��ü�� �ϴ� �߿��� �ٸ� ���� ���� �ʵ���
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
                // Item ��ũ��Ʈ
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }

    // �ٴڿ� ����� �� ���� �÷��� �ʱ�ȭ
    void OnCollisionEnter(Collision collision)
	{
        if(collision.gameObject.tag == "Floor")
        {
            isJump = false;
            anim.SetBool("isJump", false);
        }
    }

    //�ڵ� ȸ�� ����
    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        // Raycast() : Ray �� ��� ��� ������Ʈ�� �����ϴ� �Լ�
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
            // enum Ÿ��
            switch(item.type)
            {
            case Item.Type.Ammo:
                ammo += item.value; // �Ѿ� �߰�
                if(ammo > maxAmmo)
                    ammo = maxAmmo;
                break;
            case Item.Type.Coin:
                coin += item.value; // ���� �߰�
                if(coin > maxCoin)
                    coin = maxCoin;
                break;
            case Item.Type.Heart:
                health += item.value; // ü�� ȸ��
                if(health > maxHealth)
                    health = maxHealth;
                break;
            case Item.Type.Grenade:
                grenades[hasGrenades].SetActive(true);
                hasGrenades += item.value; // ����ź �߰�
                if(hasGrenades > maxHasGrenades)
                    hasGrenades = maxHasGrenades;
                break;
            }

            // �Ծ����� �ʿ��� �����ϱ�
            Destroy(other.gameObject);
        }
        else if(other.tag == "EnemyBullet")
        {
            // ���� Ÿ��
            if(!isDamage)
            {
                // �÷��̾� �ǰ� ����
                // Bullet ��ũ��Ʈ ��Ȱ���Ͽ� ������ ����
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;

                // �÷��̾ �̻��ϰ� �浹
                // �̻��ϸ��� ������ �ִ� Ư¡ -> RigidBody
                // ������ �ٵ� ���� �������� �Ͽ� Destroy() ȣ��
                if(other.GetComponent<Rigidbody>() != null)
                {
                    Destroy(other.gameObject);
                }

                // �ǰ� ���׼�
                StartCoroutine(OnDamage());
            }
        }
	}

    // ���׼��� ���� �ڷ�ƾ ���� �� ȣ��
    IEnumerator OnDamage()
    {
        // ���� �ð��� ���� �÷���
        isDamage = true;

        // ���� ���θ� �÷��̾�� Ȯ�� ��Ŵ. ���� �����
        // �ݺ����� ����Ͽ� ��� ������ ������ ����
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }

        // ���� �ð�
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
