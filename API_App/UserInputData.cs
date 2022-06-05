namespace API_App
{
    public class UserInputData
    {
        public long? Id;
        public string Name;
        public string Type;

        public UserInputData(long? id, string name, string type)
        {
            Id = id;
            Name = name;
            Type = type;
        }
    }
}
