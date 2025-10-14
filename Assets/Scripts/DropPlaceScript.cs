using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlaceScript : MonoBehaviour, IDropHandler
{
    private float placeZRot, vehicleZRot, rotDiff;
    private Vector3 placeSiz, vehicleSiz;
    private float xSizeDiff, ySizeDiff;
    public ObjectScript objScript;

    public void OnDrop(PointerEventData eventData)
    {
        if ((eventData.pointerDrag != null) &&
            Input.GetMouseButtonUp(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
        {
            if (eventData.pointerDrag.tag.Equals(tag))
            {
                placeZRot = eventData.pointerDrag.GetComponent<RectTransform>().transform.eulerAngles.z;
                vehicleZRot = GetComponent<RectTransform>().transform.eulerAngles.z;
                rotDiff = Mathf.Abs(placeZRot - vehicleZRot);

                placeSiz = eventData.pointerDrag.GetComponent<RectTransform>().localScale;
                vehicleSiz = GetComponent<RectTransform>().localScale;
                xSizeDiff = Mathf.Abs(placeSiz.x - vehicleSiz.x);
                ySizeDiff = Mathf.Abs(placeSiz.y - vehicleSiz.y);

                if ((rotDiff <= 5 || (rotDiff >= 355 && rotDiff <= 360)) &&
                    (xSizeDiff <= 0.05 && ySizeDiff <= 0.05))
                {
                    Debug.Log("Correct place");
                    objScript.rightPlace = true;
                    eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
                    eventData.pointerDrag.GetComponent<RectTransform>().localRotation = GetComponent<RectTransform>().localRotation;
                    eventData.pointerDrag.GetComponent<RectTransform>().localScale = GetComponent<RectTransform>().localScale;

                    switch (eventData.pointerDrag.tag)
                    {
                        case "Garbage":
                            objScript.effects.PlayOneShot(objScript.audioCli[2]);
                            break;
                        case "Medicine":
                            objScript.effects.PlayOneShot(objScript.audioCli[3]);
                            break;
                        case "Fire":
                            objScript.effects.PlayOneShot(objScript.audioCli[4]);
                            break;
                        case "School":
                            objScript.effects.PlayOneShot(objScript.audioCli[5]);
                            break;
                        case "B2":
                            objScript.effects.PlayOneShot(objScript.audioCli[6]);
                            break;
                        case "Cement":
                            objScript.effects.PlayOneShot(objScript.audioCli[7]);
                            break;
                        case "E46":
                            objScript.effects.PlayOneShot(objScript.audioCli[8]);
                            break;
                        case "E61":
                            objScript.effects.PlayOneShot(objScript.audioCli[9]);
                            break;
                        case "Escavator":
                            objScript.effects.PlayOneShot(objScript.audioCli[10]);
                            break;
                        case "Police":
                            objScript.effects.PlayOneShot(objScript.audioCli[11]);
                            break;
                        case "Digger":
                            objScript.effects.PlayOneShot(objScript.audioCli[12]);
                            break;
                        case "Tractor":
                            objScript.effects.PlayOneShot(objScript.audioCli[13]);
                            break;
                        default:
                            Debug.Log("Unknown tag detected");
                            break;
                    }
                }
                else
                {
                    // Same tag but wrong rotation/size
                    Debug.Log("Wrong rotation/size - resetting position");
                    objScript.rightPlace = false;
                    objScript.effects.PlayOneShot(objScript.audioCli[1]);

                    DragAndDropScript dragScript = eventData.pointerDrag.GetComponent<DragAndDropScript>();
                    if (dragScript != null)
                    {
                        dragScript.RequestPositionReset(dragScript.originalStartPosition);
                    }
                }
            }
            else
            {
                // Wrong tag entirely
                Debug.Log("Wrong tag - resetting position");
                objScript.rightPlace = false;
                objScript.effects.PlayOneShot(objScript.audioCli[1]);

                DragAndDropScript dragScript = eventData.pointerDrag.GetComponent<DragAndDropScript>();
                if (dragScript != null)
                {
                    dragScript.RequestPositionReset(dragScript.originalStartPosition);
                }
            }
        }
    }
}