using CnSharp.Updater.Util;

namespace CnSharp.Updater.CLI.CommandHandlers
{
    internal class GlobalSourceCommandHandler
    {
        public static async Task Set(string source)
        {
            try
            {
                await HttpUtil.CheckUrl(source);
            }
            catch (Exception e)
            {
                ConsoleExtensions.WriteError(e.Message);
                return;
            }
           
           var settings = Settings.Load() ?? new Settings();
           settings.GlobalSource = source;
           settings.Save();
           Console.WriteLine("Global source set successfully.");
        }

        public static async Task Remove()
        {
            var settings = Settings.Load();
            if (settings == null)
            {
                Console.WriteLine("Global source is not set.");
                return;
            }
            settings.GlobalSource = null;
            settings.Save();
            Console.WriteLine("Global source removed.");
        }
    }
}