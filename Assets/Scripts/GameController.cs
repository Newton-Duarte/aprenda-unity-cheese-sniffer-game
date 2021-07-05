using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

internal enum gameState {
    gameplay, gamewin, gameover
}

public class GameController : MonoBehaviour
{
    int levelTime = 90;
    int score;

    [Header("Gameplay Config.")]
    internal gameState currentState;
    [SerializeField] internal Transform ratHole;

    [Header("Boundary Config.")]
    [SerializeField] internal Transform leftPlayerBoundary;
    [SerializeField] internal Transform rightPlayerBoundary;

    [Header("UI Config.")]
    [SerializeField] Text scoreText;
    [SerializeField] Text timeText;

    [Header("Audio Config.")]
    [SerializeField] AudioSource fxMusic;
    [SerializeField] AudioSource fxSource;
    [SerializeField] AudioSource fxAlertSource;
    [SerializeField] AudioClip fxCollect;
    [SerializeField] AudioClip fxGameWin;
    [SerializeField] AudioClip fxGameOver;
    [SerializeField] AudioClip fxEnemy;

    [Header("Enemy Config.")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform leftSpawn;
    [SerializeField] Transform rightSpawn;
    [SerializeField] int[] enemySpawnTimes;
    bool isSpawned;
    internal Transform targetSpawn;

    // Start is called before the first frame update
    void Start()
    {
        timeText.text = levelTime.ToString();
        StartCoroutine(levelCountdown());
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSpawned && currentState == gameState.gameplay)
        {
            isSpawned = true;
            StartCoroutine(spawnEnemy());
        }
    }

    void LateUpdate()
    {
        if (levelTime == 30)
        {
            fxMusic.pitch = 1.3f;
        }
    }

    public void setIsSpawned(bool value)
    {
        isSpawned = value;
    }

    IEnumerator spawnEnemy()
    {
        yield return new WaitForSeconds(Random.Range(enemySpawnTimes[0], enemySpawnTimes[1]));

        int rand = Random.Range(0, 100);

        if (rand < 50)
        {
            fxAlertSource.panStereo = -1;
            fxAlertSource.PlayOneShot(fxEnemy);
            yield return new WaitForSeconds(1f);

            GameObject enemy = Instantiate(enemyPrefab);
            enemy.transform.position = new Vector3(leftSpawn.position.x, enemy.transform.position.y, enemy.transform.position.z);
            targetSpawn = rightSpawn;
        }
        else
        {
            fxAlertSource.panStereo = 1;
            fxAlertSource.PlayOneShot(fxEnemy);
            yield return new WaitForSeconds(1f);

            GameObject enemy = Instantiate(enemyPrefab);
            enemy.transform.position = new Vector3(rightSpawn.position.x, enemy.transform.position.y, enemy.transform.position.z);
            targetSpawn = leftSpawn;
        }
    }

    IEnumerator levelCountdown()
    {
        yield return new WaitForSeconds(1);
        levelTime--;
        timeText.text = levelTime.ToString();

        if (levelTime == 0)
        {
            StopCoroutine("levelCountdown");
            print("Game Over!");
            yield break;
        }

        StartCoroutine(levelCountdown());
    }

    public void setScore(int points)
    {
        score += points;
        scoreText.text = score.ToString();
        fxSource.PlayOneShot(fxCollect);
    }
    internal IEnumerator loadSceneWithDelay(int sceneIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneIndex);
    }
}
