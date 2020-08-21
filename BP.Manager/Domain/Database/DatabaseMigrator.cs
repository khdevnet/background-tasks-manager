using BP.Manager.Domain.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BP.Manager.Domain.Database
{
    public static class DatabaseMigrator
    {
        public static void Migrate(this IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<TaskStatesContext>();
                db.Database.Migrate();
            }
        }
    }
}
