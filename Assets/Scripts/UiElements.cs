using TMPro;
using UnityEngine;

public class UiElements : MonoBehaviour
{
    [SerializeField] Level level;

    public TMP_Text remainingText;
    public TMP_Text remainingSubtext;

    public TMP_Text targetText;
    public TMP_Text targetSubtext;

    public TMP_Text scoreText;

    public GameObject gameOverScreen;
    public TMP_Text gameOverText;

    [SerializeField] private UnityEngine.UI.Image star1full;
    [SerializeField] private UnityEngine.UI.Image star2full;
    [SerializeField] private UnityEngine.UI.Image star3full;
    [SerializeField] private UnityEngine.UI.Image star1empty;
    [SerializeField] private UnityEngine.UI.Image star2empty;
    [SerializeField] private UnityEngine.UI.Image star3empty;

    private int starsNum = 0;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
        
    }

    public void SetRemaining(string remaining)
    {
        remainingText.text = remaining.ToString();
    }

    public void SetTarget(int target)
    {
        targetText.text = target.ToString();
    }

    public void SetLevelType(Level.LevelType type)
    {
        if(type == Level.LevelType.MOVES)
        {
            remainingSubtext.text = "moves remaining";
            targetSubtext.text = "target score";
        } 
        else if (type == Level.LevelType.OBSTACLE)
        {
            
            remainingSubtext.text = "remaining moves";
            targetSubtext.fontSize = 32;
            targetSubtext.text = "remaining obstacles";
        }
        else if (type == Level.LevelType.TIMER)
        {
            remainingSubtext.text = "remaining time";
            targetSubtext.text = "target score";
        }
    }

    public void OnGameWin(int score)
    {
        gameOverScreen.SetActive(true);

        Debug.Log("score " + score);
        
        if(score >= level.score1Star && score < level.score2Star)
        {
            starsNum = 1;
            star1empty.gameObject.SetActive(false);
            star1full.gameObject.SetActive(true);
            gameOverText.text = "Good!";

        } 
        else if( score >= level.score2Star && score < level.score3Star)
        {
            starsNum = 2;
            star1empty.gameObject.SetActive(false);
            star1full.gameObject.SetActive(true);
            star2empty.gameObject.SetActive(false);
            star2full.gameObject.SetActive(true);
            gameOverText.text = "Great!";
        }
        else if( score >= level.score3Star)
        {
            starsNum = 3;

            star1empty.gameObject.SetActive(false);
            star1full.gameObject.SetActive(true);

            star2empty.gameObject.SetActive(false);
            star2full.gameObject.SetActive(true);

            star3empty.gameObject.SetActive(false);
            star3full.gameObject.SetActive(true);
            gameOverText.text = "Amazing!";
        }
        else
        {
            star1empty.gameObject.SetActive(true);
            star2empty.gameObject.SetActive(true);
            star3empty.gameObject.SetActive(true);
            gameOverText.text = "OK";
        }
        if( starsNum > PlayerPrefs.GetInt(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 0))
        {
            PlayerPrefs.SetInt(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, starsNum);
        }
    }

    public void OnGameLose(int score)
    {
        gameOverScreen.gameObject.SetActive(true);
        star1empty.gameObject.SetActive(false);
        star2empty.gameObject.SetActive(false);
        star3empty.gameObject.SetActive(false);
        gameOverText.text = "Try again";

    }

    public void OnReplayClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void OnDoneClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level Select");
    }

}
