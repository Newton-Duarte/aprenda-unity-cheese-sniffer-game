using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject player;

    [SerializeField] float timeOffset;

    [SerializeField] Vector2 posOffset;

    [SerializeField] float leftLimit;
    [SerializeField] float rightLimit;
    [SerializeField] float bottomLimit;
    [SerializeField] float topLimit;

    void Update()
    {
        if (player)
        {
            // Camera's current position
            Vector3 startPos = transform.position;

            // Player's current position
            Vector3 endPos = player.transform.position;

            endPos.x += posOffset.x;
            endPos.y += posOffset.y;
            endPos.z = -10;

            // Smoothly move the camera towards the player's posistion
            transform.position = Vector3.Lerp(startPos, endPos, timeOffset * Time.deltaTime);

            transform.position = new Vector3
            (
                // Mathf.Clamp is a way to limit a value to a particular range
                // Here we're limiting x position of the camera between leftLimit and rightLimit
                Mathf.Clamp(transform.position.x, leftLimit, rightLimit),
                Mathf.Clamp(transform.position.y, bottomLimit, topLimit),
                transform.position.z
            );
        }
    }
}
