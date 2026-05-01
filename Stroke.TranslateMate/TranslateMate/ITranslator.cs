using System.Threading.Tasks;

namespace Stroke
{
    public interface ITranslator
    {
        Task<string> TranslateAsync(string text, string sourceLanguage, string targetLanguage);
    }
}