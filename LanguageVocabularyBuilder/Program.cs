namespace LanguageVocabularyBuilder;

class Program
{
    static void Main(string[] args)
    {
        var filePath = Path.GetFullPath("../../../../LanguageVocabularyBuilder/Database/Words.json");

        var menu = new MainMenu(filePath);
        menu.Run();
    }
}