using System;
using System.Collections.Generic;

namespace Talk.CDR.FileReader.Entities
{
    public class ChamadaResumo
    {
        public ChamadaResumo()
        {
            items = new List<Chamada>();
        }
        public List<Chamada> items { get; set; }

        public void AdicionarResumo(DateTime data, double minutos, string classe, double valor)
        {
            var item = items.Find(e => e.Data == data);

            if (item == null)
            {
                item = new Chamada(data);
                items.Add(item);
            }

            if (classe.Contains("VC"))
            {
                item.MinutosMovel += minutos;
                item.QuantidadeMovel += 1;
            }
            else
            {
                item.MinutosFixo += minutos;
                item.QuantidadeFixo += 1;
            }
        }

        public void AdicionarResumoOperadora(DateTime data, double minutos, string classe, double valor, string operadora)
        {
            var item = items.Find(e => e.Operadora == operadora && e.Data == data);

            if (item == null)
            {
                item = new Chamada(operadora, data);
                items.Add(item);
            }

            if (classe.Contains("VC"))
            {
                item.MinutosMovel += minutos;
                item.QuantidadeMovel += 1;
            }
            else
            {
                item.MinutosFixo += minutos;
                item.QuantidadeFixo += 1;
            }
        }
    }
}
