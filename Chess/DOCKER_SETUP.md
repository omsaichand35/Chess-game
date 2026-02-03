# Chess Docker Setup Guide

## ?? Prerequisites

1. **Docker Desktop**: https://www.docker.com/products/docker-desktop
2. **Stockfish Engine**: https://stockfishchess.org/download/
3. **Git** (for cloning repository)

## ?? Quick Start

### Linux/Mac
```bash
chmod +x docker-setup.sh
./docker-setup.sh
```

### Windows
```bash
docker-setup.bat
```

## ?? Volume Structure

The setup uses Docker volumes for:

- **`stockfish`**: Stockfish chess engine binary
- **`chess-data`**: Game saves and application data

### Local Directory Mapping

```
./
??? stockfish/              # Mount point for Stockfish engine
?   ??? stockfish           # Binary goes here
??? chess-data/             # Mount point for data persistence
?   ??? games/              # Saved games
?   ??? config/             # Configuration files
??? docker-compose.yml      # Development setup
??? docker-compose.prod.yml # Production setup
```

## ??? Manual Setup

### 1. Create Volumes
```bash
docker volume create chess-stockfish
docker volume create chess-data
```

### 2. Prepare Stockfish

**Option A: Local Mount (Recommended for Development)**
```bash
mkdir -p ./stockfish
# Download and extract Stockfish to ./stockfish/stockfish
```

**Option B: Named Volume**
```bash
# Create container to access volume
docker run -v chess-stockfish:/mnt -it alpine sh
# cd /mnt && wget <stockfish-url>
```

### 3. Build Image
```bash
docker build -t chess:latest .
```

### 4. Run Development
```bash
docker-compose up
```

### 5. Run Production
```bash
docker-compose -f docker-compose.prod.yml up -d
```

## ?? Docker Commands

### View running containers
```bash
docker ps
```

### View logs
```bash
# Development
docker-compose logs -f

# Production
docker-compose -f docker-compose.prod.yml logs -f
```

### Stop containers
```bash
# Development
docker-compose down

# Production
docker-compose -f docker-compose.prod.yml down
```

### Access volume
```bash
# List volumes
docker volume ls

# Inspect volume
docker volume inspect chess-stockfish

# View volume contents
docker run -v chess-stockfish:/mnt -it alpine ls -la /mnt
```

### Copy files to volume
```bash
# Copy Stockfish into volume
docker run -v chess-stockfish:/mnt \
  -v $(pwd)/stockfish:/src \
  alpine cp /src/stockfish /mnt/
```

## ?? Environment Variables

You can customize behavior with:

```bash
# In docker-compose.yml or .env
STOCKFISH_PATH=/app/stockfish/stockfish
DOTNET_EnableDiagnostics=0
```

## ?? Troubleshooting

### Stockfish not found
```bash
# Check volume contents
docker run -v chess-stockfish:/mnt -it alpine ls -la /mnt/

# Copy Stockfish again
docker cp ./stockfish/stockfish <container-id>:/app/stockfish/
```

### Permission denied
```bash
# Make Stockfish executable
chmod +x ./stockfish/stockfish

# Or inside container
docker exec <container-id> chmod +x /app/stockfish/stockfish
```

### Display/GUI issues (Linux)
```bash
# Ensure X11 forwarding is enabled
docker-compose exec chess bash -c "echo \$DISPLAY"

# Add to docker-compose.yml:
# - /tmp/.X11-unix:/tmp/.X11-unix:rw
# - ~/.Xauthority:/root/.Xauthority:rw
```

### Data not persisting
```bash
# Check volume driver
docker volume inspect chess-data

# Verify mount point in container
docker exec <container-id> ls -la /app/data
```

## ?? Production Deployment

### Using Docker Swarm
```bash
docker stack deploy -c docker-compose.prod.yml chess
```

### Using Kubernetes
```bash
# Convert docker-compose to K8s manifests
kompose convert -f docker-compose.prod.yml -o chess-k8s/
kubectl apply -f chess-k8s/
```

### Health Checks
Production compose includes health checks. Verify:
```bash
docker ps
# STATUS should show "Up X seconds (healthy)"
```

## ?? Security Notes

- Don't expose ports publicly without authentication
- Use read-only volumes where possible
- Run as non-root user in production
- Keep Docker and base images updated

## ?? Notes

- Volumes persist data even after container shutdown
- Development mode includes X11 forwarding for GUI (Linux/Mac)
- Production mode runs headless without GUI
- Stockfish binary must be present in volume before starting
