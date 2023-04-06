using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class BossMemberManager : MonoBehaviour
{
    public GameObject Player1Group;
    public GameObject Player2Group;
    public GameObject Player3Group;
    public GameObject Player4Group;

    public RectTransform rectPlayer1HP;
    public RectTransform rectPlayer2HP;
    public RectTransform rectPlayer3HP;
    public RectTransform rectPlayer4HP;

    public TextMeshProUGUI txtPlayer1Name;
    public TextMeshProUGUI txtPlayer2Name;
    public TextMeshProUGUI txtPlayer3Name;
    public TextMeshProUGUI txtPlayer4Name;

    public List<BossPlayer> players = new List<BossPlayer>();

    //<ViewID, (PlayerName, (MaxHP, CURHP))>
    public Dictionary<int, (string, (int, int))> playerInfoList = new Dictionary<int, (string, (int, int))>();


    // Start is called before the first frame update
    void Start()
    {
		//DontDestroyOnLoad(this.gameObject);
		UpdatePlayerInfoList();
		UpdateUI();
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	private void LateUpdate()
	{
		
	}

	public void UpdatePlayerInfoList()
	{
		// 현재 있는 플레이어 클론들의 정보를 가져와서, Dictionary 를 업데이트 함.
		BossPlayer[] bossPlayers = FindObjectsOfType<BossPlayer>();
		foreach(BossPlayer bossPlayer in bossPlayers)
		{
			playerInfoList[bossPlayer.pv.ViewID] = (PlayerPrefs.GetString("userNickname"), (bossPlayer.maxHealth, bossPlayer.curHealth));
		}
	}

	public void UpdateUI()
    {
        Player1Group.SetActive(false);
        Player2Group.SetActive(false);
        Player3Group.SetActive(false);
        Player4Group.SetActive(false);

		if(playerInfoList.Count >= 1)
			Player1Group.SetActive(true);
		if(playerInfoList.Count >= 2)
			Player2Group.SetActive(true);
		if(playerInfoList.Count >= 3)
			Player3Group.SetActive(true);
		if(playerInfoList.Count >= 4)
			Player4Group.SetActive(true);

		int idx = 1;

		foreach(KeyValuePair<int, (string, (int, int))> kvp in playerInfoList)
		{
			// 플레이어 체력 비율 계산
			float retePlayerHP = (float)kvp.Value.Item2.Item2 / kvp.Value.Item2.Item1;

			if(idx == 1)
			{
				txtPlayer1Name.text = kvp.Value.Item1;
				rectPlayer1HP.localScale = new Vector3(retePlayerHP, 1, 1);
			}
			else if(idx == 2)
			{
				txtPlayer2Name.text = kvp.Value.Item1;
				rectPlayer2HP.localScale = new Vector3(retePlayerHP, 1, 1);
			}
			else if(idx == 3)
			{
				txtPlayer3Name.text = kvp.Value.Item1;
				rectPlayer3HP.localScale = new Vector3(retePlayerHP, 1, 1);
			}
			else if(idx == 4)
			{
				txtPlayer4Name.text = kvp.Value.Item1;
				rectPlayer4HP.localScale = new Vector3(retePlayerHP, 1, 1);
			}

			idx++;
		}
	}
}
