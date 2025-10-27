using UnityEngine;

public class PlaySoundExit : StateMachineBehaviour
{
    [SerializeField] SoundType sound;
    [SerializeField, Range(0f, 1f)] float volume = 1f;

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AudioManager.Instance.PlaySound(sound, volume);
    }
}
