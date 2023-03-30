﻿using DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.IRepository
{
    public interface IOptionRepository:IEntityRepository<Option>
    {
        Task<IEnumerable<Option>> GetByCategoryId(int categoryId, IEnumerable<string> attrs = null);
    }
}