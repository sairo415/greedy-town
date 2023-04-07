using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ShuffleManager : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField inputBetAmount;          // 배팅 금액 필드
	[SerializeField]
	private Image imageBetAmount;           // 배팅 금액 필드 (색상 변경용)
	[SerializeField]
	private TextMeshProUGUI textCredits;            // 플레이어 소지 금액
	[SerializeField]
	private TextMeshProUGUI textResult;             // 실행 결과 출력
	[SerializeField]
	private Image imageFirstCard;          // 첫 번째 릴 숫자
	[SerializeField]
	private Image imageSecondCard;         // 두 번째 릴 숫자
	[SerializeField]
	private Image imageThirdCard;          // 세 번째 릴 숫자
	[SerializeField]
	private Button buttonFirstCard;          // 첫 번째 릴 숫자
	[SerializeField]
	private Button buttonSecondCard;         // 두 번째 릴 숫자
	[SerializeField]
	private Button buttonThirdCard;          // 세 번째 릴 숫자
	[SerializeField]
	private Button buttonStart;          // 세 번째 릴 숫자
	[SerializeField]
	private Image imageEnemy;          // 세 번째 릴 숫자
	[SerializeField]
	private Button buttonGo;          // 세 번째 릴 숫자
	[SerializeField]
	private Button buttonStop;          // 세 번째 릴 숫자

	private int firstCardResult = 0;
	private int secondCardResult = 0;
	private int thirdCardResult = 0;
	private int selectedResult = 0;
	private int enemyResult = 0;

	public Sprite[] cards;

	private bool isStart = false;
	private int credits = 10000;

	private float durations = 2f;
	private float elapsedTime = 0;
	private int magni = 2;//승리 배율

	void Awake()
    {
		buttonFirstCard.interactable = false;
		buttonSecondCard.interactable = false;
		buttonThirdCard.interactable = false;
	}

    void OnEnable()
    {
		credits = CasinoManager.instance.gold;
		textCredits.text = $"남은 돈 : {credits}";
	}

    void Update()
    {
		if (!isStart)
			return;

		elapsedTime += Time.deltaTime;

		int result = Random.Range(0, 14);
		imageEnemy.sprite = cards[result];

		if (elapsedTime >= durations)
        {
			while (true)
			{
				result = Random.Range(0, 14);

				if (result != secondCardResult && result != thirdCardResult && result != firstCardResult)
					break;
			}
			imageEnemy.sprite = cards[result];
			elapsedTime = 0;
			enemyResult = result;
			isStart = false;
			CheckResult();
		}

	}

	public void CardShuffle()
	{
		imageFirstCard.sprite = cards[14];
		imageSecondCard.sprite = cards[14];
		imageThirdCard.sprite = cards[14];
		imageEnemy.gameObject.SetActive(false);

		while (true)
		{
			firstCardResult = Random.Range(0, 14);
			secondCardResult = Random.Range(0, 14);
			thirdCardResult = Random.Range(0, 14);

			if (firstCardResult != secondCardResult && secondCardResult != thirdCardResult && thirdCardResult != firstCardResult)
				break;
		}


		buttonFirstCard.interactable = true;
		buttonSecondCard.interactable = true;
		buttonThirdCard.interactable = true;
		buttonStart.interactable = false;
	}

	public void onClickCard()
    {
		GameObject clicked = EventSystem.current.currentSelectedGameObject;

        if (clicked.name.Equals("FirstButton"))
        {
			selectedResult = firstCardResult;
        }
		else if (clicked.name.Equals("SecondButton"))
        {
			selectedResult = secondCardResult;
		}
        else
        {
			selectedResult = thirdCardResult;
		}

		clicked.GetComponent<Image>().sprite = cards[selectedResult];

		buttonFirstCard.interactable = false;
		buttonSecondCard.interactable = false;
		buttonThirdCard.interactable = false;

		imageEnemy.gameObject.SetActive(true);
		isStart = true;
	}
	
	public void CheckResult()
    {

		imageFirstCard.sprite = cards[firstCardResult];
		imageSecondCard.sprite = cards[secondCardResult];
		imageThirdCard.sprite = cards[thirdCardResult];

		if (selectedResult > enemyResult)
        {
			textResult.text = "숫자가 더 높네요. 이겼습니다! 더 하시겠습니까?";
			buttonGo.gameObject.SetActive(true);
			buttonStop.gameObject.SetActive(true);
		}
        else
        {
			textResult.text = "아쉬워요. 졌습니다!";
			textCredits.text = $"남은 돈 : {credits}";
			magni = 2;
			buttonStart.interactable = true;//재시작 가능
		}
	}

	public void OnClickGo()
    {
		magni *= 2;
		textResult.text = "받고 두 배 더! (" + magni + "배)";
		CardShuffle();
		buttonGo.gameObject.SetActive(false);
		buttonStop.gameObject.SetActive(false);
	}
	public void OnClickStop()
	{
		int betAmount = int.Parse(inputBetAmount.text);
		credits += (int)(betAmount * magni);
		CasinoManager.instance.EarnGold((int)(betAmount * magni));
		textCredits.text = $"남은 돈 : {credits}";
		textResult.text = "멈췄습니다.";
		magni = 2;
		buttonStart.interactable = true;
		buttonGo.gameObject.SetActive(false);
		buttonStop.gameObject.SetActive(false);
	}

	public void OnClickPull()
	{
		// 필드의 색상과 입력 정보가 바뀌어 있을 수 있으니 초기화
		OnMessage(Color.white, string.Empty);


		// 필드에 값을 입력하지 않았을 때 에러
		if (inputBetAmount.text.Trim().Equals(""))
		{
			OnMessage(Color.red, "금액을 배팅해주세요.");
			return;
		}

		int parse = int.Parse(inputBetAmount.text);

		if (parse <= 0)
		{
			OnMessage(Color.red, "1원 이상이 필요합니다.");
		}
		else if (credits - parse >= 0)
		{
			credits -= parse;
			textCredits.text = $"남은 돈 : {credits}";
			CasinoManager.instance.EarnGold(parse * -1);

			//isStart = true;//시작
			CardShuffle();
		}
		else
		{
			OnMessage(Color.red, "돈이 충분하지 않습니다.");
		}
	}

	private void OnMessage(Color color, string msg)
	{
		imageBetAmount.color = color;
		textResult.text = msg;
	}
}
