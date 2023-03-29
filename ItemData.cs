using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType { Melee, Range, Effect, Glove, Shoe, Heal }
    public enum ItemRarity { �Ϲ�, ���, ����, ���� }

    [Header("# Main Info")]
    public ItemType itemType;
    public int itemId;
    public string itemName;
    public string itemDesc;
    public Sprite itemIcon;
    public ItemRarity itemRarity;

    [Header("# Level Data")]
    public float baseDamage;
    public int baseCount;//����->����, ���Ÿ�->����
    public float baseCoolTime;
    public float durations;
    public float[] damages;
    public int[] counts;
    public float[] coolTimes;

    [Header("# Weapon")]
    public GameObject projectile;//���� -> ������

}
