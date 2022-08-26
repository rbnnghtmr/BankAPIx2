using Microsoft.AspNetCore.Mvc;
using BankAPI.Services;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BankAPI.Controllers;


[ApiController]
[Route("api/[controller]")]
public class LoginClientController : ControllerBase
{
    private readonly LoginClientService loginClientService;

    private IConfiguration configClient;

    public LoginClientController(LoginClientService loginClientService, IConfiguration configClient)
    {
        this.loginClientService = loginClientService;
        this.configClient = configClient;
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> Login(ClientDto clientDto)
    {
        var client = await loginClientService.GetClient(clientDto);

        if(client is null)
            return BadRequest(new { message = "Credenciales inválidas."});

        
       string jwtToken = GenerateToken(client);

    return Ok(new { token = jwtToken});

    }


    private string GenerateToken(Client client)
    {
        var claims = new[]
        {            new Claim(ClaimTypes.Name, client.Name),
            new Claim(ClaimTypes.Email, client.Email),
            new Claim("Client", "Y")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configClient.GetSection("JWT2:Key").Value));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var securityToken = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: creds);

        string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

        return token;
    }
}