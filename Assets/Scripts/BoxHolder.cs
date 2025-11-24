using UnityEngine;
using System.Collections.Generic;

public class BoxHolder : MonoBehaviour
{
    public List<Transform> boxes = new List<Transform>();
    private SpriteRenderer platformRenderer;
    private Color originalColor;
    private Color highlightColor = Color.yellow;
    private Color errorColor = Color.red;

    private static BoxHolder selectedHolder = null;

    void Start()
    {
        // Get this platform's sprite renderer
        platformRenderer = GetComponent<SpriteRenderer>();
        originalColor = platformRenderer.color;

        // Find all boxes that are children of the parent (BoxHolder)
        Transform parent = transform.parent;
        foreach (Transform child in parent)
        {
            if (child.name.Contains("Box"))
            {
                boxes.Add(child);
            }
        }

        // Sort boxes by Y position (bottom to top)
        boxes.Sort((a, b) => a.position.y.CompareTo(b.position.y));
    }

    void OnMouseDown()
    {
        Debug.Log("Clicked: " + transform.parent.name);

        if (selectedHolder == null)
        {
            // Select this holder
            SelectHolder();
        }
        else if (selectedHolder == this)
        {
            // Deselect if clicking the same holder
            DeselectHolder();
        }
        else
        {
            // Move box from selected holder to this holder
            MoveBoxFromHolder(selectedHolder);
        }
    }

    void SelectHolder()
    {
        if (boxes.Count > 0)
        {
            selectedHolder = this;
            platformRenderer.color = highlightColor;
            Debug.Log("Selected: " + transform.parent.name);
        }
        else
        {
            Debug.Log("No boxes to move!");
        }
    }

    void DeselectHolder()
    {
        selectedHolder = null;
        platformRenderer.color = originalColor;
        Debug.Log("Deselected");
    }

    bool CanPlaceBox(Transform boxToPlace)
    {
        // If holder is empty, any box can be placed
        if (boxes.Count == 0)
        {
            return true;
        }

        // Get the top box on this holder
        Transform topBox = boxes[boxes.Count - 1];

        // Get the scale (width) of both boxes
        float boxToPlaceWidth = boxToPlace.localScale.x;
        float topBoxWidth = topBox.localScale.x;

        // Can only place if the box being placed is smaller than or equal to the top box
        return boxToPlaceWidth <= topBoxWidth;
    }

    void MoveBoxFromHolder(BoxHolder fromHolder)
    {
        if (fromHolder.boxes.Count > 0)
        {
            // Get the top box from the selected holder
            Transform boxToMove = fromHolder.boxes[fromHolder.boxes.Count - 1];

            // Check if this move is valid
            if (!CanPlaceBox(boxToMove))
            {
                Debug.Log("Invalid move! Can't place larger box on smaller box.");
                ShowError();
                fromHolder.DeselectHolder();
                return;
            }

            // Valid move - proceed
            fromHolder.boxes.RemoveAt(fromHolder.boxes.Count - 1);

            // Add it to this holder's parent
            boxToMove.SetParent(transform.parent);
            boxes.Add(boxToMove);

            // Calculate new position RELATIVE to the platform's position
            float platformY = transform.position.y; // Platform's Y position
            float stackHeight = 0.35f + (boxes.Count - 1) * 0.5f; // Height above platform
            float newY = platformY + stackHeight;
            float holderX = transform.parent.position.x;
            float holderZ = transform.parent.position.z;
            boxToMove.position = new Vector3(holderX, newY, holderZ);

            Debug.Log("Moved box to: " + transform.parent.name);

            // Increment move counter
            if (GameManager.Instance != null)
            {
                GameManager.Instance.IncrementMoves();
                GameManager.Instance.CheckWinCondition(this);
            }
        }

        // Deselect after moving
        fromHolder.DeselectHolder();
    }

    void ShowError()
    {
        // Flash red to indicate invalid move
        StopAllCoroutines();
        StartCoroutine(FlashError());
    }

    System.Collections.IEnumerator FlashError()
    {
        platformRenderer.color = errorColor;
        yield return new WaitForSeconds(0.3f);
        platformRenderer.color = originalColor;
    }
}