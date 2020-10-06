using System;
namespace Talk.CDR.FileReader.Entities
{
    public class TalkBot
    {
        public TalkBot(DateTime data)
        {
            Data = data;
        }

        public DateTime Data { get; set; }
        public string Tipo { get; set; }
        public string Risco { get; set; }
        public long Quantidade { get; set; }
        public double Tarifa { get; set; }
        public double ValorTotal { get; set; }
    }
}
