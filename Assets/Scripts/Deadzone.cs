using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deadzone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Characterstats>() != null)
            collision.GetComponent<Characterstats>().KillEntity();
        else
            Destroy(collision.gameObject);
    }
}
