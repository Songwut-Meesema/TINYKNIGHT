using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    [Tooltip("ดาเมจพื้นฐานของ 'อาวุธ' ชิ้นนี้")]
    public float baseDamage = 12f;
    private Collider _collider;
    private PlayerStatus _playerStatus;
    private List<Collider> _hitEnemies = new List<Collider>();

    void Awake()
    {
        _collider = GetComponent<Collider>();
        _playerStatus = GetComponentInParent<PlayerStatus>();
        if (_playerStatus == null)
        {
            Debug.LogError("PlayerHitbox cannot find PlayerStatus in parent objects!", this.gameObject);
        }
    }

    void OnEnable()
    {
        _hitEnemies.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hitEnemies.Contains(other)) return;

        if (other.TryGetComponent<BossStatus>(out BossStatus bossStatus))
        {
            float totalDamage = baseDamage;
            if (_playerStatus != null)
            {
                totalDamage += _playerStatus.runtimeStats.attackPower;
            }

            // --- [อัปเกรด] ---
            // หา "จุดที่ใกล้ที่สุด" บน Collider ของศัตรู เทียบกับตำแหน่งของ Hitbox ของเรา
            // เพื่อหาจุดปะทะที่สมจริงที่สุด
            Vector3 hitPoint = other.ClosestPoint(transform.position);

            Debug.Log("Player hit the Boss! Dealing " + totalDamage + " damage at " + hitPoint);

            // ส่งข้อมูลดาเมจ "และ" ตำแหน่งที่ปะทะไปด้วย
            bossStatus.TakeDamage(totalDamage, hitPoint);

            _hitEnemies.Add(other);
        }
    }
}