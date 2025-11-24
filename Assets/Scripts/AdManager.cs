using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdManager : MonoBehaviour
{
    public AdsInitializer adsInitializer;
    public InterstitialAd interstitialAd;
    [SerializeField] private bool turnOffInterstitialAd = false;

    public RewardedAds rewardedAds;
    [SerializeField] private bool turnOffRewardedAds = false;

    public BannerAd bannerAd;
    [SerializeField] private bool turnOffBannerAd = false;

    public static AdManager Instance { get; private set; }

    private bool firstSceneLoad = false;
    private bool adShownThisScene = false;

    private void Awake()
    {
        // Auto-assign components if not set
        if (adsInitializer == null)
            adsInitializer = FindFirstObjectByType<AdsInitializer>();

        if (interstitialAd == null)
            interstitialAd = GetComponent<InterstitialAd>();

        if (rewardedAds == null)
            rewardedAds = GetComponent<RewardedAds>();

        if (bannerAd == null)
            bannerAd = GetComponent<BannerAd>();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (adsInitializer != null)
        {
            adsInitializer.OnAdsInitialized -= HandleAdsInitialized;
            adsInitializer.OnAdsInitialized += HandleAdsInitialized;
        }
    }

    private void HandleAdsInitialized()
    {
        if (!turnOffInterstitialAd && interstitialAd != null)
            interstitialAd.LoadAd();

        if (!turnOffRewardedAds && rewardedAds != null)
            rewardedAds.LoadAd();

        if (!turnOffBannerAd && bannerAd != null)
            bannerAd.LoadBanner();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        adShownThisScene = false;
        StartCoroutine(SetupButtonsNextFrame());

        if (!firstSceneLoad)
        {
            firstSceneLoad = true;
            Debug.Log("First scene load — skipping auto ad!");
            return;
        }

        if (!turnOffInterstitialAd && interstitialAd != null && interstitialAd.isReady && !adShownThisScene)
        {
            interstitialAd.ShowAd();
            adShownThisScene = true;
        }
    }

    private IEnumerator SetupButtonsNextFrame()
    {
        yield return null;

        // --- Interstitial button ---
        Button interstitialButton = GameObject.FindGameObjectWithTag("Interstitial")?.GetComponent<Button>();
        if (interstitialAd != null && interstitialButton != null)
            interstitialAd.SetButton(interstitialButton);

        // --- Rewarded button ---
        Button rewardedButton = GameObject.FindGameObjectWithTag("RewardedButton")?.GetComponent<Button>();
        if (rewardedAds != null && rewardedButton != null)
            rewardedAds.SetButton(rewardedButton);
    }
}