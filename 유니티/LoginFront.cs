using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginFront : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ȸ������ â �̵�
    public void moveToSignUp()
    {
        GameObject LogIn = GameObject.Find("StartPage").transform.Find("Log In").gameObject;
            GameObject SignUp = GameObject.Find("StartPage").transform.Find("Sign Up").gameObject;
            LogIn.SetActive(false);
        SignUp.SetActive(true);
    }
    // �α��� â �̵�
    public void moveToSignIn()
    {
        GameObject LogIn = GameObject.Find("StartPage").transform.Find("Log In").gameObject;
        GameObject SignUp = GameObject.Find("StartPage").transform.Find("Sign Up").gameObject;
        LogIn.SetActive(true);
        SignUp.SetActive(false);
    }
}
