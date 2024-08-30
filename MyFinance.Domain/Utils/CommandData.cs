namespace MyFinance.Domain.Utils
{
    public class CommandData
    {
        public string Command { get; set; }
        public dynamic Data { get; set; }

        public CommandData(string command, dynamic data)
        {
            Command = command;
            Data = data;
        }
    }
}