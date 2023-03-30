using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;
    public Weapon weapon;
    public Gear gear;

    Image icon;
    Text textLevel;
    Text textName;
    Text textDesc;
    Text textRarity;

    void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
        textName = texts[1];
        textDesc = texts[2];
        textRarity = texts[3];

        textName.text = data.itemName;
        textDesc.text = data.itemDesc;
        textRarity.text = data.itemRarity.ToString();
        switch (textRarity.text)
        {
            case "희귀":
                textRarity.color = new Color(0, 0, 255);
                break;
            case "서사":
                textRarity.color = new Color(139, 0, 255);
                break;
            case "전설":
                textRarity.color = new Color(255, 127, 0);
                break;
            default:
                break;
        }
    }

    void OnEnable()
    {
        textLevel.text = "Lv." + level;
        
    }

    public void OnClick()
    {
        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                if(level == 0)
                {
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(data, true);
                }
                else
                {
                    //level-1인 이유는 1렙때 0번째 배열의 계수를 가져다쓰니까
                    weapon.LevelUp(data.baseDamage * data.damages[level-1], data.counts[level-1], data.baseCoolTime * data.coolTimes[level-1]);
                }
                break;
            case ItemData.ItemType.Effect:
                if(level == 0)
                {
                    GameObject newWeapon;
                    try
                    {
                        newWeapon = GameObject.Find("Support").transform.Find("Weapon " + data.itemId).gameObject;
                    }
                    catch(NullReferenceException ex)
                    {
                        //Debug.Log(ex.Data);
                        newWeapon = GameObject.Find("Player").transform.Find("Weapon " + data.itemId).gameObject;
                    }
                    weapon = newWeapon.GetComponent<Weapon>();
                    weapon.Init(data, false);
                    newWeapon.SetActive(true);
                }
                else
                {
                    weapon.LevelUp(data.baseDamage * data.damages[level-1], 0, data.baseCoolTime * data.coolTimes[level-1]);
                    if(level == data.damages.Length)//6이 만렙
                    {
                        weapon.LevelMax();
                    }
                }
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                if (level == 0)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                }
                else
                {
                    float nextRate = data.damages[level - 1] ;
                    gear.LevelUp(nextRate);
                }
                break;
            case ItemData.ItemType.Heal:
                VamsuGameManager.instance.health = VamsuGameManager.instance.maxHealth;
                break;
        }

        if(data.itemType != ItemData.ItemType.Heal)
            level++;

        if(level == data.damages.Length + 1)
        {
            GetComponent<Button>().interactable = false;
        }

        for(int i=0; i< transform.parent.childCount; i++)
        {
            transform.parent.GetChild(i).gameObject.SetActive(false);
        }
        

        Time.timeScale = 1;
    }
}
