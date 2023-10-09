namespace Whendy;



public class InfoQuery{
    public string? Select { get; set; }
    public string? From { get; set; }
    public string? Join { get; set; }
    public string? Where { get; set; }
    public string? GroupBY { get; set; }
    public string? OrderBY { get; set; }
    public string? Having { get; set; }
    public string? Sql { get; set; }
    public string? FilterBy { get; set; }

}

public abstract class Query
{
    public abstract string Parse(string query);
}