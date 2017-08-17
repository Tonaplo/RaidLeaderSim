using System;

[Serializable]
public class BaseCooldown {

    string name;
    string description;
    Enums.Cooldowns cooldown;

    public string Name { get { return name; } }
    public string Description { get { return description; } }
    public Enums.Cooldowns Cooldown { get { return cooldown; } }

    public void Initialize(string _name, string _description, Enums.Cooldowns _cooldown)
    {
        name = _name;
        description = _description;
        cooldown = _cooldown;
    }
}
