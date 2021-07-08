public class QuestProgress
{
    private int questId;
    private string questProgress;
    private int currentProgress;
    private int questQuantity;
    public QuestProgress(int questId, string questProgress, int questQuantity)
    {
        this.questId = questId;
        this.questProgress = questProgress;
        this.questQuantity = questQuantity;
    }
    public string GetProgress()
    {
        return this.questProgress;
    }
    public int GetCurrentProgress()
    {
        return this.currentProgress;
    }
    public int GetQuestId()
    {
        return this.questId;
    }
    public void UpdateProgress(int progress)
    {
        currentProgress = progress;
    }
    public override string ToString()
    {
        return $"- {questProgress} {currentProgress}/{questQuantity}";
    }
}