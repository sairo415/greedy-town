using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemData.ItemType type;
    public float rate;
    public int id;

    public void Init(ItemData data)
    {
        id = data.itemId;
        name = "Gear " + id;
        transform.parent = GameObject.Find("Support").transform;
        transform.localPosition = Vector3.zero;

        type = data.itemType;
        rate = data.damages[0];
        ApplyGear();
    }

    //몇 퍼 씩 더하는 값
    public void LevelUp(float rate)
    {
        this.rate = rate;
        ApplyGear();
    }

    void ApplyGear()
    {
        switch (type)
        {
            case ItemData.ItemType.Glove:
                CoolDown();
                break;
            case ItemData.ItemType.Shoe:
                SpeedUp();
                break;
        }
    }

    void CoolDown()
    {
        switch (id)
        {
            case 1:
                VamsuGameManager.instance.extraDamage += rate;
                break;
            case 2:
                VamsuGameManager.instance.extraCoolDown += rate;
                break;
            case 3:
                VamsuGameManager.instance.extraArmor += rate;
                break;
            case 4:
                VamsuGameManager.instance.extraExp += rate;
                break;
            case 5:
                VamsuGameManager.instance.extraGold += rate;
                break;
            default:
                break;
        }
        
    }

    void SpeedUp()
    {
        VamsuGameManager.instance.player.baseSpeed += rate;
        VamsuGameManager.instance.player.speed += rate;
    }
}
