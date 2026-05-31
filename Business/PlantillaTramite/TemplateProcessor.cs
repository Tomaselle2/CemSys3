using CemSys3.Interfaces.PlantillaTramite;

namespace CemSys3.Business.PlantillaTramite
{
    public class TemplateProcessor : ITemplateProcessor
    {
        public string Procesar(string html, Dictionary<string, string> variables)
        {
            foreach (var v in variables)
            {
                html = html.Replace($"{{{v.Key}}}", v.Value);
            }

            return html;
        }
    }
}
