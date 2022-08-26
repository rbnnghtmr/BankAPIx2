using Microsoft.EntityFrameworkCore;
using BankAPI.Data;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;

namespace BankAPI.Services;

public class BankTransactionService{
    private readonly BankContext _context;
    private readonly AccountService accountService;
    private readonly ClientService clientService;

    public BankTransactionService(BankContext context, AccountService accountService, ClientService clientService)
    {
        _context = context;
        this.accountService = accountService;
        this.clientService = clientService;
    }

    public async Task<IEnumerable<BankTransactionDtoOut>> GetAll()
    {
        return await _context.BankTransactions.Select(b => new BankTransactionDtoOut
        {
            Id = b.Id,
            AccountId = b.AccountId,
            TransactionName = b.TransactionTypeNavigation.Name,
            Amount = b.Amount,
            ExternalAccount = b.ExternalAccount,
            RegDate = b.RegDate
        }).ToListAsync();
    }

   public async Task<BankTransactionDtoOut?> GetDtoById(int id)
    {
        return await _context.BankTransactions.
        Where(a => a.Id == id).
        Select(a => new BankTransactionDtoOut
        {
            Id = a.Id,
            AccountId = a.AccountId, 
            TransactionName = a.TransactionTypeNavigation.Name,
            Amount = a.Amount,
            ExternalAccount = a.ExternalAccount,
            RegDate = a.RegDate
        }).SingleOrDefaultAsync();
    }
    public async Task<BankTransaction?> GetById(int id)
    {
        return await _context.BankTransactions.FindAsync(id);
    }


     public async Task<BankTransaction> Create(BankTransactionDtoIn newTransactionDtoIn)
    {
        var bankTransaction = new BankTransaction();
        bankTransaction.AccountId = newTransactionDtoIn.AccountId;
        bankTransaction.TransactionType = newTransactionDtoIn.TransactionType;
        bankTransaction.Amount = newTransactionDtoIn.Amount;
        bankTransaction.ExternalAccount = newTransactionDtoIn.ExternalAccount;
        _context.BankTransactions.Add(bankTransaction);
        await _context.SaveChangesAsync();
        
        return bankTransaction;
        
    }

    public async Task Update(BankTransactionDtoIn bankTransaction)
    {
        var existinBankTransaction = await GetById(bankTransaction.Id);

        if(existinBankTransaction is not null)
        {
            existinBankTransaction.AccountId = bankTransaction.AccountId;
            existinBankTransaction.TransactionType = bankTransaction.TransactionType;
            existinBankTransaction.Amount = bankTransaction.Amount;
            existinBankTransaction.ExternalAccount = bankTransaction.ExternalAccount;

           await _context.SaveChangesAsync();
        }
        
    }

    public async Task Delete(int id)
    {
        var bankTransactionToDelete = await GetById(id);

        if(bankTransactionToDelete is not null)
        {
            _context.BankTransactions.Remove(bankTransactionToDelete);
            await _context.SaveChangesAsync();
        }
        
    }

    public async Task<IEnumerable<AccountDtoOut?>> GetAllClientAccounts(int clientId)
    {
        return await accountService.GetDtoAccountByClientId(clientId);
    }

}