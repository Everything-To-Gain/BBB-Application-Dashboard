using System.Text;
using BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;
using BBB_ApplicationDashboard.Application.DTOs.User;
using BBB_ApplicationDashboard.Application.Interfaces;
using BBB_ApplicationDashboard.Infrastructure.Data.Context;
using BBB_ApplicationDashboard.Infrastructure.Exceptions.User;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;

namespace BBB_ApplicationDashboard.Infrastructure.Services.User;

public class UserService(ApplicationDbContext context) : IUserService
{
    public async Task<Domain.Entities.User?> FindUser(string email) =>
        await context.Users.FirstOrDefaultAsync(user => user.Email == email);

    public async Task CreateUser(Domain.Entities.User user)
    {
        user.Email = user.Email.ToLowerInvariant();
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }

    public async Task<PaginatedResponse<AdminDashboardUserResponse>> GetAdminDashboardUsers(
        UserPaginationRequest request
    )
    {
        //! 1) Get all users with Source Internal
        var query = context
            .Users.AsNoTracking()
            .Where(u => u.UserSource == Domain.ValueObjects.Source.Internal);

        //! 2) filter by email
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim();
            query = query.Where(u => EF.Functions.ILike(u.Email, $"%{searchTerm}%"));
        }

        //! 3) filter by isAdmin
        if (request.IsAdmin.HasValue)
        {
            query = query.Where(u => u.IsAdmin == request.IsAdmin.Value);
        }

        //! 4) filter by isCSVSync
        if (request.IsCSVSync.HasValue)
        {
            query = query.Where(u => u.IsCSVSync == request.IsCSVSync.Value);
        }

        //! 5) filter by isActive
        if (request.IsActive.HasValue)
        {
            query = query.Where(u => u.IsCSVSync == request.IsActive.Value);
        }

        //! 6) get total count
        int total = await query.CountAsync();

        //! 7) Apply pagination
        int pageIndex = request.PageNumber - 1;
        int pageSize = Math.Max(1, Math.Min(100, request.PageSize));

        //! 8) execute query
        IEnumerable<AdminDashboardUserResponse> users = await query
            .OrderBy(a => a.Email)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .Select(a => new AdminDashboardUserResponse
            {
                UserId = a.UserId,
                Email = a.Email,
                IsActive = a.IsActive,
                IsAdmin = a.IsAdmin,
                IsCSVSync = a.IsCSVSync,
            })
            .ToListAsync();

        //! 9) return result
        return new PaginatedResponse<AdminDashboardUserResponse>(pageIndex, pageSize, total, users);
    }

    public async Task DeleteUser(Guid id)
    {
        var user =
            await context.Users.FindAsync(id) ?? throw new UserNotFoundException("user not found");
        context.Users.Remove(user);
        await context.SaveChangesAsync();
    }

    public async Task CreateAdminDashboardUser(AdminDashboardCreateUserRequest request)
    {
        //! 1) check if email is duplicate
        var user = await FindUser(request.Email.ToLowerInvariant());
        if (user is not null)
            throw new UserBadRequestException("User already exists");

        //! 2) create user
        var newUser = new Domain.Entities.User
        {
            UserSource = Domain.ValueObjects.Source.Internal,
            Email = request.Email.ToLowerInvariant().Trim(),
            IsAdmin = request.IsAdmin,
            IsCSVSync = request.IsCSVSync,
        };

        //! 3) save into database
        await context.AddAsync(newUser);
        await context.SaveChangesAsync();
    }

    public async Task CreateAdminDashboardUsers(string usersCsv)
    {
        if (string.IsNullOrWhiteSpace(usersCsv))
            return;

        using var reader = new StringReader(usersCsv);
        string? rawLine;
        int lineNumber = 0;

        const int BatchSize = 1000;
        const int MaxLines = 50_000;

        var currentBatch = new List<Domain.Entities.User>(capacity: BatchSize);
        var failed = new List<string>(); // you could later replace this with a DTO if you want feedback

        while ((rawLine = await reader.ReadLineAsync()) != null)
        {
            lineNumber++;
            if (lineNumber > MaxLines)
                throw new UserBadRequestException(
                    $"Input exceeds maximum allowed lines of {MaxLines}"
                );

            var line = rawLine.Trim();
            if (line.Length == 0 || line.StartsWith("#"))
                continue;

            // Handle UTF-8 BOM on first line
            if (lineNumber == 1 && line.Length > 0 && line[0] == '\uFEFF')
                line = line.Substring(1);

            var fields = SplitSemicolonAware(line);
            if (fields.Count == 0)
                continue;

            var rawEmail = fields[0].Trim().Trim('"').Trim();
            if (string.IsNullOrWhiteSpace(rawEmail))
            {
                failed.Add($"Line {lineNumber}: Email field is empty");
                continue;
            }

            string normalizedEmail;
            try
            {
                var addr = new System.Net.Mail.MailAddress(rawEmail);
                normalizedEmail = addr.Address.Trim().ToLowerInvariant();
            }
            catch (FormatException)
            {
                failed.Add($"Line {lineNumber}: Invalid email format ({rawEmail})");
                continue;
            }

            // Parse flags
            var flags = fields
                .Skip(1)
                .Select(p => p.Trim().Trim('"').ToLowerInvariant())
                .Where(p => p.Length > 0)
                .ToArray();

            var validFlags = new[] { "admin", "csvsync" };
            var invalidFlags = flags.Where(f => !validFlags.Contains(f)).ToArray();
            if (invalidFlags.Length > 0)
            {
                failed.Add($"Line {lineNumber}: Invalid flags: {string.Join(", ", invalidFlags)}");
                continue;
            }

            var isAdmin = flags.Contains("admin");
            var isCSVSync = flags.Contains("csvsync");

            if (isAdmin)
            {
                failed.Add($"Line {lineNumber}: Admins cannot be created via bulk import");
                continue;
            }

            // Check duplicates in DB
            var exists = await context.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail);
            if (exists)
            {
                failed.Add($"Line {lineNumber}: User {normalizedEmail} already exists");
                continue;
            }

            // Build entity
            currentBatch.Add(
                new Domain.Entities.User
                {
                    UserSource = Domain.ValueObjects.Source.Internal,
                    Email = normalizedEmail,
                    IsAdmin = false, // bulk import can't set Admin
                    IsCSVSync = isCSVSync,
                    IsActive = true,
                }
            );

            // Save in batches
            if (currentBatch.Count >= BatchSize)
            {
                await context.Users.AddRangeAsync(currentBatch);
                await context.SaveChangesAsync();
                currentBatch.Clear();
            }
        }

        // Final save
        if (currentBatch.Count > 0)
        {
            await context.Users.AddRangeAsync(currentBatch);
            await context.SaveChangesAsync();
        }

        if (failed.Count > 0)
        {
            // for now just throw — or you could return a DTO like BulkCreateResult
            throw new UserBadRequestException(
                $"Some users failed to import:\n{string.Join("\n", failed)}"
            );
        }
    }

    private static List<string> SplitSemicolonAware(string input)
    {
        var result = new List<string>();
        if (string.IsNullOrEmpty(input))
            return result;

        var sb = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < input.Length; i++)
        {
            var c = input[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < input.Length && input[i + 1] == '"')
                {
                    sb.Append('"');
                    i++;
                    continue;
                }
                inQuotes = !inQuotes;
                continue;
            }

            if (!inQuotes && c == ';')
            {
                result.Add(sb.ToString());
                sb.Clear();
                continue;
            }

            sb.Append(c);
        }

        result.Add(sb.ToString());
        return result;
    }

    public async Task UpdateAdminDashboardUser(Guid id, AdminDashboardUpdateUserRequest request)
    {
        //! 1) find user by id
        var user =
            await context.Users.FindAsync(id)
            ?? throw new UserNotFoundException($"User not found.");

        //! 2) update email if provided
        if (!string.IsNullOrEmpty(request.Email))
        {
            user.Email = request.Email.ToLowerInvariant();
        }

        //! 3) update IsCsvSync if provided
        if (request.IsCSVSync.HasValue)
        {
            user.IsCSVSync = request.IsCSVSync.Value;
        }

        //! 4) update IsActive if provided
        if (request.IsActive.HasValue)
        {
            user.IsActive = request.IsActive.Value;
        }

        await context.SaveChangesAsync();
    }

    public async Task<List<string>> GetAdminDashboardCSVUsers()
    {
        return await context
            .Users.AsNoTracking()
            .Where(u => u.IsAdmin == false && u.IsCSVSync == true)
            .Select(u => u.Email)
            .ToListAsync();
    }
}
