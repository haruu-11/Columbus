
using UnityEngine;
using System.Collections;

[System.Serializable]
public class DialogueLine
{
    public AudioClip[] variations;
}

public class NPCVoice : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private DialogueLine[] dialogueLines;
    [SerializeField] private DialogueLine codeLine;
    [SerializeField] private float pauseBetweenLines = 0.5f;

    private bool _isPlaying;
    private bool _hasFinishedDialogue;

    public void PlayDialogue()
    {
        if (_isPlaying)
            return;

        if (audioSource == null)
        {
            Debug.LogError("NPCVoice: AudioSource is not assigned.", this);
            return;
        }

        if (!audioSource.enabled || !audioSource.gameObject.activeInHierarchy)
        {
            Debug.LogError("NPCVoice: AudioSource is disabled or inactive.", this);
            return;
        }

        StartCoroutine(PlayDialogueCoroutine());
    }

    private IEnumerator PlayDialogueCoroutine()
    {
        _isPlaying = true;

        try
        {
            if (!_hasFinishedDialogue)
            {
                foreach (DialogueLine line in dialogueLines)
                {
                    yield return PlayLine(line);

                    if (line != null &&
                        line.variations != null &&
                        line.variations.Length > 0)
                    {
                        yield return new WaitForSeconds(pauseBetweenLines);
                    }
                }

                _hasFinishedDialogue = true;
            }
            else
            {
                yield return PlayLine(codeLine);
            }
        }
        finally
        {
            _isPlaying = false;
        }
    }

    private IEnumerator PlayLine(DialogueLine line)
    {
        Debug.Log("PlayLine ejecutado");

        if (line == null || line.variations == null || line.variations.Length == 0)
        {
            Debug.LogWarning("La línea no tiene variaciones.");
            yield break;
        }

        AudioClip clip = line.variations[
            Random.Range(0, line.variations.Length)
        ];

        if (clip == null)
        {
            Debug.LogWarning("El AudioClip seleccionado es null.");
            yield break;
        }

        Debug.Log($"Intentando reproducir: {clip.name}");

        audioSource.PlayOneShot(clip);

        yield return new WaitForSeconds(clip.length);
    }
}
    