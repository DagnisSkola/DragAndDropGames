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
    private static Transform movingBox = null;
    private static Vector3 originalBoxPosition;
    private static BoxHolder hoveredHolder = null;

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

    void Update()
    {
        // If we have a box being moved, make it follow the cursor
        if (movingBox != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = movingBox.position.z; // Keep the same Z depth
            movingBox.position = mousePos;

            // Check if mouse button is released
            if (Input.GetMouseButtonUp(0))
            {
                // Try to place the box on the hovered holder
                if (hoveredHolder != null)
                {
                    hoveredHolder.TryPlaceBox();
                }
                else
                {
                    // No holder hovered, cancel the move
                    selectedHolder.CancelMove();
                }
            }
        }
    }

    void OnMouseEnter()
    {
        // Track which holder the mouse is hovering over
        if (movingBox != null)
        {
            hoveredHolder = this;
        }
    }

    void OnMouseExit()
    {
        // Clear hovered holder when mouse leaves
        if (hoveredHolder == this)
        {
            hoveredHolder = null;
        }
    }

    void OnMouseDown()
    {
        Debug.Log("Clicked: " + transform.parent.name);

        // Only start a new selection if we're not already moving a box
        if (movingBox == null && selectedHolder == null)
        {
            SelectHolder();
        }
    }

    void SelectHolder()
    {
        if (boxes.Count > 0)
        {
            selectedHolder = this;
            platformRenderer.color = highlightColor;

            // Get the top box and make it follow cursor
            movingBox = boxes[boxes.Count - 1];
            originalBoxPosition = movingBox.position;

            // Remove it from the list temporarily (but don't change parent yet)
            boxes.RemoveAt(boxes.Count - 1);

            Debug.Log("Selected: " + transform.parent.name + " - Box is now following cursor");
        }
        else
        {
            Debug.Log("No boxes to move!");
        }
    }

    void CancelMove()
    {
        if (movingBox != null)
        {
            // Put the box back to its original position
            movingBox.position = originalBoxPosition;
            boxes.Add(movingBox);
            movingBox = null;
        }

        selectedHolder = null;
        platformRenderer.color = originalColor;
        hoveredHolder = null;
        Debug.Log("Move cancelled");
    }

    void TryPlaceBox()
    {
        if (movingBox != null && selectedHolder != null)
        {
            // Check if this move is valid
            if (!CanPlaceBox(movingBox))
            {
                Debug.Log("Invalid move! Can't place larger box on smaller box.");
                ShowError();
                selectedHolder.CancelMove();
                return;
            }

            // Valid move - proceed
            // Change parent to this holder's parent
            movingBox.SetParent(transform.parent);
            boxes.Add(movingBox);

            // Calculate new position RELATIVE to the platform's position
            float platformY = transform.position.y; // Platform's Y position
            float stackHeight = 0.35f + (boxes.Count - 1) * 0.5f; // Height above platform
            float newY = platformY + stackHeight;
            float holderX = transform.parent.position.x;
            float holderZ = transform.parent.position.z;
            movingBox.position = new Vector3(holderX, newY, holderZ);

            Debug.Log("Moved box to: " + transform.parent.name);

            // Increment move counter
            if (GameManager.Instance != null)
            {
                GameManager.Instance.IncrementMoves();
                GameManager.Instance.CheckWinCondition(this);
            }

            // Clear the moving box reference
            movingBox = null;
            selectedHolder.DeselectHolder();
            selectedHolder = null;
            hoveredHolder = null;
        }
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

    void DeselectHolder()
    {
        platformRenderer.color = originalColor;
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