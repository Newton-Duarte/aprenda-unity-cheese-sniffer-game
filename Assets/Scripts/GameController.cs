using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Text scoreText;
    public Text timeText;
    int score;
    int levelTime = 5;

    [SerializeField] AudioSource fxSource;
    [SerializeField] AudioClip fxCollect;

    // Start is called before the first frame update
    void Start()
    {
        timeText.text = levelTime.ToString();
        StartCoroutine(levelCountdown());
    }

    // Update is called once per frame
    void Update()
    {
        
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
