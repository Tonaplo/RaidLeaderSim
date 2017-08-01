using System;

[Serializable]
public class BaseCharacter {

    string name;
    int baseHealth;
    int maxHealth;

    public string GetName() { return name; }
    public int GetMaxHealth() { return maxHealth; }
    protected void SetMaxHealth(int newMax) { maxHealth = newMax; }
    protected void SetBaseHealth(int newBase) { baseHealth = newBase; }

    public BaseCharacter() { }

    public BaseCharacter(string _name)
    {
        name = _name;
        baseHealth = 1;
    }

    public BaseCharacter(string _name, int _baseHealth)
    {
        name = _name;
        baseHealth = _baseHealth;
    }

    public virtual void CalculateMaxHealth() {maxHealth = baseHealth;}

}
