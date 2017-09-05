using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiderDebuff : MonoBehaviour {

    RaiderScript m_raider;
    bool m_active = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Initialize(RaiderScript r, int damagePerSecond)
    {
        m_raider = r;
        StartCoroutine(DealDotDamage(1.0f, damagePerSecond));
    }

    IEnumerator DealDotDamage(float castTime, int damage)
    {
        yield return new WaitForSeconds(castTime);

        if (!m_raider.IsDead() && !m_raider.IsBossDead())
        {
            m_raider.TakeDamage(damage);
            StartCoroutine(DealDotDamage(castTime, damage));
        }
    }
}
