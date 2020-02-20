using UnityEngine;

public class TimeScaleController : MonoBehaviour
{
	
    public void DoubleTimeScale()
    {
        Time.timeScale = Time.timeScale*2.0f;
    }

    public void HalfTimeScale()
    {
        Time.timeScale = Time.timeScale/2.0f;
    }    

    public void PauseTimeScale()
    {
        Time.timeScale = 0.0f;
    }

    public void PlayTimeScale()
    {
        Time.timeScale = 1.0f;
    }

    public void SetTimeScale(float newTimeScale) {
        if (newTimeScale < 0) {
            Time.timeScale = Mathf.Abs(1/newTimeScale);
        } else {
            Time.timeScale = newTimeScale;
        }
        
    }
    

}
