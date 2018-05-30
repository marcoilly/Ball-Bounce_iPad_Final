using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo : MonoBehaviour
{

    // Use this for initialization

    public void DestroyBanner()
    {
        GoogleMobileAdController.Instance.DestoryBanner();
    }
    public void LoadInterstaitial()
    {
        GoogleMobileAdController.Instance.ShowInterstitial();
    }
    public void LoadRewardVideo()
    {
        if (GoogleMobileAdController.Instance.HasRewardVideoAD())
        {
            GoogleMobileAdController.Instance.ShowRewardBasedVideo((bool isCompleted) =>
            {
                Debug.Log("GoogleMobileAdController.ShowRewardBasedVideo - " + isCompleted);
            });
        }

    }

}
