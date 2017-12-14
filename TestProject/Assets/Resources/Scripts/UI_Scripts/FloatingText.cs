using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Used to create Floating Text on the Screen for Damage popups. 
/// </summary>
public class FloatingText : MonoBehaviour {

    /// <summary>
    /// Animator animator - The Animator that displays the damage.
    /// Text damage - the Text to display. 
    /// </summary>
    public Animator animator;
    private Text damage { get; set; }

	// Use this for initialization
	void Start () {
        AnimatorClipInfo[] clip_info = animator.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clip_info[0].clip.length);
        damage = animator.GetComponent<Text>();
	}
	
    /// <summary>
    /// Sets the text for the Floating Text.
    /// </summary>
    /// <param name="text">What to set the Text to.</param>
	public void SetText(string text)
    {
        animator.GetComponent<Text>().text = text;
    }
}
