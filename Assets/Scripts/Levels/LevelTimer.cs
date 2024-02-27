using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTimer : Level
{
    [SerializeField] int timeInSeconds;
    [SerializeField] int targetScore;

    private float timer;
    private bool timeOut = false;

    // Start is called before the first frame update
    void Start()
    {
        type = LevelType.TIMER;
        
        uiElements.SetLevelType(type);
        uiElements.SetScore(currentScore);
        uiElements.SetTarget(targetScore);
        uiElements.SetRemaining(string.Format("{0}:{1:00}", timeInSeconds/60, timeInSeconds % 60));
    }

    // Update is called once per frame
    void Update()
    {
        if (!timeOut)
        {
            timer += Time.deltaTime;
            uiElements.SetRemaining(string.Format("{0}:{1:00}", (int)Mathf.Max((timeInSeconds - timer) / 60, 0), (int)Mathf.Max((timeInSeconds - timer) % 60), 0));

            if (timeInSeconds - timer <= 0)
            {
                if (currentScore >= targetScore)
                {
                    WinGame();
                }
                else
                {
                    LoseGame();
                }
                timeOut = true;
            }
        }
        
    }
}
