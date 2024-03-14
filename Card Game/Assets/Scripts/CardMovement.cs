using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CardMovement : MonoBehaviour
{
    public Transform target;

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, target.position) > 0.001f)
        {
            transform.position += (target.position - transform.position) * Time.deltaTime * 10;
        }
    }
}
