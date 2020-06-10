using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadClipsInOverrideController : MonoBehaviour
{
    public AnimationClip[] clipOverrides;

    protected Animator animator;
    protected AnimatorOverrideController animatorOverrideController;
    protected int weaponIndex;

    public void Start()
    {
        animator = GetComponent<Animator>();

        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;
        List<KeyValuePair<AnimationClip, AnimationClip>> clipPairList = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        animatorOverrideController.GetOverrides(clipPairList);
        
        int minLen = Mathf.Min(clipPairList.Count, clipOverrides.Length);
        for (int i = 0; i < minLen; i++)
        {
            clipPairList[i] = new KeyValuePair<AnimationClip, AnimationClip>(clipPairList[i].Key, clipOverrides[i]);
        }
        animatorOverrideController.ApplyOverrides(clipPairList);
    }

}