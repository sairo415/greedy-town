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

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateUI()
    {
        Player1Group.SetActive(false);
        Player2Group.SetActive(false);
        Player3Group.SetActive(false);
        Player4Group.SetActive(false);

        if(players.Count >= 1)
            Player1Group.SetActive(true);
        if(players.Count >= 2)
            Player2Group.SetActive(true);
        if(players.Count >= 3)
            Player3Group.SetActive(true);
        if(players.Count >= 4)
            Player4Group.SetActive(true);

        int idx = 1;

        foreach(BossPlayer bossPlayer in players)
        {
            float retePlayerHP = (float)bossPlayer.curHealth / bossPlayer.maxHealth;
            if(idx == 1)
            {
                txtPlayer1Name.text = bossPlayer.bossPlayerName;
                rectPlayer1HP.localScale = new Vector3(retePlayerHP, 1, 1);
            }
            else if(idx == 2)
            {
                txtPlayer2Name.text = bossPlayer.bossPlayerName;
                rectPlayer2HP.localScale = new Vector3(retePlayerHP, 1, 1);
            }
            else if(idx == 3)
            {
                txtPlayer3Name.text = bossPlayer.bossPlayerName;
                rectPlayer3HP.localScale = new Vector3(retePlayerHP, 1, 1);
            }
            else if(idx == 4)
            {
                txtPlayer4Name.text = bossPlayer.bossPlayerName;
                rectPlayer4HP.localScale = new Vector3(retePlayerHP, 1, 1);
            }
        }
    }
}
