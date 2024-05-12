using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostObj : MonoBehaviour
{
    Animator _animator;
    GameObject player;
    bool checkDone;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        checkDone = false;
    }

    void Update()
    {
        if (player != null && !checkDone)
        {
            if (Vector3.Distance(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z), transform.position) < 0.1f)
            {
                if (Vector3.Dot(player.transform.forward, transform.forward) > 0.5f)
                {
                    player.transform.forward = -transform.right;
                    checkDone = true;
                }
                else if (Vector3.Dot(player.transform.forward, transform.right) > 0.5f)
                {
                    player.transform.forward = -transform.forward;
                    checkDone = true;
                }
                _animator.SetTrigger("Trigger");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = null;
            checkDone = false;
        }
    }
}
