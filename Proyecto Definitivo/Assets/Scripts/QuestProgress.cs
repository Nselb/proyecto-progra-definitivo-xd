public class QuestProgress
{
    private int questId;
    private string questProgress;
    public QuestProgress(int questId, string questProgress)
    {
        this.questId = questId;
        this.questProgress = questProgress;
    }
    public string GetProgress()
    {
        return this.questProgress;
    }
    public int GetQuestId()
    {
        return this.questId;
    }
    public void UpdateProgress(int progress)
    {
        
    }
}