using System;

[Serializable]
public class BaseCooldown {

    [Serializable]
    public class CooldownEffects
    {
        public float m_damageMultiplier = 1.0f;
        public float m_healingMultiplier = 1.0f;
        public float m_leechMultiplier = 0.0f;
        public float m_damageReductionMultiplier = 0.0f;
        public float m_castTimeMultiplier = 1.0f;
        public int m_critChanceIncrease = 0;
        public float m_critEffectIncrease = 0.0f;
        public float m_HoTMultiplier = 0.0f;
        public float m_deepHealingMultiplier = 1.0f;
        public Enums.CooldownTargets m_targets = Enums.CooldownTargets.Self;
    }

    string m_name;
    string m_description;
    Enums.Cooldowns m_cooldown;
    CooldownEffects m_effects;
    float m_duration;

    public string Name { get { return m_name; } }
    public string Description { get { return m_description; } }
    public Enums.Cooldowns Cooldown { get { return m_cooldown; } }
    public CooldownEffects Cooldowneffects { get { return m_effects; } }
    public float Duration { get { return m_duration; } }

    public BaseCooldown(string _name, string _description, Enums.Cooldowns _cooldown, CooldownEffects cdes, float dur)
    {
        m_name = _name;
        m_description = _description;
        m_cooldown = _cooldown;
        m_effects = cdes;
        m_duration = dur;
        ParseDescription();
    }

    void ParseDescription()
    {
        m_description = m_description.Replace("<damage>", (m_effects.m_damageMultiplier*100.0f - 100.0f).ToString() + "%");
        m_description = m_description.Replace("<heal>", (m_effects.m_healingMultiplier * 100.0f - 100.0f).ToString() + "%");
        m_description = m_description.Replace("<leech>", (m_effects.m_leechMultiplier * 100).ToString() + "%");
        m_description = m_description.Replace("<dr>", (m_effects.m_damageReductionMultiplier * 100).ToString() + "%");
        m_description = m_description.Replace("<ct>", (100.0f - m_effects.m_castTimeMultiplier * 100.0f).ToString() + "%");
        m_description = m_description.Replace("<dur>", m_duration.ToString() + " seconds");
        m_description = m_description.Replace("<hot>", (m_effects.m_HoTMultiplier * 100).ToString() + "%");
        m_description = m_description.Replace("<critchance>", (m_effects.m_critChanceIncrease).ToString() + "%");
        m_description = m_description.Replace("<criteffect>", (m_effects.m_critEffectIncrease * 100.0f).ToString() + "%");
        m_description = m_description.Replace("<deep>", (m_effects.m_deepHealingMultiplier * 100).ToString() + "%");
    }
}
