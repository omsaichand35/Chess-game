namespace Chess
{
    public class NetworkMessage
    {
        public string Type { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
    }

    public class MoveMessage
    {
        public string From { get; set; } = string.Empty; // e.g., "e2"
        public string To { get; set; } = string.Empty;   // e.g., "e4"
        public string? Promotion { get; set; }           // For pawn promotion
    }

    public class GameStartMessage
    {
        public string GameId { get; set; } = string.Empty;
        public string OpponentName { get; set; } = string.Empty;
        public string YourColor { get; set; } = string.Empty; // "White" or "Black"
    }

    public class PlayerInfo
    {
        public string ConnectionId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public int Rating { get; set; } = 800;
    }
}
