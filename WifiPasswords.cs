using System;
using System.Diagnostics;
using System.Threading;

class WifiPasswords
{
    static void Print(string text, ConsoleColor color = ConsoleColor.Green, bool newline = true)
    {
        Console.ForegroundColor = color;
        foreach (char c in text)
        {
            Console.Write(c);
            Thread.Sleep(8);
        }
        if (newline) Console.WriteLine();
        Console.ResetColor();
    }

    static void Main()
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Clear();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(@"  ██╗    ██╗██╗███████╗██╗    ██████╗  █████╗ ███████╗███████╗");
        Console.WriteLine(@"  ██║    ██║██║██╔════╝██║    ██╔══██╗██╔══██╗██╔════╝██╔════╝");
        Console.WriteLine(@"  ██║ █╗ ██║██║█████╗  ██║    ██████╔╝███████║███████╗███████╗");
        Console.WriteLine(@"  ██║███╗██║██║██╔══╝  ██║    ██╔═══╝ ██╔══██║╚════██║╚════██║");
        Console.WriteLine(@"  ╚███╔███╔╝██║██║     ██║    ██║     ██║  ██║███████║███████║");
        Console.WriteLine(@"   ╚══╝╚══╝ ╚═╝╚═╝     ╚═╝    ╚═╝     ╚═╝  ╚═╝╚══════╝╚══════╝");
        Console.ResetColor();
        Console.WriteLine();

        Print("[*] Initializing...", ConsoleColor.DarkGreen);
        Thread.Sleep(300);
        Print("[*] Accessing WLAN module...", ConsoleColor.DarkGreen);
        Thread.Sleep(300);
        Print("[*] Fetching network profiles...", ConsoleColor.DarkGreen);
        Thread.Sleep(400);

        string profilesOutput = RunCommand("netsh wlan show profiles");

        if (string.IsNullOrEmpty(profilesOutput))
        {
            Print("[!] ERROR: no profiles found. Is WiFi enabled?", ConsoleColor.Red);
            return;
        }

        var profiles = new System.Collections.Generic.List<string>();
        foreach (string line in profilesOutput.Split(new char[]{'\r','\n'}, StringSplitOptions.RemoveEmptyEntries))
        {
            int colonIdx = line.IndexOf(':');
            if (colonIdx < 0) continue;
            string label = line.Substring(0, colonIdx).Trim().ToLower();
            string value = line.Substring(colonIdx + 1).Trim();
            if (string.IsNullOrEmpty(value) || value.StartsWith("<") || value.StartsWith("-")) continue;
            if (label.Contains("profile") || label.Contains("profil"))
                profiles.Add(value);
        }

        if (profiles.Count == 0)
        {
            Print("[!] No saved networks found.", ConsoleColor.Yellow);
            return;
        }

        Print("[+] " + profiles.Count + " networks found. Decrypting...", ConsoleColor.Green);
        Thread.Sleep(600);
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("  {0,-35} {1}", "SSID", "PASSWORD");
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("  " + new string('-', 60));
        Console.ResetColor();

        foreach (string profile in profiles)
        {
            string details = RunCommand("netsh wlan show profile name=\"" + profile + "\" key=clear");
            string password = "(not available)";

            foreach (string line in details.Split(new char[]{'\r','\n'}, StringSplitOptions.RemoveEmptyEntries))
            {
                int colonIdx = line.IndexOf(':');
                if (colonIdx < 0) continue;
                string label = line.Substring(0, colonIdx).Trim().ToLower();
                string value = line.Substring(colonIdx + 1).Trim();
                if (label.Contains("key content") || label.Contains("contenuto chiave"))
                {
                    password = value;
                    break;
                }
                if ((label.Contains("authentication") || label.Contains("autenticazione")) && value.ToLower().Contains("open"))
                    password = "(open network)";
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("  ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("{0,-35} ", profile);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(password);
            Thread.Sleep(120);
        }

        Console.WriteLine();
        Print("[+] Operation completed.", ConsoleColor.DarkGreen);
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("                                    ~ by danilo22");
        Console.ResetColor();
        Console.WriteLine();
        Thread.Sleep(Timeout.Infinite);
    }

    static string RunCommand(string command)
    {
        try
        {
            var psi = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using (var proc = Process.Start(psi))
            {
                string output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();
                return output;
            }
        }
        catch { return string.Empty; }
    }
}
