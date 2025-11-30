using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BossHitbox : MonoBehaviour
{
    public LayerMask targetLayer;
    public float damage = 20f;
    private Collider _collider;
    void Awake()
    {
        _collider = GetComponent<Collider>();
        if (!_collider.isTrigger)
        {
            Debug.LogError("'" + gameObject.name + "' is missing 'Is Trigger' setting on its Collider!", gameObject);
        }
    }

    public void SetActive(bool active)
    {
        if (_collider != null)
        {
            _collider.enabled = active;
            Debug.Log("'" + gameObject.name + "' Collider state set to: " + active, gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("'" + gameObject.name + "' triggered with '" + other.name + "' on layer " + LayerMask.LayerToName(other.gameObject.layer), other.gameObject);

        if (((1 << other.gameObject.layer) & targetLayer.value) != 0)
        {
            Debug.Log("Correct Target Layer ('" + LayerMask.LayerToName(other.gameObject.layer) + "') detected!");

            if (other.TryGetComponent<PlayerStatus>(out PlayerStatus playerStatus))
            {
                Debug.Log("PlayerStatus found! Dealing " + damage + " damage.");
                playerStatus.TakeDamage(damage);

                _collider.enabled = false;
            }
        }
    }
}
