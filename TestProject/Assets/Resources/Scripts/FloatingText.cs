using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour {

    public Animator animator;
    private Text damage;

	// Use this for initialization
	void Start () {
        AnimatorClipInfo[] clip_info = animator.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clip_info[0].clip.length);
        damage = animator.GetComponent<Text>();
	}
	
	public void SetText(string text)
    {
        animator.GetComponent<Text>().text = text;
    }
}
