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

    // プレイヤーとの距離関数を作成
    // 発見判定関数作成
    // 見失ったか判定する関数

    // 列挙型にコードを追加記述

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

    float DistanceToPlayer()
    {
        return Vector3.Distance(target.transform.position, transform.position);
    }

    bool CanSeePlayer()
    {
        if (DistanceToPlayer() < 15)
        {
            return true;
        }

        return false;
    }

    bool ForGetPlayer()
    {
        if (DistanceToPlayer() > 20)
        {
            return true;
        }

        return false;
    }

    // Update is called once per frame
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

            case STATE.CHASE:
                agent.SetDestination(target.transform.position);
                agent.stoppingDistance = 3;

                TurnOffTrigger();

                agent.speed = runSpeed;
                animator.SetBool("Run", true);

                if (ForGetPlayer())
                {
                    agent.ResetPath();
                    state = STATE.WANDER;
                }

                break;
        }
    }
}
