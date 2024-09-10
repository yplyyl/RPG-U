using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShockStrike_Controller : MonoBehaviour
{
    [SerializeField] private Characterstats targetstats;
    [SerializeField] private float speed;
    private int damage;

    private Animator anim;
    private bool triggered;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void Setup(int _damage, Characterstats _targetstats)
    {
        damage = _damage;
        targetstats = _targetstats;
    }

    private void Update()
    {
        if (!targetstats)
            return;

        if (triggered)
            return;

        transform.position = Vector2.MoveTowards(transform.position, targetstats.transform.position, speed * Time.deltaTime);
        transform.right = transform.position - targetstats.transform.position;

        if (Vector2.Distance(transform.position, targetstats.transform.position) < .1f)
        {
            anim.transform.localPosition = new Vector3(0, .5f);
            anim.transform.localRotation = Quaternion.identity;

            transform.localRotation = Quaternion.identity;
            transform.localScale = new Vector3(3, 3);


            Invoke("DamageAndSelfDestory", .2f);
            triggered = true;
            anim.SetTrigger("Hit");
        }
    }

    private void DamageAndSelfDestory()
    {
        targetstats.ApplyShock(true);
        targetstats.TakeDamage(damage);
        Destroy(gameObject, .4f);
    }
}
