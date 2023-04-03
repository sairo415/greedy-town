using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverRotation : MonoBehaviour
{
	public void onClickRotate()
	{
		GetComponent<RectTransform>().localRotation = Quaternion.Euler(180, 0, 0);
		StartCoroutine(RotateBack());
	}

	IEnumerator RotateBack()
	{
		yield return new WaitForSeconds(4.2f);
		GetComponent<RectTransform>().localRotation = Quaternion.identity;
	}
}
