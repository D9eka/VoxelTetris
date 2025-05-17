public class AutoDropper {
    private float slowDelay = 1.5f;
    private float mediumDelay = 1f;
    private float fastDelay = 0.5f;
    
    private float defaultDelay;
    private float currentDelay;
    
    private bool isSlowFallActive;
    private float timeModifier;
    

    public AutoDropper(ScoreManager scoreManager) 
    {
        scoreManager.OnScoreStateChanged += OnScoreStateChanged;
        OnScoreStateChanged(scoreManager.ScoreState);
        UpdateDropSpeed();
    }

    private void OnScoreStateChanged(ScoreState scoreState)
    {
        defaultDelay = scoreState switch
        {
            ScoreState.Low => slowDelay,
            ScoreState.Medium => mediumDelay,
            ScoreState.High => fastDelay,
            _ => defaultDelay
        };
        UpdateDropSpeed();
    }

    public void UpdateDropSpeed() 
    {
        currentDelay = defaultDelay;
        if (isSlowFallActive) {
            currentDelay *= timeModifier;
        }
    }

    public void ActivateSlowFall(float timeModifier) 
    {
        if (isSlowFallActive)
        {
            return;
        }
        this.timeModifier = timeModifier;
        isSlowFallActive = true;
        UpdateDropSpeed();
    }

    public void DeactivateSlowFall()
    {
        isSlowFallActive = false;
        UpdateDropSpeed();
    }

    public float GetCurrentDelay() => currentDelay;
}