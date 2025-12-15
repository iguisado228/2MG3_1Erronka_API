using Microsoft.AspNetCore.Mvc;
using _1Erronka_API.Repositorioak;
using _1Erronka_API.DTOak;
using _1Erronka_API.Modeloak;

namespace _1Erronka_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MahaiakController : ControllerBase
    {
        private readonly MahaiaRepository _repo;

        public MahaiakController(MahaiaRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var mahaiak = _repo.GetAll();

            var dtoList = mahaiak.Select(m => new MahaiaDto
            {
                Id = m.Id,
                Zenbakia = m.Zenbakia,
                PertsonaKopuru = m.PertsonaKopuru,
                Kokapena = m.Kokapena
            }).ToList();

            return Ok(dtoList);
        }
    }
}
