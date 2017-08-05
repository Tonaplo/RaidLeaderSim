using System;

[Serializable]
public class Raider : BaseCharacter {

    
    RaiderStats stats;
    public RaiderStats RaiderStats() { return stats; }

    
    public Raider(string _name, RaiderStats _stats) : base(_name) {
        stats = _stats;
    }

    public void RecalculateRaider()
    {
        stats.ComputeAverageThroughput();
        stats.ComputeThroughput();
        stats.ComputeSkillThisAttempt();
        CalculateMaxHealth();
    }

    //Calculate Max Health based on Class
    public override void CalculateMaxHealth()
    {
        float value = (int)Enums.StaticValues.baseRaiderHealth;
        SetBaseHealth((int)value);

        switch (stats.GetRole())
        {
            case Enums.CharacterRole.Tank:
                //Guardians have 15% more health
                if(stats.GetCurrentSpec() == Enums.CharacterSpec.Guardian)
                    value *= 2.0f * 1.15f;
                else
                    value *= 2.0f;
                break;
            case Enums.CharacterRole.Healer:
                value *= 0.8f;
                break;
            case Enums.CharacterRole.RangedDPS:
                value *= 0.9f;
                break;
            case Enums.CharacterRole.MeleeDPS:
                value *= 1.1f;
                break;
            default:
                break;
        }
        value += RaiderStats().GetGearLevel();
        SetMaxHealth((int)value);
    }
}
