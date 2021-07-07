using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private static QuestManager manager;
    public List<Quest> quests;
    public List<Quest> completed;
    private PlayerController player;
    public static QuestManager Instance 
    {
        get { return manager; }
    }
    private void Awake()
    {
        if (manager != null && manager != this) Destroy(this);
        else manager = this;
        quests = new List<Quest>();
        completed = new List<Quest>();
    }
    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    public void AddQuest(Quest quest)
    {
        if (!quests.Contains(quest)) quests.Add(quest);
    }

    public void CompleteQuest(Quest quest)
    {
        completed.Add(quest);
        quests.Remove(quest);
        player.AddXP(quest.GetXp());
    }
}
