public sealed class Store : Singleton<Store>
{
    public string Name { get; private set; } = "initial_name";

    public void setName(string name)
    {
        Name = name;
    }
}