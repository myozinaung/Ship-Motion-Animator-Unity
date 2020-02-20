using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    // [SerializeField]
    public Image progress;
    public Transform totalTime;
    private void Awake()
    {
        // progress = GetComponent<Image>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        progress.fillAmount = Time.time / totalTime.position.x;
    }
}
