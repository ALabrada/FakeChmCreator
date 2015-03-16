using CLAP;

namespace FakeChmCreator.CmdLine
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.RunConsole<Tools>(args);
        }
    }
}
