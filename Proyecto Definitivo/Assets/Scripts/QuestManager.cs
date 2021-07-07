using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private static QuestManager manager;
    public List<Quest> Quests { get; set; }
    public List<Quest> Completed { get; set;}
    public static QuestManager Instance 
    {
        get { return manager; }
    }
    private void Awake()
    {
        if (manager != null && manager != this) Destroy(this);
        else manager = this;
        Quests = new List<Quest>();
    }

    public void AddQuest(Quest quest)
    {
        if (!Quests.Contains(quest)) Quests.Add(quest);
    }

    public void CompleteQuest(Quest quest)
    {
        Completed.Add(quest);
    }
}
