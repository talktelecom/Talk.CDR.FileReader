using System;
using System.Collections.Generic;

namespace Talk.CDR.FileReader.Entities
{
    public class TalkBotResumo
    {
        public TalkBotResumo()
        {
            items = new List<TalkBot>();
        }
        public List<TalkBot> items { get; set; }

        public void AdicionarResumo(DateTime data, string tipo, string risco, double valor)
        {
            //var tarifa = Math.Round(valor);
            var item = items.Find(e => e.Data == data && e.Risco == risco && e.Tipo == tipo);

            if (item == null)
            {
                item = new TalkBot(data);
                items.Add(item);
            }

            item.Risco = risco;
            item.Tipo = tipo;
            item.Tarifa = valor;
            item.Quantidade += 1;
            item.ValorTotal += valor;
        }
    }
}
