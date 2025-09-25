using UnityEngine;
using System.Collections.Generic;

public class CarSpawner : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public Transform[] spawnPoints; // 12 spawn points
    public GameObject[] cars; // 12 car prefabs

    [Header("Render / Sorting Options")]
    public bool overrideZ = false;
    public float spawnZ = 0f;
    public bool setSortingLayer = true;
    public string sortingLayerName = "Cars";
    public int sortingOrder = 10;

    void Start()
    {
        SpawnCars();
    }

    void SpawnCars()
    {
        if (spawnPoints.Length != cars.Length)
        {
            Debug.LogError("Cars and spawn points must have the same length!");
            return;
        }

        // Shuffle spawn indices
        List<int> spawnIndices = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++) spawnIndices.Add(i);
        for (int i = 0; i < spawnIndices.Count - 1; i++)
        {
            int r = Random.Range(i, spawnIndices.Count);
            int tmp = spawnIndices[i];
            spawnIndices[i] = spawnIndices[r];
            spawnIndices[r] = tmp;
        }

        for (int i = 0; i < cars.Length; i++)
        {
            int spawnIndex = spawnIndices[i];
            Transform spawnPoint = spawnPoints[spawnIndex];

            Vector3 pos = spawnPoint.position;
            Quaternion rot = spawnPoint.rotation;

            // Apply hardcoded offsets based on spawn point name
            ApplyOffset(ref pos, ref rot, spawnPoint.name);

            if (overrideZ) pos.z = spawnZ;

            GameObject inst = Instantiate(cars[i], pos, rot, spawnPoint);

            if (setSortingLayer)
            {
                SpriteRenderer sr = inst.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingLayerName = sortingLayerName;
                    sr.sortingOrder = sortingOrder;
                }
                else
                {
                    SpriteRenderer[] children = inst.GetComponentsInChildren<SpriteRenderer>();
                    foreach (var c in children)
                    {
                        c.sortingLayerName = sortingLayerName;
                        c.sortingOrder = sortingOrder;
                    }
                }
            }
        }
    }

    void ApplyOffset(ref Vector3 pos, ref Quaternion rot, string spawnPointName)
    {
        float offsetX = 0f;
        float offsetY = 0f;
        float offsetRotZ = 0f;

        switch (spawnPointName)
        {
            case "SpawnPoint1":
                offsetX = -470f;
                offsetY = -402f;
                offsetRotZ = 5f;
                break;
            case "SpawnPoint2":
                offsetX = -659f;
                offsetY = -123f;
                offsetRotZ = -23f;
                break;
            case "SpawnPoint3":
                offsetX = -899f;
                offsetY = 130f;
                offsetRotZ = 23f;
                break;
            case "SpawnPoint4":
                offsetX = 901f;
                offsetY = -267f;
                offsetRotZ = 90f;
                break;
            case "SpawnPoint5":
                offsetX = 278f;
                offsetY = 0f;
                offsetRotZ = 23f;
                break;
            case "SpawnPoint6":
                offsetX = -490f;
                offsetY = -187f;
                offsetRotZ = -23f;
                break;
            case "SpawnPoint7":
                offsetX = 849f;
                offsetY = 225f;
                offsetRotZ = 34f;
                break;
            case "SpawnPoint8":
                offsetX = 646f;
                offsetY = 80f;
                offsetRotZ = 180f;
                break;
            case "SpawnPoint9":
                offsetX = 404f;
                offsetY = -43f;
                offsetRotZ = 223f;
                break;
            case "SpawnPoint10":
                offsetX = 222f;
                offsetY = 91f;
                offsetRotZ = 223f;
                break;
            case "SpawnPoint11":
                offsetX = -69f;
                offsetY = 249f;
                offsetRotZ = 223f;
                break;
            case "SpawnPoint12":
                offsetX = -894f;
                offsetY = -387f;
                offsetRotZ = 223f;
                break;
            default:
                // No offset
                break;
        }

        pos.x += offsetX;
        pos.y += offsetY;

        Vector3 euler = rot.eulerAngles;
        euler.z += offsetRotZ;
        rot = Quaternion.Euler(euler);
    }
}
