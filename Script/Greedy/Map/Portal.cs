using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    int playerCnt;
    int triggerInPlayerCnt;

	private void Awake()
	{
        // 초기 값으로 활성화되지 않도록 아예 크게 줌.
        playerCnt = 100;
        triggerInPlayerCnt = 0;
    }

    // Update is called once per frame
    void Update()
    {
        BossGameManager gameManager = GameObject.FindObjectOfType<BossGameManager>();
        if(gameManager == null)
            return;

        BossPlayer[] bossPlayers = FindObjectsOfType<BossPlayer>();
        playerCnt = bossPlayers.Length;

        if(playerCnt != 0 && playerCnt * 2 == triggerInPlayerCnt)
        {
            gameManager.playerCntPanel.SetActive(false);
            gameManager.MoveFirstScene();
        }
        else
        {
            gameManager.playerCntPanel.SetActive(true);
        }
    }

	private void OnTriggerEnter(Collider other)
	{
        if(other.tag == "Player")
        {
            triggerInPlayerCnt++;

            Debug.Log(triggerInPlayerCnt);
        }
	}

	private void OnTriggerExit(Collider other)
	{
        if(other.tag == "Player")
        {
            triggerInPlayerCnt--;
            Debug.Log(triggerInPlayerCnt);
        }
    }
}
