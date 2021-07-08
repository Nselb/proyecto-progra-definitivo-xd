using UnityEngine;

public class Quester : MonoBehaviour
{
    [Header("Obligatorios")]
    public string description;
    public string objective;
    public QuestType type;
    public int xp;
    [Space]
    [Header("Talk type")]
    public GameObject target;
    [Space]
    [Header("Gather type")]
    public int itemId;
    public int itemQuantity;
    private Quest quest;

    private void Start()
    {
        if (target != null) quest = new Quest(description, objective, type, xp,target);
        else if (itemId != -1) quest = new Quest(description, objective, type, xp, itemId,itemQuantity);
        else quest = new Quest(description, objective, type, xp);
    }
    public Quest GetQuest()
    {
        if (type == QuestType.Talk)
        {
            quest.GetTarget().tag = "TalkTarget";
            quest.GetTarget().layer = LayerMask.NameToLayer("Interactable");
            Destroy(this, 1);
        }
        return quest;
    }
    public void CompleteGatherQuest()
    {
        Destroy(this);
    }
}