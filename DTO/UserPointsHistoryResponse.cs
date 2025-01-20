namespace Backend.DTO
{
    public class UserPointsHistoryResponse
    {
        public DateTime Date { get; set; } // Data operacji
        public required string OperationDescription { get; set; } // Opis operacji (np. punkty dodane/odjęte)
    }
}
