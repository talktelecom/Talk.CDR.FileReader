using System;
namespace Talk.CDR.FileReader.Entities
{
    public class Chamada
    {
        public Chamada(DateTime data)
        {
            Data = data;
        }

        public Chamada(string operadora, DateTime data)
        {
            Operadora = operadora;
            Data = data;
        }
        public DateTime Data { get; set; }
        public string Operadora { get; set; }
        public long QuantidadeFixo { get; set; }
        public double MinutosFixo { get; set; }
        public long QuantidadeMovel { get; set; }
        public double MinutosMovel { get; set; }
        public long QuantidadeTotal { get { return QuantidadeFixo + QuantidadeMovel; } }
        public double MinutosTotal { get { return MinutosFixo + MinutosMovel; } }
    }
}
