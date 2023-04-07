using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotMachine : MonoBehaviour
{
	[SerializeField]
	private	TMP_InputField	inputBetAmount;			// 배팅 금액 필드
	[SerializeField]
	private	Image			imageBetAmount;			// 배팅 금액 필드 (색상 변경용)
	[SerializeField]
	private	TextMeshProUGUI	textCredits;			// 플레이어 소지 금액
	[SerializeField]
	private	TextMeshProUGUI	textResult;             // 실행 결과 출력
	[SerializeField]
	private Image imageFirstReel;          // 첫 번째 릴 숫자
	[SerializeField]
	private Image imageSecondReel;         // 두 번째 릴 숫자
	[SerializeField]
	private Image imageThirdReel;          // 세 번째 릴 숫자

	public Sprite[] gems;

	private	float	firstSpinDuration = 1f;			// 릴 굴리기 지속 시간 (0.2초)
	private	float	secondSpinDuration = 1.5f;			// 릴 굴리기 지속 시간 (0.2초)
	private	float	thirdSpinDuration = 2f;			// 릴 굴리기 지속 시간 (0.2초)
	private	float	elapsedTime = 0;				// 숫자 선택 지연시간 (릴이 실제 돌아가는 것처럼)
	private	bool	isStartSpin = false;			// 이 값이 true이면 릴 굴리기 시작
	private	int		credits = 10000;				// 플레이어 소지 금액
	
	// 릴의 상태 (이 값이 false이면 릴을 굴리는 중)
	private	bool	isFirstReelSpinned = false;
	private	bool	isSecondReelSpinned = false;
	private	bool	isThirdReelSpinned = false;
	// 릴의 결과 값
	private	int		firstReelResult = 0;
	private	int		secondReelResult = 0;
	private	int		thirdReelResult = 0;		

	private	List<int>	weightedReelPoll;			// 릴이 등장하는 숫자의 확률 조작 리스트
	private	int			zeroProbability = 30;       // 0이 나올 확률 (30 = 30%)


	private void Awake()
	{
		weightedReelPoll = new List<int>(100);
		// zeroProbability 개수인 30개 만큼 0으로 채워준다.
		for ( int i = 0; i < zeroProbability; ++ i )
		{
			weightedReelPoll.Add(0);
		}

		// 0이 나올 확률이 30%이기 때문에 1~9가 나올 확률이 70%
		// 7.7777 = (100 - 30) / 9;
		int remaining_values_prob = (100 - zeroProbability) / 9;

		// 1~9까지 숫자를 7개씩 weightReelPoll 리스트에 채워넣는다.
		for ( int i = 1; i < 10; ++ i )
		{
			for ( int j = 0; j < remaining_values_prob; ++ j )
			{
				weightedReelPoll.Add(i);
			}
		}

		// 해당 코드는 들어간 값 확인용으로 삭제. 0이 30개, 1~9가 7개씩 총 97개의 숫자가 저장
		/*Debug.Log($"weightedReelPoll 개수 : {weightedReelPoll.Count}");
		for ( int i = 0; i < weightedReelPoll.Count; ++ i )
		{
			Debug.Log(weightedReelPoll[i]);
		}*/
		textCredits.text = $"남은 돈 : {credits}";
	}

	void OnEnable()
	{
		credits = CasinoManager.instance.gold;
		textCredits.text = $"남은 돈 : {credits}";
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
		// 필드의 색상과 입력 정보가 바뀌어 있을 수 있으니 초기화
		OnMessage(Color.white, string.Empty);

		// 필드에 값을 입력하지 않았을 때 에러
		if ( inputBetAmount.text.Trim().Equals("") )
		{
			OnMessage(Color.red, "금액을 배팅해주세요.");
			return;
		}

		int parse = int.Parse(inputBetAmount.text);

		if(parse <= 0)
        {
			OnMessage(Color.red, "1원 이상이 필요합니다.");
		}
		else if ( credits - parse >= 0 )
		{
			credits -= parse;
			//CasinoManager.instance.gold -= parse;
			textCredits.text = $"남은 돈 : {credits}";

			isStartSpin = true;
		}
		else
		{
			OnMessage(Color.red, "돈이 충분하지 않습니다.");
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

			textResult.text = "잭팟이 터졌습니다!!!! 100배 당첨!!!";// "YOU WIN!";
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
			textResult.text = "아쉽네요~ 절반을 돌려드리겠습니다.";
		}
		else if ( firstReelResult == thirdReelResult )
		{
			credits += betAmount * 2;
			CasinoManager.instance.EarnGold(betAmount);
			textResult.text = "대칭 보너스!. 2배로 돌려드립니다.";
		}
		else
		{
			CasinoManager.instance.EarnGold(betAmount * -1);
			textResult.text = "꽝입니다!";
		}

		textCredits.text = $"남은 돈 : {credits}";
	}

	private void OnMessage(Color color, string msg)
	{
		imageBetAmount.color = color;
		textResult.text		 = msg;
	}
}

