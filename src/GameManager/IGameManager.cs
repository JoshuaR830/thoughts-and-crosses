﻿using Chat.RoomManager;

namespace Chat.GameManager
{
    public interface IGameManager
    {
        void SetupNewGame(string roomId, string userId, GameType game);
        void SetupNewThoughtsAndCrossesUser(string roomId, string userId, GameThoughtsAndCrosses game);
        void ResetThoughtsAndCrosses(string roomId, GameThoughtsAndCrosses game);
        void ResetThoughtsAndCrosssesForUser(string roomId, string userId, GameThoughtsAndCrosses game);
        void SetUpNewWordGameUser(string roomId, string userId, GameWordGame game);
        void ResetWordGame(string roomId);
        void ResetWordGameForUser(string roomId, string userId);
    }
}