using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatObject : MonoBehaviour
{
    private Animator animator;

    private float _timer = 0;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // I want to swap Idle animations every 3-6 seconds
        // Add chance to sleep animation

        float random = UnityEngine.Random.Range(0, 10);
        if (random < 1)
        {
            animator.SetBool("Sleep", true);
        }

        _timer += Time.deltaTime;
        if (_timer >= Random.Range(3, 6))
        {   
            // Idle Idle, Idle2, Idle3
            animator.SetInteger("Idle", Random.Range(0, 3));
        }
    }


    private void OnMouseDown()
    {
        animator.SetTrigger("Scared");
        animator.SetBool("Sleep", false);
    }
}
