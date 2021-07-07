using UnityEngine;
[System.Serializable]
public class Quest
{
    private static int currentId = 0;
    private int id;
    private string description;
    private string objective;
    private QuestType type;
    private GameObject target;
    private int xp;

    public Quest(string desc, string objt, QuestType typex, int xp)
    {
        id = currentId++;
        description = desc;
        objective = objt;
        type = typex;
        target = null;
        this.xp = xp;
    }
    public Quest(string desc, string objt, QuestType typex, int xp, GameObject targ)
    {
        id = currentId++;
        description = desc;
        objective = objt;
        type = typex;
        target = targ;
        this.xp = xp;
    }

    public int GetXp()
    {
        return this.xp;
    }
    public QuestType GetQuestType()
    {
        return this.type;
    }
    public string GetObjective()
    {
        return this.objective;
    }
    public string GetDescription()
    {
        return this.description;
    }
    public GameObject GetTarget()
    {
        return this.target;
    }
}

public enum QuestType
{
    Talk, Chrono, Gather
}