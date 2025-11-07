using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHP;
    private int currentHP;
    private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;

    private readonly int _hurtHash = Animator.StringToHash("Hurt");


    private void Awake()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>();
    }

    private void PlaySound(AudioClip sound) {
        audioSource.clip = sound;
        audioSource.Play();
    }

    public void GetDamage(int amount)
  {
        currentHP -= amount;
        PlaySound(hurtSound);
        animator.SetTrigger(_hurtHash);
        if (currentHP <= 0)
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            PlaySound(deathSound);
            animator.Play("fall");
            Destroy(gameObject, 1);
        }
  }

}
