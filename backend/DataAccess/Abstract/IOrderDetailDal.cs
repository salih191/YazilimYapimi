﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Core.DataAccess;
using Entities.Concrete;
using Entities.DTOs;

namespace DataAccess.Abstract
{
    public interface IOrderDetailDal:IEntityRepository<OrderDetail>
    {
        List<OrderDetailsDto> getAllDetailsDtos(Expression<Func<OrderDetail, bool>> filter = null);
    }
}