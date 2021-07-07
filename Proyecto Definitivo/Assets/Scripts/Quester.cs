using UnityEngine;

public class Quester : MonoBehaviour
{
    public string description;
    public string objective;
    public QuestType type;
    private Quest quest;

    private void Start()
    {
        quest = new Quest(description, objective, type);
    }
    public Quest GetQuest()
    {
        return quest;
    }
}