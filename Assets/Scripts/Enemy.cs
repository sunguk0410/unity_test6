using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {
    [SerializeField] private float playerRangeDistance, speed;
    private Transform target;
    private bool isFollowingMode = false;
    [SerializeField] private int health, healthMax;
    [HideInInspector] public UnityEvent dieEvent;
    private SpawnManager enemySpawner;
    private IEnumerator followingRoutine;

    private Slider slider;


    private void Awake() {
        enemySpawner = FindObjectOfType<SpawnManager>();
        target = GameManager.Instance.Player.transform;
        const float IntialDelayTime = 1f;
        Invoke("Wander", IntialDelayTime);

        HpBar_test hpController = GetComponent<HpBar_test>();
        hpController.MakeHpBar();
        slider = hpController.prfHpBar;
    }


    private void FixedUpdate() {  
        DetectPlayer();
        UpdateHP();
    }

    private void DetectPlayer() {
        if(!GameManager.Instance.IsGame)
            return;
        float distanceToPlayer = Vector2.Distance(transform.position, target.position);
        if(distanceToPlayer > playerRangeDistance && isFollowingMode)
            EnterWanderingMode();
        else if(distanceToPlayer <= playerRangeDistance && !isFollowingMode)
            EnterFollowingMode();
    }

    private IEnumerator KeepFollowingPlayer() {
        while(GameManager.Instance.IsGame) {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            yield return null;
        }
        EnterWanderingMode();
    }

    private void EnterFollowingMode() {
        GetComponent<SpriteRenderer>().color = Color.red;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        isFollowingMode = true;
        CancelInvoke("Wander");
        followingRoutine = KeepFollowingPlayer();
        StartCoroutine(followingRoutine);
    }

    private void EnterWanderingMode() {
        GetComponent<SpriteRenderer>().color = Color.white;
        isFollowingMode = false;
        StopCoroutine(followingRoutine);
        Wander();
    }

    private void Wander() {
        float nextMoveX = Random.Range(-1, 2);
        float nextMoveY = Random.Range(-1, 2);
        GetComponent<Rigidbody2D>().velocity = new Vector2(nextMoveX, nextMoveY);
        const float WanderingDuration = 1f;
        Invoke("Wander", WanderingDuration);
    }

    public void Hit() {
        --health;
        if(health <= 0)
            Die();
    }

    private void Die() {
        dieEvent.Invoke();
        enemySpawner.RemoveEnemy(this);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Player") {
            Player player = other.gameObject.GetComponent<Player>();
            player.HitFrom(transform);
        }
    }

    private void UpdateHP() {
        slider.value = Mathf.Lerp(slider.value, (float)health / (float)healthMax, Time.deltaTime * 10);
    }
}
