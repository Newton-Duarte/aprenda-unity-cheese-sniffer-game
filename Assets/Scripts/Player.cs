using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    GameController _gameController;
    SpriteRenderer playerSr;
    Rigidbody2D playerRb;
    Animator playerAnim;

    [SerializeField] float moveSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] AudioSource fxSource;
    [SerializeField] AudioClip fxJump;
    bool isLookLeft;
    bool isGrounded;
    float speedX;
    float speedY;
    public LayerMask whatIsGround;
    public Transform groundCheck;

    // Start is called before the first frame update
    void Start()
    {
        _gameController = FindObjectOfType(typeof(GameController)) as GameController;
        playerSr = GetComponent<SpriteRenderer>();
        playerRb = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameController.currentState == gameState.gameplay)
        {
            speedX = Input.GetAxisRaw("Horizontal");
            speedY = playerRb.velocity.y;

            if (!isLookLeft && speedX < 0)
            {
                flip();
            }
            else if (isLookLeft && speedX > 0)
            {
                flip();
            }

            if (transform.position.x < _gameController.leftPlayerBoundary.position.x)
            {
                transform.position = new Vector3(_gameController.leftPlayerBoundary.position.x, transform.position.y, transform.position.z);
            }
            else if (transform.position.x > _gameController.rightPlayerBoundary.position.x)
            {
                transform.position = new Vector3(_gameController.rightPlayerBoundary.position.x, transform.position.y, transform.position.z);
            }

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                jump();
            }
        }
        else if (_gameController.currentState == gameState.gamewin)
        {
            playerRb.gravityScale = 0;
            playerRb.velocity = new Vector2(0, 0);
            isGrounded = true;
            transform.position = Vector3.MoveTowards(transform.position, _gameController.ratHole.position, 2 * Time.deltaTime);

            if (transform.position == _gameController.ratHole.position)
            {
                Destroy(gameObject);
            }
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.02f, whatIsGround);
        playerRb.velocity = new Vector2(speedX * moveSpeed, speedY);
    }

    void LateUpdate()
    {
        updateAnimations();
    }

    private void flip()
    {
        isLookLeft = !isLookLeft;
        playerSr.flipX = isLookLeft;
    }

    void updateAnimations()
    {
        playerAnim.SetInteger("speedX", (int)speedX);
        playerAnim.SetFloat("speedY", speedY);
        playerAnim.SetBool("isGrounded", isGrounded);
    }

    void jump()
    {
        playerRb.AddForce(new Vector2(playerRb.velocity.x, jumpForce));
        fxSource.PlayOneShot(fxJump);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch(collision.tag)
        {
            case "collectable":
                Destroy(collision.gameObject);
                _gameController.setScore(1);
                break;

            default:
                break;
        }
    }
}
