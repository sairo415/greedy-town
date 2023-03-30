using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public bool isPause = false;
    RectTransform rect;
    Item[] items;
    public GameObject parent;
    GameObject[] infos;
    public Font neodgm;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        items = VamsuGameManager.instance.canvas.transform.Find("LevelUp").GetComponentsInChildren<Item>(true);

        infos = new GameObject[items.Length];
        for(int i=0; i<items.Length; i++)
        {
            GameObject info = new GameObject();
            info.AddComponent<RectTransform>();
            info.GetComponent<RectTransform>().SetParent(parent.transform);
            info.GetComponent<RectTransform>().localPosition = Vector3.zero;
            info.GetComponent<RectTransform>().localScale = Vector3.one;
            info.name = "Info " + i;
            infos[i] = info;

            RectTransform forRect;

            GameObject NewObj = new GameObject(); //Create the GameObject
            NewObj.name = "Image";
            Image NewImage = NewObj.AddComponent<Image>(); //Add the Image Component script
            NewImage.sprite = items[i].data.itemIcon;
            forRect = NewObj.GetComponent<RectTransform>();
            forRect.SetParent(info.transform);
            NewObj.SetActive(true);
            forRect.localPosition = new Vector3(-150,0,0);
            forRect.localScale = Vector3.one;

            GameObject NewObj2 = new GameObject(); //Create the GameObject
            NewObj2.name = "Text";
            Text NewText = NewObj2.AddComponent<Text>(); //Add the Image Component script
            NewText.fontSize = 40;
            NewText.font = neodgm;
            NewText.color = Color.black;
            NewText.text = "Level ";
            forRect = NewObj2.GetComponent<RectTransform>();
            forRect.SetParent(info.transform);
            NewObj2.SetActive(true);
            forRect.localPosition = new Vector3(150,-30,0);
            forRect.localScale = Vector3.one;
            forRect.sizeDelta = new Vector2(400, 100);



            info.SetActive(false);
        }
    }

    public void OnClick()
    {
        if (isPause)
        {
            isPause = false;
            Hide();
        }
        else
        {
            isPause = true;
            Show();
        }
    }

    public void Show()
    {
        parent.GetComponent<RectTransform>().localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        VamsuGameManager.instance.Stop();

        for (int i = 0; i < items.Length; i++)
        {
            //дя╠Б
            if (items[i].level > 0)
            {
                infos[i].SetActive(true);
                infos[i].transform.GetChild(1).GetComponent<Text>().text = "Level " + items[i].level;
            }
        }
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
        VamsuGameManager.instance.Resume();
    }


}
