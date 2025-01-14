﻿using System;
using System.Collections.Generic;
using System.IO;
using Chat.WordGame;
using Chat.WordGame.LocalDictionaryHelpers;
using Chat.WordGame.WordHelpers;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace PartyGamesTests.WordGame.WordHelpers.WordServiceTests
{
    public class AddWordToGuessedWordsTest : IDisposable
    {
        private readonly IWordDefinitionHelper _wordDefinitionHelper;
        private readonly IWordExistenceHelper _wordExistenceHelper;
        private readonly IWordHelper _wordHelper;
        private readonly FileHelper _fileHelper = new FileHelper();
        private readonly List<string> _words = new List<string> {"cow", "dog", "frog", "pigeon"};
        private readonly Dictionary _dictionary;
        private readonly WordService _wordService;
        private IFilenameHelper _filenameHelper;


        private const string GuessedWordsFilename = "./test-guessed-words";
        private const string DictionaryFilename = "./guessed-words-dictionary";

        public AddWordToGuessedWordsTest()
        {
            _dictionary = new Dictionary
            {
                Words = new List<WordData>
                {
                    new WordData
                    {
                        Word = _words[0],
                        PermanentDefinition = null,
                        TemporaryDefinition = null,
                        Status = WordStatus.Permanent
                    },
                    new WordData
                    {
                        Word = _words[1],
                        PermanentDefinition = null,
                        TemporaryDefinition = null,
                        Status = WordStatus.Temporary
                    },
                    new WordData()
                    {
                        Word = _words[2],
                        PermanentDefinition = null,
                        TemporaryDefinition = null,
                        Status = WordStatus.Suffix
                    },
                    new WordData()
                    {
                        Word = _words[3],
                        PermanentDefinition = null,
                        TemporaryDefinition = null,
                        Status = WordStatus.DoesNotExist
                    }
                }
            };
            
            _filenameHelper = Substitute.For<IFilenameHelper>();
            _filenameHelper
                .GetGuessedWordsFilename()
                .Returns(GuessedWordsFilename);
            _filenameHelper.GetDictionaryFilename().Returns(DictionaryFilename);

            TestFileHelper.CreateCustomFile(GuessedWordsFilename, null);
            TestFileHelper.CreateCustomFile(DictionaryFilename, _dictionary);

            _wordService = new WordService(_wordExistenceHelper, _wordHelper, _wordDefinitionHelper, _fileHelper, _filenameHelper);
            
        }
        
        [Fact]
        public void AllWordsGuessedAndTheirStatusesShouldBeAddedToGuessedWordsList()
        {
            _wordService.AddWordToGuessedWords(DictionaryFilename, GuessedWordsFilename, _words[0]);
            _wordService.AddWordToGuessedWords(DictionaryFilename, GuessedWordsFilename, _words[1]);
            _wordService.AddWordToGuessedWords(DictionaryFilename, GuessedWordsFilename, _words[2]);
            _wordService.AddWordToGuessedWords(DictionaryFilename, GuessedWordsFilename, _words[3]);
            _wordService.UpdateGuessedWordsFile();

            var json = TestFileHelper.Read(GuessedWordsFilename);
            var guessedWords = JsonConvert.DeserializeObject<GuessedWords>(json);
            

            
            guessedWords
                .Words
                .Should()
                .BeEquivalentTo(new List<GuessedWord>
                {
                    new GuessedWord(_words[0], WordStatus.Permanent),
                    new GuessedWord(_words[1], WordStatus.Temporary),
                    new GuessedWord(_words[2], WordStatus.Suffix),
                    new GuessedWord(_words[3], WordStatus.DoesNotExist)
                });
            
        }
        
        [Fact]
        public void WhenWordExistsInGuessedWordsButHasADifferentStatusThenTheStatusShouldBeUpdated()
        {
            var originalGuessedWords = new GuessedWords();
            originalGuessedWords.AddWord(_words[0], WordStatus.Temporary);
            
            TestFileHelper.CreateCustomFile(GuessedWordsFilename, originalGuessedWords);
            
            _wordService.AddWordToGuessedWords(DictionaryFilename, GuessedWordsFilename, _words[0]);
            _wordService.UpdateGuessedWordsFile();

            var json = TestFileHelper.Read(GuessedWordsFilename);
            var guessedWords = JsonConvert.DeserializeObject<GuessedWords>(json);
            
            guessedWords
                .Words
                .Should()
                .BeEquivalentTo(new List<GuessedWord>
                {
                    new GuessedWord(_words[0], WordStatus.Permanent)
                });
        }
        
        [Fact]
        public void WhenWordIsGuessedTwiceItShouldOnlyBeWrittenToTheFileOnce()
        {
            _wordService.AddWordToGuessedWords(DictionaryFilename, GuessedWordsFilename, _words[0]);
            _wordService.AddWordToGuessedWords(DictionaryFilename, GuessedWordsFilename, _words[0]);
            _wordService.AddWordToGuessedWords(DictionaryFilename, GuessedWordsFilename, _words[0]);
            _wordService.UpdateGuessedWordsFile();

            var json = TestFileHelper.Read(GuessedWordsFilename);
            var guessedWords = JsonConvert.DeserializeObject<GuessedWords>(json);
            
            guessedWords
                .Words
                .Should()
                .BeEquivalentTo(new List<GuessedWord>
                {
                    new GuessedWord(_words[0], WordStatus.Permanent)
                });

            guessedWords.Words.Should().HaveCount(1);
        }
        
        [Fact]
        public void IfWordDoesNotExistInDictionaryThenTheWordShouldBeAddedToGuessedWordsAsNotExists()
        {
            _wordService.AddWordToGuessedWords(DictionaryFilename, GuessedWordsFilename, "telescope");
            _wordService.UpdateGuessedWordsFile();

            var json = TestFileHelper.Read(GuessedWordsFilename);
            var guessedWords = JsonConvert.DeserializeObject<GuessedWords>(json);
            
            guessedWords
                .Words
                .Should()
                .BeEquivalentTo(new List<GuessedWord>
                {
                    new GuessedWord("telescope", WordStatus.DoesNotExist)
                });
        }

        public void Dispose()
        {
            if (File.Exists(DictionaryFilename))
                File.Delete(DictionaryFilename);
            
            if (File.Exists(GuessedWordsFilename))
                File.Delete(GuessedWordsFilename);
        }
    }
}