using System;

[Serializable]
public class BaseAbility {

    string name;
    string description;
    Enums.Ability ability;

    public string Name { get { return name; } }
    public string Description { get { return description; } }
    public Enums.Ability Ability { get { return ability; } }

    public BaseAbility(string _name, string _description, Enums.Ability _ability)
    {
        name = _name;
        description = _description;
        ability = _ability;
    }
}
