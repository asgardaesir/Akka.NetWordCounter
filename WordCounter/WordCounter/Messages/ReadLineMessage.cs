namespace WordCounter
{
    public class ReadLineMessage
    {
        public string LineFromFile { get; private set; }

        public ReadLineMessage(string lineFromFile)
        {
            LineFromFile = lineFromFile;
        }
    }
}