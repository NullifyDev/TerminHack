namespace TerminHack;

public class History 
{
    
    private List<string> Entries;

    public History()
    {
        this.Entries = new();
    }

    public void Add(string input) => this.Entries.Add(input);
    public void Clear() => this.Entries.Clear();
}