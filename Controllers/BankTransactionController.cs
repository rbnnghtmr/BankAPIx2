using Microsoft.AspNetCore.Mvc;
using BankAPI.Services;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace BankAPI.Controllers;

[Authorize(Policy = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class BankTransactionController : ControllerBase
{
    private readonly AccountService accountService;

    private readonly BankTransactionService bankTransactionService;
    private readonly TransactionTypeService _transactionTypeService;
    public BankTransactionController(AccountService accountService, BankTransactionService bankTransactionService, TransactionTypeService transactionypeService)
    {
        this.accountService = accountService;
        this.bankTransactionService = bankTransactionService;
        _transactionTypeService = transactionypeService;


    }

    [HttpGet("getall")]
    public async Task<IEnumerable<BankTransactionDtoOut>> Get()
    {
        return await bankTransactionService.GetAll();
    }

    [Authorize(Policy =  "Client")]
    [HttpGet("get/{id}")]
    public async Task<ActionResult<BankTransactionDtoOut>> GetById(int id)
    {
        var bankTransaction = await bankTransactionService.GetDtoById(id);
        if (bankTransaction is null)
            return AccountNotFound(id);

        return bankTransaction;
    }

    public AccountService GetServiceA()
    {
        return accountService;
    }

    [Authorize(Policy = "Client")]
    [HttpPost("getmyaccounts/{clientId}")]
    public async Task<IEnumerable<AccountDtoOut?>> GetMyAccounts(int clientId)
    {
        
        return await bankTransactionService.GetAllClientAccounts(clientId);
    }

    [Authorize(Policy = "Client")]
    [HttpPost("create")]
    public async Task<IActionResult> Create(BankTransactionDtoIn bank)
    {

        string validationResult = await validateAccount(bank);

        if (!validationResult.Equals("Valid"))
            return BadRequest(new { message = validationResult });

        if (bank.TransactionType.Equals(1))
        {
            var newBank = await bankTransactionService.Create(bank);
            return CreatedAtAction(nameof(GetById), new { id = newBank.Id }, newBank);
        }
        else if (bank.TransactionType.Equals(2))
        {
            var newBank = await bankTransactionService.Create(bank);
            return CreatedAtAction(nameof(GetById), new { id = newBank.Id }, newBank);
        }
        else if (bank.TransactionType.Equals(4))
        {
            var newBank = await bankTransactionService.Create(bank);
            return CreatedAtAction(nameof(GetById), new { id = newBank.Id }, newBank);
        }
        else
        {
            return BadRequest(new { message = "This transaction type doesn't support yet." });
        }

    }

    [Authorize(Policy = "Client")]
    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(int id, BankTransactionDtoIn bank)
    {
        if (id != bank.Id)
            return BadRequest(new { message = $"El ID{id} de la URL no coincide con el ID{bank.Id} del cuerpo de la solciitud." });

        var bankToUpdate = await bankTransactionService.GetById(id);

        if (bankToUpdate is not null)
        {
            string validationResult = await validateAccount(bank);
            if (!validationResult.Equals("Valid"))
                return BadRequest(new { message = validationResult });

            await bankTransactionService.Update(bank);
            return NoContent();
        }
        else
        {
            return AccountNotFound(id);
        }


    }

    [Authorize(Policy = "Client")]
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var bankToDelete = await bankTransactionService.GetById(id);
        if (bankToDelete is not null)
        {
            await bankTransactionService.Delete(id);
            return Ok();
        }
        else
        {
            return AccountNotFound(id);
        }
            
    }

    public NotFoundObjectResult AccountNotFound(int id)
    {
        return NotFound(new { message = $"La cuenta con ese ID = {id} no existe." });
    }

    public async Task<string> validateAccount(BankTransactionDtoIn bank)
    {
        string result = "Valid";

        var transactionType = await _transactionTypeService.GetById(bank.TransactionType);

        if (transactionType is null)
            result = $"El tipo de transaction {bank.TransactionType} no existe.";

        var accountID = bank.AccountId.GetValueOrDefault();

        var account = await accountService.GetById(accountID);

        if (account is null)
            result = $"La cuenta {accountID} no existe.";

        return result;
    }

   

}