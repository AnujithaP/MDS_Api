using MDSServiceApp.Models;

namespace MDSServiceApp.Services
{
    public interface IRepository
    {
        public void SetPerson(Person person);
        public Person GetPerson();
        public void SetMedarbetare(Medarbetare medarbetare);
        public Medarbetare GetMedarbetare();
        public void Clear();
    }
}
