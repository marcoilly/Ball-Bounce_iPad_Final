using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerSelectionController : MonoBehaviour {

	public Text diamondScoreText;

		void OnEnable ()
		{
			IPScrollView.OnScrollButtonClickAtIndex += UnlockCharacter;
		}
	
		void OnDisable ()
		{
			IPScrollView.OnScrollButtonClickAtIndex -= UnlockCharacter;
		}

	void Start()
	{
		UpdateDiamondScore ();
		LabelCheck ();
	}

	void LabelCheck()
	{
		int[] ballP = PlayerPrefsX.GetIntArray (GameKeys.kprefBallPrize);
		bool[] ballU = PlayerPrefsX.GetBoolArray (GameKeys.kprefBallPrizeUnlock);
		for (int i = 0; i < IPScrollView.Instance.scrollableObjects.Count; i++) 
		{
			if (ballU [i]) 
			{
				IPScrollView.Instance.scrollableObjects [i].GetChild (0).GetChild (0).gameObject.SetActive (false);
			}
			else
			{
				IPScrollView.Instance.scrollableObjects [i].GetChild (0).GetChild (0).GetComponent<TextMesh> ().text = ballP [i].ToString ();
			}
		}
	}

	public void UnlockCharacter(int _index)
	{
		bool[] ballU = PlayerPrefsX.GetBoolArray (GameKeys.kprefBallPrizeUnlock);

		int totalDiamonds = PlayerPrefs.GetInt (GameKeys.kprefDiamonds);
		int[] ball_PrizeC = PlayerPrefsX.GetIntArray (GameKeys.kprefBallPrize);
		if (ballU [_index]) {
			PlayerPrefs.SetInt ("INDEX", _index);
			return;
		}
		else {
			if (ball_PrizeC [_index] <= totalDiamonds)
			{
				int new_Diamond = totalDiamonds - ball_PrizeC [_index];
//				GameController.instance.diamondScoreText.text = new_Diamond.ToString ();
				PlayerPrefs.SetInt (GameKeys.kprefDiamonds, new_Diamond);
				UpdateDiamondScore ();
				IPScrollView.Instance.scrollableObjects [_index].GetChild (0).GetChild (0).gameObject.SetActive (false);
				ballU [_index] = true;
				PlayerPrefsX.SetBoolArray (GameKeys.kprefBallPrizeUnlock, ballU);
				PlayerPrefs.SetInt ("INDEX", _index);
			}
		}
	}

	void UpdateDiamondScore()
	{
		
		diamondScoreText.text = PlayerPrefs.GetInt(GameKeys.kprefDiamonds).ToString ();
	}

	public void GameScene()
	{
		SceneManager.LoadScene (0);	
	}
}
