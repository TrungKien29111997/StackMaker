using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivoteUnBrick : MonoBehaviour
{
    [SerializeField] GameObject brickObj;
    bool hasBrick;
    private void Start()
    {
        brickObj.SetActive(false);
        hasBrick = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasBrick)
        {
            other.GetComponent<Player>().RemoveBrick();
            brickObj.SetActive(true);
            hasBrick = true;
        }
    }
}
