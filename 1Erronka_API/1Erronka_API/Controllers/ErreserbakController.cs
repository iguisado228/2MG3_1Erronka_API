using _1Erronka_API.Domain;
using _1Erronka_API.DTOak;
using _1Erronka_API.Modeloak;
using _1Erronka_API.Repositorioak;
using Microsoft.AspNetCore.Mvc;

// iText7
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Font.Constants;
using iText.Kernel.Font;

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
                Ordainduta = e.Ordainduta,
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
                EgunaOrdua = dto.EgunaOrdua,
                PrezioTotala = dto.PrezioTotala,
                FakturaRuta = dto.FakturaRuta,
                Langilea = new Langilea { Id = dto.LangileaId },
                Mahaia = new Mahaia { Id = dto.MahaiakId }
            };

            _repo.Add(erreserba);
            return Ok(new { mezua = "Erreserba sortuta", erreserbaId = erreserba.Id });
        }


        [HttpPost("ordaindu")]
        public IActionResult Ordaindu([FromBody] ErreserbaOrdainduDto dto)
        {
            using var session = _repo.OpenSession(); 
            using var tx = session.BeginTransaction();

            var erreserba = session.Get<Erreserba>(dto.ErreserbaId);
            if (erreserba == null)
                return NotFound();

            erreserba.Ordainduta = 1;
            erreserba.PrezioTotala = dto.Guztira;

            var produktuak = _repo.LortuProduktuakErreserbarako(dto.ErreserbaId);

            string fakturaRuta = SortuTicketPdf(
                erreserba,
                produktuak,
                dto.Jasotakoa,
                dto.Itzulia,
                dto.OrdainketaModua
            );

            erreserba.FakturaRuta = fakturaRuta;

            session.Update(erreserba);
            tx.Commit();

            return Ok();
        }

        private string SortuTicketPdf(
            Erreserba erreserba,
            List<EskariaProduktuaDto> produktuak,
            double jasotakoa,
            double itzulia,
            string ordainketaModua)
        {
            string root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tiketak");
            Directory.CreateDirectory(root);

            string pdfPath = Path.Combine(root, $"tiket_{erreserba.Id}.pdf");
            string fakturaRuta = $"/tiketak/tiket_{erreserba.Id}.pdf";

            using (var writer = new PdfWriter(pdfPath))
            using (var pdf = new PdfDocument(writer))
            using (var doc = new Document(pdf))
            {
                var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                doc.Add(new Paragraph("TXAPELA JATETXEA")
                    .SetFont(boldFont)
                    .SetFontSize(18)
                    .SetTextAlignment(TextAlignment.CENTER));

                doc.Add(new Paragraph("CIF: B12345678\nSan Juan Kalea 12, Lazkao\nTel: 943 000 000")
                    .SetTextAlignment(TextAlignment.CENTER));

                doc.Add(new Paragraph($"Ticket Zenbakia: {erreserba.Id:D6}"));
                doc.Add(new Paragraph($"Data: {DateTime.Now:dd/MM/yyyy HH:mm}"));
                doc.Add(new Paragraph($"Langilea: {erreserba.Langilea.Izena}"));
                doc.Add(new Paragraph("----------------------------------------"));

                Table table = new Table(3).UseAllAvailableWidth();
                table.AddHeaderCell("Kop");
                table.AddHeaderCell("Produktua");
                table.AddHeaderCell("Totala");

                foreach (var p in produktuak)
                {
                    double lineTotal = p.Kantitatea * p.Prezioa;
                    table.AddCell(p.Kantitatea.ToString());
                    table.AddCell(p.ProduktuaIzena);
                    table.AddCell($"{lineTotal:0.00} €");
                }

                doc.Add(table);
                doc.Add(new Paragraph("----------------------------------------"));

                double subtotal = produktuak.Sum(p => p.Kantitatea * p.Prezioa);
                double iva = subtotal * 0.10;
                double guztira = subtotal + iva;

                doc.Add(new Paragraph($"Subtotala: {subtotal:0.00} €"));
                doc.Add(new Paragraph($"IVA (10%): {iva:0.00} €"));
                doc.Add(new Paragraph($"GUZTIRA: {guztira:0.00} €"));
                doc.Add(new Paragraph("----------------------------------------"));

                doc.Add(new Paragraph($"Ordaintza modua: {ordainketaModua}"));

                if (ordainketaModua == "Eskudirua")
                {
                    doc.Add(new Paragraph($"Jasotakoa: {jasotakoa:0.00} €"));
                    doc.Add(new Paragraph($"Itzulia: {itzulia:0.00} €"));
                }

                doc.Add(new Paragraph("----------------------------------------"));
                doc.Add(new Paragraph("Eskerrik asko bisitagatik!")
                    .SetTextAlignment(TextAlignment.CENTER));
            }

            return fakturaRuta;
        }
    }
}
