using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameController _gameController;

    public Transform enemy;
    public Transform[] checkpoints;
    public float moveSpeed;
    public float stopDelay;

    private int checkpointId;
    private bool isMove;

    private void Start()
    {
        _gameController = FindObjectOfType(typeof(GameController)) as GameController;
        StartCoroutine("startMoving");
    }

    // Update is called once per frame
    void Update()
    {
        if (isMove && enemy && _gameController.currentState == gameState.bossFight)
        {
            enemy.position = Vector3.MoveTowards(enemy.position, checkpoints[checkpointId].position, moveSpeed * Time.deltaTime);

            if (enemy.position == checkpoints[checkpointId].position)
            {
                isMove = false;
                StartCoroutine("startMoving");
            }
        }
    }

    IEnumerator startMoving()
    {
        checkpointId += 1;

        if (checkpointId >= checkpoints.Length) { checkpointId = 0; }

        yield return new WaitForSeconds(stopDelay);
        isMove = true;
    }
}
