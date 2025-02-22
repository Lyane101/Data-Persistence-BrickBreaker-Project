using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;
    public Transform Paddle;

    public Text ScoreText;
    public Text BestScoreText;
    public Text LevelText;
    public GameObject GameOverText;
    public GameObject MainMenuButton;
    public AudioClip[] clangSounds;

    private AudioSource audioSource;
    private bool m_Started = false;
    private int m_Points;
    private int bricksLeft = 36;
    private int level = 1;
    private bool m_GameOver = false;

    private Vector3 initialBallPosition;
    private Vector3 initialPaddlePosition;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        LevelText.text = "Level: " + level;

        initialBallPosition = Ball.transform.position;
        initialPaddlePosition = Paddle.transform.position;

        StartNewLevel();

        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.LoadHighScore();

            if (MenuManager.Instance.HighScore > 0)
            {
                BestScoreText.text = $"Best Score: {MenuManager.Instance.NameHighScore}: {MenuManager.Instance.HighScore}";
            }
            else
            {
                BestScoreText.text = $"Best Score: 0";
            }
        }

    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                LaunchBall();
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        if (bricksLeft == 0)
        {
            bricksLeft = 36;
            level++;
            LevelText.text = "Level: " + level;
            m_Started = false;
            GetBallReady();
            StartNewLevel();
        }
    }

    private void StartNewLevel()
    { 
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };

        for (int i = 0; i < LineCount; ++i)
        {
            Color rowColor = GetRandomVibrantColor();
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.RowColor = rowColor;
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void GetBallReady()
    {
        // Reset the position to its initial position
        Ball.transform.position = initialBallPosition;
        Paddle.transform.position = initialPaddlePosition;

        // Parent the ball to the paddle
        Ball.transform.SetParent(Paddle);

        // Reset the ball's velocity and angular velocity to zero
        Ball.linearVelocity = Vector3.zero;
        Ball.angularVelocity = Vector3.zero;

        // Make the ball kinematic to ensure it stays still
        Ball.isKinematic = true;
    }

    private void LaunchBall()
    {
        Ball.isKinematic = false;

        float randomDirection = Random.Range(-1.0f, 1.0f);
        Vector3 forceDir = new Vector3(randomDirection, 1, 0);
        forceDir.Normalize();

        Ball.transform.SetParent(null);
        Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
    }

    private Color GetRandomVibrantColor()
    {
        float hue = Random.Range(0f, 1f); // Full range of hues
        float saturation = Random.Range(0.7f, 1f); // High saturation for vibrant colors
        float value = Random.Range(0.7f, 1f); // High value for brightness

        return Color.HSVToRGB(hue, saturation, value);
    }

    private void AddPoint(int point)
    {
        audioSource.PlayOneShot(clangSounds[Random.Range(0,2)]);
        bricksLeft--;
        m_Points += point;
        ScoreText.text = $"Score: {m_Points}";
    }

    private void AddHighScore(int score)
    {
        if (score > MenuManager.Instance.HighScore)
        {
            MenuManager.Instance.HighScore = score;
            MenuManager.Instance.SaveHighScore();
        }

        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.LoadHighScore();

            if (MenuManager.Instance.HighScore > 0)
            {
                BestScoreText.text = $"Best Score: {MenuManager.Instance.NameHighScore}: {MenuManager.Instance.HighScore}";
            }
            else
            {
                BestScoreText.text = $"Best Score: 0";
            }
        }
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        MainMenuButton.SetActive(true);
        AddHighScore(m_Points);
    }
}
