using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdManager : MonoBehaviour
{
    public AdsInitializer adsInitializer;
    public InterstitialAd interstitialAd;

    [SerializeField] bool turnOffInterstitialAd = false;
    [SerializeField] bool showAdOnSceneChange = true;
    private bool firstAdShown = false;
    private bool isFirstSceneLoad = true;

    public RewardedAds rewardedAds;
    [SerializeField] bool turnOffRewardedAds = false;

    public BannerAd bannerAd;
    [SerializeField] bool turnOffBannerAd = false;

    public static AdManager Instance { get; private set; }

    public void Awake()
    {
        if (adsInitializer == null)
        {
            adsInitializer = FindFirstObjectByType<AdsInitializer>();
        }

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        adsInitializer.OnAdsInitialized += HandleAdsInitialized;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void HandleAdsInitialized()
    {
        if (!turnOffInterstitialAd)
        {
            interstitialAd.OnInterstitialAdReady += HandleInterstitialReady;
            interstitialAd.LoadAd();
        }

        if (!turnOffRewardedAds)
        {
            rewardedAds.LoadAd();
        }

        if (!turnOffBannerAd)
        {
            bannerAd.LoadBanner();
        }
    }

    private void HandleInterstitialReady()
    {
        if (!firstAdShown)
        {
            Debug.Log("First interstitial ad is ready");
            firstAdShown = true;
        }
        else
        {
            Debug.Log("Next interstitial ad is ready");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (interstitialAd == null)
            interstitialAd = FindFirstObjectByType<InterstitialAd>();

        // Find and set up the interstitial button if it exists
        try
        {
            GameObject buttonObject = GameObject.FindGameObjectWithTag("InterstitialButton");
            if (buttonObject != null)
            {
                Button interstitialButton = buttonObject.GetComponent<Button>();
                if (interstitialAd != null && interstitialButton != null)
                {
                    interstitialAd.SetButton(interstitialButton);
                }
            }
        }
        catch (UnityException)
        {
            Debug.Log("No InterstitialButton found in scene - manual button control unavailable");
        }

        // Skip showing ad on the very first scene load
        if (isFirstSceneLoad)
        {
            isFirstSceneLoad = false;
            Debug.Log("First scene loaded - skipping ad");
            return;
        }

        Debug.Log("Scene Loaded!");
        HandleAdsInitialized();

        if (rewardedAds == null)
            rewardedAds = FindFirstObjectByType<RewardedAds>();

        // Fixed: Handle the array returned by FindGameObjectsWithTag
        try
        {
            GameObject[] rewardedButtonObjects = GameObject.FindGameObjectsWithTag("RewardedButton");
            if (rewardedButtonObjects.Length > 0)
            {
                Button rewardedAdButton = rewardedButtonObjects[0].GetComponent<Button>();
                if (rewardedAds != null && rewardedAdButton != null)
                {
                    rewardedAds.SetButton(rewardedAdButton);
                }
            }
        }
        catch (UnityException)
        {
            Debug.Log("No RewardedButton found in scene - rewarded ad button unavailable");
        }

        // Show interstitial ad on scene change
        if (showAdOnSceneChange && !turnOffInterstitialAd && interstitialAd != null)
        {
            Debug.Log("Scene changed - attempting to show interstitial ad");
            interstitialAd.ShowAd();
        }
    }
}