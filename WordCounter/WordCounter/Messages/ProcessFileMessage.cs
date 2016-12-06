namespace WordCounter
{
    public class ProcessFileMessage
    {
        public string File { get; private set; }

        public ProcessFileMessage(string file)
        {
            File = file;
        }
    }
}