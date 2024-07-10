using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftedPrev : MonoBehaviour
{
    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= 10f)
        {
            Destroy(this);
        }
    }
}
