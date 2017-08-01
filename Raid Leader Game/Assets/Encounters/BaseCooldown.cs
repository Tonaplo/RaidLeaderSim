using System;

[Serializable]
public class BaseCooldown {

    string name;
    string description;
    Enums.Cooldowns cooldown;

    public void Initialize(string _name, string _description, Enums.Cooldowns _cooldown)
    {
        name = _name;
        description = _description;
        cooldown = _cooldown;
    }
}
