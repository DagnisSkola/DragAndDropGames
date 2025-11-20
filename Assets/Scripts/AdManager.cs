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
        if (adsInitializer == null)
            adsInitializer = FindFirstObjectByType<AdsInitializer>();

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
        adShownThisScene = false; // reset for new scene

        // Delay button setup until all scene objects are active
        StartCoroutine(SetupButtonsNextFrame());

        if (!firstSceneLoad)
        {
            firstSceneLoad = true;
            Debug.Log("First scene load — skipping auto ad!");
            return;
        }

        // --- Auto-show interstitial once per scene ---
        if (!turnOffInterstitialAd && interstitialAd != null && interstitialAd.isReady && !adShownThisScene)
        {
            interstitialAd.ShowAd();
            adShownThisScene = true;
        }
    }

    private IEnumerator SetupButtonsNextFrame()
    {
        yield return null; // wait one frame for objects to be active

        // --- Interstitial button ---
        Button interstitialButton = GameObject.FindGameObjectWithTag("Interstitial")?.GetComponent<Button>();
        if (interstitialAd != null && interstitialButton != null)
            interstitialAd.SetButton(interstitialButton);

        // --- Rewarded button ---
        Button rewardedButton = GameObject.FindGameObjectWithTag("RewardedButton")?.GetComponent<Button>();
        if (rewardedAds != null && rewardedButton != null)
            rewardedAds.SetButton(rewardedButton);

        // --- Banner button ---
        Button bannerButton = GameObject.FindGameObjectWithTag("Banner")?.GetComponent<Button>();
        if (bannerAd != null && bannerButton != null)
            bannerAd.SetButton(bannerButton);
    }
}
