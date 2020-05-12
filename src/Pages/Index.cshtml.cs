﻿using System.Collections.Generic;
using Chat.Letters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chat.Pages
{
    public class IndexModel : PageModel
    {

        public GamesLibrary Games { get; set; }
        
        public void OnGet()
        {
            Games = new GamesLibrary(new List<Game>
            {
                new Game("Thoughts and crosses", "/thoughtsandcrosses", "9 categories, 1 starting letter - mission: ensure each answer is unique to win big" ),
                new Game("The word game", "/letters", "A bunch of random letters, a short amount of time - mission: come up with as many words as possible before you're out of time")
            });
        }
    }
}
