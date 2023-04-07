using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotMachine : MonoBehaviour
{
	[SerializeField]
	private	TMP_InputField	inputBetAmount;			// ���� �ݾ� �ʵ�
	[SerializeField]
	private	Image			imageBetAmount;			// ���� �ݾ� �ʵ� (���� �����)
	[SerializeField]
	private	TextMeshProUGUI	textCredits;			// �÷��̾� ���� �ݾ�
	[SerializeField]
	private	TextMeshProUGUI	textResult;             // ���� ��� ���
	[SerializeField]
	private Image imageFirstReel;          // ù ��° �� ����
	[SerializeField]
	private Image imageSecondReel;         // �� ��° �� ����
	[SerializeField]
	private Image imageThirdReel;          // �� ��° �� ����

	public Sprite[] gems;

	private	float	firstSpinDuration = 1f;			// �� ������ ���� �ð� (0.2��)
	private	float	secondSpinDuration = 1.5f;			// �� ������ ���� �ð� (0.2��)
	private	float	thirdSpinDuration = 2f;			// �� ������ ���� �ð� (0.2��)
	private	float	elapsedTime = 0;				// ���� ���� �����ð� (���� ���� ���ư��� ��ó��)
	private	bool	isStartSpin = false;			// �� ���� true�̸� �� ������ ����
	private	int		credits = 10000;				// �÷��̾� ���� �ݾ�
	
	// ���� ���� (�� ���� false�̸� ���� ������ ��)
	private	bool	isFirstReelSpinned = false;
	private	bool	isSecondReelSpinned = false;
	private	bool	isThirdReelSpinned = false;
	// ���� ��� ��
	private	int		firstReelResult = 0;
	private	int		secondReelResult = 0;
	private	int		thirdReelResult = 0;		

	private	List<int>	weightedReelPoll;			// ���� �����ϴ� ������ Ȯ�� ���� ����Ʈ
	private	int			zeroProbability = 30;       // 0�� ���� Ȯ�� (30 = 30%)


	private void Awake()
	{
		weightedReelPoll = new List<int>(100);
		// zeroProbability ������ 30�� ��ŭ 0���� ä���ش�.
		for ( int i = 0; i < zeroProbability; ++ i )
		{
			weightedReelPoll.Add(0);
		}

		// 0�� ���� Ȯ���� 30%�̱� ������ 1~9�� ���� Ȯ���� 70%
		// 7.7777 = (100 - 30) / 9;
		int remaining_values_prob = (100 - zeroProbability) / 9;

		// 1~9���� ���ڸ� 7���� weightReelPoll ����Ʈ�� ä���ִ´�.
		for ( int i = 1; i < 10; ++ i )
		{
			for ( int j = 0; j < remaining_values_prob; ++ j )
			{
				weightedReelPoll.Add(i);
			}
		}

		// �ش� �ڵ�� �� �� Ȯ�ο����� ����. 0�� 30��, 1~9�� 7���� �� 97���� ���ڰ� ����
		/*Debug.Log($"weightedReelPoll ���� : {weightedReelPoll.Count}");
		for ( int i = 0; i < weightedReelPoll.Count; ++ i )
		{
			Debug.Log(weightedReelPoll[i]);
		}*/
		textCredits.text = $"���� �� : {credits}";
	}

	void OnEnable()
	{
		credits = CasinoManager.instance.gold;
		textCredits.text = $"���� �� : {credits}";
	}

	private void Update()
	{
		if ( !isStartSpin ) return;

		elapsedTime += Time.deltaTime;
		int random_spinResult = Random.Range(0, 10);

		if ( !isFirstReelSpinned )
		{
			firstReelResult = random_spinResult;
			if ( elapsedTime >= firstSpinDuration)
			{
				int weighted_random = Random.Range(0, weightedReelPoll.Count);
				firstReelResult		= (int)weightedReelPoll[weighted_random];
				isFirstReelSpinned	= true;
				elapsedTime			= 0;
			}
		}
		else if ( !isSecondReelSpinned )
		{
			secondReelResult = random_spinResult;
			if ( elapsedTime >= secondSpinDuration )
			{
				isSecondReelSpinned = true;
				elapsedTime = 0;
			}
		}
		else if ( !isThirdReelSpinned )
		{
			thirdReelResult = random_spinResult;
			if ( elapsedTime >= thirdSpinDuration )
			{
				int weighted_random = Random.Range(0, weightedReelPoll.Count);
				thirdReelResult		= (int)weightedReelPoll[weighted_random];

				if ( (firstReelResult == secondReelResult) && (thirdReelResult != firstReelResult) )
				{
					if ( thirdReelResult < firstReelResult )	thirdReelResult = firstReelResult - 1;
					if ( thirdReelResult > firstReelResult )	thirdReelResult = firstReelResult + 1;
				}

				isStartSpin			= false;
				elapsedTime			= 0;
				isFirstReelSpinned	= false;
				isSecondReelSpinned	= false;
				isThirdReelSpinned	= false;

				CheckBet();
			}
		}

		imageFirstReel.sprite = gems[firstReelResult];
		imageSecondReel.sprite = gems[secondReelResult];
		imageThirdReel.sprite = gems[thirdReelResult];
	}

	public void OnClickPull()
	{
		// �ʵ��� ����� �Է� ������ �ٲ�� ���� �� ������ �ʱ�ȭ
		OnMessage(Color.white, string.Empty);

		// �ʵ忡 ���� �Է����� �ʾ��� �� ����
		if ( inputBetAmount.text.Trim().Equals("") )
		{
			OnMessage(Color.red, "�ݾ��� �������ּ���.");
			return;
		}

		int parse = int.Parse(inputBetAmount.text);

		if(parse <= 0)
        {
			OnMessage(Color.red, "1�� �̻��� �ʿ��մϴ�.");
		}
		else if ( credits - parse >= 0 )
		{
			credits -= parse;
			//CasinoManager.instance.gold -= parse;
			textCredits.text = $"���� �� : {credits}";

			isStartSpin = true;
		}
		else
		{
			OnMessage(Color.red, "���� ������� �ʽ��ϴ�.");
		}
	}


	private void CheckBet()
	{
		int betAmount = int.Parse(inputBetAmount.text);

		if ( firstReelResult == secondReelResult && secondReelResult == thirdReelResult )
		{
			credits += betAmount * 100;//int.Parse(inputBetAmount.text) * 100;
			CasinoManager.instance.EarnGold(betAmount * 99);
			//textCredits.text = $"Credits : {credits}";

			textResult.text = "������ �������ϴ�!!!! 100�� ��÷!!!";// "YOU WIN!";
		}
		/*else if ( firstReelResult == 0 && thirdReelResult == 0 )
		{
			credits += (int)(betAmount * 0.5f);
			textResult.text = $"There are two 0! You Win! {betAmount * 0.5f}";
		}*/
		else if ( firstReelResult == secondReelResult )
		{
			credits += (int)(betAmount * 0.5f);
			CasinoManager.instance.EarnGold((int)(betAmount * 0.5f) * -1);
			textResult.text = "�ƽ��׿�~ ������ �����帮�ڽ��ϴ�.";
		}
		else if ( firstReelResult == thirdReelResult )
		{
			credits += betAmount * 2;
			CasinoManager.instance.EarnGold(betAmount);
			textResult.text = "��Ī ���ʽ�!. 2��� �����帳�ϴ�.";
		}
		else
		{
			CasinoManager.instance.EarnGold(betAmount * -1);
			textResult.text = "���Դϴ�!";
		}

		textCredits.text = $"���� �� : {credits}";
	}

	private void OnMessage(Color color, string msg)
	{
		imageBetAmount.color = color;
		textResult.text		 = msg;
	}
}

