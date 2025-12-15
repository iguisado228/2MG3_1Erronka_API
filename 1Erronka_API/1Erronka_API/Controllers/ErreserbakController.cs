using _1Erronka_API.Domain;
using _1Erronka_API.DTOak;
using _1Erronka_API.Modeloak;
using _1Erronka_API.Repositorioak;
using Microsoft.AspNetCore.Mvc;

namespace _1Erronka_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ErreserbakController : ControllerBase
    {
        private readonly ErreserbaRepository _repo;

        public ErreserbakController(ErreserbaRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var erreserbak = _repo.GetAll();

            var dtoList = erreserbak.Select(e => new ErreserbaDto
            {
                Id = e.Id,
                BezeroIzena = e.BezeroIzena,
                Telefonoa = e.Telefonoa,
                PertsonaKopurua = e.PertsonaKopurua,
                EgunaOrdua = e.EgunaOrdua,
                PrezioTotala = e.PrezioTotala,
                FakturaRuta = e.FakturaRuta,
                LangileaId = e.Langilea.Id,
                MahaiakId = e.Mahaia.Id

            }).ToList();

            return Ok(dtoList);
        }

        [HttpPost]
        public IActionResult Sortu([FromBody] ErreserbaSortuDto dto)
        {
            var erreserba = new Erreserba
            {
                BezeroIzena = dto.BezeroIzena,
                Telefonoa = dto.Telefonoa,
                PertsonaKopurua = dto.PertsonaKopurua,
                EgunaOrdua = dto.EgunaOrdua, // fecha + hora
                PrezioTotala = dto.PrezioTotala,
                FakturaRuta = dto.FakturaRuta,
                Langilea = new Langilea { Id = dto.LangileaId },
                Mahaia = new Mahaia { Id = dto.MahaiakId }

            };

            _repo.Add(erreserba);
            return Ok(new { mezua = "Erreserba sortuta", erreserbaId = erreserba.Id });
        }
    }
}
