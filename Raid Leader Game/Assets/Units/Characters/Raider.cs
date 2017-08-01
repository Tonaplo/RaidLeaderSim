using System;

[Serializable]
public class Raider : BaseCharacter {

    
    RaiderStats stats;
    public RaiderStats RaiderStats() { return stats; }

    
    public Raider(string _name, RaiderStats _stats) : base(_name) {
        stats = _stats;
    }

    //Calculate Max Health based on Class
    //this should be based on gear and the like later
    public override void CalculateMaxHealth()
    {
        float value = (int)Enums.StaticValues.baseRaiderHealth;
        SetBaseHealth((int)value);

        switch (stats.GetRole())
        {
            case Enums.CharacterRole.Tank:
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
