using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : MonoBehaviour
{
    [SerializeField] GameObject diamondObj;
    bool hasDiamond;

    private void Start()
    {
        hasDiamond = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && hasDiamond)
        {
            other.GetComponent<Player>().AddDiamond();
            diamondObj.SetActive(false);
            hasDiamond = false;
        }
    }
}
