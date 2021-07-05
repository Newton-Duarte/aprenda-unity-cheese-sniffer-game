using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public class BossIA : MonoBehaviour
{
    private GameController _gameController;
    private EnemyController _enemyController;
    private SpriteRenderer enemySr;
    private Collider2D enemyCol;
    private Material whiteMaterial;
    private Material defaultMaterial;
    private UnityEngine.Object explosionRef;

    [Header("Shot Config.")]
    public int idBullet;
    public bulletsTag bulletTag;
    public Transform weapon;
    public float bulletSpeed;
    public float shotDelay;

    [Header("Health Config.")]
    public int healthPoints;

    [Header("Fx Config.")]
    public AudioSource fxSource;
    public AudioClip fxShot;

    [Header("Score Config.")]
    public int points;

    [Header("PowerUp Config.")]
    public float powerUpShotDelay;
    public float powerUpStopDelay;
    public float powerUpMoveSpeed;
    public float powerUpTime;
    private bool isPowerUp;

    // Start is called before the first frame update
    void Start()
    {
        _gameController = FindObjectOfType(typeof(GameController)) as GameController;
        _enemyController = FindObjectOfType(typeof(EnemyController)) as EnemyController;
        enemySr = GetComponent<SpriteRenderer>();
        enemyCol = GetComponent<Collider2D>();
        whiteMaterial = Resources.Load("WhiteFlash", typeof(Material)) as Material;
        defaultMaterial = enemySr.material;
        explosionRef = Resources.Load("Explosion");
        enemyCol.enabled = false;
        enabled = false;
    }

    private void OnBecameVisible()
    {
        enabled = true;
        StartCoroutine(activeCollider());
        StartCoroutine(shotControl());
        StartCoroutine(powerUpControl());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch(collision.gameObject.tag)
        {
            case "playerShot":
                hitEnemy(collision);
                break;
        }
    }

    private void hitEnemy(Collider2D col)
    {
        healthPoints--;

        if (healthPoints <= 0)
        {
            Die(col);
            return;
        }

        enemySr.material = whiteMaterial;
        Invoke("ResetMaterial", 0.025f);
        Destroy(col.gameObject);
    }

    void ResetMaterial()
    {
        enemySr.material = defaultMaterial;
    }

    private void Die(Collider2D collision)
    {
        GameObject temp = Instantiate(_gameController.explosionPrefab, transform.position, _gameController.explosionPrefab.transform.rotation);
        GameObject explosion = (GameObject)Instantiate(explosionRef);
        explosion.transform.position = new Vector3(transform.position.x, transform.position.y + .3f, transform.position.z);
        _gameController.playExplosionFx();
        Destroy(gameObject);
        Destroy(temp.gameObject, 0.5f);
        Destroy(collision.gameObject);

        _gameController.setScore(points);

        spawnLoot();

        _gameController.currentState = gameState.gameplay;
        Destroy(transform.parent.gameObject);
    }

    private void shot()
    {
        if (_gameController.isPlayerAlive)
        {
            weapon.up = _gameController._player.transform.position - transform.position;
            GameObject temp = Instantiate(_gameController.bulletsPrefab[idBullet], weapon.position, weapon.localRotation);
            temp.transform.tag = _gameController.getBulletTag(bulletTag);
            temp.GetComponent<Rigidbody2D>().velocity = weapon.up * bulletSpeed;
            fxSource.PlayOneShot(fxShot);
        }
    }

    IEnumerator activeCollider()
    {
        while (_gameController.currentState != gameState.bossFight)
        {
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(activeCollider());
        }

        enemyCol.enabled = true;
    }

    IEnumerator shotControl()
    {
        while (_gameController.currentState != gameState.bossFight)
        {
            yield return new WaitForSeconds(2);
            StartCoroutine(shotControl());
        }

        yield return new WaitForSeconds(shotDelay);
        shot();
        StartCoroutine(shotControl());
    }

    IEnumerator powerUpControl()
    {
        while (_gameController.currentState != gameState.bossFight)
        {
            yield return new WaitForSeconds(2);
            StartCoroutine(powerUpControl());
        }

        yield return new WaitForSeconds(6);

        int rand = Random.Range(0, 100);

        if (rand > (healthPoints < 50 ? 50 : 80) && !isPowerUp)
        {
            print("Power up! Rand: " + rand.ToString());
            StartCoroutine(powerUp());
        }

        yield return new WaitForSeconds(6);

        StartCoroutine(powerUpControl());
    }

    IEnumerator powerUp()
    {
        StopCoroutine(powerUpControl());
        isPowerUp = true;

        float currentShotDelay = shotDelay;
        float currentStopDelay = _enemyController.stopDelay;
        float currentMoveSpeed = _enemyController.moveSpeed;

        shotDelay = powerUpShotDelay;
        _enemyController.stopDelay = powerUpStopDelay;
        _enemyController.moveSpeed = powerUpMoveSpeed;

        yield return new WaitForSeconds(powerUpTime);

        shotDelay = currentShotDelay;
        _enemyController.stopDelay = currentStopDelay;
        _enemyController.moveSpeed = currentMoveSpeed;
        isPowerUp = false;
        print("Power up! OFF");

        yield return new WaitForSeconds(6);
        StartCoroutine(powerUpControl());
    }

    private void spawnLoot()
    {
        int idLoot;
        int rand = Random.Range(0, 100);

        if (rand < 50)
        {
            rand = Random.Range(0, 100);

            if (rand > 85)
            {
                idLoot = 1;
            }
            else if (rand > 50)
            {
                idLoot = 0;
            }
            else
            {
                idLoot = 0;
            }

            GameObject temp = Instantiate(_gameController.enemyLoot[idLoot], transform.position, new Quaternion());
            temp.transform.parent = _gameController.level;
        }
    }
}
