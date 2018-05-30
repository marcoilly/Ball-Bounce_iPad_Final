using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoxelBusters.NativePlugins;
using VoxelBusters.Utility;

public class IPController : MonoBehaviour
{

	public static string PURCHASED = "purchased";
	public static string RESTORED = "restored";
	public const string feedbackID = "indeep.parth@gmail.com";

	public delegate void IPControllerDelegateVoid ();

	public delegate void IPControllerDelegateInt (int indexPurchasedProduct, string purchaseType);

	public delegate void IPControllerDelegateString (string index);

	public static event IPControllerDelegateVoid OnSharingFinished;
	//	public static event IPControllerDelegateVoid OnAlertViewClosed;
	public static event IPControllerDelegateVoid OnReportScoreCompletion;
	public static event IPControllerDelegateVoid OnReportAchievementCompletion;
	public static event IPControllerDelegateInt OnProductPurchasedAtIndex;
	public static event IPControllerDelegateString OnAlertViewClosedWithButtonName;

	public static IPController instance = null;
	public static bool isGameCenterLocalUserAuthenticated = false;
	private string[] sLeaderboardID;
	private string[] sAchievementID;


	void OnEnable ()
	{
		#if USES_BILLING

		Billing.DidFinishProductPurchaseEvent += OnDidFinishProductPurchase;
		Billing.DidFinishRestoringPurchasesEvent	+= OnDidFinishRestoringPurchases;
		Billing.DidFinishRequestForBillingProductsEvent	+= OnDidFinishRequestForBillingProducts;
		Billing.DidFinishProductsRequestEvent+=OnDidFinishProductsRequestEvent;


		#endif
	}

	void OnDisable ()
	{
		#if USES_BILLING
		Billing.DidFinishProductPurchaseEvent -= OnDidFinishProductPurchase;
		Billing.DidFinishRestoringPurchasesEvent	-= OnDidFinishRestoringPurchases;
		Billing.DidFinishRequestForBillingProductsEvent	-= OnDidFinishRequestForBillingProducts;
		Billing.DidFinishProductsRequestEvent-=OnDidFinishProductsRequestEvent;

		#endif
	}

	void Awake ()
	{
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}
	}

	void Start ()
	{
		#if USES_GAME_SERVICES
		if (NPSettings.Application.SupportedFeatures.UsesGameServices) {
			SetsLeaderboardID ();
			SetsAchievementID ();
			if (InternetStatus ()) {
				LocalUserAuthentication ();
			}	
		}
		#endif

		#if USES_BILLING
		if (NPSettings.Application.SupportedFeatures.UsesBilling) {
			NPBinding.Billing.RequestForBillingProducts ();
		}
		#endif

	}




	#region Ask For Review

	public void OpenRateUsURL ()
	{
		AndroidDebug.debug ("IP OpenRateUsURL");
		NPBinding.Utility.OpenStoreLink (NPSettings.Application.StoreIdentifier);
	}

	public void AskForReview ()
	{
		if (NPSettings.Utility.RateMyApp.IsEnabled) {
			NPBinding.Utility.RateMyApp.AskForReview ();
		}
	}

	#endregion

	#region Social Sharing
	#if USES_SHARING

	public void SharingScreenshotWithMessage (string mesaage)
	{
		ShareSheet shareSheet = new ShareSheet ();
		shareSheet.Text = mesaage;
		shareSheet.URL = StoreURL ();
		shareSheet.AttachScreenShot ();
		NPBinding.Sharing.ShowView (shareSheet, FinishedSharing);
	}

	public void SharingImageWithMessage (Texture2D image, string mesaage)
	{
		ShareSheet shareSheet = new ShareSheet ();
		shareSheet.Text = mesaage;
		shareSheet.URL = StoreURL ();
		shareSheet.AttachImage (image);
		NPBinding.Sharing.ShowView (shareSheet, FinishedSharing);
	}

	// CallBack
	private void FinishedSharing (eShareResult _result)
	{
		Debug.Log ("Finished sharing");
		Debug.Log ("Share Result = " + _result);
		if (OnSharingFinished != null)
			OnSharingFinished ();
	}

	#endif

	private string StoreURL ()
	{
		#if UNITY_ANDROID

		string url = "https://play.google.com/store/apps/details?id=" + NPSettings.Application.StoreIdentifier;
		return url;

		#elif UNITY_IOS
		return  "https://itunes.apple.com/in/app/id" + NPSettings.Application.StoreIdentifier +"?mt=8";
	
		#endif
	}

	#endregion

	#region AlertView

	public void ShowAlertview (string sTitle, string sMessage)
	{
		NPBinding.UI.ShowAlertDialogWithSingleButton (sTitle, sMessage, "OK", AlertViewClosed);
	}

	public void ShowAlertviewWithYesNo (string sTitle, string sMessage)
	{
		string[] strButtons = new string[2]{ "No", "Yes" }; 
		NPBinding.UI.ShowAlertDialogWithMultipleButtons (sTitle, sMessage, strButtons, AlertViewClosed);
	}

	public void SHowAlertviewWith (string sTitle, string sMessage, string btn1, string btn2, string btn3)
	{
		string[] strButtons = new string[3]{ btn1, btn2, btn3 }; 
		NPBinding.UI.ShowAlertDialogWithMultipleButtons (sTitle, sMessage, strButtons, AlertViewClosed);
	}


	// CallBack
	private void AlertViewClosed (string _buttonPressed)
	{
		if (OnAlertViewClosedWithButtonName != null)
			OnAlertViewClosedWithButtonName (_buttonPressed);

		Debug.Log ("Alert dialog closed.");
		Debug.Log ("Clicked button name is " + _buttonPressed + ".");
	}

	#endregion

	#region Leaderboard / Game Center
	#if USES_GAME_SERVICES
	public void ShowLeaderBoard ()
	{
		ShowLeaderBoardAtIndex (0);
	}

	public void ShowLeaderBoardAtIndex (int leaderboardIndex)
	{
		if (NPBinding.GameServices.LocalUser.IsAuthenticated) {
			NPBinding.GameServices.ShowLeaderboardUIWithID (NPSettings.GameServicesSettings.LeaderboardMetadataCollection [leaderboardIndex].GetCurrentPlatformID (), eLeaderboardTimeScope.ALL_TIME, null);
		} else {
			NPBinding.GameServices.LocalUser.Authenticate ((bool _success, string _error) => {

				if (_success) {
					Debug.Log ("Sign-In Successfully");
					Debug.Log ("Local User Details : " + NPBinding.GameServices.LocalUser.ToString ());
					AndroidDebug.debug ("Sign-In Successfully = Local User Details : " + NPBinding.GameServices.LocalUser.ToString ());
					isGameCenterLocalUserAuthenticated = true;
					LoadAchievements ();
					NPBinding.GameServices.ShowLeaderboardUIWithID (sLeaderboardID [leaderboardIndex], eLeaderboardTimeScope.ALL_TIME, null);
				} else {
					AndroidDebug.debug ("Sign-In Failed with error " + _error);
					Debug.Log ("Sign-In Failed with error " + _error);
				}
			});
		}
	}


	public void ReportScoreToLeaderboard (int score)
	{
		ReportScoreToLeaderboard (score, 0);
	}

	public void ReportScoreToLeaderboard (long score, int leaderboradIndex)
	{
		if (!Application.isEditor) {
			if (NPBinding.GameServices.LocalUser.IsAuthenticated) {
				NPBinding.GameServices.ReportScoreWithID (sLeaderboardID [leaderboradIndex], score, ReportScoreCompletion);
			}
		}
	}

	private void LocalUserAuthentication ()
	{
		NPBinding.GameServices.LocalUser.Authenticate (AuthenticateCompletion);
	}

	private static bool InternetStatus ()
	{
		if (Application.internetReachability != NetworkReachability.NotReachable) {
			return true;
		} else {
			return false;
		}
	}

	// CallBack
	private void AuthenticateCompletion (bool _success, string _error)
	{
		if (_success) {
			isGameCenterLocalUserAuthenticated = true;
			LoadAchievements ();
			Debug.Log ("Sign-In Successfully");
			Debug.Log ("Local User Details : " + NPBinding.GameServices.LocalUser.ToString ());
			AndroidDebug.debug ("Sign-In Successfully");

		} else {
			Debug.Log ("Sign-In Failed with error " + _error);
			AndroidDebug.debug ("Sign-In Failed with error " + _error);

		}
	}
		
	// CallBack
	private void ReportScoreCompletion (bool _success, string _error)
	{
		if (_success) {
			Debug.Log ("ReportScoreCompletion");
			if (OnReportScoreCompletion != null)
				OnReportScoreCompletion ();
		} else {
			Debug.Log ("Sign-In Failed with error " + _error);
		}
	}

	void SetsLeaderboardID ()
	{
		sLeaderboardID = new string[NPSettings.GameServicesSettings.LeaderboardMetadataCollection.Length];
		for (int i = 0; i < NPSettings.GameServicesSettings.LeaderboardMetadataCollection.Length; i++) {
			sLeaderboardID [i] = NPSettings.GameServicesSettings.LeaderboardMetadataCollection [i].GetCurrentPlatformID ();
		}
	}
	#endif
	#endregion

	#region Achievements / Game Center
	#if USES_GAME_SERVICES
	public void ShowAchievementUI ()
	{
		if (NPBinding.GameServices.LocalUser.IsAuthenticated) {
			NPBinding.GameServices.ShowAchievementsUI (null);
			NPBinding.GameServices.LocalUser.Authenticate ((bool _success, string _error) => {

				if (_success) {
					Debug.Log ("Sign-In Successfully");
					Debug.Log ("Local User Details : " + NPBinding.GameServices.LocalUser.ToString ());
					isGameCenterLocalUserAuthenticated = true;
					LoadAchievements ();
					NPBinding.GameServices.ShowAchievementsUI (null);
				} else {
					Debug.Log ("Sign-In Failed with error " + _error);
				}
			});
		}
	}

	public void ReportAchievementToLeaderboard (int achievementIndex)
	{
		ReportAchievementToLeaderboard (achievementIndex, 100);
	}

	public void ReportAchievementToLeaderboard (int achievementIndex, int progresssPoint)
	{
		if (NPBinding.GameServices.LocalUser.IsAuthenticated) {
			PlayerPrefsX.SetBool (sAchievementID [achievementIndex], true);
			NPBinding.GameServices.ReportProgressWithID (sAchievementID [achievementIndex], 100.0f, ReportAchievementCompletion);

		}
	}

	private void LoadAchievements ()
	{
		NPBinding.GameServices.LoadAchievementDescriptions (LoadAchievementDescriptionsCompletion);
		NPBinding.GameServices.LoadAchievements (LoadAchievementsCompletion);
	}

	public bool CheckIsAchievementUnlockedAtIndex (int index)
	{
		bool result = false;
		result = PlayerPrefsX.GetBool (sAchievementID [index]);
		return result;
	}

	// CallBack
	private void ReportAchievementCompletion (bool _success, string _error)
	{
		if (_success) {
			Debug.Log ("ReportAchievementCompletion -" + _success);
			if (OnReportAchievementCompletion != null)
				OnReportAchievementCompletion ();
		} else {
			Debug.Log ("ReportAchievementCompletion error - " + _error);
		}
	}



	private void LoadAchievementDescriptionsCompletion (AchievementDescription[] _descriptions, string _error)
	{
		if (_descriptions == null) {
			Debug.Log ("Couldn't load achievement description list with error = " + _error);
			return;
		}

		int _descriptionCount	= _descriptions.Length;
		Debug.Log (string.Format ("Successfully loaded achievement description list. Count={0}.", _descriptionCount));

		for (int _iter = 0; _iter < _descriptionCount; _iter++) {
			Debug.Log (string.Format ("[Index {0}]: {1}", _iter, _descriptions [_iter]));
		}
	}

	private void LoadAchievementsCompletion (Achievement[] achievements, string error)
	{
		Debug.Log ("LoadAchievementsCompletion error - " + error);


		if (error == null) {

			foreach (Achievement achievement in achievements) {
				PlayerPrefsX.SetBool (achievement.Identifier, achievement.Completed);
			}
		}


	}

	void SetsAchievementID ()
	{
		sAchievementID = new string[NPSettings.GameServicesSettings.AchievementMetadataCollection.Length];
		for (int i = 0; i < NPSettings.GameServicesSettings.AchievementMetadataCollection.Length; i++) {
			sAchievementID [i] = NPSettings.GameServicesSettings.AchievementMetadataCollection [i].GetCurrentPlatformID ();
		}
	}
	#endif
	#endregion


	#region In-App Purchase
	#if USES_BILLING

	public void BuyItemAtIndex (int productIndex)
	{
		BuyItemFromStore (NPSettings.Billing.Products [productIndex]);
	}

	public void RestoreItems ()
	{
		NPBinding.Billing.RestorePurchases ();
	}

	public bool IsProductPurchasedItemAtIndex (int productIndex)
	{
		return NPBinding.Billing.IsProductPurchased (NPSettings.Billing.Products [productIndex]);
	}

	private void BuyItemFromStore (BillingProduct _product)
	{
		AndroidDebug.debug ("BuyItemFromStore -" + _product.ProductIdentifier);
		NPBinding.Billing.BuyProduct (_product);
	}

	private void OnDidFinishProductPurchase (BillingTransaction _transaction)
	{
		if (_transaction.TransactionState == eBillingTransactionState.PURCHASED) {
			AndroidDebug.debug ("OnDidFinishProductPurchase -  Purchased - " + _transaction.ProductIdentifier);
			CallDelegateForPurchasedProductIdentifier (_transaction.ProductIdentifier, PURCHASED);

		} else if (_transaction.TransactionState == eBillingTransactionState.FAILED) {
			AndroidDebug.debug ("OnDidFinishProductPurchase -  FAILED - " + _transaction.ProductIdentifier);
			ShowAlertview ("Sorry!!!", "Something wrong.");
		}
	}

	private void OnDidFinishRestoringPurchases (BillingTransaction[] _transactions, string _error)
	{
		if (_error == null) {
			foreach (BillingTransaction _eachTransaction in _transactions) {
				AndroidDebug.debug ("OnDidFinishRestoringPurchases -  Purchased - " + _eachTransaction.ProductIdentifier);
				CallDelegateForPurchasedProductIdentifier (_eachTransaction.ProductIdentifier, RESTORED);
			}
			ShowAlertview ("Congratulations!", "You have suceessfully Restored");
		} else {
			AndroidDebug.debug ("OnDidFinishRestoringPurchases -  Purchased - " + _transactions [0].ProductIdentifier);
			ShowAlertview ("Sorry!!!", _error);	
		}
	}


	private void CallDelegateForPurchasedProductIdentifier (string productIdentifier, string purchaseType)
	{
		for (int i = 0; i < NPSettings.Billing.Products.Length; i++) {
			if (NPSettings.Billing.Products [i].ProductIdentifier == productIdentifier) {
				if (OnProductPurchasedAtIndex != null)
					OnProductPurchasedAtIndex (i, purchaseType);
			}
		}
	}


	private void OnDidFinishProductsRequestEvent (BillingProduct[] _products, string _error)
	{
		 		if (_error == null)
		 		{
		 			foreach (BillingProduct _currentProduct in _products)
		 			{
		 				// Insert your code, eg: populating app's store screen
		 			}
		 		}
		 		else
		 		{
		 			// Something went wrong
		 			// Show an alert message informing user that store initialisation failed
		 		}
	}

	private void OnDidFinishRequestForBillingProducts (BillingProduct[] _products, string _error)
	{
		 		if (_error == null)
		 		{
		 			foreach (BillingProduct _currentProduct in _products)
		 			{
		 				// Insert your code, eg: populating app's store screen
		 			}
		 		}
		 		else
		 		{
		 			// Something went wrong
		 			// Show an alert message informing user that store initialisation failed
		 		}
	 }
	#endif

	#endregion

	public void SendFeedbackMail ()
	{
		if (NPBinding.Sharing.IsMailServiceAvailable ()) {
			string m_mailSubject = "Feedback";
			string m_plainMailBody = "\n"+"\n"+"\n"+"\n"+"DO NOT EDIT BELOW INFO"+"\n"+"Devide Name - "+ SystemInfo.deviceName+"\n"+
									 "Devide Model - "+ SystemInfo.deviceModel+"\n"+
									 "Devide Type - "+ SystemInfo.deviceType+"\n"+
									 "OS version - "+ SystemInfo.operatingSystem+"\n";
			string[] m_mailToRecipients = new string[] { feedbackID };
			MailShareComposer _composer = new MailShareComposer ();
			_composer.Subject = m_mailSubject;
			_composer.Body = m_plainMailBody;
			_composer.ToRecipients = m_mailToRecipients;
//_composer.CCRecipients = m_mailCCRecipients;
//_composer.BCCRecipients = m_mailBCCRecipients;
// Add below line if you want to attach a file, for ex : image. Else, ignore.
//			_composer.AddAttachmentAtPath (Application.persistentDataPath + "test.png", MIMEType.kPNG);
// Show share view
			NPBinding.Sharing.ShowView (_composer, FinishedSharing);
		}
	}
}
