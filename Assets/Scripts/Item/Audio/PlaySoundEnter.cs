using UnityEngine;

public class PlaySoundEnter : StateMachineBehaviour
{
    [SerializeField] SoundType sound;
    [SerializeField, Range(0f, 1f)] float volume = 1f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AudioManager.Instance.PlaySound(sound, volume);
    }
}
