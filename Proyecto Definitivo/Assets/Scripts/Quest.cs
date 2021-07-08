using UnityEngine;
[System.Serializable]
public class Quest
{
    
    private static int currentId = 0;
    [SerializeField]
    private int id;
    [SerializeField]
    private string description;
    [SerializeField]
    private string objective;
    [SerializeField]
    private QuestType type;
    [SerializeField]
    private GameObject target;
    [SerializeField]
    private int itemId;
    [SerializeField]
    private int goalQuantity;
    [SerializeField]
    private int xp;
    [SerializeField]
    private string goal;


    public Quest(string desc, string objt, QuestType typex, int xp, string goal)
    {
        id = currentId++;
        description = desc;
        objective = objt;
        type = typex;
        target = null;
        this.xp = xp;
        itemId = -1;
        this.goalQuantity = -1;
        this.goal = goal;
    }
    public Quest(string desc, string objt, QuestType typex, int xp, GameObject targ, string goal,int goalQuantity)
    {
        id = currentId++;
        description = desc;
        objective = objt;
        type = typex;
        target = targ;
        this.xp = xp;
        itemId = -1;
        this.goalQuantity = goalQuantity;
        this.goal = goal;
    }
    public Quest(string desc, string objt, QuestType typex, int xp, int itemId, int itemQuantity, string goal)
    {
        id = currentId++;
        description = desc;
        objective = objt;
        type = typex;
        target = null;
        this.xp = xp;
        this.itemId = itemId;
        this.goalQuantity = itemQuantity;
        this.goal = goal;
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
    public int GetItemId()
    {
        return this.itemId;
    }
    public int GetGoalQuantity()
    {
        return this.goalQuantity;
    }
    public string GetGoal()
    {
        return this.goal;
    }
}

public enum QuestType
{
    Talk, Gather, Kill
}