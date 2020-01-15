﻿using Arch.EntityFrameworkCore.UnitOfWork;
using GrowRoomEnvironment.DataAccess.Core.Constants;
using Microsoft.AspNetCore.Http;

namespace GrowRoomEnvironment.DataAccess
{
    public class HttpUnitOfWork : UnitOfWork<ApplicationDbContext>
    {
        public HttpUnitOfWork(ApplicationDbContext context, IHttpContextAccessor httpAccessor) : base(context)
        {
            context.CurrentUserId = httpAccessor.HttpContext.User.FindFirst(Claims.Subject)?.Value?.Trim();
        }
    }
}
