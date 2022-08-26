namespace BankAPI.Data.DTOs;

    public class BankTransactionDtoOut
    {
        public int Id { get; set; }
        public int?  AccountId { get; set; }
        public string TransactionName { get; set; } = null!;
        public decimal Amount { get; set; }
        public int? ExternalAccount { get; set; }
        public DateTime RegDate { get; set; }
}