using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TentacleDamage : MonoBehaviour
{
    public int damage = 10;
    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var player = other.GetComponent<PlayerController>();
        if (player == null) return;

        Debug.Log($"�ٴڿ� ���� �˼��� ���� :  {other.gameObject.name}");
        player.TakeDamage(damage);
    }
}
