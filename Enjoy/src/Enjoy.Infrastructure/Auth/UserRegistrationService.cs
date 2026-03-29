using Enjoy.Application.Abstractions.Authentication;
using Enjoy.Domain.Shared.Errors;
using Enjoy.Domain.Shared.Results;
using Enjoy.Domain.Users.Entities;
using Enjoy.Domain.Users.Repositories;
using Enjoy.Domain.Users.ValueObjects;
using Enjoy.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Enjoy.Infrastructure.Auth;

internal sealed class UserRegistrationService(
    UserManager<IdentityUser> userManager,
    ApplicationIdentityDbContext identityDbContext,
    ApplicationDbContext applicationDbContext,
    IUserRepository userRepository,
    IAuthResultIssuer authResultIssuer) : IUserRegistrationService
{
    public Task<Result<AuthResult>> RegisterAsync(
        string name,
        string email,
        string password,
        CancellationToken cancellationToken = default) =>
        RegisterInternalAsync(name, email, password, Role.User, cancellationToken);

    public Task<Result<AuthResult>> RegisterAsync(
        string name,
        string email,
        string password,
        string role,
        CancellationToken cancellationToken = default) =>
        RegisterInternalAsync(name, email, password, role, cancellationToken);

    private async Task<Result<AuthResult>> RegisterInternalAsync(
        string name,
        string email,
        string password,
        string role,
        CancellationToken cancellationToken)
    {
        Result<Role> roleResult = Role.Create(role);
        if (roleResult.IsFailure)
            return Result.Failure<AuthResult>(roleResult.Errors);

        string domainRole = roleResult.Value.Value;

        await using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync(cancellationToken);
        applicationDbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
        await applicationDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);

        var identityUser = new IdentityUser
        {
            UserName = email,
            Email = email
        };

        IdentityResult createResult = await userManager.CreateAsync(identityUser, password);
        if (!createResult.Succeeded)
        {
            string message = string.Join("; ", createResult.Errors.Select(e => e.Description));
            return Result.Failure<AuthResult>(Error.Validation("Identity.CreateFailed", message));
        }

        IdentityResult addRoleResult = await userManager.AddToRoleAsync(identityUser, domainRole);
        if (!addRoleResult.Succeeded)
        {
            string message = string.Join("; ", addRoleResult.Errors.Select(e => e.Description));
            return Result.Failure<AuthResult>(Error.Validation("Identity.AddRoleFailed", message));
        }

        string passwordHash = identityUser.PasswordHash ?? throw new InvalidOperationException("Identity did not set PasswordHash.");
        Result<User> userResult = User.Create(name, email, passwordHash, domainRole);
        if (userResult.IsFailure)
            return Result.Failure<AuthResult>(userResult.Errors);

        User user = userResult.Value;
        user.IdentityId = Guid.Parse(identityUser.Id);

        await userRepository.AddAsync(user);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return await authResultIssuer.IssueAsync(
            user.Id, user.Email.Value, [user.Role.Value], user.IdentityId, cancellationToken);
    }

    public async Task<Result<AuthResult>> RegisterExternalAsync(
        string name,
        string email,
        CancellationToken cancellationToken = default)
    {
        await using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync(cancellationToken);
        applicationDbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
        await applicationDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);

        string randomPassword = $"Aa1{Guid.NewGuid():N}";
        var identityUser = new IdentityUser
        {
            UserName = email,
            Email = email
        };

        IdentityResult createResult = await userManager.CreateAsync(identityUser, randomPassword);
        if (!createResult.Succeeded)
        {
            string message = string.Join("; ", createResult.Errors.Select(e => e.Description));
            return Result.Failure<AuthResult>(Error.Validation("Identity.CreateFailed", message));
        }

        IdentityResult addRoleResult = await userManager.AddToRoleAsync(identityUser, Role.User);
        if (!addRoleResult.Succeeded)
        {
            string message = string.Join("; ", addRoleResult.Errors.Select(e => e.Description));
            return Result.Failure<AuthResult>(Error.Validation("Identity.AddRoleFailed", message));
        }

        string passwordHash = identityUser.PasswordHash ?? throw new InvalidOperationException("Identity did not set PasswordHash.");
        Result<User> userResult = User.Create(name, email, passwordHash, Role.User);
        if (userResult.IsFailure)
            return Result.Failure<AuthResult>(userResult.Errors);

        User user = userResult.Value;
        user.IdentityId = Guid.Parse(identityUser.Id);
        await userRepository.AddAsync(user);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return await authResultIssuer.IssueAsync(
            user.Id, user.Email.Value, [user.Role.Value], user.IdentityId, cancellationToken);
    }
}
