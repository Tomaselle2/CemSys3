namespace CemSys3.Interfaces.PlantillaTramite
{
    public interface ITemplateProcessor
    {
        string Procesar(string html, Dictionary<string, string> variables);

    }
}
