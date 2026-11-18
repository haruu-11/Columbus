
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerFootsteps : MonoBehaviour
{
    private AudioSource footstepSource;

    private void Awake()
    {
        footstepSource = GetComponent<AudioSource>();
    }

    public void PlayFootstep()
    {
        footstepSource.PlayOneShot(footstepSource.clip);
    }
}
