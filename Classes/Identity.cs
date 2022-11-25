namespace Initiative.Classes;

public class Identity
{
    private int identity;
    public int Next => identity++;

    public Identity()
    {
        identity = 0;
    }
}