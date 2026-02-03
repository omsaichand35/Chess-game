# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Chess/Chess.csproj Chess/
RUN dotnet restore Chess/Chess.csproj
COPY . .
RUN dotnet publish Chess/Chess.csproj -c Release -o /app/publish --self-contained false

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app

# Install X11 and Avalonia dependencies
RUN apt-get update && apt-get install -y --no-install-recommends \
    libfontconfig1 \
    libfreetype6 \
    libx11-6 \
    libxext6 \
    libxrender1 \
    libxfixes3 \
    libxrandr2 \
    libxcursor1 \
    libxi6 \
    libxinerama1 \
    libxcomposite1 \
    libxdamage1 \
    libgl1 \
    libcairo2 \
    libpango-1.0-0 \
    libpangocairo-1.0-0 \
    libgdk-pixbuf-2.0-0 \
    libatk1.0-0 \
    libatspi2.0-0 \
    libice6 \
    libsm6 \
    && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=build /app/publish .

# Create volumes for Stockfish and data
VOLUME ["/app/stockfish", "/app/data"]

ENV DOTNET_EnableDiagnostics=0
ENV STOCKFISH_PATH=/app/stockfish/stockfish

ENTRYPOINT ["./Chess"]
