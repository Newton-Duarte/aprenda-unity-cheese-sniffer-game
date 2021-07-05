using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private GameController _gameController;
    private Rigidbody2D playerRb;
    private SpriteRenderer playerSr;
    public SpriteRenderer planeGasSr;
    public GameObject planeShadow;

    [Header("Spawn Config.")]
    public float invulnerabilityDelay;
    public float blinkDelay;
    public Color invulnerabilityColor;

    [Header("Movement Config.")]
    public float moveSpeed;

    [Header("Shot Config.")]
    public int idBullet;
    public bulletsTag bulletTag;
    public Transform weaponPos;
    public float bulletSpeed;
    public float shotDelay;
    private int powerUpLevel;
    private bool isFire;

    [Header("FX Config.")]
    public AudioSource fxSource;
    public AudioClip fxShot;

    private void Awake()
    {
        _gameController = FindObjectOfType(typeof(GameController)) as GameController;
        _gameController._player = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        playerSr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameController.currentState == gameState.gameplay || _gameController.currentState == gameState.bossFight)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            playerRb.velocity = new Vector2(horizontal * moveSpeed, vertical * moveSpeed);
        
            if (Input.GetButton("Fire1") && !isFire)
            {
                shot();
            }
        }
    }

    void shot()
    {
        isFire = true;

        GameObject temp = Instantiate(_gameController.bulletsPrefab[idBullet]);
        temp.transform.tag = _gameController.getBulletTag(bulletTag);
        temp.transform.position = weaponPos.position;
        temp.GetComponent<Rigidbody2D>().velocity = new Vector2(0, bulletSpeed);
        fxSource.PlayOneShot(fxShot);
        StartCoroutine("shotControl");
    }

    IEnumerator shotControl()
    {
        yield return new WaitForSeconds(shotDelay);
        isFire = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "enemyShot":
                if (!_gameController.isPlayerAlive) { return; }

                _gameController.hitPlayer();
                Destroy(collision.gameObject);
                powerUpLevel = 0;
                break;
            case "skyEnemy":
                if (!_gameController.isPlayerAlive) { return; }

                _gameController.hitPlayer();
                powerUpLevel = 0;
                break;
            case "collectable":
                collect(collision);
                break;
        }
    }

    private void collect(Collider2D collision)
    {
        IdCollectable idCollectable = collision.gameObject.GetComponent<IdCollectable>();

        if (!idCollectable) { return; };

        switch (idCollectable.objName)
        {
            case "extraLife":
                _gameController.setExtraLives(1);
                _gameController.playBonusFx();
                break;
            case "coin":
                _gameController.setScore(idCollectable.points);
                _gameController.playCollectFx();
                break;
            case "bomb":
                powerUp();
                Destroy(collision.gameObject);
                _gameController.playCollectFx();
                break;
            default:
                break;
        }

        Destroy(collision.gameObject);
    }

    private void powerUp()
    {
        if (powerUpLevel >= 2) { return; }

        shotDelay -= 0.075f;
        powerUpLevel++;

        if (powerUpLevel == 2)
        {
            idBullet = 3;
        }
    }

    IEnumerator invulnerability()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.enabled = false;
        playerSr.color = invulnerabilityColor;
        planeGasSr.color = invulnerabilityColor;
        StartCoroutine("blinkPlayer");

        yield return new WaitForSeconds(invulnerabilityDelay);
        col.enabled = true;
        playerSr.color = Color.white;
        planeGasSr.color = Color.white;
        playerSr.enabled = true;
        planeGasSr.enabled = true;
        StopCoroutine("blinkPlayer");
    }

    IEnumerator blinkPlayer()
    {
        yield return new WaitForSeconds(blinkDelay);
        playerSr.enabled = !playerSr.enabled;
        planeGasSr.enabled = !planeGasSr.enabled;
        StartCoroutine("blinkPlayer");
    }
}
