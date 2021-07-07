using UnityEngine;

public class Quester : MonoBehaviour
{
    [Header("Obligatorios")]
    public string description;
    public string objective;
    public QuestType type;
    public int xp;
    [Space]
    [Header("Solo si es de tipo talk")]
    public GameObject target;
    private Quest quest;

    private void Start()
    {
        if (target != null) quest = new Quest(description, objective, type, xp,target);
        else quest = new Quest(description, objective, type, xp);
    }
    public Quest GetQuest()
    {
        if (type == QuestType.Talk)
        {
            quest.GetTarget().tag = "TalkTarget";
            quest.GetTarget().layer = LayerMask.NameToLayer("Interactable");
        }
        return quest;
    }
}