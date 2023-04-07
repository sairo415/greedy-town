using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CasinoPlay : MonoBehaviour
{
    public TextMeshPro text;
    bool isClose;
    public GameObject gameUI;
    void Awake()
    {
        text = GetComponentInChildren<TextMeshPro>(true);
    }

    void Update()
    {
        if(isClose && Input.GetKeyDown(KeyCode.F))
        {
            //gameUI.GetComponent<RectTransform>().localScale = Vector3.one;
            gameUI.SetActive(true);
            CasinoManager.instance.player.moveOn = false;
        }
    }
    
    public void GameCancel()
    {
        //gameUI.GetComponent<RectTransform>().localScale = Vector3.zero;
        gameUI.SetActive(false);
        CasinoManager.instance.player.moveOn = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            text.gameObject.SetActive(true);
            isClose = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            text.gameObject.SetActive(false);
            isClose = false;
        }
    }
}
