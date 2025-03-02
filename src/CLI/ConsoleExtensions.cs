namespace CnSharp.Updater.CLI
{
    public static class ConsoleExtensions
    {
        public static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
