using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] GameObject cameraObj;
    Player player;
    [SerializeField] float speedLerf = 2f;
    Vector3 startPos;

    private void Start()
    {
        startPos = cameraObj.transform.localPosition;
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, player.transform.position, Time.deltaTime * speedLerf);

        cameraObj.transform.localPosition = startPos - cameraObj.transform.forward * player.brickLeft * 0.3f;
    }
}
