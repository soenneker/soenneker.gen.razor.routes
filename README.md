[![](https://img.shields.io/nuget/v/soenneker.gen.razor.routes.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.gen.razor.routes/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.gen.razor.routes/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.gen.razor.routes/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.gen.razor.routes.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.gen.razor.routes/)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Gen.Razor.Routes
### Automatic route file generation for Razor apps at build time. Fast, deterministic, and zero-runtime overhead.

## Installation

```
dotnet add package Soenneker.Gen.Razor.Routes
```

## Usage

Configure the consuming app project:

```xml
<PropertyGroup>
  <RazorRoutesEnabled>true</RazorRoutesEnabled>
  <RazorRoutesBlazorAppDirectory>..\MyBlazorApp</RazorRoutesBlazorAppDirectory>
  <RazorRoutesOutputPath>wwwroot/routes.txt</RazorRoutesOutputPath>
</PropertyGroup>
```

`RazorRoutesBlazorAppDirectory` can be absolute or relative to the consuming project. `RazorRoutesOutputPath` can also be absolute or relative to the consuming project.

The generated file contains one route template per line, discovered from `.razor` files with `@page` directives. Dynamic templates such as `/products/{id:int}` are included by default. To omit them:

```xml
<PropertyGroup>
  <RazorRoutesIncludeDynamicRoutes>false</RazorRoutesIncludeDynamicRoutes>
</PropertyGroup>
```

To disable generation:

```xml
<PropertyGroup>
  <RazorRoutesEnabled>false</RazorRoutesEnabled>
</PropertyGroup>
```
