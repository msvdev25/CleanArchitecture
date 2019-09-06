using MFS.Domain;
using Microsoft.Extensions.Logging;
using RS2.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace MFS.Repo
{

    public interface ICountryRepository:IGetRepository<Country, int> { }

    internal class CountryRepository : RepositoryBase<Country, int>, ICountryRepository
    {
        public CountryRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
