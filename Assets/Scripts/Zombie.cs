using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EnemyMove : MonoBehaviour {
    [SerializeField] private float loading = 1;
    private Rigidbody2D rigid;
    [SerializeField] private float playerRangeDistance, speed;
    private Transform target;
    private bool isFollowingMode = false;
    public UnityEvent<Enemy> dieEvent;

    private void Start() {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    private void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        Invoke("Wander", loading);
    }


    private void FixedUpdate() {   
        DetectPlayer();
    }

    private void DetectPlayer() {
        if(target == null && isFollowingMode) {
            EnterWanderingMode();
            return;
        }
        float distanceToPlayer = Vector2.Distance(transform.position, target.position);
        if(distanceToPlayer > playerRangeDistance && isFollowingMode)
            EnterWanderingMode();
        else if(distanceToPlayer <= playerRangeDistance && !isFollowingMode)
            EnterFollowingMode();
    }

    private IEnumerator KeepFollowingPlayer() {
        while(isFollowingMode) {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            yield return null;
        }
    }

    private void EnterFollowingMode() {
        isFollowingMode = true;
        CancelInvoke("Wander");
        StartCoroutine("KeepFollowingPlayer");
    }

    private void EnterWanderingMode() {
        isFollowingMode = false;
        Invoke("Wander", 0);
    }

    private void Wander() {
        float nextMoveX = Random.Range(-1, 2);
        float nextMoveY = Random.Range(-1, 2);
        rigid.velocity = new Vector2(nextMoveX, nextMoveY);
        Invoke("Wander", loading);
    }
}
