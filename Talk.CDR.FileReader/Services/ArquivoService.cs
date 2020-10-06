using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Talk.CDR.FileReader.Entities;
using Talk.CDR.FileReader.Enum;

namespace Talk.CDR.FileReader.Services
{
    public class ArquivoService
    {
        public ArquivoService()
        {
        }

        public async Task ReadFile(string fileName, bool showDetail = false, Layout layout = Layout.OI)
        {
            Console.Clear();
            int index = 1;
            ChamadaResumo resumoChamada = new ChamadaResumo();
            TalkBotResumo resumoTalkBot = new TalkBotResumo();

            await Task.Run(() =>
            {
                var watch = Stopwatch.StartNew();
                Console.WriteLine($"Inicio Processamento v2({Path.GetFileName(fileName)}): {DateTime.Now:dd/MM/yyyy HH:mm:ss}");

                CultureInfo provider = CultureInfo.InvariantCulture;
                string line = string.Empty;


                string md5 = string.Empty;
                string sha256 = string.Empty;
                Console.WriteLine($"Calculando HASH MD5");
                md5 = GetMd5File(fileName);
                Console.WriteLine($"Calculando HASH SHA256");
                sha256 = GetSHA256File(fileName);

                using (FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (BufferedStream buffer = new BufferedStream(stream))
                using (StreamReader reader = new StreamReader(buffer))
                {
                    Console.WriteLine($"HASH MD5:{md5}");
                    Console.WriteLine($"HASH SHA256: {sha256}");

                    while ((line = reader.ReadLine()) != null)
                    {

                        Console.Write("");
                        Console.Write($"\rLendo a linha : {index}");
                        switch (layout)
                        {
                            case Layout.Claro:
                                ProcessarLayoutClaro(resumoChamada, line, provider);
                                break;
                            case Layout.OI:
                                ProcessarLayoutOI(resumoChamada, line, provider);
                                break;
                            case Layout.TALK:
                                if (index > 2)
                                    ProcessarLayoutTalk(resumoChamada, line, provider);
                                break;
                            case Layout.IPCORP:
                                if (index > 1)
                                    ProcessarLayoutIPCorp(resumoChamada, line, provider);
                                break;
                            case Layout.TablkBot:
                                if (index > 1)
                                    ProcessarLayoutTalkBot(resumoTalkBot, line, provider);
                                break;
                            default:
                                break;
                        }
                        index++;
                    }

                    watch.Stop();
                    NumberFormatInfo culture = new CultureInfo("pt-BR").NumberFormat;
                    Console.WriteLine($"\nFim Processamento: {DateTime.Now:dd/MM/yyyy HH:mm:ss} - {watch.ElapsedMilliseconds / 1000}s");

                    if (layout != Layout.TablkBot)
                    {

                        if (showDetail)
                        {
                            Console.WriteLine("");
                            Console.WriteLine($"Quantidade Total:{resumoChamada.items.Sum(e => e.QuantidadeTotal).ToString("n0", culture)}");
                            Console.WriteLine($"Minutos Total   :{resumoChamada.items.Sum(e => e.MinutosTotal).ToString("n0", culture)}");
                            Console.WriteLine("");
                            Console.WriteLine($"{"Data",-15}|{"Quantidade",-15}|{"Minutos",-15}");
                            foreach (var item in resumoChamada.items.OrderBy(e => e.Data))
                            {
                                Console.WriteLine($"{item.Data:dd/MM/yyyy}     {item.QuantidadeTotal.ToString("n0", culture),15} {item.MinutosTotal.ToString("n0", culture),15}");
                            }
                            Console.WriteLine("");
                        }

                        Console.WriteLine($"Gerando arquivo de resumo em {Path.GetDirectoryName(fileName)}/resumo-{Path.GetFileNameWithoutExtension(fileName)}.csv");
                        using (StreamWriter sw = new StreamWriter($"{Path.GetDirectoryName(fileName)}/resumo-{Path.GetFileName(fileName)}.csv"))
                        {
                            sw.WriteLine("ArquivoNome;MD5; SHA256;FornecedorId;Fornecedor Nome; Data; Quantidade Movel; Minutos Movel; Quantidade Fixo; Minutos Fixo;  Quantidade Total; Minutos Total;Tempo Processamento; Operadora");
                            foreach (var item in resumoChamada.items)
                            {
                                sw.WriteLine($"{Path.GetFileName(fileName)};{md5};{sha256};{layout.GetHashCode()};{layout};{item.Data:yyyy-MM-dd};{item.QuantidadeMovel.ToString("n0", culture)};{item.MinutosMovel.ToString("n1", culture)};{item.QuantidadeFixo.ToString("n0", culture)};{item.MinutosFixo.ToString("n1", culture)};{item.QuantidadeTotal.ToString("n0", culture)};{item.MinutosTotal.ToString("n1", culture)};{watch.ElapsedMilliseconds}ms;{item.Operadora}");
                            }
                        }
                        Console.WriteLine($"Fim do processamento.");

                    }
                    else
                    {
                        if (showDetail)
                        {
                            Console.WriteLine("");
                            Console.WriteLine($"Quantidade Total:{resumoTalkBot.items.Sum(e => e.Quantidade).ToString("n0", culture)}");
                            Console.WriteLine($"Valor Total   :{resumoTalkBot.items.Sum(e => e.ValorTotal).ToString("n0", culture)}");
                            Console.WriteLine("");
                            Console.WriteLine($"{"Data",-15}|{"Quantidade",-15}|{"Valor",-15}");
                            foreach (var item in resumoTalkBot.items.OrderBy(e => e.Data))
                            {
                                Console.WriteLine($"{item.Data:dd/MM/yyyy}     {item.Quantidade.ToString("n0", culture),15} {item.ValorTotal.ToString("c2", culture),15}");
                            }
                            Console.WriteLine("");
                        }

                        Console.WriteLine($"Gerando arquivo de resumo em {Path.GetDirectoryName(fileName)}/resumo.csv");

                        bool append = File.Exists($"{Path.GetDirectoryName(fileName)}/resumo.csv");

                        using (StreamWriter sw = new StreamWriter($"{Path.GetDirectoryName(fileName)}/resumo.csv", append))
                        {
                            if(!append)
                                sw.WriteLine("ArquivoNome;MD5; SHA256;Data; Tipo; Risco; Tarifa; Quantidade;  Valor");

                            foreach (var item in resumoTalkBot.items)
                            {
                                sw.WriteLine($"{Path.GetFileName(fileName)};{md5};{sha256};{item.Data:yyyy-MM-dd};{item.Tipo};{item.Risco};{item.Tarifa.ToString("n2", culture)};{item.Quantidade.ToString("n0", culture)};{item.ValorTotal.ToString("n2", culture)}");
                            }
                        }
                        Console.WriteLine($"Fim do processamento.");
                    }
                }
            });
        }

        private string GetMd5File(string fileName)
        {
            string md5 = string.Empty;
            using (FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var hashMd5 = MD5.Create().ComputeHash(stream);
                md5 = BitConverter.ToString(hashMd5).Replace("-", "").ToLowerInvariant();
            }

            return md5;
        }

        private string GetSHA256File(string fileName)
        {
            string sha256 = string.Empty;
            using (FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var hashSha256 = SHA256.Create().ComputeHash(stream);
                sha256 = BitConverter.ToString(hashSha256).Replace("-", "").ToLowerInvariant();
            }

            return sha256;
        }

        private void ProcessarLayoutClaro(ChamadaResumo resumo, string line, CultureInfo provider)
        {
            if (line.Substring(0, 2) == "30")
            {
                string format = "yyyyMMdd";
                DateTime data = DateTime.ParseExact(line.Substring(100 - 1, 8), format, provider);

                double duracao = Convert.ToDouble(line.Substring(189 - 1, 7)) / 60;
                double valor = Convert.ToDouble(line.Substring(238 - 1, 13)) / 100;
                string classe = line.Substring(199 - 1, 3);

                resumo.AdicionarResumo(data, duracao, classe, valor);

            }
        }

        private void ProcessarLayoutOI(ChamadaResumo resumo, string line, CultureInfo provider)
        {
            if (line.Substring(0, 1) == "3")
            {
                string format = "yyyyMMdd";
                DateTime data = DateTime.ParseExact(line.Substring(89 - 1, 8), format, provider);

                double duracao = Convert.ToDouble(line.Substring(172 - 1, 6)) / 10;
                double valor = Convert.ToDouble(line.Substring(272 - 1, 13)) / 100;
                string classe = line.Substring(178 - 1, 3);

                resumo.AdicionarResumo(data, duracao, classe, valor);
            }
        }

        private void ProcessarLayoutTalk(ChamadaResumo resumo, string line, CultureInfo provider)
        {
            var columns = line.Split(';');

            if (columns.Length == 20)
            {
                string format = "yyyy-MM-dd";
                DateTime data = DateTime.ParseExact(columns[9].Substring(0, 10), format, provider).Date;

                double duracao = TimeSpan.Parse(columns[17].Substring(0, 8)).TotalSeconds / 60;

                double valor = Convert.ToDouble(columns[18]);
                string classe = columns[15] == "M" ? "VC1" : "LOC";

                string operadora = columns[19];

                resumo.AdicionarResumoOperadora(data, duracao, classe, valor, operadora);
            }
        }

        private void ProcessarLayoutTalkBot(TalkBotResumo resumo, string line, CultureInfo provider)
        {
            var columns = line.Split(';');

            if (columns.Length == 11)
            {
                string format = "yyyy-MM-dd";
                DateTime data = DateTime.ParseExact(columns[1].Substring(0, 10), format, provider).Date;

                double valor = Convert.ToDouble(columns[9].Replace(",","."));
                string tipo = columns[6];
                string risco = columns[7];

                resumo.AdicionarResumo(data, tipo, risco, valor);
            }
        }

        private void ProcessarLayoutIPCorp(ChamadaResumo resumo, string line, CultureInfo provider)
        {
            var columns = line.Split(';');

            if (columns.Length == 20)
            {
                string format = "yyyy-MM-dd";
                DateTime data = DateTime.ParseExact(columns[10].Substring(0, 10), format, provider).Date;

                double duracao = TimeSpan.Parse(columns[18].Substring(0, 8)).TotalSeconds / 60;

                double valor = Convert.ToDouble(columns[19]);
                string classe = columns[17] == "M" ? "VC1" : "LOC";

                resumo.AdicionarResumo(data, duracao, classe, valor);
            }
        }
    }
}
