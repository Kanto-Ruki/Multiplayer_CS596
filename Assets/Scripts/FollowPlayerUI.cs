using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerUI : MonoBehaviour
{
    public Transform playerTransform; // Reference to the player's transform
    public Vector3 offset; // Offset from the player

    private RectTransform rectTransform;
    private Camera mainCamera;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (playerTransform != null)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(playerTransform.position + offset);
            rectTransform.position = screenPos;
        }
    }
}
