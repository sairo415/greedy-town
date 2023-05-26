using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;

public class LoginNetworkManager : MonoBehaviourPunCallbacks
{
    // 로그인 UI
    public TMP_InputField userEmail;
    public TMP_InputField userPassword;
    public Button signInBtn;
    public Button moveSignUpBtn;
    // 회원가입 UI
    public TMP_InputField email;
    public TMP_InputField nickname;
    public TMP_InputField password;
    public TMP_InputField checkPassword;
    public Button signUpBtn;
    public Button emailCheckBtn;
    public Button nicknameCheckBtn;
    // alert 창
    public TMP_Text alert;
    public Button closeBtn;

    private bool[] possSignIn;
    private bool[] possSignUp;
    private bool validSignIn;
    private bool validSignUp;

    private string baseUrl = "http://j8a808.p.ssafy.io:8080/";
//    private string baseUrl = "localhost:8080/";



    // Start is called before the first frame update
    void Start()
    {
        GameObject LogIn = GameObject.Find("StartPage").transform.Find("Log In").gameObject;
        GameObject SignUp = GameObject.Find("StartPage").transform.Find("Sign Up").gameObject;
        GameObject Alert = GameObject.Find("StartPage").transform.Find("Alert").gameObject;
        LogIn.SetActive(true);
        SignUp.SetActive(false);
        Alert.SetActive(false);
        signInBtn.interactable = false;
        signUpBtn.interactable = false;
        possSignIn = new bool[2];
        possSignUp = new bool[4];
        validSignIn = true;
        validSignUp = true;
    }

    // Update is called once per frame
    void Update()
    {
        if ("".Equals(userEmail.text))
        {
            possSignIn[0] = false;
        }
        else
        {
            possSignIn[0] = true;
        }
        if ("".Equals(userPassword.text))
        {
            possSignIn[1] = false;
        }
        else
        {
            possSignIn[1] = true;
        }
        for (int i = 0; i < 2; i++)
        {
            if (!possSignIn[i]) validSignIn = false;
        }
        if (validSignIn)
        {
            signInBtn.interactable = true;
        }
        else
        {
            signInBtn.interactable = false;
            validSignIn = true;
        }

        if ("".Equals(email.text))
        {
            possSignUp[0] = false;
        }
        else
        {
            possSignUp[0] = true;
            if ("".Equals(nickname.text))
            {
                possSignUp[1] = false;
            }
            else
            {
                possSignUp[1] = true;
            }
        }
        if ("".Equals(password.text))
        {
            possSignUp[2] = false;
        }
        else
        {
            possSignUp[2] = true;
        }
        if ("".Equals(checkPassword.text))
        {
            possSignUp[3] = false;
        }
        else
        {
            possSignUp[3] = true;
        }
        for (int i = 0; i < 4; i++)
        {
            if (!possSignUp[i]) validSignUp = false;
        }
        if (validSignUp)
        {
            signUpBtn.interactable = true;
        }
        else
        {
            signUpBtn.interactable = false;
            validSignUp = true;
        }

    }

    // Sign In
    public void Connect()
    {
        StartCoroutine(Signin());
    }

    // 이메일 중복 체크
    public void CheckEmail()
    {
        StartCoroutine(PostCheckEmail());
    }
    // 닉네임 중복 체크
    public void CheckNickname()
    {
        StartCoroutine(PostCheckNickname());
    }
    // 회원가입
    public void SignUp()
    {
        if (!password.text.Equals(checkPassword.text))
        {
            alert.text = "비밀번호를 확인해주세요.";
            GameObject.Find("StartPage").transform.Find("Alert").gameObject.SetActive(true);
            password.text = "";
            checkPassword.text = "";
        }
        else
        {
            StartCoroutine(PostSignUp());
        }
    }
    // 창 닫기
    public void CloseAlert()
    {
        GameObject.Find("StartPage").transform.Find("Alert").gameObject.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("서버 접속");
        SceneManager.LoadScene("MyPageCustom");
    }

    /*public override void OnJoinedLobby()
    {
        Debug.Log("로비 접속");
    }*/

    private IEnumerator Signin()
    {
        print("signin 호출");
        PlayerPrefs.DeleteAll();
        string url = baseUrl + "login";

        // 로그인 정보
        LoginRequest loginRequest = new LoginRequest(userEmail.text, userPassword.text);
        print(loginRequest.getUserEmail());
        print(loginRequest.getUserPassword());

        string data = JsonConvert.SerializeObject(loginRequest);
        print(" 변환데이터 :  " + data);


        using (UnityWebRequest request = UnityWebRequest.Post(url, data))
        {
            /*PlayerPrefs.DeleteAll();*/
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(data);
            request.uploadHandler.Dispose();
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler.Dispose();
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest(); // 응답 대기

            print(request.responseCode);

            if (request.responseCode == 200)
            {
                string authorization = request.GetResponseHeader("authorization");
                print("authorization " + authorization);
                string[] tokens = authorization.Split("_AND_");
                string accessToken = tokens[0].Replace("Bearer ", "");
                string refreshToken = tokens[1].Replace("Bearer ", "");
                print("accessToken " + accessToken);
                print("refreshToken " + refreshToken);
                PlayerPrefs.SetString("accessToken", accessToken);
                PlayerPrefs.SetString("refreshToken", refreshToken);
                PlayerPrefs.SetString("userEmail", loginRequest.getUserEmail());
                if (request.isDone) //Returns true after the UnityWebRequest has finished communicating with the remote server. (Read Only)
                {
                    Debug.Log("연결");
                    PhotonNetwork.ConnectUsingSettings();
                }
            }
            else if (request.responseCode == 401)
            {
                Debug.Log("비밀번호를 확인해주세요");
                alert.text = "비밀번호를 확인해주세요.";
                GameObject.Find("StartPage").transform.Find("Alert").gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("회원가입 하시겠습니까?");
                alert.text = "회원가입이 필요합니다.";
                GameObject.Find("StartPage").transform.Find("Alert").gameObject.SetActive(true);
            }
            request.Dispose();
        }
    }
    public IEnumerator PostCheckEmail()
    {
        string url = baseUrl + "check-email";
        string data = email.text;
        using (UnityWebRequest request = UnityWebRequest.Post(url, data))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(data);
            request.uploadHandler.Dispose();
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler.Dispose();
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.responseCode == 200)
            {
                print(request.downloadHandler.text);
                byte[] results = request.downloadHandler.data;

                JObject response = JObject.Parse(request.downloadHandler.text);
                string message = response["message"].ToString();
                print(message);

                if ("사용 가능한 이메일".Equals(message))
                {
                    print("사용 가능한 이메일입니다.");
                    alert.text = "사용 가능한 이메일입니다.";
                    GameObject.Find("StartPage").transform.Find("Alert").gameObject.SetActive(true);
                }
                else
                {
                    print("사용 중인 이메일입니다.");
                    alert.text = "사용 중인 이메일입니다.";
                    GameObject.Find("StartPage").transform.Find("Alert").gameObject.SetActive(true);
                }
            }
            request.Dispose();
        }
    }
    public IEnumerator PostCheckNickname()
    {
        string url = baseUrl + "check-nickname";
        string data = nickname.text;
        using (UnityWebRequest request = UnityWebRequest.Post(url, data))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(data);
            request.uploadHandler.Dispose();
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler.Dispose();
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.responseCode == 200)
            {
                print(request.downloadHandler.text);
                byte[] results = request.downloadHandler.data;

                JObject response = JObject.Parse(request.downloadHandler.text);
                string message = response["message"].ToString();
                print(message);

                if ("사용 가능한 닉네임".Equals(message))
                {
                    print("사용 가능한 닉네임입니다.");
                    alert.text = "사용 가능한 닉네임입니다.";
                    GameObject.Find("StartPage").transform.Find("Alert").gameObject.SetActive(true);
                }
                else
                {
                    print("사용 중인 닉네임입니다.");
                    alert.text = "사용 중인 닉네임입니다.";
                    GameObject.Find("StartPage").transform.Find("Alert").gameObject.SetActive(true);
                }
            }
            request.Dispose();
        }
    }
    public IEnumerator PostSignUp()
    {
        string url = baseUrl + "regist";

        // 로그인 정보
        RegistRequest registRequest = new RegistRequest(email.text, password.text, nickname.text);
        string data = JsonConvert.SerializeObject(registRequest);

        using (UnityWebRequest request = UnityWebRequest.Post(url, data))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(data);
            request.uploadHandler.Dispose();
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler.Dispose();
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.responseCode == 200)
            {
                print(request.downloadHandler.text);
                byte[] results = request.downloadHandler.data;

                JObject response = JObject.Parse(request.downloadHandler.text);
                string message = response["message"].ToString();
                print(message);

                if ("다 성공".Equals(message))
                {
                    print("회원가입 성공");
                    alert.text = "환영합니다.";
                    GameObject.Find("StartPage").transform.Find("Alert").gameObject.SetActive(true);
                    GameObject LogIn = GameObject.Find("StartPage").transform.Find("Log In").gameObject;
                    GameObject SignUp = GameObject.Find("StartPage").transform.Find("Sign Up").gameObject;
                    LogIn.SetActive(true);
                    SignUp.SetActive(false);
                }
                else
                {
                    print("다시 시도해 주세요.");
                    alert.text = "다시 시도해 주세요.";
                    GameObject.Find("StartPage").transform.Find("Alert").gameObject.SetActive(true);
                }
            }
            request.Dispose();
        }
    }
}