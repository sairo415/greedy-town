using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType { Melee, Range, Glove, Shoe, Heal }

    [Header("# Main Info")]
    public ItemType itemType;
    public int itemId;
    public string itemName;
    public string itemDesc;
    public Sprite itemIcon;

    [Header("# Level Data")]
    public float baseDamage;
    public int baseCount;//근접->개수, 원거리->관통
    public float baseCoolTime;
    public float[] damages;
    public int[] counts;
    public float[] coolTimes;

    [Header("# Weapon")]
    public GameObject projectile;//무기 -> 프리팹

}
