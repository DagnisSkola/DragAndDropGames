using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class WinConditionScript : MonoBehaviour
{
    [Header("References")]
    public ObjectScript objectScript;
    public SimpleTimer timerScript;
    public GameObject winScreen;
    public TextMeshProUGUI finalTimeText;
    public TextMeshProUGUI carsPlacedText;
    public AudioSource effects;
    public AudioClip winSound;
    public CameraScript cameraScript;

    [Header("Star System")]
    public GameObject[] stars; // Assign 3 star GameObjects
    public Sprite starFilled;  // Filled star sprite
    public Sprite starEmpty;   // Empty star sprite
    public int threeStarThreshold = 12; // All cars for 3 stars
    public int twoStarThreshold = 9;    // 9+ cars for 2 stars
    public int oneStarThreshold = 6;    // 6+ cars for 1 star

    [Header("Settings")]
    public int totalCarsToPlace = 12;
    public float cameraCenterDuration = 0.5f; // Duration to center camera

    private int carsPlacedSuccessfully = 0;
    private int carsDestroyed = 0;
    private bool gameWon = false;
    private Camera mainCamera;
    private Vector3 originalCameraPosition;

    void Start()
    {
        if (winScreen != null)
        {
            winScreen.SetActive(false);
        }

        // Find camera script if not assigned
        if (cameraScript == null)
        {
            cameraScript = Object.FindFirstObjectByType<CameraScript>();
        }

        // Get main camera
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.transform.position;
        }

        // Initialize stars as empty
        if (stars != null && starEmpty != null)
        {
            foreach (GameObject star in stars)
            {
                if (star != null)
                {
                    Image img = star.GetComponent<Image>();
                    if (img != null) img.sprite = starEmpty;
                }
            }
        }
    }

    // Call this method when a car is successfully placed in correct position
    public void CarPlacedSuccessfully()
    {
        if (gameWon) return;

        carsPlacedSuccessfully++;
        Debug.Log($"Cars placed successfully: {carsPlacedSuccessfully}/{totalCarsToPlace}");

        CheckWinCondition();
    }

    // NEW: Call this when a car is destroyed by an obstacle
    public void CarDestroyed()
    {
        if (gameWon) return;

        carsDestroyed++;
        Debug.Log($"Car destroyed! Total destroyed: {carsDestroyed}");

        CheckWinCondition();
    }

    void CheckWinCondition()
    {
        // Calculate how many cars are still available to place
        int remainingCars = totalCarsToPlace - carsPlacedSuccessfully - carsDestroyed;

        Debug.Log($"Placed: {carsPlacedSuccessfully}, Destroyed: {carsDestroyed}, Remaining: {remainingCars}");

        // If all cars are either placed or destroyed, end the game
        if (remainingCars <= 0)
        {
            TriggerGameEnd();
        }
    }

    void TriggerGameEnd()
    {
        if (gameWon) return;

        gameWon = true;
        Debug.Log($"Game Over! Cars placed: {carsPlacedSuccessfully}/{totalCarsToPlace}, Destroyed: {carsDestroyed}");

        // Stop the timer
        if (timerScript != null)
        {
            timerScript.timerIsRunning = false;
        }

        // Disable camera movement
        if (cameraScript != null)
        {
            cameraScript.enabled = false;
            Debug.Log("Camera controls disabled");
        }

        // Play win sound
        if (effects != null && winSound != null)
        {
            effects.PlayOneShot(winSound);
        }

        // Center camera and show win screen
        StartCoroutine(CenterCameraAndShowWinScreen());
    }

    IEnumerator CenterCameraAndShowWinScreen()
    {
        if (mainCamera != null)
        {
            Vector3 startPos = mainCamera.transform.position;
            Vector3 centerPos = new Vector3(0f, 0f, startPos.z); // Center at (0, 0)

            float elapsed = 0f;

            // Smoothly move camera to center
            while (elapsed < cameraCenterDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / cameraCenterDuration;

                // Use smooth easing
                t = Mathf.SmoothStep(0f, 1f, t);

                mainCamera.transform.position = Vector3.Lerp(startPos, centerPos, t);
                yield return null;
            }

            mainCamera.transform.position = centerPos;
            Debug.Log("Camera centered at (0, 0)");
        }

        // Small delay before showing win screen
        yield return new WaitForSeconds(0.2f);

        // Show win screen
        if (winScreen != null)
        {
            winScreen.SetActive(true);

            // Display final time
            if (finalTimeText != null && timerScript != null)
            {
                int minutes = Mathf.FloorToInt(timerScript.timeElapsed / 60);
                int seconds = Mathf.FloorToInt(timerScript.timeElapsed % 60);
                finalTimeText.text = $"Time: {minutes:00}:{seconds:00}";
            }

            // Display cars placed
            if (carsPlacedText != null)
            {
                carsPlacedText.text = $"Cars Placed: {carsPlacedSuccessfully}/{totalCarsToPlace}";
            }

            // Update stars
            UpdateStars();
        }
    }

    void UpdateStars()
    {
        if (stars == null || stars.Length == 0) return;

        int starsToFill = GetStarCount();

        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] == null) continue;

            Image img = stars[i].GetComponent<Image>();
            if (img != null)
            {
                if (i < starsToFill && starFilled != null)
                {
                    img.sprite = starFilled;
                    // Optional: Add animation or scale effect
                    StartCoroutine(AnimateStar(stars[i].transform, i * 0.2f));
                }
                else if (starEmpty != null)
                {
                    img.sprite = starEmpty;
                }
            }
        }
    }

    int GetStarCount()
    {
        if (carsPlacedSuccessfully >= threeStarThreshold)
            return 3;
        else if (carsPlacedSuccessfully >= twoStarThreshold)
            return 2;
        else if (carsPlacedSuccessfully >= oneStarThreshold)
            return 1;
        else
            return 0;
    }

    System.Collections.IEnumerator AnimateStar(Transform star, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 originalScale = star.localScale;
        star.localScale = Vector3.zero;

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Bounce effect
            float scale = Mathf.Sin(t * Mathf.PI);
            star.localScale = originalScale * Mathf.Lerp(0f, 1.2f, scale);
            yield return null;
        }

        // Return to normal size
        elapsed = 0f;
        duration = 0.2f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            star.localScale = Vector3.Lerp(originalScale * 1.2f, originalScale, elapsed / duration);
            yield return null;
        }

        star.localScale = originalScale;
    }

    // Call this from the Main Menu button
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Change to your main menu scene name
    }

    // Call this from a Restart button if you want to add one
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Public method to manually trigger game end (for testing or time limit)
    public void ForceGameEnd()
    {
        TriggerGameEnd();
    }
}