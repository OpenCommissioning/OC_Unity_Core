namespace OC.Data
{
    [System.Serializable]
    public class ProductDataDirectory
    {
        public string Name;
        public string Path;

        public ProductDataDirectory(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}
