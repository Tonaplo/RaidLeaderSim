using System;
using UnityEngine;

[Serializable]
public class Raider : BaseCharacter {

    
    RaiderStats stats;
    public RaiderStats RaiderStats { get { return stats; } }

    DateTime m_activityFinished;
    public DateTime ActivityFinished { get { return m_activityFinished; } }

    Enums.CharacterStatus m_charStatus;
    public Enums.CharacterStatus CharacterStatus { get { return m_charStatus; } }

    public Raider(string _name, RaiderStats _stats) : base(_name) {
        stats = _stats;
        RecalculateRaider();
    }

    public void RecalculateRaider()
    {
        CheckForTrainingEnd();
        stats.ReCalculateRaiderStats();
        CalculateMaxHealth();
    }

    public void ChangeSpec()
    {
        stats.ChangeSpec();
        RecalculateRaider();
    }

    //Calculate Max Health based on Class
    public override void CalculateMaxHealth()
    {
        float value = StaticValues.BaseRaiderHealth;
        SetBaseHealth((int)value);
        float pointsPerGearlevel = 1.0f;
        switch (stats.GetRole())
        {
            case Enums.CharacterRole.Tank:
                //Guardians have 15% more health
                if (stats.GetCurrentSpec() == Enums.CharacterSpec.Guardian)
                    value *= 2.0f * 1.15f;
                else
                    value *= 2.0f;

                pointsPerGearlevel = 2.0f;

                break;
            case Enums.CharacterRole.Healer:
                value *= 0.8f;
                break;
            case Enums.CharacterRole.RangedDPS:
                value *= 0.9f;
                break;
            case Enums.CharacterRole.MeleeDPS:
                pointsPerGearlevel = 1.5f;
                value *= 1.1f;
                break;
            default:
                break;
        }
        value += (int)(RaiderStats.Gear.TotalItemLevel * pointsPerGearlevel);
        SetMaxHealth((int)value);
    }

    public void TrainRaider()
    {
        if (!IsEligibleForActivity())
            return;

        m_activityFinished = DateTime.Now;
        m_activityFinished = m_activityFinished.AddSeconds(StaticValues.TrainingDuration);
        m_charStatus = Enums.CharacterStatus.InTraining;
    }

    public bool CheckForTrainingEnd()
    {
        if (m_charStatus != Enums.CharacterStatus.InTraining)
            return false;
        
        if (DateTime.Now > m_activityFinished)
        {
            stats.TrainingFinished();
            m_charStatus = Enums.CharacterStatus.Ready;
            return true;
        }
        return false;
    }

    public bool IsEligibleForActivity()
    {
        switch (m_charStatus)
        {
            case Enums.CharacterStatus.OnVacation:
            case Enums.CharacterStatus.InTraining:
                return false;
            default:
                return true;
        }
    }

    public bool IsInStatus(Enums.CharacterStatus s)
    {
        return s == m_charStatus;
    }
}
