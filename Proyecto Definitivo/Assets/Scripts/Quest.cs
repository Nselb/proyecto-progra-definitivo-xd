public class Quest
{
    private static int currentId;
    public int Id { get; set; }
    public string Description { get; set; }
    public string Objective { get; set; }
    public QuestType Type {get; set;}
    public Quest(string desc, string objt, QuestType type)
    {
        Id = currentId++;
        Description = desc;
        Objective = objt;
        Type = type;
    }
}

public enum QuestType
{
    Talk, Chrono, Gather
}