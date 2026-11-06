
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

        StartCoroutine(PlayDialogueCoroutine());
    }

    private IEnumerator PlayDialogueCoroutine()
    {
        _isPlaying = true;

        if (!_hasFinishedDialogue)
        {
            foreach (DialogueLine line in dialogueLines)
            {
                yield return PlayLine(line);

                if (line.variations != null && line.variations.Length > 0)
                    yield return new WaitForSeconds(pauseBetweenLines);
            }

            _hasFinishedDialogue = true;
        }
        else
        {
            yield return PlayLine(codeLine);
        }

        _isPlaying = false;
    }

    private IEnumerator PlayLine(DialogueLine line)
    {
        if (line.variations == null || line.variations.Length == 0)
            yield break;

        AudioClip clip = line.variations[
            Random.Range(0, line.variations.Length)
        ];

        if (clip == null)
            yield break;

        audioSource.PlayOneShot(clip);

        yield return new WaitForSeconds(clip.length);
    }
}
