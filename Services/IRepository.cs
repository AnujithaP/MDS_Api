using MDSPermissions.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
