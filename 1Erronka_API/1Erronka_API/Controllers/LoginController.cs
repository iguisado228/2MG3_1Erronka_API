using _1Erronka_API;
using _1Erronka_API.Domain;
using _1Erronka_API.DTOak;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using _1Erronka_API.Repositorioak;
using System.Security.Cryptography;
using System;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    [HttpPost]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            var langilea = session.Query<Langilea>()
                .FirstOrDefault(u => u.Erabiltzaile_izena == request.Erabiltzailea);

            if (langilea == null)
                return Unauthorized();

            string pasahitzaHash = HashPassword(request.Pasahitza);

            if (langilea.Pasahitza != pasahitzaHash)
                return Unauthorized();

            return Ok(new LangileaDto
            {
                Id = langilea.Id,
                Izena = langilea.Izena,
                Erabiltzaile_izena = langilea.Erabiltzaile_izena,
                Gerentea = langilea.Gerentea,
                TpvSarrera = langilea.TpvSarrera
            });
        }
    }

    private string HashPassword(string input)
    {
        using (SHA256 sha = SHA256.Create())
        {
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}

public class LoginRequest
{
    public string Erabiltzailea { get; set; }
    public string Pasahitza { get; set; }
}
