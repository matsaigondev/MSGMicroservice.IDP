using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MSGMicroservice.IDP.Infrastructure.Entities;
using MSGMicroservice.IDP.Persistence;

namespace MSGMicroservice.IDP.Extensions
{
    public class MsgUserStore : UserStore<User, IdentityRole, MsgIdentityContext>
    {
        public MsgUserStore(MsgIdentityContext context, IdentityErrorDescriber describer = null) : base(context,
            describer)
        {

        }

        public override async Task<IList<string>> GetRolesAsync(User user,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var query = from userRole in Context.UserRoles
                join role in Context.Roles on userRole.RoleId equals role.Id
                where userRole.UserId.Equals(user.Id)
                select role.Id;
            return await query.ToListAsync(cancellationToken);
        }
    }
}