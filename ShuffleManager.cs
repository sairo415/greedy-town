using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ShuffleManager : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField inputBetAmount;          // ���� �ݾ� �ʵ�
	[SerializeField]
	private Image imageBetAmount;           // ���� �ݾ� �ʵ� (���� �����)
	[SerializeField]
	private TextMeshProUGUI textCredits;            // �÷��̾� ���� �ݾ�
	[SerializeField]
	private TextMeshProUGUI textResult;             // ���� ��� ���
	[SerializeField]
	private Image imageFirstCard;          // ù ��° �� ����
	[SerializeField]
	private Image imageSecondCard;         // �� ��° �� ����
	[SerializeField]
	private Image imageThirdCard;          // �� ��° �� ����
	[SerializeField]
	private Button buttonFirstCard;          // ù ��° �� ����
	[SerializeField]
	private Button buttonSecondCard;         // �� ��° �� ����
	[SerializeField]
	private Button buttonThirdCard;          // �� ��° �� ����
	[SerializeField]
	private Button buttonStart;          // �� ��° �� ����
	[SerializeField]
	private Image imageEnemy;          // �� ��° �� ����
	[SerializeField]
	private Button buttonGo;          // �� ��° �� ����
	[SerializeField]
	private Button buttonStop;          // �� ��° �� ����

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
	private int magni = 2;//�¸� ����

	void Awake()
    {
		buttonFirstCard.interactable = false;
		buttonSecondCard.interactable = false;
		buttonThirdCard.interactable = false;
	}

    void OnEnable()
    {
		credits = CasinoManager.instance.gold;
		textCredits.text = $"���� �� : {credits}";
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
			textResult.text = "���ڰ� �� ���׿�. �̰���ϴ�! �� �Ͻðڽ��ϱ�?";
			buttonGo.gameObject.SetActive(true);
			buttonStop.gameObject.SetActive(true);
		}
        else
        {
			textResult.text = "�ƽ�����. �����ϴ�!";
			textCredits.text = $"���� �� : {credits}";
			magni = 2;
			buttonStart.interactable = true;//����� ����
		}
	}

	public void OnClickGo()
    {
		magni *= 2;
		textResult.text = "�ް� �� �� ��! (" + magni + "��)";
		CardShuffle();
		buttonGo.gameObject.SetActive(false);
		buttonStop.gameObject.SetActive(false);
	}
	public void OnClickStop()
	{
		int betAmount = int.Parse(inputBetAmount.text);
		credits += (int)(betAmount * magni);
		CasinoManager.instance.EarnGold((int)(betAmount * magni));
		textCredits.text = $"���� �� : {credits}";
		textResult.text = "������ϴ�.";
		magni = 2;
		buttonStart.interactable = true;
		buttonGo.gameObject.SetActive(false);
		buttonStop.gameObject.SetActive(false);
	}

	public void OnClickPull()
	{
		// �ʵ��� ����� �Է� ������ �ٲ�� ���� �� ������ �ʱ�ȭ
		OnMessage(Color.white, string.Empty);


		// �ʵ忡 ���� �Է����� �ʾ��� �� ����
		if (inputBetAmount.text.Trim().Equals(""))
		{
			OnMessage(Color.red, "�ݾ��� �������ּ���.");
			return;
		}

		int parse = int.Parse(inputBetAmount.text);

		if (parse <= 0)
		{
			OnMessage(Color.red, "1�� �̻��� �ʿ��մϴ�.");
		}
		else if (credits - parse >= 0)
		{
			credits -= parse;
			textCredits.text = $"���� �� : {credits}";
			CasinoManager.instance.EarnGold(parse * -1);

			//isStart = true;//����
			CardShuffle();
		}
		else
		{
			OnMessage(Color.red, "���� ������� �ʽ��ϴ�.");
		}
	}

	private void OnMessage(Color color, string msg)
	{
		imageBetAmount.color = color;
		textResult.text = msg;
	}
}
