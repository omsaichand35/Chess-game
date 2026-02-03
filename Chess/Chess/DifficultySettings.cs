using System.Collections.Generic;

namespace Chess
{
    public static class DifficultySettings
    {
        public static readonly List<DifficultyLevel> Levels = new()
        {
            // Beginner levels - Very weak play for true beginners (200-600 ELO)
            new() { Level = 1, Name = "Beginner I", Skill = 0, Depth = 1, LimitStrength = true, Elo = 300 },      // ~300 ELO - Almost random
            new() { Level = 2, Name = "Beginner II", Skill = 0, Depth = 2, LimitStrength = true, Elo = 500 },     // ~500 ELO - Very weak
            new() { Level = 3, Name = "Beginner III", Skill = 1, Depth = 2, LimitStrength = true, Elo = 700 },    // ~700 ELO - Still weak
            
            // Easy levels - Casual player (700-1000 ELO)
            new() { Level = 4, Name = "Easy I", Skill = 2, Depth = 2, LimitStrength = true, Elo = 850 },          // ~850 ELO
            new() { Level = 5, Name = "Easy II", Skill = 3, Depth = 3, LimitStrength = true, Elo = 1000 },        // ~1000 ELO
            new() { Level = 6, Name = "Easy III", Skill = 4, Depth = 3, LimitStrength = true, Elo = 1150 },       // ~1150 ELO
            
            // Intermediate levels - Club player (1200-1500 ELO)
            new() { Level = 7, Name = "Intermediate I", Skill = 5, Depth = 4, LimitStrength = true, Elo = 1250 }, // ~1250 ELO
            new() { Level = 8, Name = "Intermediate II", Skill = 6, Depth = 5, LimitStrength = true, Elo = 1400 },// ~1400 ELO
            new() { Level = 9, Name = "Intermediate III", Skill = 8, Depth = 6, LimitStrength = true, Elo = 1550 },// ~1550 ELO
            
            // Advanced levels - Strong club/tournament player (1600-1900 ELO)
            new() { Level = 10, Name = "Advanced I", Skill = 10, Depth = 7, LimitStrength = true, Elo = 1650 },   // ~1650 ELO
            new() { Level = 11, Name = "Advanced II", Skill = 12, Depth = 8, LimitStrength = true, Elo = 1800 },  // ~1800 ELO
            new() { Level = 12, Name = "Advanced III", Skill = 14, Depth = 9, LimitStrength = true, Elo = 1950 }, // ~1950 ELO
            
            // Expert levels - Expert/Master level (2000-2300+ ELO)
            new() { Level = 13, Name = "Expert I", Skill = 16, Depth = 11, LimitStrength = true, Elo = 2100 },    // ~2100 ELO
            new() { Level = 14, Name = "Expert II", Skill = 18, Depth = 13, LimitStrength = true, Elo = 2300 },   // ~2300 ELO
            new() { Level = 15, Name = "Stockfish Max", Skill = 20, Depth = 20, LimitStrength = false, Elo = null } // 3000+ ELO (unlimited)
        };
    }
}
