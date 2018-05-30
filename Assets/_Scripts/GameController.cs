using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
public class GameController : MonoBehaviour
{
	public static GameController instance = null;
	public GameObject stand;
	public GameObject moving_stand;
	public GameObject diamond;
	public GameObject particleSys;
	public RectTransform hand_sliding;

	public GameObject playerStand;

	public GameObject homePanel;
	public GameObject gameOverPanel;

	public Vector3 standValues;
	public Vector3 playerStandValues;
	public int standCount;
	public float lastZPos;
	public float increZ;
	public List<Transform> allStands;

	public Text scoreText;
	public Text onlyScoreText;
	public int score;
	public Text newRecord;

	public Text diamondScoreText;
	private static int DiamondScore;

	public Text newBestScore;

	public PlayerJump player;
	public bool isGameStart = false;

	private int oldScore;

	public Color[] colorList;
	public Color[] frontColor_List;
	public Color[] rimColor_List;
	public Color[] rightColor_List;

	Vector3 standPosition;
	public GameObject[] diamondParticle;

	public Camera orthoCamera;
	public Material[] playerMaterial;

	public GameObject setting_Panel;
	public GameObject iAP_Panel;
	public AudioSource audioSource;
	bool isAnimation_Running;

	void OnEnable()
	{
		IPController.OnProductPurchasedAtIndex += IPController_OnProductPurchasedAtIndex;
	}

	void OnDisable()
	{
		IPController.OnProductPurchasedAtIndex -= IPController_OnProductPurchasedAtIndex;
	}

	void Awake()
	{
		if (instance == null) 
		{
//			PlayerPrefs.DeleteAll ();
//			PlayerPrefs.SetInt(GameKeys.kprefDiamonds,1000);
			score = 0;
			//			DiamondScore = 0;
			UpdateScore ();
			UpdateDiamondScore ();
			instance = this;
			scoreText.text = "";
			DisplayBestScore ();
			//			gameOverPanel.SetActive (false);
			StandWaves ();
			player = GameObject.FindWithTag ("Player").transform.GetChild(0).GetComponent<PlayerJump>();
			oldScore = PlayerPrefs.GetInt (Utixx.kPrefBestScore, 0);

		}
		else 
		{
			Destroy (this);
		}
	}

	void Start()
	{
		Invoke ("OnAnimationComplete", 1.0f);
		audioSource = GetComponent<AudioSource> ();
		if (!PlayerPrefsX.GetBool (GameKeys.kPrefSoundOn)) 
		{
			audioSource.Play ();
		}
		int i = Random.Range (0, colorList.Length);
		allStands[0].transform.GetComponent<Renderer> ().sharedMaterial.SetColor("_TopColor",colorList[i]);
		allStands [0].transform.GetComponent<Renderer> ().sharedMaterial.SetColor ("_FrontColor", frontColor_List [i]);
		allStands [0].transform.GetComponent<Renderer> ().sharedMaterial.SetColor ("_RimColor", rimColor_List [i]);
		allStands [0].transform.GetComponent<Renderer> ().sharedMaterial.SetColor ("_RightColor", rimColor_List [i]);
		SetMaterial ();
		Hand_Sliding ();
	}

	void Hand_Sliding()
	{
		hand_sliding.DOAnchorPosX (150.0f, 1.0f).SetLoops (-1, LoopType.Yoyo).SetEase (Ease.Linear);
//		hand_sliding.DOLocalMove(new Vector3(-150.0f,150.0f,0.0f),0.5f,false).SetLoops(-1,LoopType.Yoyo);
	}

	void StandWaves()
	{

		Vector3 playerStandPos = Vector3.zero;
		GameObject _playStand = Instantiate (playerStand, playerStandPos, Quaternion.identity) as GameObject;
		allStands.Add (_playStand.transform);

		for (int i = 0; i < standCount; i++)
		{
			AddStand ();
		}
	}

	public void AddStand()
	{
		lastZPos = lastZPos + increZ;
		standPosition = new Vector3 (Random.Range(-standValues.x,standValues.x), standValues.y, standValues.z + lastZPos);
		int move_Range = Random.Range (0, 10);
		if (move_Range >= 5) {
			GameObject _stand = Instantiate (stand, standPosition, Quaternion.identity) as GameObject;
			allStands.Add (_stand.transform);
			int d = Random.Range(0,10);
			if (d <= 3)
			{
				AddDiamond (standPosition);
				//	AddParticles (standPosition);

			}
		} else 
		{
			GameObject _movingStand = Instantiate (moving_stand, standPosition, Quaternion.identity)as GameObject;
			allStands.Add (_movingStand.transform);
		}


//		int d = Random.Range(0,10);
//		if (d <= 3)
//		{
//			AddDiamond (standPosition);
//			//	AddParticles (standPosition);
//
//		}
	}

	void AddDiamond(Vector3 standPosition)
	{
		Quaternion angle = new Quaternion (0, 0, 0, 0);
		Vector3 diamondPosition = new Vector3 (standPosition.x, standPosition.y + 0.39f, standPosition.z);
		GameObject _diamond = Instantiate (diamond, diamondPosition, angle) as GameObject;
		_diamond.transform.rotation = Quaternion.Euler(-90, 0, 0);

		//		Quaternion angle = new Quaternion (0, 0, 0, 0);
		//		Vector3 pariclePosition = new Vector3 (standPosition.x, standPosition.y + 0.39f, standPosition.z);
		//		GameObject _particleSys = Instantiate (particleSys, pariclePosition, angle) as GameObject;
		//		_particleSys.transform.rotation = Quaternion.Euler(-90, 0, 0);
	}



	public void AddScore(int newScoreValue)
	{
		score += newScoreValue;
		PlayerPrefs.SetInt (GameKeys.kprefScore, score);

		int _bestScore = PlayerPrefs.GetInt (Utixx.kPrefBestScore, 0);

		if (_bestScore < score) 
		{
			PlayerPrefs.SetInt (Utixx.kPrefBestScore, score);
		}

		UpdateScore ();
	}

	void UpdateScore()
	{
		scoreText.text = score.ToString();
	}

	void DisplayBestScore()
	{
		newBestScore.text = "BEST : "+PlayerPrefs.GetInt (Utixx.kPrefBestScore);
	}


	public void AddParticles(Vector3 diamond_pos)
	{
		Quaternion angle = new Quaternion (0, 0, 0, 0);
		GameObject _particleSys = Instantiate (particleSys, diamond_pos, angle) as GameObject;
		_particleSys.transform.rotation = Quaternion.Euler(-90, 0, 0);
	}
	public void AddDiamondScore(int newDiamondScore)
	{
		DiamondScore = PlayerPrefs.GetInt (GameKeys.kprefDiamonds);
		DiamondScore += newDiamondScore;
		PlayerPrefs.SetInt (GameKeys.kprefDiamonds, DiamondScore);
		UpdateDiamondScore ();
	}

	void UpdateDiamondScore()
	{
		diamondScoreText.text = PlayerPrefs.GetInt(GameKeys.kprefDiamonds).ToString ();
	}

	public void GamePlay()
	{
		if (!isAnimation_Running)
		{
			return;
		}
		isGameStart = true;
		player.rb.useGravity = true;
		homePanel.SetActive (false);
	}

	void OnAnimationComplete()
	{
		isAnimation_Running = true;
	}


	public void DelayGameOverMenu()
	{
		Invoke ("GameOver", 0.5f);
	}

	public void GameOver()
	{
		int gameOverCount = PlayerPrefs.GetInt (GameKeys.kprefGameOverCount);
		gameOverCount++;
		PlayerPrefs.SetInt (GameKeys.kprefGameOverCount, gameOverCount);

		if (gameOverCount >= 3 && GoogleMobileAdController.Instance.HasInterstitialAD() && !PlayerPrefsX.GetBool(GameKeys.kprefIsAdRemoved)) 
		{
			GoogleMobileAdController.Instance.ShowInterstitial ();
			PlayerPrefs.SetInt (GameKeys.kprefGameOverCount, 0);
		}


		scoreText.gameObject.SetActive (false);
		gameOverPanel.SetActive(true);
		int _bestScore = PlayerPrefs.GetInt (Utixx.kPrefBestScore, 0);
		if (oldScore < score) {
			newRecord.text = "BEST : " + _bestScore;
		} else
		{
			newRecord.text = "BEST : "+_bestScore;
			//			scoreText.text = "";
		}
		onlyScoreText.text = "SCORE : " + score;
		IPController.instance.ReportScoreToLeaderboard (_bestScore);
	}
	public void GameRestart()
	{
		SceneManager.LoadScene (0);	
	}
	public void WatchVideo()
	{
		if (GoogleMobileAdController.Instance.HasRewardVideoAD ()) {
			GoogleMobileAdController.Instance.ShowRewardBasedVideo ((bool isVideoComplete) => {
				if (isVideoComplete) {
					AddDiamondScore (20);
				}
			});
		}

	}

	public void PlayerSelectionScene()
	{
		SceneManager.LoadScene (1);	
	}

	public void SettingsMenu()
	{
		setting_Panel.SetActive (true);
	}

	public void Close_Panel()
	{
		setting_Panel.SetActive (false);
	}

	public void Open_IAP_Menu()
	{
		iAP_Panel.SetActive (true);
	}

	public void Close_IAP_Panel()
	{
		iAP_Panel.SetActive (false);
	}

	public void SetMaterial()
	{
		int index = PlayerPrefs.GetInt (IPScrollView.INDEX);
		//		SetMaterial (index);
		Debug.Log("Index = "+index);
		player.GetComponent<Renderer> ().material = playerMaterial [index];

	}

	public void BuyItemAtIndex(int _iIndex)
	{
		IPController.instance.BuyItemAtIndex (_iIndex);
	}

	public void RestorePurchase()
	{
		IPController.instance.RestoreItems ();
	}

	void IPController_OnProductPurchasedAtIndex (int indexPurchasedProduct, string purchaseType)
	{
		if (purchaseType == IPController.PURCHASED) {
			if (indexPurchasedProduct == 0) {
				PlayerPrefsX.SetBool (GameKeys.kprefIsAdRemoved, true);
				GoogleMobileAdController.Instance.DestoryBanner ();
				// RemoveAds
			} else if (indexPurchasedProduct == 1) {
				AddDiamondScore (100);
			} else if (indexPurchasedProduct == 2) {
				AddDiamondScore (500);
			} else if (indexPurchasedProduct == 3) {
				AddDiamondScore (1000);
			} else if (indexPurchasedProduct == 4) {
				AddDiamondScore (2000);
			}
		} else if(purchaseType== IPController.RESTORED)
		{
			PlayerPrefsX.SetBool (GameKeys.kprefIsAdRemoved, true);
			GoogleMobileAdController.Instance.DestoryBanner ();
		}
	}

	public void LeaderBoard()
	{
		IPController.instance.ShowLeaderBoard ();
	}

	public void Share()
	{
		int _shareBest = PlayerPrefs.GetInt (Utixx.kPrefBestScore);
		string msg="My Best Score is "+_shareBest +" Try to beat me!!";
		IPController.instance.SharingScreenshotWithMessage (msg);
	}

	public void RateUs()
	{
		IPController.instance.OpenRateUsURL ();
	}

	public void Sound()
	{
		SoundOnOff.instance.changeSound ();
	}
}
