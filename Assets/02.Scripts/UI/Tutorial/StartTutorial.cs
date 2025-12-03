using UnityEngine;
using UnityEngine.UI;

public class StartTutorial : MonoBehaviour
{

    [SerializeField] Tutorial _tutorial;
    [SerializeField] Story _story;

    private bool _isTutorialStarted = false;
    private bool _isStoryStarted = false;

    void Start()
    {
        _tutorial.gameObject.SetActive(false);
        _story.gameObject.SetActive(false);
        if (DayManager.Instance.CurrentDay == 1) _isStoryStarted = false;
    }

    public void StartIfFirstDay()
    {
        if (DayManager.Instance.CurrentDay == 1 && DayManager.Instance.currentPhase == DayPhase.Morning)
        {
            _isStoryStarted = true;
            _isTutorialStarted = false;
            _story.StoryInit();
        }
    }

    private void Update()
    {
        
        if (!_isStoryStarted) 
        {
            StartIfFirstDay();
        }
        if (!_story.IsStoryEnd)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                _story.NextTextStory();
            }
        }
        else if (!_isTutorialStarted)
        {
            _isTutorialStarted = true;
            _tutorial.TutorialInit();
        }
    }
}
