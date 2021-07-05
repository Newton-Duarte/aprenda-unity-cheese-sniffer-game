using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankIA : MonoBehaviour
{
    private GameController _gameController;

    [Header("Shot Config.")]
    public int idBullet;
    public bulletsTag bulletTag;
    public Transform weapon;
    public float bulletSpeed;
    public float[] shotDelay;

    [Header("Score Config.")]
    public int points;

    // Start is called before the first frame update
    void Start()
    {
        _gameController = FindObjectOfType(typeof(GameController)) as GameController;
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnBecameVisible()
    {
        StartCoroutine("shotControl");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "playerShot":
                Die(collision);
                break;
        }
    }

    private void Die(Collider2D collision)
    {
        GameObject temp = Instantiate(_gameController.explosionPrefab, transform.position, _gameController.explosionPrefab.transform.rotation);
        temp.transform.parent = _gameController.level;
        _gameController.playExplosionFx();
        Destroy(gameObject);
        Destroy(temp.gameObject, 0.5f);
        Destroy(collision.gameObject);

        _gameController.setScore(points);
        spawnLoot();
    }

    private void shot()
    {
        if (_gameController.isPlayerAlive)
        {
            weapon.up = _gameController._player.transform.position - transform.position;
            GameObject temp = Instantiate(_gameController.bulletsPrefab[idBullet], weapon.position, weapon.localRotation);
            temp.transform.tag = _gameController.getBulletTag(bulletTag);
            temp.GetComponent<Rigidbody2D>().velocity = weapon.up * bulletSpeed;
        }
    }

    IEnumerator shotControl()
    {
        yield return new WaitForSeconds(Random.Range(shotDelay[0], shotDelay[1]));
        shot();
        StartCoroutine("shotControl");
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

            GameObject temp = Instantiate(_gameController.enemyLoot[idLoot], transform.position, transform.rotation);
            temp.transform.parent = _gameController.level;
        }
    }
}
