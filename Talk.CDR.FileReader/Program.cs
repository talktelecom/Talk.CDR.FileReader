using System;
using System.Threading.Tasks;
using Talk.CDR.FileReader.Enum;
using Talk.CDR.FileReader.Services;
using static System.Net.Mime.MediaTypeNames;

namespace FIlerReader
{


    public class Arquivo
    {
        public Layout layout { get; set; }
        public string FileName { get; set; }
    }


    class Program
    {
        static async Task Main(string[] args)
        {

            ArquivoService service = new ArquivoService();

            string fileName = string.Empty;
            string layoutKey= string.Empty;

            if (args != null && args.Length == 1)
            {
                Console.WriteLine("Escolha o layout: \n1 - Claro\n2 - OI\n3 - IPCORP\n4 - TalkTelecom\n5- TalkBot");
                layoutKey = Console.ReadKey().KeyChar.ToString();
            }

            if (args != null && args.Length > 1)
                layoutKey = args[1];

            Layout layout = Layout.Undefined;
            switch (layoutKey)
            {
                case "1":
                    layout = Layout.Claro;
                    break;
                case "2":
                    layout = Layout.OI;
                    break;
                case "3":
                    layout = Layout.IPCORP;
                    break;
                case "4":
                    layout = Layout.TALK;
                    break;
                case "5":
                    layout = Layout.TablkBot;
                    break;
                default:
                    Console.Write("Layout inválido!");
                    break;
            }

            bool showDetail = true;

            if (args != null && args.Length > 0)
                fileName = args[0];

            if(layout != Layout.Undefined)
                await service.ReadFile(fileName, showDetail, layout);
           
        }

    }
}
