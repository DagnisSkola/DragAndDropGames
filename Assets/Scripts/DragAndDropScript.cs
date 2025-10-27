using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropScript : MonoBehaviour, IPointerDownHandler, IBeginDragHandler,
    IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGro;
    private RectTransform rectTra;
    public ObjectScript objectScr;
    public ScreenBehaviorScript screenBou;
    private WinConditionScript winCondition;
    private bool hasBeenPlaced = false;
    private bool shouldResetPosition = false;
    private Vector2 resetToPosition;

    [HideInInspector]
    public Vector2 originalStartPosition;

    private Vector3 dragOffsetWorld;
    private Camera uiCamera;
    private Canvas canvas;

    void Awake()
    {
        canvasGro = GetComponent<CanvasGroup>();
        rectTra = GetComponent<RectTransform>();

        if(objectScr == null)
        {
            objectScr = Object.FindFirstObjectByType<ObjectScript>();
        }

        if (screenBou == null)
        {
            screenBou = GetComponent<ScreenBehaviorScript>();
        }

        canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
            uiCamera = canvas.worldCamera;
        else
            Debug.LogError("Canvas not found for DragAndDropScript");

    }

    void Start()
    {
        canvasGro = GetComponent<CanvasGroup>();
        rectTra = GetComponent<RectTransform>();

        objectScr = Object.FindFirstObjectByType<ObjectScript>();
        screenBou = Object.FindFirstObjectByType<ScreenBehaviorScript>();
        winCondition = Object.FindFirstObjectByType<WinConditionScript>();

        // Store the starting position AFTER spawning
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        originalStartPosition = rectTra.anchoredPosition;
        Debug.Log($"{gameObject.name} stored start position: {originalStartPosition}");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
        {
            Debug.Log("OnPointerDown");
            objectScr.effects.PlayOneShot(objectScr.audioCli[0]);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ObjectScript.drag = true;
        canvasGro.blocksRaycasts = false;
        canvasGro.alpha = 0.6f;
        shouldResetPosition = false;

        int positionIndex = transform.parent.childCount - 1;
        int position = Mathf.Max(0, positionIndex - 1);
        transform.SetSiblingIndex(position);

        Vector3 pointerWorld;
        if(ScreenPointToWorld(eventData.position, out pointerWorld))    
        {
            dragOffsetWorld = rectTra.position - pointerWorld;
        }
        else
        {
            dragOffsetWorld = Vector3.zero;
        }

        ObjectScript.lastDragged = eventData.pointerDrag;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 pointerWorld;
        if (!ScreenPointToWorld(eventData.position, out pointerWorld))
            return;
        Vector3 desired = pointerWorld + dragOffsetWorld;
        desired.z = rectTra.position.z;
        screenBou.RecalculateBounds();

        Vector2 clamped = screenBou.GetClampedPosition(desired);
        transform.position = new Vector3(clamped.x, clamped.y, desired.z);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
            ObjectScript.drag = false;
            canvasGro.blocksRaycasts = true;
            canvasGro.alpha = 1.0f;

            // Check if we need to reset position
            if (shouldResetPosition)
            {
                Debug.Log($"Resetting {gameObject.name} to position: {resetToPosition}");
                rectTra.anchoredPosition = resetToPosition;
                shouldResetPosition = false;
            }
            else if (objectScr.rightPlace)
            {
                canvasGro.blocksRaycasts = false;
                ObjectScript.lastDragged = null;

                // Notify win condition if this is the first time placing this car
                if (!hasBeenPlaced && winCondition != null)
                {
                    hasBeenPlaced = true;
                    winCondition.CarPlacedSuccessfully();
                }
            }

            objectScr.rightPlace = false;
    }

    // Call this from DropPlaceScript when wrong placement detected
    public void RequestPositionReset(Vector2 position)
    {
        shouldResetPosition = true;
        resetToPosition = position;
    }

    private bool ScreenPointToWorld(Vector2 screenPoint, out Vector3 worldPoint)
    {
        worldPoint = Vector3.zero;
        if (uiCamera == null)
            return false;
        float z = Mathf.Abs(uiCamera.transform.position.z - transform.position.z);
        Vector2 sp = new Vector3(screenPoint.x, screenPoint.y, z);
        return true;
    }
}