using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Model;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using WriteModel;

namespace EntityFrameworkCore.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public PersonRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        /// <inheritdoc />
        public async Task CreateAsync(Person person)
        {
            var personData = new PersonData()
            {
                Id = person.Id,
                FirstName = person.FirstName,
                LastName = person.LastName,
                BirthDate = person.BirthDate,
                IdentificationNumber = person.IdentificationNumber,
            };

            await _applicationDbContext.People.AddAsync(personData);
        }

        /// <inheritdoc />
        public async Task<Person> GetByIdAsync(Guid id)
        {
            var personData = await _applicationDbContext.People.SingleOrDefaultAsync(p => p.Id == id);

            if (personData is null)
            {
                return null;
            }

            return Person.Factory.FromSnapshot(personData);
        }

        /// <inheritdoc />
        public async Task<List<Person>> GetAllAsync()
        {
            var peopleData = await _applicationDbContext.People.ToListAsync();
            return peopleData.Select(Person.Factory.FromSnapshot).ToList();
        }

        /// <inheritdoc />
        public async Task SaveChangesAsync()
        {
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
