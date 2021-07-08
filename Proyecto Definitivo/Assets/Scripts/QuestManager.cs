using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private static QuestManager manager;
    public List<Quest> quests;
    public List<QuestProgress> questsInProgress;
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
        questsInProgress = new List<QuestProgress>();
    }
    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    public void AddQuest(Quest quest)
    {
        if (!quests.Contains(quest)) quests.Add(quest);
    }
    public void AddInProgress(Quest quest)
    {
        bool has = false;
        foreach (var item in questsInProgress)
        {
            if (item.GetQuestId() == quest.GetId())
            {
                has = true;
            }
        }
        if (!has)
        {
            questsInProgress.Add(new QuestProgress(quest.GetId(),  quest.GetGoal(), quest.GetGoalQuantity()));
        }
    }
    public void CompleteQuest(Quest quest)
    {
        completed.Add(quest);
        foreach (var item in questsInProgress)
        {
            if (quest.GetId() == item.GetQuestId())
            {
                questsInProgress.Remove(item);
                break;
            }
        }
        quests.Remove(quest);
        player.AddXP(quest.GetXp());
    }
}
