using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMainManager : MonoBehaviour
{
    public static BossMainManager Instance;
    public Color TeamColor;
    public int sceneNumber = 1;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
