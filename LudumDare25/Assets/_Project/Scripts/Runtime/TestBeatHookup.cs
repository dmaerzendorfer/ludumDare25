using _Project.Scripts.Runtime.AudioManager;
using _Project.Scripts.Runtime.Feedback;
using UnityEngine;

public class TestBeatHookup : MonoBehaviour
{
    public ScaleFeedback pop;

    private void Start()
    {
        AudioManager.Instance.beatSettings.intervals[0].trigger.AddListener(pop.Play);
    }
}