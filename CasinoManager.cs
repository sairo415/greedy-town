using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasinoManager : MonoBehaviour
{
    public static CasinoManager instance;

    public CasinoPlayer player;

    public int gold;

    void Awake()
    {
        instance = this;
        //현재 골드 량 받아오기 api
        //Debug.Log(PlayerPrefs.GetString("money"));
        gold = Int32.Parse(PlayerPrefs.GetString("money"));
    }

    private void Start()
    {
        GameObject player = GameObject.Find("Player 1");
        player.transform.GetChild(PlayerPrefs.GetInt("dressNum")).gameObject.SetActive(true);
        if (PlayerPrefs.GetInt("backNum") != 100)
        {
            if (PlayerPrefs.GetInt("backNum") < 3)
            {
                player.transform.GetChild(PlayerPrefs.GetInt("backNum") + 20).gameObject.SetActive(true);
            }
            else
            {
                player.transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("BackpackBone").GetChild(PlayerPrefs.GetInt("backNum") - 3).gameObject.SetActive(true);
            }
        }
        if (PlayerPrefs.GetInt("sheildNum") != 100)
        {
            player.transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("clavicle_l").Find("upperarm_l").Find("lowerarm_l").Find("hand_l").Find("weapon_l").GetChild(PlayerPrefs.GetInt("sheildNum") + 17).gameObject.SetActive(true);
        }
        player.transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("clavicle_r").Find("upperarm_r").Find("lowerarm_r").Find("hand_r").Find("weapon_r").GetChild(PlayerPrefs.GetInt("weaponNum") + 1).gameObject.SetActive(true);
        if (PlayerPrefs.GetInt("acsNum") != 100)
        {
            player.transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(PlayerPrefs.GetInt("acsNum")).gameObject.SetActive(true);
        }
        if (PlayerPrefs.GetInt("hairNum") != 100)
        {
            player.transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(PlayerPrefs.GetInt("hairNum") + 63).gameObject.SetActive(true);
        }
        player.transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(PlayerPrefs.GetInt("headNum") + 76).gameObject.SetActive(true);
        if (PlayerPrefs.GetInt("hatNum") != 100)
        {
            player.transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(PlayerPrefs.GetInt("hatNum") + 96).gameObject.SetActive(true);
        }
        player.transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").Find("Eyebrow02").gameObject.SetActive(true);
    }

    public void EarnGold(int earnGold)
    {
        //Debug.Log("Earn: " + earnGold);
        gold += earnGold;
        StartCoroutine(transform.GetComponent<Commerce>().Earn((long)earnGold));
    }
}
