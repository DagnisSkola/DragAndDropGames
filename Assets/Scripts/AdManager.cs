using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdManager : MonoBehaviour
{
    public AdsInitializer adsInitializer;
    public InterstitialAd interstitialAd;
    [SerializeField] bool turnOffInterstitialAd = false;
    [SerializeField] bool showAdOnSceneChange = true; // New option to control scene change ads
    private bool firstAdShown = false;
    private bool isFirstSceneLoad = true;

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
            // Tag doesn't exist or no object with that tag in this scene - that's okay
            Debug.Log("No InterstitialButton found in scene - manual button control unavailable");
        }

        // Skip showing ad on the very first scene load
        if (isFirstSceneLoad)
        {
            isFirstSceneLoad = false;
            Debug.Log("First scene loaded - skipping ad");
            return;
        }

        // Show interstitial ad on scene change
        if (showAdOnSceneChange && !turnOffInterstitialAd && interstitialAd != null)
        {
            Debug.Log("Scene changed - attempting to show interstitial ad");
            interstitialAd.ShowAd();
        }
    }
}