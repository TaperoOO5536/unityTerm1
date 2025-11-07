using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask enemyLayer;
    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawn;
    [Header("Audio")]
    [SerializeField] private AudioSource movementAudioSource;
    [SerializeField] private AudioSource shootAudioSource;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip shootSound;
    [Header("Win")]
    [SerializeField] private GameObject winPanel;

    private Rigidbody2D rb;
    private Animator animator;
    private bool _isGrounded;
    private bool _isPlayingMovingSound;
    

    private readonly int _speedHash = Animator.StringToHash("Speed");
    private readonly int _jumpHash = Animator.StringToHash("Jump");

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Jump();
        Attack();
        RestartGame();
    }

    private void PlaySound(AudioClip sound, AudioSource source) {
        source.clip = sound;
        source.Play();
    }

    private void LoopAudioSource(bool isLooped = false) {
        movementAudioSource.loop = isLooped;
    }

    private void Move() {
        Vector3 moveVelocity = Vector3.zero;
        rb.linearVelocity = new Vector2(Input.GetAxis("Horizontal")*speed, rb.linearVelocity.y);
        animator.SetFloat(_speedHash, Mathf.Abs(rb.linearVelocity.x));
        if (Input.GetAxisRaw("Horizontal") < 0) {
            moveVelocity = Vector3.left;
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (Input.GetAxisRaw("Horizontal") > 0) {
            moveVelocity = Vector3.right;
            transform.localScale = new Vector3(1, 1, 1);
        }
        transform.position += moveVelocity * speed * Time.deltaTime;
        if (Input.GetAxisRaw("Horizontal") != 0) {
            if (!_isPlayingMovingSound && _isGrounded) {
                LoopAudioSource(true);
                _isPlayingMovingSound = true;
                PlaySound(walkSound, movementAudioSource);
            }
        }
        if (_isPlayingMovingSound && Input.GetAxisRaw("Horizontal") == 0) {
            LoopAudioSource(false);
            _isPlayingMovingSound = false;
            movementAudioSource.Stop();
        }
    }

    private void Jump() {
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            LoopAudioSource(false);
            PlaySound(jumpSound, movementAudioSource);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            animator.SetTrigger(_jumpHash);
        }
        
    }

    private void Attack() {
        if (Input.GetMouseButtonDown(0)) {
            Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
            PlaySound(shootSound, shootAudioSource);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (ContainsLayer(groundLayer, other.gameObject)) {
            _isGrounded = true;
        }
        if (ContainsLayer(enemyLayer, other.gameObject)) {
            Application.LoadLevel(Application.loadedLevel);
        }
        if (other.gameObject.tag == "Finish") {
            winPanel.SetActive(true);
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (ContainsLayer(groundLayer, other.gameObject)) {
            _isGrounded = false;
        }
    }

    private bool ContainsLayer(LayerMask layerMask, GameObject gameObject) {
        return (layerMask.value  & 1 << gameObject.layer) > 0;
    }

    private void RestartGame() {
        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
