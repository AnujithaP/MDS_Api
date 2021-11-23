using MDSServiceApp.Models;

namespace MDSServiceApp.Services
{
    public class Repository : IRepository
    {
        public Person personRepository { get; set; }
        public Medarbetare medarbetareRepository { get; set; }

        public void SetPerson(Person person)
        {
            personRepository = person;
        }

        public Person GetPerson()
        {
            return personRepository;
        }

        public void SetMedarbetare(Medarbetare medarbetare)
        {
            medarbetareRepository = medarbetare;
        }

        public Medarbetare GetMedarbetare()
        {
            return medarbetareRepository;
        }

        public void Clear()
        {
            personRepository = null;
            medarbetareRepository = null;
        }
    }
}
