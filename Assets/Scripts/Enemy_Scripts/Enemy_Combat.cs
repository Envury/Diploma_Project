using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;


public class Enemy_Combat : MonoBehaviour
{
    [SerializeField] private Transform attackPoint;

    private EnemyConfig config;
    private Enemy enemy;
    private float lastAttackTime;
    private StateMachine stateMachine;
    public MyQueue<int> stateQueue = new MyQueue<int>();
    public TMP_Text attackProb;
    public TMP_Text dodgeProb;




    private void Start()
    {
        attackProb.text = AttackProb()*100 + "%";
        dodgeProb.text = DodgeProb() * 100 + "%";

        enemy = GetComponent<Enemy>();
        config = enemy.Config;
        stateMachine = enemy.StateMachine;
        for (int i = 0; i < 10; i++)
        {
            stateQueue.Enqueue(0);
            stateQueue.Enqueue(1);
        }
    }

    private void Update()
    {
        //if(Keyboard.current.qKey.isPressed)
        //{
        //    PreformDodge();
        //}
        
        Debug.Log(String.Format("\t\tattackProb {0}\n\t\t\tdodgeProb {1}", AttackProb(), DodgeProb()));
        Debug.Log(stateQueue.ToString());
        attackProb.text = AttackProb() * 100 + "%";
        dodgeProb.text = DodgeProb() * 100 + "%";
    }
    public bool CanMeleeAttack() => Time.time >= lastAttackTime + config.meleeCooldown;

    public void PreformMeleeAttack()
    {
        lastAttackTime = Time.time;

        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, config.meleeRange, config.targetLayer);
        if (!hit)
            return;

        stateQueue.Enqueue(1);
        stateQueue.Dequeue();

        Health health = hit.GetComponentInChildren<Health>();
        if (health != null)
            health.ChangeHealth(-config.meleeDamage, transform.position);
    }

    public void PreformDodge()
    {
        stateMachine.ChangeState(new DodgeState(enemy));
    }

    public float AttackProb()
    {
        int totalSum = 0;
        foreach (int number in stateQueue)
        {
            totalSum += number;
        }


        return totalSum / 20f;
    }
    public float DodgeProb() => 1 - AttackProb();
}

public class MyQueue<T> : Queue<T>
{
    //public new void Enqueue(T item)
    //{
    //    if (Count >= 20)
    //        Dequeue();
    //    base.Enqueue(item);
    //}

    public override string ToString()
    {
        string x = "";
        foreach (T item in this)
        {
            x += item.ToString() + " ";
        }
        return x;
    }
}