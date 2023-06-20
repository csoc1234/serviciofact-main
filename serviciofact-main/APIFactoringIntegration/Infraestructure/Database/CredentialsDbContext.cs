using APIFactoringIntegration.Domain.Entity;
using APIFactoringIntegration.Infraestructure.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace APIFactoringIntegration.Infraestructure.Database
{
    public class CredentialsDbContext : DbContext, ICredentialsDbContext
    {
        private readonly IConfiguration _configuration;

        public CredentialsDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<CredentialsManagement> CredentialsManagement { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_configuration["ConnectionStrings:DefaultConnection"]);
            }
        }

        public async Task<bool> ValidateCredentials(CredentialsManagement credentials, IConfiguration configuration)
        {
            try
            {
                SqlParameter user = new()
                {
                    ParameterName = "@user",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = credentials.User
                };

                SqlParameter password = new()
                {
                    ParameterName = "@password",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = credentials.Password
                };

                SqlParameter[] parameters = new SqlParameter[]
                {
                    user,
                    password
                };

                using (CredentialsDbContext? dbo = new(configuration))
                {
                    int result = await dbo.Database
                        .ExecuteSqlRawAsync("EXEC dbo.uspCredentialsMgmtRead @user, @password;", parameters);

                    //Se actualizo el Last Connection por que encontro el registro
                    if (result == 1)
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
