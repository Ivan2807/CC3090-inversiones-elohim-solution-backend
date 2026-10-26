using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ElohimShop.API.Controllers;
using ElohimShop.Application.Platform;
using ElohimShop.Domain.Platform;
using ElohimShop.Infrastructure.Persistence;
using ElohimShop.Infrastructure.Platform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using PlatformUser = ElohimShop.Domain.Platform.User;

namespace ElohimShop.Tests.Platform;

public class CrearProductosBulkSecurityTests
{
    private const string TenantId = "tenant-current";
    private const string OtherTenantId = "tenant-other";

    [Fact]
    public void CrearBulk_RequiresAuthorization()
    {
        var method = typeof(ProductosV1Controller).GetMethod(nameof(ProductosV1Controller.CrearBulk));

        Assert.NotNull(method);
        Assert.NotNull(Attribute.GetCustomAttribute(method, typeof(AuthorizeAttribute)));
    }

    [Fact]
    public async Task CrearProductosBulkAsync_AnonymousUser_ThrowsUnauthorizedAccessException()
    {
        var (service, dbContext) = CreateService(new ClaimsPrincipal(new ClaimsIdentity()));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.CrearProductosBulkAsync([CreateProductInput()], CancellationToken.None));

        Assert.Empty(await dbContext.Productos.ToListAsync());
    }

    [Fact]
    public async Task CrearProductosBulkAsync_AdministratorFromOtherTenant_ThrowsUnauthorizedAccessException()
    {
        var userId = Guid.NewGuid().ToString();
        var (service, dbContext) = CreateService(CreatePrincipal(userId));
        dbContext.Users.Add(CreateAdministrator(userId, OtherTenantId));
        await dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.CrearProductosBulkAsync([CreateProductInput()], CancellationToken.None));

        Assert.Empty(await dbContext.Productos.ToListAsync());
    }

    [Fact]
    public async Task CrearProductosBulkAsync_CategoryFromOtherTenant_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid().ToString();
        var (service, dbContext) = CreateService(CreatePrincipal(userId));
        var foreignCategory = new Categoria
        {
            Id = Guid.NewGuid().ToString(),
            TiendaId = OtherTenantId,
            Nombre = "Foreign category",
            Slug = "foreign-category"
        };
        dbContext.Users.Add(CreateAdministrator(userId, TenantId));
        dbContext.Categorias.Add(foreignCategory);
        await dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CrearProductosBulkAsync([CreateProductInput(foreignCategory.Id)], CancellationToken.None));

        Assert.Empty(await dbContext.Productos.ToListAsync());
    }

    [Fact]
    public async Task CrearProductosBulkAsync_TenantAdministratorAndCategoryInTenant_CreatesProduct()
    {
        var userId = Guid.NewGuid().ToString();
        var (service, dbContext) = CreateService(CreatePrincipal(userId));
        var category = new Categoria
        {
            Id = Guid.NewGuid().ToString(),
            TiendaId = TenantId,
            Nombre = "Valid category",
            Slug = "valid-category"
        };
        dbContext.Users.Add(CreateAdministrator(userId, TenantId));
        dbContext.Categorias.Add(category);
        await dbContext.SaveChangesAsync();

        var products = await service.CrearProductosBulkAsync(
            [CreateProductInput(category.Id)],
            CancellationToken.None);

        var product = Assert.Single(products);
        Assert.Equal(TenantId, product.TiendaId);
        Assert.Equal(category.Id, product.CategoriaId);
    }

    private static (PlatformService Service, PlatformDbContext DbContext) CreateService(ClaimsPrincipal user)
    {
        var tenantProvider = new Mock<ITenantProvider>();
        tenantProvider.Setup(provider => provider.GetTenantId()).Returns(TenantId);

        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var dbContext = new PlatformDbContext(options, tenantProvider.Object);
        var httpContext = new DefaultHttpContext { User = user };
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

        return (
            new PlatformService(dbContext, tenantProvider.Object, httpContextAccessor.Object),
            dbContext);
    }

    private static ClaimsPrincipal CreatePrincipal(string userId)
    {
        var identity = new ClaimsIdentity(
            [new Claim(JwtRegisteredClaimNames.Sub, userId)],
            "TestAuth");
        return new ClaimsPrincipal(identity);
    }

    private static PlatformUser CreateAdministrator(string userId, string tenantId)
    {
        return new PlatformUser
        {
            Id = userId,
            Name = "Tenant administrator",
            Email = $"{userId}@example.test",
            TiendaId = tenantId,
            TipoUsuario = "staff",
            RolStaff = "administrador",
            Estado = true
        };
    }

    private static CrearProductoBulkInput CreateProductInput(string? categoryId = null)
    {
        return new CrearProductoBulkInput(
            "Test product",
            10m,
            15m,
            categoryId);
    }
}