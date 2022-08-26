namespace BankAPI.Data.DTOs;

public class ClientDto
{
    public ClientDto(string Email, string Pwd)
    {
        this.Email = Email;
        this.Pwd = Pwd;
    }
    public string Email { get; set; } = null!;

    public string Pwd { get; set; } = null!;
    
}