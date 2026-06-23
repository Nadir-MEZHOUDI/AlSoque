# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release

# Tailwind v4 standalone CLI (linux-x64). The binary committed under tools/ for
# local Windows dev won't run here; fetch the matching Linux build instead —
# no Node.js needed in the image.
RUN curl -sLO https://github.com/tailwindlabs/tailwindcss/releases/latest/download/tailwindcss-linux-x64 \
 && chmod +x tailwindcss-linux-x64 \
 && mv tailwindcss-linux-x64 /usr/local/bin/tailwindcss

WORKDIR /src

COPY ["src/AlSoque.Web/AlSoque.Web.csproj",   "src/AlSoque.Web/"]
COPY ["src/AlSoque.Data/AlSoque.Data.csproj", "src/AlSoque.Data/"]

RUN dotnet restore "src/AlSoque.Web/AlSoque.Web.csproj"

COPY . .

WORKDIR "/src/src/AlSoque.Web"

# Compile Tailwind v4 BEFORE dotnet publish so wwwroot/app.css is in the output.
RUN tailwindcss \
        -i Styles/app.tailwind.css \
        -o wwwroot/app.css \
        --minify

RUN dotnet publish "AlSoque.Web.csproj" \
        -c $BUILD_CONFIGURATION \
        -o /app/publish \
        /p:UseAppHost=false

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "AlSoque.Web.dll"]
