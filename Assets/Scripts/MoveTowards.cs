using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowards : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] Transform target;
    [SerializeField] bool repeat;
    Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }


    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (repeat && transform.position == target.position)
        {
            transform.position = new Vector3(startPosition.x, startPosition.y, startPosition.z);
        }
    }
}
