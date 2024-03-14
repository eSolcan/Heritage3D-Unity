public class CatalogItem<T>
{
    public string id;
    public string name;
    public T item;

    public CatalogItem(string i, string n, T itm)
    {
        id = i;
        name = n;
        item = itm;
    }
}
