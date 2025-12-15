using Microsoft.AspNetCore.Mvc;
using _1Erronka_API.Repositorioak;
using _1Erronka_API.DTOak;
using _1Erronka_API.Modeloak;

namespace _1Erronka_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EskariakController : ControllerBase
    {
        private readonly EskariaRepository _repo;
        private readonly ProduktuaRepository _produktuaRepo;
        private readonly ErreserbaRepository _erreserbaRepo;

        public EskariakController(EskariaRepository repo, ProduktuaRepository produktuaRepo, ErreserbaRepository erreserbaRepo)
        {
            _repo = repo;
            _produktuaRepo = produktuaRepo;
            _erreserbaRepo = erreserbaRepo;
        }

        [HttpPost]
        public IActionResult Sortu([FromBody] EskariaSortuDto dto)
        {
            var erreserba = _erreserbaRepo.Get(dto.ErreserbaId);
            if (erreserba == null) return NotFound("Erreserba ez da aurkitu");

            if (erreserba.Langilea == null) return BadRequest("Erreserbak ez du langilerik asignatuta");

            var eskaria = new Eskaria
            {
                Erreserba = erreserba,
                Prezioa = dto.Prezioa,
                Egoera = dto.Egoera,
                Langilea = erreserba.Langilea,
                Mahaia = erreserba.Mahaia,
                Produktuak = new List<EskariaProduktua>()
            };

            foreach (var p in dto.Produktuak)
            {
                var produktua = _produktuaRepo.Get(p.ProduktuaId);
                if (produktua == null) continue;

                eskaria.Produktuak.Add(new EskariaProduktua
                {
                    Eskaria = eskaria,
                    Produktua = produktua,
                    Kantitatea = p.Kantitatea,
                    Prezioa = p.Prezioa
                });
            }

            _repo.Add(eskaria);
            return Ok(new { mezua = "Eskaria sortuta", eskariaId = eskaria.Id });
        }


        [HttpGet("{id}")]
        public IActionResult GetEskaria(int id)
        {
            var eskaria = _repo.Get(id);
            if (eskaria == null) return NotFound();

            var dto = new EskariaDto
            {
                Id = eskaria.Id,
                Prezioa = eskaria.Prezioa,
                Egoera = eskaria.Egoera,
                ErreserbaId = eskaria.Erreserba.Id,
                Produktuak = eskaria.Produktuak.Select(p => new EskariaProduktuaDto
                {
                    ProduktuaId = p.Produktua.Id,
                    ProduktuaIzena = p.Produktua.Izena,
                    Kantitatea = p.Kantitatea,
                    Prezioa = p.Prezioa
                }).ToList()
            };

            return Ok(dto);
        }

        [HttpGet("erreserba/{erreserbaId}")]
        public IActionResult GetEskariakByErreserba(int erreserbaId)
        {
            var eskariak = _repo.GetAll().Where(e => e.Erreserba.Id == erreserbaId).ToList();

            var dtoList = eskariak.Select(e => new EskariaDto
            {
                Id = e.Id,
                Prezioa = e.Prezioa,
                Egoera = e.Egoera,
                ErreserbaId = e.Erreserba.Id,
                Produktuak = e.Produktuak.Select(p => new EskariaProduktuaDto
                {
                    ProduktuaId = p.Produktua.Id,
                    ProduktuaIzena = p.Produktua.Izena,
                    Kantitatea = p.Kantitatea,
                    Prezioa = p.Prezioa
                }).ToList()
            }).ToList();

            return Ok(dtoList);
        }
    }
}
