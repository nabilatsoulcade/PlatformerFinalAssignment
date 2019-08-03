using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public GameObject player;
    private Vector3 offset;
    private float deathBoundary;

    // Use this for initialization
    void Start()
    {
        offset = transform.position - player.transform.position;
        deathBoundary = player.GetComponent<CharacterController2D>().deathBoundary * 0.33f;
    }

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, player.transform.position + offset, 0.25f);
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, deathBoundary, transform.position.y), transform.position.z);
    }

}