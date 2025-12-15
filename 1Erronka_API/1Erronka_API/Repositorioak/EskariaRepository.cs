using NHibernate;
using _1Erronka_API.Modeloak;

namespace _1Erronka_API.Repositorioak
{
    public class EskariaRepository
    {
        private readonly NHibernate.ISession _session;

        public EskariaRepository(NHibernate.ISessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }


        public IList<Eskaria> GetAll() => _session.Query<Eskaria>().ToList();

        public Eskaria? Get(int id) =>
            _session.Query<Eskaria>().FirstOrDefault(x => x.Id == id);

        public void Add(Eskaria eskaria)
        {
            using var tx = _session.BeginTransaction();
            _session.Save(eskaria);
            tx.Commit();
        }
    }
}
