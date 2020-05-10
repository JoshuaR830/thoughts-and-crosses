﻿namespace Chat.WordGame.WordHelpers
{
    public interface IWordService
    {
        bool GetWordStatus(string word);
        string GetDefinition(string word);
    }
}