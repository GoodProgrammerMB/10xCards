using System.Text.Json;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace FlashCard.App
{
    public static class FlashcardFormatter
    {
        public static string FixJson(string brokenJson)
        {
            // Dodaj przecinki pomiędzy zamknięciem wartości a rozpoczęciem nowego klucza
            brokenJson = Regex.Replace(
                brokenJson,
                "(\"\\s*:\\s*\"[^\"]*\")\\s*(\"[^\"]*\"\\s*:\\s*)",
                "$1,$2"
            );

            // Upewnij się, że string jest zawinięty w nawiasy klamrowe
            brokenJson = brokenJson.Trim();
            if (!brokenJson.StartsWith("{"))
                brokenJson = "{" + brokenJson;
            if (!brokenJson.EndsWith("}"))
                brokenJson += "}";

            return brokenJson;
        }
        // 1. Formatuje fiszkę
        public static string FormatFlashcard(string json)
        {
            if (json.Contains("Słowo:"))
            {
                return json;
            }
            json = FixJson(json);
            try
            {
                var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                string word = root.TryGetProperty("word", out var w) ? w.GetString() : null;
                string translation = root.TryGetProperty("translation", out var t) ? t.GetString() : null;
                string definition = root.TryGetProperty("definition", out var d) ? d.GetString() : null;
                string example = root.TryGetProperty("example", out var e) ? e.GetString() : null;
                string exampleTranslation = root.TryGetProperty("example_Translation", out var et) ? et.GetString() : null;

                // Składanie tekstu tylko z dostępnych fragmentów
                var parts = new List<string>();
                if (word != null || translation != null)
                    parts.Add($"Słowo: <b>{word}</b> - {translation}");
                if (definition != null)
                    parts.Add($"Definicja: {definition}");
                if (example != null)
                    parts.Add($"Przykład: {example}");
                if (exampleTranslation != null)
                    parts.Add($"Przykładowe tłumaczenie: {exampleTranslation}");

                return string.Join(" <br/> ", parts);
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        // 2. Formatuje przykładowe tłumaczenie
        public static string FormatExampleTranslation(string json)
        {
            if (json.Contains("Przykładowe"))
            {
                return json;
            }
            json = FixJson(json);
            try
            {
                var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                string exampleTranslation = root.GetProperty("example_Translation").GetString();

                return $"Przykładowe tłumaczenie: {exampleTranslation}";
            }catch(Exception ex)
            {
                var d = ex;
                return "";
            }
        }

        public static string FormatPage(string input)
        {
            try
            {
                string fixedJson = Regex.Replace(
                    input,
                    @"(?<![\,\{\n])\s*""(word|translation|definition|example)""",
                    @",""$1"""
                );
                var doc = JsonDocument.Parse(fixedJson);
                var root = doc.RootElement;

                string word = root.GetProperty("word").GetString();
                string translation = root.GetProperty("translation").GetString();
                string definition = root.GetProperty("definition").GetString();
                string example = root.GetProperty("example").GetString();

                string html = $"SŁOWO: <b>{word}</b> - {translation} <br/> Definicja: {definition} <br/> Przykład: {example}";
                return html;
            }
            catch (Exception)
            {
                // Jeśli nie jest poprawnym JSON-em, zwróć oryginalny tekst
                return input;
            }
        }
    }
}
