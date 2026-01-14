using NHibernate;
using _1Erronka_API.Modeloak;

namespace _1Erronka_API.Repositorioak
{
    public class ErreserbaRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public ErreserbaRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public IList<Erreserba> GetAll()
        {
            using var session = _sessionFactory.OpenSession();
            return session.Query<Erreserba>().ToList();
        }

        public Erreserba? Get(int id)
        {
            using var session = _sessionFactory.OpenSession();
            return session.Query<Erreserba>().FirstOrDefault(x => x.Id == id);
        }

        public void Add(Erreserba erreserba)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();
            session.Save(erreserba);
            tx.Commit();
        }

        public void Update(Erreserba erreserba)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();
            session.Update(erreserba);
            tx.Commit();
        }

        public void Delete(Erreserba erreserba)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();
            session.Delete(erreserba);
            tx.Commit();
        }
    }
}
