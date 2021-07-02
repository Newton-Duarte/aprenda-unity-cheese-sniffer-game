using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum bulletsTag
{
    player, enemy
}

public enum gameState
{
    intro, gameplay, bossFight, endGame
}

public class GameController : MonoBehaviour
{
    [Header("Game Config.")]
    public gameState currentState;

    [Header("Intro Config.")]
    public Camera camera;
    public float playerInitialScale;
    public float playerOriginalScale;
    public Transform introInitialPos;
    public Transform introFinalPos;
    public float takeOffSpeed;
    private float currentTakeOffSpeed;
    private bool isTakeOff;
    public Color initialPlaneGasColor;
    public Color originalPlaneGasColor;
    public AudioClip fxIntro;

    [Header("Player Config.")]
    public Player _player;
    public GameObject playerPrefab;
    public Transform spawnPlayerPos;
    public int extraLives;
    public int spawnDelay;
    public bool isPlayerAlive;

    [Header("Boundaries Config.")]
    public Transform leftPlayerBoundary;
    public Transform rightPlayerBoundary;
    public Transform topPlayerBoundary;
    public Transform bottomPlayerBoundary;

    [Header("Level Config.")]
    public Transform level;
    public Transform levelFinalPos;
    public Transform levelBossPos;
    public float levelSpeed;

    [Header("Prefabs Config.")]
    public GameObject explosionPrefab;
    public GameObject[] bulletsPrefab;
    public GameObject[] enemyLoot;

    [Header("HUD Config.")]
    public Text livesText;
    public Text scoreText;
    public Text highScoreText;

    [Header("Audio Config.")]
    public AudioSource musicSource;
    public AudioSource fxSource;
    public AudioClip levelMusic;
    public AudioClip level2Music;
    public AudioClip level3Music;
    public AudioClip bossMusic;
    public AudioClip bossSpeak;
    public AudioClip fxCollect;
    public AudioClip fxExplosion;
    public AudioClip fxCompleted;
    public AudioClip fxBonus;
    public int currentMusic;

    [Header("Score Config.")]
    public int bonusInterval;
    private int bonus;
    private int score;
    private int highScore;

    // Start is called before the first frame update
    void Start()
    {
        bonus = bonusInterval;
        StartCoroutine("levelIntro");
        updateExtraLivesText();
        updateScoreText();
        getHighScore();
    }

    private void getHighScore()
    {
        int savedHighScore = PlayerPrefs.GetInt("highScore");
        highScore = savedHighScore;
        updateHighScoreText(savedHighScore);
    }

    private void updateHighScoreText(int highScore)
    {
        highScoreText.text = "High Score: " + highScore.ToString();
    }

    private void updateExtraLivesText()
    {
        livesText.text = "x" + extraLives.ToString();
    }

    private void updateScoreText()
    {
        scoreText.text = score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerAlive)
        {
            playerMovementControl();
        }

        if (isTakeOff && currentState == gameState.intro)
        {
            _player.transform.position = Vector3.MoveTowards(_player.transform.position, introFinalPos.position, currentTakeOffSpeed * Time.deltaTime);

            if (_player.transform.position == introFinalPos.position)
            {
                print("Take Off!");
                StartCoroutine("takeOff");
                currentState = gameState.gameplay;
            }

            _player.planeGasSr.color = Color.Lerp(initialPlaneGasColor, originalPlaneGasColor, 0.1f);
        }
    }

    public void setHighScore()
    {
        int highScore = PlayerPrefs.GetInt("highScore");

        if (score <= highScore) { return; }

        updateHighScoreText(score);

        PlayerPrefs.SetInt("highScore", score);
    }

    private void LateUpdate()
    {
        if (currentState == gameState.gameplay)
        {
            level.position = Vector3.MoveTowards(level.position, new Vector3(level.position.x, levelFinalPos.position.y, 0), levelSpeed * Time.deltaTime);

            if (level.position.y <= -50 && currentMusic == 0)
            {
                changeMusic(level2Music);
            }

            if (level.position.y <= -100 && currentMusic == 1)
            {
                changeMusic(level3Music);

            }

            if (level.position.y <= levelBossPos.position.y && currentMusic == 2)
            {
                StartCoroutine(changeBossMusic());
                currentState = gameState.bossFight;
            }

            if (level.position.y <= levelFinalPos.position.y && currentState != gameState.endGame)
            {
                currentState = gameState.endGame;

                musicSource.Stop();
                fxSource.pitch = 1;
                fxSource.volume = 0.7f;
                fxSource.PlayOneShot(fxCompleted);
                Invoke("win", 4);
            }
        }
    }

    private void gameover()
    {
        print("Game Over!");
        SceneManager.LoadScene(2);
    }

    private void win()
    {
        print("You Win!");
        SceneManager.LoadScene(3);
    }

    IEnumerator changeBossMusic()
    {
        musicSource.Stop();
        float currentVolume = fxSource.volume;
        fxSource.volume = 1;
        fxSource.PlayOneShot(bossSpeak);

        yield return new WaitForSeconds(2);
        fxSource.volume = currentVolume;
        changeMusic(bossMusic);
    }

    private void changeMusic(AudioClip clip)
    {
        musicSource.volume = Mathf.Lerp(musicSource.volume, 0.00f, Time.deltaTime * 2);
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.Play();
        musicSource.volume = Mathf.Lerp(musicSource.volume, 1f, Time.deltaTime * 2);
        currentMusic++;
    }

    internal void setExtraLives(int value)
    {
        extraLives += value;
        updateExtraLivesText();
    }

    private void playerMovementControl()
    {
        float posX = _player.transform.position.x;
        float posY = _player.transform.position.y;

        if (posX < leftPlayerBoundary.position.x)
        {
            _player.transform.position = new Vector3(leftPlayerBoundary.position.x, posY, 0);
        }
        else if (posX > rightPlayerBoundary.position.x)
        {
            _player.transform.position = new Vector3(rightPlayerBoundary.position.x, posY, 0);
        }

        if (posY < bottomPlayerBoundary.position.y)
        {
            _player.transform.position = new Vector3(posX, bottomPlayerBoundary.position.y, 0);
        }
        else if (posY > topPlayerBoundary.position.y)
        {
            _player.transform.position = new Vector3(posX, topPlayerBoundary.position.y, 0);
        }
    }

    public string getBulletTag(bulletsTag tag)
    {
        switch(tag)
        {
            case bulletsTag.player:
                return "playerShot";
            case bulletsTag.enemy:
                return "enemyShot";
            default:
                return null;
        }
    }

    public void hitPlayer()
    {
        Destroy(_player.gameObject);
        isPlayerAlive = false;
        playExplosionFx();
        GameObject temp = Instantiate(explosionPrefab, _player.transform.position, explosionPrefab.transform.rotation);
        Destroy(temp.gameObject, 0.5f);
        extraLives--;

        if (extraLives >= 0)
        {
            StartCoroutine("spawnPlayer");
        }
        else
        {
            gameover();
        }

        if (extraLives < 0) { extraLives = 0; }
        updateExtraLivesText();
    }

    IEnumerator spawnPlayer()
    {
        yield return new WaitForSeconds(spawnDelay);
        GameObject temp = Instantiate(playerPrefab, spawnPlayerPos.position, spawnPlayerPos.localRotation);
        isPlayerAlive = true;
        yield return new WaitForEndOfFrame();
        _player.StartCoroutine("invulnerability");
    }

    IEnumerator levelIntro()
    {
        //yield return new WaitForEndOfFrame();
        camera.orthographicSize = 2;
        camera.transform.position = new Vector3(camera.transform.position.x, -2.25f, camera.transform.position.z);
        _player.transform.position = introInitialPos.position;
        _player.transform.localScale = new Vector3(playerInitialScale, playerInitialScale, playerInitialScale);
        _player.planeGasSr.color = initialPlaneGasColor;
        _player.planeShadow.SetActive(false);
        _player.fxSource.pitch = 1;
        _player.fxSource.volume = 1;
        _player.fxSource.PlayOneShot(fxIntro);

        yield return new WaitForSeconds(2);

        isTakeOff = true;

        for (currentTakeOffSpeed = 0; currentTakeOffSpeed < takeOffSpeed; currentTakeOffSpeed += 0.2f)
        {
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator takeOff()
    {
        musicSource.clip = levelMusic;
        musicSource.Play();

        _player.fxSource.pitch = 2;
        _player.fxSource.volume = 0.1f;
        _player.planeGasSr.enabled = true;
        _player.planeShadow.SetActive(true);
        StartCoroutine(resetCameraSize());

        for (float scale = playerInitialScale; scale < playerOriginalScale; scale += 0.025f)
        {
            _player.transform.localScale = new Vector3(scale, scale, scale);
            _player.planeShadow.transform.localScale = new Vector3(scale, scale, scale);
            _player.planeGasSr.color = Color.Lerp(_player.planeGasSr.color, originalPlaneGasColor, 0.1f);
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator resetCameraSize()
    {
        float elapsed = 0;
        float oldSize = camera.orthographicSize;

        while (elapsed <= 20)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / 1);

            camera.orthographicSize = Mathf.Lerp(oldSize, 5, t);
            camera.transform.position = Vector3.Lerp(camera.transform.localPosition, new Vector3(camera.transform.localPosition.x, 0, camera.transform.localPosition.z), Time.deltaTime * 0.7f);
            yield return null;
        }
    }

    public void setScore(int points)
    {
        score += points;
        updateScoreText();

        if (score >= bonus)
        {
            extraLives++;
            bonus += bonusInterval;
            playBonusFx();
            updateExtraLivesText();
        }
        
        if (score > highScore)
        {
            setHighScore();
        }
    }

    public void playBonusFx()
    {
        fxSource.PlayOneShot(fxBonus);
    }

    public void playCollectFx()
    {
        fxSource.PlayOneShot(fxCollect);
    }

    public void playExplosionFx()
    {
        fxSource.PlayOneShot(fxExplosion);
    }
}
