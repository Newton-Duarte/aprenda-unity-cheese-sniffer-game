using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    GameController _gameController;
    Animator enemyAnim;

    [SerializeField] float moveSpeed;
    [SerializeField] bool isLookLeft;
    bool isCaughtTheMouse;

    // Start is called before the first frame update
    void Start()
    {
        _gameController = FindObjectOfType(typeof(GameController)) as GameController;
        enemyAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameController.currentState != gameState.gamewin)
        {
            transform.position = Vector3.MoveTowards(transform.position, _gameController.targetSpawn.position, moveSpeed * Time.deltaTime);

            if (!isLookLeft && transform.position.x > _gameController.targetSpawn.position.x) { flip(); }
        }
    }

    private void LateUpdate()
    {
        enemyAnim.SetBool("isCaughtTheMouse", isCaughtTheMouse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(collision.gameObject);
            isCaughtTheMouse = true;
            _gameController.gameOver();
        }
    }

    void flip()
    {
        isLookLeft = !isLookLeft;
        float scaleX = transform.localScale.x;
        scaleX *= -1;
        transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
    }

    private void OnBecameInvisible()
    {
        _gameController.setIsSpawned(false);
        Destroy(gameObject);
    }
}
