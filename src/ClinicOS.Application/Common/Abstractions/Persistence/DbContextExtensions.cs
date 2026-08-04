using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;

namespace ClinicOS.Application.Common.Abstractions.Persistence.Data
{
    public static class DbContextExtensions
    {
        public static Task<T?> FirstOrDefaultAsync<T>(this IApplicationDbContext context, IQueryable<T> query, CancellationToken cancellationToken = default)
            => EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(query, cancellationToken);

        public static Task<T?> FirstOrDefaultAsync<T>(this IReadDbContext context, IQueryable<T> query, CancellationToken cancellationToken = default)
            => EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(query, cancellationToken);

        public static Task<List<T>> ToListAsync<T>(this IApplicationDbContext context, IQueryable<T> query, CancellationToken cancellationToken = default)
            => EntityFrameworkQueryableExtensions.ToListAsync(query, cancellationToken);

        public static Task<List<T>> ToListAsync<T>(this IReadDbContext context, IQueryable<T> query, CancellationToken cancellationToken = default)
            => EntityFrameworkQueryableExtensions.ToListAsync(query, cancellationToken);

        public static Task<bool> AnyAsync<T>(this IApplicationDbContext context, IQueryable<T> query, CancellationToken cancellationToken = default)
            => EntityFrameworkQueryableExtensions.AnyAsync(query, cancellationToken);

        public static Task<bool> AnyAsync<T>(this IReadDbContext context, IQueryable<T> query, CancellationToken cancellationToken = default)
            => EntityFrameworkQueryableExtensions.AnyAsync(query, cancellationToken);

        public static Task<int> CountAsync<T>(this IApplicationDbContext context, IQueryable<T> query, CancellationToken cancellationToken = default)
            => EntityFrameworkQueryableExtensions.CountAsync(query, cancellationToken);

        public static Task<int> CountAsync<T>(this IReadDbContext context, IQueryable<T> query, CancellationToken cancellationToken = default)
            => EntityFrameworkQueryableExtensions.CountAsync(query, cancellationToken);

        public static IQueryable<T> AsNoTracking<T>(this IApplicationDbContext context, IQueryable<T> query) where T : class
            => EntityFrameworkQueryableExtensions.AsNoTracking(query);

        public static IQueryable<T> AsNoTracking<T>(this IReadDbContext context, IQueryable<T> query) where T : class
            => EntityFrameworkQueryableExtensions.AsNoTracking(query);
    }
}
