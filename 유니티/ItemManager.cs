using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemManager : MonoBehaviour
{
    //슬롯 버튼들

    public GameObject Hat;
    public GameObject Hair;
    public GameObject Head;
    public GameObject Acs;
    public GameObject Weapon;
    public GameObject Shield;
    public GameObject Body;
    public GameObject Back;

    //슬롯들
    public GameObject HatSlot;
    public GameObject HeadSlot;
    public GameObject AcsSlot;
    public GameObject HairSlot;
    public GameObject WeaponSlot;
    public GameObject SheildSlot;
    public GameObject DressSlot;
    public GameObject BackSlot;

    //현재 활성화된 슬롯
    private int ActiveSlot;

    public Transform player;

    //현재 입은 아이템 num
    private int hatNum;
    private int headNum;
    private int acsNum;
    private int hairNum;
    private int weaponNum;
    private int sheildNum;
    private int dressNum;
    private int backNum;

    // Start is called before the first frame update
    void Start()
    {
        ActiveSlot = 0; //처음엔 모자가 엑티스 슬롯
        player = player.GetComponent<Transform>();

        HatSlot.SetActive(true);
        HeadSlot.SetActive(false);
        AcsSlot.SetActive(false);
        HairSlot.SetActive(false);
        WeaponSlot.SetActive(false);
        SheildSlot.SetActive(false);
        DressSlot.SetActive(false);
        BackSlot.SetActive(false);

        //여기에 지금 입고 있는 옷들을 넣어줘야함!
        hatNum = 100;
        headNum = 0;
        acsNum = 100;
        hairNum = 100;
        weaponNum = 100;
        sheildNum = 100;
        dressNum = 0;
        backNum = 100;

        //test


 /*       int[] hats = { 1, 2, 3, 4, 8 };

        for (int i = 0; i < hats.Length; i++)
        {
            HatSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("HatSlots").GetChild(hats[i]).gameObject.SetActive(true);
        }
*/

    }



    public void ClickItem(Button Item)

    {   //모자 96부터 109까지
        Transform item = Item.GetComponent<Transform>();
        int num = item.GetSiblingIndex();

        switch (ActiveSlot) {
            case 0:
                //벗어주기
                if (hatNum != 100) player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(hatNum + 96).gameObject.SetActive(false);
                //착용안함 (그냥 벗기)
                if (num == 15){ hatNum = 100; break; }
                //입어주기
                player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(num + 96).gameObject.SetActive(true);
                hatNum = num;
                break;
            case 1:
                //벗어주기
                player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(headNum + 76).gameObject.SetActive(false);
                //착용 안함이면 0번째 머리로 입어주기           
                if(num == 20) num = 0; 
                player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(num + 76).gameObject.SetActive(true);
                headNum = num;
                Debug.Log("몇번이길래.." + num);
                break;
            case 2:
                if (hairNum != 100) player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(hairNum + 63).gameObject.SetActive(false);
                if (num == 13) { hairNum = 100; break; }
                player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(num + 63).gameObject.SetActive(true);
                hairNum = num;
                break;
            case 3:
                if (acsNum != 100) player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(acsNum).gameObject.SetActive(false);
                if (num == 39) { hairNum = 100; break; }
                player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(num).gameObject.SetActive(true);
                acsNum = num;
                break;
            case 4:
                if (weaponNum != 100) player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("clavicle_r").Find("upperarm_r").Find("lowerarm_r").Find("hand_r").Find("weapon_r").GetChild(weaponNum+1).gameObject.SetActive(false);
                if (num == 30) num = 0;
                player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("clavicle_r").Find("upperarm_r").Find("lowerarm_r").Find("hand_r").Find("weapon_r").GetChild(num+1).gameObject.SetActive(true);
                weaponNum = num;
                break;
            case 5:
                if (sheildNum != 100) player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("clavicle_l").Find("upperarm_l").Find("lowerarm_l").Find("hand_l").Find("weapon_l").GetChild(sheildNum+17).gameObject.SetActive(false);
                if (num == 20) { sheildNum = 100; break; }
                player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("clavicle_l").Find("upperarm_l").Find("lowerarm_l").Find("hand_l").Find("weapon_l").GetChild(num+17).gameObject.SetActive(true);
                sheildNum = num;
                break;
            case 6:
                player.GetChild(dressNum).gameObject.SetActive(false);
                if (num == 20) num = 0;
                player.GetChild(num).gameObject.SetActive(true);
                dressNum = num;
                break;
            case 7:
                //망토
                if (num < 3)
                {
                    //원래 가방이었는지 망토였는지 확인

                    if (backNum != 100)
                    {
                        //원래 망토였으면,
                        if (backNum < 3) player.GetChild(backNum + 20).gameObject.SetActive(false);
                        else player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("BackpackBone").GetChild(backNum - 3).gameObject.SetActive(false);
                    }
                    player.GetChild(num + 20).gameObject.SetActive(true);
                }
                //가방
                else
                {

                    if (backNum != 100)
                    {
                        if (backNum < 3) player.GetChild(backNum + 20).gameObject.SetActive(false);
                        else player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("BackpackBone").GetChild(backNum - 3).gameObject.SetActive(false);
                    }
                    //착용안함
                    if (num == 6) {backNum = 100; break; }
                    
                    player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("BackpackBone").GetChild(num-3).gameObject.SetActive(true);
                }

                backNum = num;
                break;
        }

        
    
    }



    public void OnClickParts(GameObject input)
    {
        if (input.GetComponent<Transform>().GetSiblingIndex() == 7)
           player.transform.rotation = Quaternion.Euler(0, 0, 0);
        
        else player.transform.rotation = Quaternion.Euler(0, 158.131f, 0);
            
        
        HatSlot.SetActive(false);
        HeadSlot.SetActive(false);
        AcsSlot.SetActive(false);
        HairSlot.SetActive(false);
        WeaponSlot.SetActive(false);
        SheildSlot.SetActive(false);
        DressSlot.SetActive(false);
        BackSlot.SetActive(false);

        input.SetActive(true);
        //현재 활성화된 슬롯 넘버 저장
        Transform it = input.GetComponent<Transform>();
        ActiveSlot = it.GetSiblingIndex();


    }
}
