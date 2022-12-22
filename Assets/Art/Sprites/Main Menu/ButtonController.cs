using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField] Animator animPlay;
    [SerializeField] Animator animPuzzle4x4;
    [SerializeField] Animator animPuzzle3x3;
    [SerializeField] Animator animHUD;

    
    public bool normalMode { get; set; }



    public void playButton()
    {
        StartCoroutine(Anims());
    }

    IEnumerator Anims()
    {
        animPlay.SetTrigger("Play");
        yield return new WaitForSeconds(2);
        animHUD.SetTrigger("Hud");
        yield return new WaitForSeconds(3);

        if (normalMode)
        {
            animPuzzle3x3.SetTrigger("Puzzle");
            animPuzzle4x4.gameObject.SetActive(false);
        }
        else
        {
            animPuzzle4x4.SetTrigger("Puzzle");
            animPuzzle3x3.gameObject.SetActive(false);
        }
        


    }
}
