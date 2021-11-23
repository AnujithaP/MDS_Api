using MDS;
using MDSServiceApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MDSServiceApp.Services
{
    public interface IMDSService
    {
        public Task<IEnumerable<MDSServiceApp.Models.Organisation>> GetOrganisations();
        public Task<IEnumerable<Person>> GetPersoner();
        public Task<IEnumerable<Medarbetare>> GetCoWorkers();
        public Task<Medarbetare> GetCoWorker(string coworkerId);
        public Task<Medarbetare> AddCoworker(Medarbetare medarbetare);
        public Task<Person> GetPerson(Guid personId);
        public Task<Person> AddPerson(Person person);
        public Task UpdatePerson(Person person);

        public Task<IEnumerable<User>> GetPermissions(string entityName, PermissionType permissionType, AccessPermissionType accessPermissionType);
        public Task<IEnumerable<User>> GetPrincipals();

        public int GetLastError();
    }
}
