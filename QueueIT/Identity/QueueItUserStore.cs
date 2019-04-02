using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Remotion.Linq.Clauses;

namespace QueueIT.Identity
{
    public class QueueItUserStore : IUserStore<QueueItUser>, IUserPasswordStore<QueueItUser>
    {
        public void Dispose()
        {
        }

        public Task<string> GetUserIdAsync(QueueItUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(QueueItUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(QueueItUser user, string UserName, CancellationToken cancellationToken)
        {
            user.UserName = UserName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(QueueItUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task SetNormalizedUserNameAsync(QueueItUser user, string normalizedName,
            CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }


        public static DbConnection GetOpenConnection()
        {
            var connection = new SqlConnection("Data Source=BAUTISTADT;" +
                                               "database=QueueIt;" +
                                               "trusted_connection=yes");
            connection.Open();

            return connection;
        }

        public async Task<IdentityResult> CreateAsync(QueueItUser user, CancellationToken cancellationToken)
        {
            using (var connection = GetOpenConnection())
            {
                await connection.ExecuteAsync(
                    "insert into QueueItUsers([Id]," +
                    "[UserName]," +
                    "[NormalizedUserName]," +
                    "[PasswordHash])" +
                    "Values(@id,@UserName,@normalizedUserName,@passwordHash)",
                    new
                    {
                        id = user.Id,
                        UserName = user.UserName,
                        normalizedUserName = user.NormalizedUserName,
                        passwordHash = user.PasswordHash
                    }
                );
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(QueueItUser user, CancellationToken cancellationToken)
        {
            using (var connection = GetOpenConnection())
            {
                await connection.ExecuteAsync(
                    "update QueueItUsers " +
                    "set [Id] = @id, " +
                    "[UserName] = @UserName," +
                    "[NormalizedUserName] = @normalizedUserName," +
                    "[PasswordHash] = @passwordHash " +
                    "where [Id] = @id",
                    new
                    {
                        id = user.Id,
                        UserName = user.UserName,
                        normalizedUserName = user.NormalizedUserName,
                        passwordHash = user.PasswordHash
                    }
                );
            }
            
            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(QueueItUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public async Task<QueueItUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            using (var connection = GetOpenConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<QueueItUser>(
                    "select * From QueueItUsers where Id = @id",
                    new {id = userId});
            }
        }

        public async Task<QueueItUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            using (var connection = GetOpenConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<QueueItUser>(
                    "select * From QueueItUsers where NormalizedUserName = @name",
                    new {name = normalizedUserName});
            }
        }

        public Task SetPasswordHashAsync(QueueItUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(QueueItUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(QueueItUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }
    }
}