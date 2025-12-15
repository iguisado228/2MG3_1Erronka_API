namespace _1Erronka_API.Modeloak
{
    public class Mahaia
    {
        public virtual int Id { get; set; }
        public virtual int Zenbakia { get; set; }
        public virtual int PertsonaKopuru { get; set; }
        public virtual string Kokapena { get; set; } = string.Empty;
    }
}
