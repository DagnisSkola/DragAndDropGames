using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class CarSpawnPlaceScript : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public RectTransform[] placePoints; // 12 placement points (RectTransforms for UI)
    public GameObject[] carPlaces; // 12 car place prefabs (silhouettes/targets)

    [Header("Render / Sorting Options")]
    public bool overrideZ = false;
    public float placeZ = 0f;

    [Header("Scale Options")]
    public bool applyScale = true;

    void Start()
    {
        SpawnCarPlaces();
    }

    void SpawnCarPlaces()
    {
        if (placePoints.Length != carPlaces.Length)
        {
            Debug.LogError("Car places and placement points must have the same length!");
            return;
        }

        // Shuffle placement indices to match the randomized car spawning
        List<int> placeIndices = new List<int>();
        for (int i = 0; i < placePoints.Length; i++) placeIndices.Add(i);
        for (int i = 0; i < placeIndices.Count - 1; i++)
        {
            int r = Random.Range(i, placeIndices.Count);
            int tmp = placeIndices[i];
            placeIndices[i] = placeIndices[r];
            placeIndices[r] = tmp;
        }

        for (int i = 0; i < carPlaces.Length; i++)
        {
            int placeIndex = placeIndices[i];
            RectTransform placePoint = placePoints[placeIndex];

            Vector2 pos = Vector2.zero;
            Quaternion rot = Quaternion.identity;

            // Apply hardcoded offsets based on place point name
            ApplyOffset(ref pos, ref rot, placePoint.name);

            if (overrideZ)
            {
                Vector3 pos3d = pos;
                pos3d.z = placeZ;
                pos = pos3d;
            }

            GameObject inst = Instantiate(carPlaces[i], placePoint);
            RectTransform instRect = inst.GetComponent<RectTransform>();

            if (instRect != null)
            {
                instRect.anchoredPosition = pos;
                instRect.rotation = rot;

                // Apply scale based on placement point
                ApplyScale(inst, placePoint.name);

                // Ensure DropPlaceScript has the objScript reference
                DropPlaceScript dropScript = inst.GetComponent<DropPlaceScript>();
                if (dropScript != null)
                {
                    dropScript.objScript = Object.FindFirstObjectByType<ObjectScript>();
                }
            }
        }
    }

    void ApplyOffset(ref Vector2 pos, ref Quaternion rot, string placePointName)
    {
        float offsetX = 0f;
        float offsetY = 0f;
        float offsetRotZ = 0f;

        switch (placePointName)
        {
            case "SpawnPointPlace1":
                offsetX = -504f;
                offsetY = -139f;
                offsetRotZ = 0f;
                break;
            case "SpawnPointPlace2":
                offsetX = -221f;
                offsetY = -270f;
                offsetRotZ = 0f;
                break;
            case "SpawnPointPlace3":
                offsetX = 546f;
                offsetY = 85f;
                offsetRotZ = 0f;
                break;
            case "SpawnPointPlace4":
                offsetX = 594f;
                offsetY = -165f;
                offsetRotZ = 0f;
                break;
            case "SpawnPointPlace5":
                offsetX = 226f;
                offsetY = -358f;
                offsetRotZ = 0f;
                break;
            case "SpawnPointPlace6":
                offsetX = 530f;
                offsetY = 333f;
                offsetRotZ = 0f;
                break;
            case "SpawnPointPlace7":
                offsetX = 15f;
                offsetY = 326f;
                offsetRotZ = 0f;
                break;
            case "SpawnPointPlace8":
                offsetX = -649f;
                offsetY = 336f;
                offsetRotZ = 0f;
                break;
            case "SpawnPointPlace9":
                offsetX = -652f;
                offsetY = 155f;
                offsetRotZ = 0f;
                break;
            case "SpawnPointPlace10":
                offsetX = -28f;
                offsetY = -275f;
                offsetRotZ = 0f;
                break;
            case "SpawnPointPlace11":
                offsetX = 761f;
                offsetY = 476f;
                offsetRotZ = 0f;
                break;
            case "SpawnPointPlace12":
                offsetX = 173f;
                offsetY = -169f;
                offsetRotZ = 0f;
                break;
            default:
                break;
        }

        pos.x += offsetX;
        pos.y += offsetY;

        Vector3 euler = rot.eulerAngles;
        euler.z += offsetRotZ;
        rot = Quaternion.Euler(euler);
    }

    void ApplyScale(GameObject inst, string placePointName)
    {
        float scaleX = 0.65f;
        float scaleY = 0.65f;

        switch (placePointName)
        {
            case "SpawnPointPlace1":
                scaleX = 0.62f;
                scaleY = 0.63f;
                break;
            case "SpawnPointPlace2":
                scaleX = 0.68f;
                scaleY = 0.65f;
                break;
            case "SpawnPointPlace3":
                scaleX = 0.64f;
                scaleY = 0.67f;
                break;
            case "SpawnPointPlace4":
                scaleX = 0.66f;
                scaleY = 0.64f;
                break;
            case "SpawnPointPlace5":
                scaleX = 0.63f;
                scaleY = 0.66f;
                break;
            case "SpawnPointPlace6":
                scaleX = 0.67f;
                scaleY = 0.62f;
                break;
            case "SpawnPointPlace7":
                scaleX = 0.65f;
                scaleY = 0.68f;
                break;
            case "SpawnPointPlace8":
                scaleX = 0.69f;
                scaleY = 0.64f;
                break;
            case "SpawnPointPlace9":
                scaleX = 0.61f;
                scaleY = 0.65f;
                break;
            case "SpawnPointPlace10":
                scaleX = 0.66f;
                scaleY = 0.66f;
                break;
            case "SpawnPointPlace11":
                scaleX = 0.64f;
                scaleY = 0.63f;
                break;
            case "SpawnPointPlace12":
                scaleX = 0.68f;
                scaleY = 0.67f;
                break;
            default:
                break;
        }

        if (applyScale)
        {
            Vector3 scale = inst.transform.localScale;
            scale.x = scaleX;
            scale.y = scaleY;
            inst.transform.localScale = scale;
        }
    }
}