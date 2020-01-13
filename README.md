# Armaiti.Identity

## What is Armaiti.Identity?

Armaiti.Identity is a nuget package that focuses on localization features to localize identity models, identity pages and identity errors.

-   **ArmaitiUser** is a custom implementation of [IdentityUser](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.entityframeworkcore.identityuser?view=aspnetcore-1.1) with fullname feature.

-   **IdentityLocalizerExtensions** Adds a set of common identity services to the application, including UI, localizer, token providers, and configures authentication to use identity cookies.
    You can add localized Identity UI in one step by using AddIdentityLocalizer extension method.
    ```
    public class Startup
    {
        ...
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // IdentityResource is a resource class that include all localization resources of Identity models, Identity pages and Identity errors. All resx files are available [here](https://github.com/aras-it/Armaiti.Identity/tree/master/_files/resources).
            services.AddIdentityLocalizer<ArmaitiUser, IdentityResource>(options =>
                options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            ...
		}
        ...
	}
    ```

-   **IdentityErrorLocalizer** is a generic service to enable localization for application facing identity errors.

-   **IdentityModelsLocalizer** is a generic service to enable localization for application facing identity models.

-   **IdentityPagesLocalizer** is a generic service to enable localization for application facing identity pages.


## Installation [![nuget](https://cdn.arasit.com/img/nuget/nuget1.1.0.svg)](https://www.nuget.org/packages/Armaiti.Identity/)

**Package Manager:**  `PM> Install-Package Armaiti.Identity -Version 1.1.0`

**.NET CLI:**         `> dotnet add package Armaiti.Identity --version 1.1.0`

**PackageReference:** `<PackageReference Include="Armaiti.Identity" Version="1.1.0" />`

## Dependencies
.NETCoreApp 3.1

Armaiti.Core (>= 1.1.0)

Microsoft.Extensions.Identity.Core (>= 3.1.0)

## License
MIT
