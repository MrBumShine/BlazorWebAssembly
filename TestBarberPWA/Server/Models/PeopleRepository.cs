﻿using Microsoft.EntityFrameworkCore;
using TestBarberPWA.Shared;

namespace TestBarberPWA.Server.Models
{
    public class PeopleRepository : IPeopleRepository
    {
        private readonly AppDBContext appDBContext;

        public PeopleRepository(AppDBContext appDBContext)
        {
            this.appDBContext = appDBContext;
        }

        public async Task<Person> AddPerson(Person person)
        {
            var result = await appDBContext.People.AddAsync(person);
            await appDBContext.SaveChangesAsync();

            return result.Entity;
        }

        public async Task DeletePerson(int personID)
        {
            var result = await appDBContext.People.FirstOrDefaultAsync(p => p.PersonID == personID);

            if (result != null)
            {
                appDBContext.People.Remove(result);
                await appDBContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Person>> GetEmployeesOrCustomers(bool ec)
        {
            IQueryable<Person> query = appDBContext.People;

            query = query.Where(p => p.IsEmployee == ec);

            return await query.ToListAsync();
        }

        public async Task<PeopleDataResult> GetPeople(int skip = 0, int take = 10)
        {
            PeopleDataResult result = new PeopleDataResult()
            {
                // Skip and Take methods here are for server-side paging
                People = appDBContext.People.Skip(skip).Take(take),
                Count = await appDBContext.People.CountAsync()
            };
            return result;
        }

        public async Task<Person> GetPerson(int personID)
        {
            return await appDBContext.People
                .FirstOrDefaultAsync(p => p.PersonID == personID);
        }

        public async Task<Person> GetPersonByEmail(string email)
        {
            return await appDBContext.People
                .FirstOrDefaultAsync(p => p.Email == email);
        }

        public async Task<IEnumerable<Person>> Search(string name, Gender? gender)
        {
            IQueryable<Person> query = appDBContext.People;

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.Forename.Contains(name) || p.Surname.Contains(name));
            }

            if (gender != null)
            {
                query = query.Where(p => p.Gender == gender);
            }

            return await query.ToListAsync();
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            var result = await appDBContext.People.FirstOrDefaultAsync(p => person.PersonID == person.PersonID);

            if (result != null)
            {
                result.Forename = person.Forename;
                result.Surname = person.Surname;
                result.DateOfBirth = person.DateOfBirth;
                result.Gender = person.Gender;
                result.PhoneNo = person.PhoneNo;
                result.Email = person.Email;
                result.IsEmployee = person.IsEmployee;

                await appDBContext.SaveChangesAsync();

                return result;
            }

            return null;
        }
    }
}
