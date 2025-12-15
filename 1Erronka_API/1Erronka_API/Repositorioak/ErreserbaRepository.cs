using NHibernate;
using _1Erronka_API.Modeloak;

namespace _1Erronka_API.Repositorioak
{
    public class ErreserbaRepository
    {
        private readonly NHibernate.ISession _session;

        public ErreserbaRepository(NHibernate.ISessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }

        public IList<Erreserba> GetAll() => _session.Query<Erreserba>().ToList();

        public Erreserba? Get(int id) =>
            _session.Query<Erreserba>().FirstOrDefault(x => x.Id == id);

        public void Add(Erreserba erreserba)
        {
            using var tx = _session.BeginTransaction();
            _session.Save(erreserba);
            tx.Commit();
        }

        public void Update(Erreserba erreserba)
        {
            using var tx = _session.BeginTransaction();
            _session.Update(erreserba);
            tx.Commit();
        }

        public void Delete(Erreserba erreserba)
        {
            using var tx = _session.BeginTransaction();
            _session.Delete(erreserba);
            tx.Commit();
        }
    }
}
