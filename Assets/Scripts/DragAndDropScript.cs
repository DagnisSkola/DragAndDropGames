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

    void Start()
    {
        canvasGro = GetComponent<CanvasGroup>();
        rectTra = GetComponent<RectTransform>();

        objectScr = Object.FindFirstObjectByType<ObjectScript>();
        screenBou = Object.FindFirstObjectByType<ScreenBehaviorScript>();
        winCondition = Object.FindFirstObjectByType<WinConditionScript>();
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
        if (Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
        {
            ObjectScript.drag = true;
            canvasGro.blocksRaycasts = false;
            canvasGro.alpha = 0.6f;
            // Pēdējais sarakstā
            //rectTra.SetAsLastSibling();
            // Pirmspēdējais
            int positionIndex = transform.parent.childCount - 1;
            int position = Mathf.Max(0, positionIndex - 1);
            transform.SetSiblingIndex(position);
            Vector3 cursorWorldPos = Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenBou.screenPoint.z));
            rectTra.position = cursorWorldPos;

            screenBou.screenPoint = Camera.main.WorldToScreenPoint(rectTra.localPosition);

            screenBou.offset = rectTra.localPosition - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenBou.screenPoint.z));

            ObjectScript.lastDragged = eventData.pointerDrag;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
        {
            Vector3 curSreenPoint =
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenBou.screenPoint.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curSreenPoint) + screenBou.offset;
            rectTra.position = screenBou.getClampedPosition(curPosition);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButtonUp(0))
        {
            ObjectScript.drag = false;
            canvasGro.blocksRaycasts = true;
            canvasGro.alpha = 1.0f;

            if (objectScr.rightPlace)
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
    }
}