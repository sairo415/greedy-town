using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { Exp, Level, Kill, Time, Health }

    public InfoType type;

    Text myText;
    Slider mySlider;

    void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
    }

    //정보들이 다 업데이트 된 후에
    void LateUpdate()
    {
        if (Time.timeScale == 0)
            return;

        switch (type)
        {
            case InfoType.Exp:
                mySlider.value = GameManager.instance.exp / (float)GameManager.instance.nextExp[GameManager.instance.level];
                break;
            case InfoType.Level:
                myText.text = string.Format("LV.{0:F0}", GameManager.instance.level);
                break;
            case InfoType.Kill:
                myText.text = string.Format("{0:F0}", GameManager.instance.kill);
                break;
            case InfoType.Time:
                float remainTime = GameManager.instance.maxGameTime - GameManager.instance.gameTime;
                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;
            case InfoType.Health:
                mySlider.value = GameManager.instance.health / (float)GameManager.instance.maxHealth;
                break;
        }
    }
}
