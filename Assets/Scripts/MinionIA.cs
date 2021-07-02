using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public class MinionIA : MonoBehaviour
{
    private GameController _gameController;
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

    // Start is called before the first frame update
    void Start()
    {
        _gameController = FindObjectOfType(typeof(GameController)) as GameController;
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
        Destroy(transform.parent.gameObject);
    }

    private void shot()
    {
        if (_gameController.isPlayerAlive)
        {
            GameObject temp = Instantiate(_gameController.bulletsPrefab[idBullet], weapon.position, transform.localRotation);
            temp.transform.tag = _gameController.getBulletTag(bulletTag);
            temp.GetComponent<Rigidbody2D>().velocity = transform.up * -1 * bulletSpeed;
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
