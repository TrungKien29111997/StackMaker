using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivoteBrick : MonoBehaviour
{
    [SerializeField] GameObject brickObj;
    bool hasBrick;
    private void Start()
    {
        hasBrick = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && hasBrick)
        {
            other.GetComponent<Player>().AddBrick();
            brickObj.SetActive(false);
            hasBrick = false;
        }
    }
}
