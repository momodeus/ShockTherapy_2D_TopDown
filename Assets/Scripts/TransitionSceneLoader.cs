using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionSceneLoader : MonoBehaviour
{
    public Animator animator;
    public float transitionTime;

    public void LoadScene(string sceneName)
    {
        StartCoroutine(TransitionOut(sceneName));
    }


    IEnumerator TransitionOut(string sceneName)
    {
        animator.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
    }
}
