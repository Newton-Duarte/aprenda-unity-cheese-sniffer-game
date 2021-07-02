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

        playerRb.velocity = new Vector2(speedX * moveSpeed, speedY);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jump();
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.02f, whatIsGround);
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
