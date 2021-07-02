using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameController _gameController;
    private Player _player;

    public GameObject explosionPrefab;
    public GameObject[] loot;

    public Transform weapon;
    public GameObject bulletPrefab;
    public float[] shotDelay;

    // Start is called before the first frame update
    void Start()
    {
        _gameController = FindObjectOfType(typeof(GameController)) as GameController;
        _player = FindObjectOfType(typeof(Player)) as Player;
        StartCoroutine("shot");
     }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch(collision.gameObject.tag)
        {
            case "playerShot":
                Die(collision);
                break;
        }
    }

    private void Die(Collider2D collision)
    {
        GameObject temp = Instantiate(explosionPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
        Destroy(temp.gameObject, 0.5f);
        Destroy(collision.gameObject);

        spawnLoot();
    }

    IEnumerator shot()
    {
        yield return new WaitForSeconds(Random.Range(shotDelay[0], shotDelay[1]));

        if (_gameController.isPlayerAlive)
        {
            weapon.right = _player.transform.position - transform.position;
            GameObject temp = Instantiate(bulletPrefab, weapon.position, weapon.rotation);
            temp.GetComponent<Rigidbody2D>().velocity = weapon.right * 8;

        }
        StartCoroutine("shot");
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
                idLoot = 2;
            }
            else if (rand > 50)
            {
                idLoot = 1;
            }
            else
            {
                idLoot = 0;
            }

            Instantiate(loot[idLoot], transform.position, transform.rotation);
        }
    }
}
