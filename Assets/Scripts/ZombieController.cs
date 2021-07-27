using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    // 変数の宣言(アニメーター、AI)
    Animator animator;
    NavMeshAgent agent;

    public float walkingSpeed;

    // 列挙型の作成
    public enum STATE {IDLE, WANDER, ATTACK, CHASE, DEAD};
    public STATE state = STATE.IDLE;

    // 変数の宣言(プレイヤーオブジェクト格納:走るスピード)
    GameObject target;
    public float runSpeed;

    // ZombieController
    // 変数の作成(攻撃力)
    public int attackDamage;

    // スタート時に変数にコンポーネントを格納
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }
    }

    public void TurnOffTrigger()
    {
        animator.SetBool("Walk", false);
        animator.SetBool("Run", false);
        animator.SetBool("Death", false);
        animator.SetBool("Attack", false);
    }

    // プレイヤーとの距離関数を作成
    float DistanceToPlayer()
    {
        if (GameState.GameOver)
        {
            return Mathf.Infinity;
        }
        return Vector3.Distance(target.transform.position, transform.position);
    }

    // 発見判定関数作成
    bool CanSeePlayer()
    {
        if (DistanceToPlayer() < 15)
        {
            return true;
        }

        return false;
    }

    // 見失ったか判定する関数
    bool ForGetPlayer()
    {
        if (DistanceToPlayer() > 20)
        {
            return true;
        }

        return false;
    }

    // 攻撃関数を作成
    public void DamagePlayer()
    {
        if(target != null)
        {
            target.GetComponent<FPSController>().TakeHit(attackDamage);
        }
    }

    // ZombieController
    // 死んだ時の関数
    public void ZombieDeath()
    {
        TurnOffTrigger();
        animator.SetBool("Death", true);
        state = STATE.DEAD;
    }

    void Update()
    {
        // switch文はcaseの部分で分岐してstate変数がどのSTATEに当てはまるか確認する
        // if文の中の条件に当てはまらない場合は何もしない
        switch (state)
        {
            case STATE.IDLE:
                TurnOffTrigger();

                if (CanSeePlayer())
                {
                    state = STATE.CHASE;
                }
                else if(Random.Range(0, 100) < 5)
                {
                    state = STATE.WANDER;
                }

                break;

            case STATE.WANDER:
                if (!agent.hasPath)
                {
                    float newX = transform.position.x + Random.Range(-5, 5);
                    float newZ = transform.position.z + Random.Range(-5, 5);

                    Vector3 NextPos = new Vector3(newX, transform.position.y, newZ);

                    agent.SetDestination(NextPos);
                    agent.stoppingDistance = 0;

                    TurnOffTrigger();

                    agent.speed = walkingSpeed;
                    animator.SetBool("Walk", true);
                }

                if (Random.Range(0, 100) < 5)
                {
                    state = STATE.IDLE;
                    agent.ResetPath();
                }

                if (CanSeePlayer())
                {
                    state = STATE.CHASE;
                }

                break;

            // 列挙型にコードを追加記述
            case STATE.CHASE:

                // 列挙型にコード記述
                if (GameState.GameOver)
                {
                    TurnOffTrigger();
                    agent.ResetPath();
                    state = STATE.WANDER;

                    return;
                }

                agent.SetDestination(target.transform.position);
                agent.stoppingDistance = 3;

                TurnOffTrigger();

                agent.speed = runSpeed;
                animator.SetBool("Run", true);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    state = STATE.ATTACK;
                }

                if (ForGetPlayer())
                {
                    agent.ResetPath();
                    state = STATE.WANDER;
                }

                break;

            case STATE.ATTACK:
                if (GameState.GameOver)
                {
                    TurnOffTrigger();
                    agent.ResetPath();
                    state = STATE.WANDER;

                    return;
                }
                TurnOffTrigger();
                animator.SetBool("Attack", true);

                transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));

                if (DistanceToPlayer() > agent.stoppingDistance +2)
                {
                    state = STATE.CHASE;
                }

                break;

            // 列挙型にコード記述
            case STATE.DEAD:

                Destroy(agent);

                break;
        }
    }
}
