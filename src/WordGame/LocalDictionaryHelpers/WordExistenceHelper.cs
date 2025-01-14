﻿using System;
using System.Linq;
using Chat.WordGame.WordHelpers;

namespace Chat.WordGame.LocalDictionaryHelpers
{
    public class WordExistenceHelper : IWordExistenceHelper
    {
        private readonly IFileHelper _fileHelper;
        private const string Filename = "./word-dictionary.json";
        private readonly Dictionary _dictionary;

        public WordExistenceHelper(IFileHelper fileHelper)
        {
            _fileHelper = fileHelper;

            if(_dictionary == null)
                _dictionary = _fileHelper.ReadDictionary(Filename);
        }

        public bool DoesWordExist(string word)
        {
            Console.WriteLine("Match 1");
            var matchingWords = _dictionary
                .Words
                .Where(x => x.Word.ToLower() == word.ToLower())
                .ToList();

            if (!matchingWords.Any())
                return false;
            
            var selectedWord = matchingWords.First();

            if (selectedWord.Status == WordStatus.DoesNotExist)
                return false;

            var definitionParts = selectedWord.PermanentDefinition ?? selectedWord.TemporaryDefinition ?? "";
            
            var definitionList = definitionParts.Split(new Char[] {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '\n', ';'})
                .Where(y => !string.IsNullOrWhiteSpace(y))
                .ToList();

            return definitionList
                .Where(x => (!x.ToLower().Contains("obs.") && !x.ToLower().Contains("archaic") && !x.ToLower().Contains("scot.") && !x.ToLower().Contains("[irish]")))
                .ToList()
                .Any();
        }
    }
}