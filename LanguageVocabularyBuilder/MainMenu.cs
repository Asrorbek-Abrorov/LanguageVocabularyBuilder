using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace LanguageVocabularyBuilder
{
    public class Word
    {
        public string Name { get; set; }
        public string Meaning { get; set; }
    }

    public class MainMenu
    {
        private string filePath;

        public MainMenu(string filePath)
        {
            this.filePath = filePath;
        }

        public void Run()
        {
            bool keepRunning = true;
            while (keepRunning)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(
                    new FigletText("* Language Vocabulary Builder *")
                        .LeftJustified()
                        .Color(Color.Green));

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]*** Language Vocabulary Builder Menu ***[/]?")
                        .PageSize(10)
                        .AddChoices(new[]
                        {
                            "Search Word Definition",
                            "Add New Word",
                            "Update Word",
                            "Delete Word",
                            "Show Added Words",
                            "Exit"
                        }));

                switch (choice)
                {
                    case "Search Word Definition":
                        SearchWordDefinition();
                        break;

                    case "Add New Word":
                        AddNewWord();
                        break;

                    case "Update Word":
                        UpdateWord();
                        break;

                    case "Delete Word":
                        DeleteWord();
                        break;

                    case "Show Added Words":
                        ShowAddedWords();
                        break;

                    case "Exit":
                        keepRunning = false;
                        break;
                }

                Console.WriteLine();
                Console.WriteLine("Enter to continue");
                Console.ReadKey();
            }
        }

        private void UpdateWord()
        {
            AnsiConsole.WriteLine();
            var addedWords = LoadWordsFromFile();

            var wordChoices = addedWords.Select(w => w.Name).ToArray();
            var selectedWord = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select the word you want to update:")
                    .PageSize(10)
                    .AddChoices(wordChoices));

            var word = addedWords.Find(w => w.Name.Equals(selectedWord, StringComparison.OrdinalIgnoreCase));
            if (word != null)
            {
                var updateChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select what you want to update:")
                        .PageSize(10)
                        .AddChoices(new[] { "Name", "Definition" }));

                if (updateChoice == "Name")
                {
                    var updatedName = AnsiConsole.Ask<string>($"Enter the updated name for '{word.Name}':");
                    word.Name = updatedName;
                }
                else if (updateChoice == "Definition")
                {
                    var updatedDefinition = AnsiConsole.Ask<string>($"Enter the updated definition for '{word.Name}':");
                    word.Meaning = updatedDefinition;
                }

                AnsiConsole.WriteLine($"The word '{word.Name}' has been updated.");
                SaveWordsToFile(addedWords);
            }
            else
            {
                AnsiConsole.WriteLine($"The selected word '{selectedWord}' does not exist in the vocabulary.");
            }
        }

        private void ShowAddedWords()
        {
            var addedWords = LoadWordsFromFile();

            if (addedWords.Any())
            {
                AnsiConsole.WriteLine("Added Words:");

                var table = new Table();
                table.AddColumn("Word");
                table.AddColumn("Meaning");

                foreach (var word in addedWords)
                {
                    table.AddRow(word.Name, word.Meaning);
                }

                AnsiConsole.Write(table);
            }
            else
            {
                AnsiConsole.WriteLine("No words have been added to the vocabulary.");
                Console.ReadKey();
            }
        }

        private void AddNewWord()
        {
            var addedWords = LoadWordsFromFile();

            var newWord = AnsiConsole.Ask<string>("Enter the new word:");
            var meaning = AnsiConsole.Ask<string>($"Enter the meaning for '{newWord}':");

            var word = new Word { Name = newWord, Meaning = meaning };
            addedWords.Add(word);

            AnsiConsole.WriteLine($"The word '{newWord}' has been added to the vocabulary.");

            SaveWordsToFile(addedWords);
        }

        private void DeleteWord()
        {
            AnsiConsole.WriteLine();
            var addedWords = LoadWordsFromFile();

            var wordChoices = addedWords.Select(w => w.Name).ToArray();
            var selectedWord = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select the word you want to delete:")
                    .PageSize(10)
                    .AddChoices(wordChoices));

            var word = addedWords.Find(w => w.Name.Equals(selectedWord, StringComparison.OrdinalIgnoreCase));
            if (word != null)
            {
                addedWords.Remove(word);
                AnsiConsole.WriteLine($"The word '{selectedWord}' has been deleted from the vocabulary.");
                SaveWordsToFile(addedWords);
            }
            else
            {
                AnsiConsole.WriteLine($"The selected word '{selectedWord}' does not exist in the vocabulary.");
            }
        }

        private void SearchWordDefinition()
        {
            var addedWords = LoadWordsFromFile();

            var wordChoices = addedWords.Select(w => w.Name).ToArray();
            var selectedWord = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select the word to search:")
                    .PageSize(10)
                    .AddChoices(wordChoices));

            var word = addedWords.Find(w => w.Name.Equals(selectedWord, StringComparison.OrdinalIgnoreCase));
            if (word != null)
            {
                AnsiConsole.WriteLine("Definition:");
                AnsiConsole.WriteLine(word.Meaning);
            }
            else
            {
                AnsiConsole.WriteLine($"The selected word '{selectedWord}' does not exist in the vocabulary.");
            }
        }

        private List<Word> LoadWordsFromFile()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<Word>>(json);
            }

            return new List<Word>();
        }

        private void SaveWordsToFile(List<Word> addedWords)
        {
            string json = JsonConvert.SerializeObject(addedWords, Formatting.Indented);
            File.WriteAllText(filePath, json);

            AnsiConsole.WriteLine("The vocabulary has been saved to the file.");
        }
    }
}