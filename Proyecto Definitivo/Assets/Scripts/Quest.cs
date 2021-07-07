using UnityEngine;
[System.Serializable]
public class Quest
{
    private static int currentId;
    public int id;
    public string description;
    public string objective;
    public QuestType type;
    public GameObject target;
    public Quest(string desc, string objt, QuestType typex)
    {
        id = currentId++;
        description = desc;
        objective = objt;
        type = typex;
        target = null;
    }
    public Quest(string desc, string objt, QuestType typex, GameObject targ)
    {
        id = currentId++;
        description = desc;
        objective = objt;
        type = typex;
        target = targ;
    }
}

public enum QuestType
{
    Talk, Chrono, Gather
}