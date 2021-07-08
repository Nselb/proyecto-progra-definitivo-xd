using UnityEngine;

public class Quester : MonoBehaviour
{
    [Header("Obligatorios")]
    public string description;
    public string objective;
    public QuestType type;
    public int xp;
    public string goal;
    public int goalQuantity;
    [Space]
    [Header("Talk/Kill type")]
    public GameObject target;
    [Space]
    [Header("Gather type")]
    public int itemId;

    public bool inProgress;
    private Quest quest;

    private void Start()
    {
        if (target != null) quest = new Quest(description, objective, type, xp, target, goal, goalQuantity);
        else if (itemId != -1) quest = new Quest(description, objective, type, xp, itemId, goalQuantity, goal);
        else if (type == QuestType.Kill) quest = new Quest(description, objective, type, xp, target, goal, goalQuantity);
        else quest = new Quest(description, objective, type, xp, goal);
    }
    public Quest GetQuest()
    {
        if (type == QuestType.Talk)
        {
            quest.GetTarget().tag = "TalkTarget";
            quest.GetTarget().layer = LayerMask.NameToLayer("Interactable");
        }
        inProgress = true;
        return quest;
    }
    public void CompleteGatherQuest()
    {
        Destroy(this);
    }
}