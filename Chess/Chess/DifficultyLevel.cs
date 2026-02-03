namespace Chess
{
    public class DifficultyLevel
    {
        public int Level { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Skill { get; set; }
        public int Depth { get; set; }
        public bool LimitStrength { get; set; }
        public int? Elo { get; set; }
    }
}
