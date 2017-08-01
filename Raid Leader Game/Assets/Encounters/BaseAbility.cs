using System;

[Serializable]
public class BaseAbility {

    string name;
    string description;
    Enums.Ability ability;

    public string Name() { return name; }
    public string Description() { return description; }
    public Enums.Ability Ability() {return ability;}

    public BaseAbility(string _name, string _description, Enums.Ability _ability)
    {
        name = _name;
        description = _description;
        ability = _ability;
    }
}
