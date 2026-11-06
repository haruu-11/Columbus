using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Snapshots")]
    [SerializeField] private AudioMixerSnapshot explore;
    [SerializeField] private AudioMixerSnapshot chase;
    [SerializeField] private AudioMixerSnapshot keyFound;

    [Header("Tiempos de transición (segundos)")]
    [SerializeField] private float toChaseTime = 0.2f;
    [SerializeField] private float toKeyTime = 1.5f;
    [SerializeField] private float toExploreTime = 2f;

    private bool hasKey;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        explore.TransitionTo(0f);
    }

    public void OnEnemySpotted()
    {
        chase.TransitionTo(toChaseTime);
    }

    public void OnEnemyLostPlayer()
    {
        if (hasKey) keyFound.TransitionTo(toKeyTime);
        else explore.TransitionTo(toExploreTime);
    }

    public void OnKeyPickedUp()
    {
        hasKey = true;
        keyFound.TransitionTo(toKeyTime);
    }
}