using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ObstaclesControllerScript : MonoBehaviour
{
    [HideInInspector]
    public float speed = 1f;
    public float waveAmplitude = 25f;
    public float waveFrequency = 1f;
    public float fadeDuration = 1.5f;
    private ObjectScript objectScript;
    private ScreenBehaviorScript screenBoundriesScript;
    private WinConditionScript winCondition;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private bool isFadingOut = false;
    private bool isExploding = false;
    private Image image;
    private Color originalColor;
    private Canvas canvas;
    private Camera uiCamera;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        rectTransform = GetComponent<RectTransform>();

        image = GetComponent<Image>();
        if (image != null)
        {
            originalColor = image.color;
        }

        objectScript = Object.FindFirstObjectByType<ObjectScript>();
        screenBoundriesScript = Object.FindFirstObjectByType<ScreenBehaviorScript>();
        winCondition = Object.FindFirstObjectByType<WinConditionScript>();

        // Get the canvas and its camera
        canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            uiCamera = canvas.worldCamera;
            Debug.Log($"Canvas Render Mode: {canvas.renderMode}, Camera: {(uiCamera != null ? uiCamera.name : "null")}");
        }
        else
        {
            Debug.LogError("Canvas not found for ObstaclesControllerScript!");
        }

        StartCoroutine(FadeIn());
    }

    void Update()
    {
        float waveOffset = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
        rectTransform.anchoredPosition += new Vector2(-speed * Time.deltaTime, waveOffset * Time.deltaTime);

        //Iznīcinās ja lido pa kreisi
        if (speed > 0 && transform.position.x < (screenBoundriesScript.minX + 80) && !isFadingOut)
        {
            isFadingOut = true;
            StartCoroutine(FadeOutAndDestroy());
        }

        //Iznīcinās ja lido pa labi
        if (speed < 0 && transform.position.x > (screenBoundriesScript.maxX - 80) && !isFadingOut)
        {
            isFadingOut = true;
            StartCoroutine(FadeOutAndDestroy());
        }

        //Ja neko nevelk un kursors pieskaras bumbai
        Vector2 inputPosition;
        if (!TryGetInputPosition(out inputPosition))
        {
            return;
        }

        // Validate input position
        if (float.IsInfinity(inputPosition.x) || float.IsInfinity(inputPosition.y) ||
            float.IsNaN(inputPosition.x) || float.IsNaN(inputPosition.y))
        {
            Debug.LogWarning($"Invalid input position: {inputPosition}");
            return;
        }

        // Use the correct camera for UI raycasting
        Camera raycastCamera = null;
        if (canvas != null)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                raycastCamera = null; // Overlay uses screen coordinates directly
            }
            else
            {
                raycastCamera = uiCamera; // Camera or World Space uses the assigned camera
            }
        }

        // Additional check: make sure rectTransform is valid
        if (rectTransform == null || !rectTransform.gameObject.activeInHierarchy)
        {
            return;
        }

        bool isOverObstacle = false;
        try
        {
            isOverObstacle = RectTransformUtility.RectangleContainsScreenPoint(rectTransform, inputPosition, raycastCamera);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in RectangleContainsScreenPoint: {e.Message}");
            return;
        }

        if (CompareTag("Bomb") && !isExploding && isOverObstacle)
        {
            Debug.Log("Bomb hit by cursor (without dragging)");
            TriggerExplosion();
        }

        if (ObjectScript.drag && !isFadingOut && isOverObstacle)
        {
            Debug.Log("Obstacle hit by drag");
            if (ObjectScript.lastDragged != null)
            {
                // Check if the dragged object is a car/vehicle before notifying
                if (IsVehicle(ObjectScript.lastDragged))
                {
                    NotifyCarDestroyed();
                }

                StartCoroutine(ShrinkAndDestroy(ObjectScript.lastDragged, 0.5f));
                ObjectScript.lastDragged = null;
                ObjectScript.drag = false;
            }

            if (CompareTag("Bomb"))
            {
                StartToDestroy(Color.red);
            }
            else
            {
                StartToDestroy(Color.cyan);
            }
        }
    }

    // Check if a GameObject is a vehicle based on its tag
    bool IsVehicle(GameObject obj)
    {
        string tag = obj.tag;
        return tag == "Garbage" || tag == "Medicine" || tag == "Fire" ||
               tag == "School" || tag == "B2" || tag == "Cement" ||
               tag == "E46" || tag == "E61" || tag == "Escavator" ||
               tag == "Police" || tag == "Digger" || tag == "Tractor";
    }

    // Notify the win condition script that a car was destroyed
    void NotifyCarDestroyed()
    {
        if (winCondition != null)
        {
            winCondition.CarDestroyed();
        }
    }

    bool TryGetInputPosition(out Vector2 position)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        position = Input.mousePosition;
        return true;

#elif UNITY_ANDROID
                    if(Input.touchCount > 0)
                    {
                        position = Input.GetTouch(0).position;
                        return true;
                    }
                    else
                    {
                        position = Vector2.zero;
                        return false;
                    }
#else
                position = Vector2.zero;
                return false;
#endif
    }


    public void TriggerExplosion()
    {
        isExploding = true;
        if (objectScript != null && objectScript.effects != null && objectScript.audioCli != null && objectScript.audioCli.Length > 15)
        {
            objectScript.effects.PlayOneShot(objectScript.audioCli[15], 5f);
        }

        if (TryGetComponent<Animator>(out Animator animator))
        {
            animator.SetBool("explode", true);
        }

        if (image != null)
        {
            image.color = Color.red;
            StartCoroutine(RecoverColor(0.3f));
        }

        StartCoroutine(Vibrate());
        StartCoroutine(WaitBeforeExplode());
    }

    IEnumerator WaitBeforeExplode()
    {
        float radius = 0;
        if (TryGetComponent<CircleCollider2D>(out CircleCollider2D circleCollider))
        {
            radius = circleCollider.radius * transform.lossyScale.x;
            ExplodeAndDestroyNearbyObjects(radius);
            yield return new WaitForSeconds(1f);
            ExplodeAndDestroyNearbyObjects(radius);
            Destroy(gameObject);
        }
    }

    void ExplodeAndDestroyNearbyObjects(float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D hit in hits)
        {
            if (hit != null && hit.gameObject != gameObject)
            {
                ObstaclesControllerScript obj = hit.GetComponent<ObstaclesControllerScript>();
                if (obj != null && !obj.isExploding)
                {
                    obj.StartToDestroy(Color.cyan);
                }

                // Check if it's a vehicle that got caught in the explosion
                DragAndDropScript dragScript = hit.GetComponent<DragAndDropScript>();
                if (dragScript != null && IsVehicle(hit.gameObject))
                {
                    NotifyCarDestroyed();
                    StartCoroutine(ShrinkAndDestroy(hit.gameObject, 0.5f));
                }
            }
        }
    }

    public void StartToDestroy(Color c)
    {
        if (!isFadingOut)
        {
            StartCoroutine(FadeOutAndDestroy());
            isFadingOut = true;

            if (image != null)
            {
                image.color = c;
                StartCoroutine(RecoverColor(0.5f));
            }

            StartCoroutine(Vibrate());

            if (objectScript != null && objectScript.effects != null && objectScript.audioCli != null && objectScript.audioCli.Length > 14)
            {
                objectScript.effects.PlayOneShot(objectScript.audioCli[14]);
            }
        }
    }

    IEnumerator FadeIn()
    {
        float a = 0f;
        while (a < fadeDuration)
        {
            a += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, a / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    IEnumerator FadeOutAndDestroy()
    {
        float a = 0f;
        float startAlpha = canvasGroup.alpha;

        while (a < fadeDuration)
        {
            a += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, a / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        Destroy(gameObject);
    }

    IEnumerator ShrinkAndDestroy(GameObject target, float duration)
    {
        if (target == null) yield break;

        Vector3 originalScale = target.transform.localScale;
        Quaternion originalRotation = target.transform.rotation;
        float t = 0f;

        while (t < duration && target != null)
        {
            t += Time.deltaTime;
            target.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t / duration);
            float angle = Mathf.Lerp(0, 360, t / duration);
            target.transform.rotation = Quaternion.Euler(0, 0, angle);

            yield return null;
        }

        if (target != null)
        {
            Destroy(target);
        }
    }

    IEnumerator RecoverColor(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (image != null)
        {
            image.color = originalColor;
        }
    }

    IEnumerator Vibrate()
    {
#if UNITY_ANDROID
        Handheld.Vibrate();
#endif
        Vector2 originalPosition = rectTransform.anchoredPosition;
        float duration = 0.3f;
        float elpased = 0f;
        float intensity = 5f;

        while (elpased < duration)
        {
            rectTransform.anchoredPosition = originalPosition + Random.insideUnitCircle * intensity;
            elpased += Time.deltaTime;
            yield return null;
        }
    }
}