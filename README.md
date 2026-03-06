# Whispr Signaling Server
Backend for WebRTC signaling. Handles session coordination and CORS configuration.

## Backend Configuration

The backend primarily needs **CORS configuration** to allow frontend connections. You can set this in **two ways**:

1. **`appsettings.Production.json`**  

Create or update `appsettings.Production.json` with:

```json
{
  "AllowedOrigins": [
    "https://your-frontend-domain.com",
    "https://another-allowed-origin.com"
  ]
}
```

2. **Environment variables**  

Set each allowed origin via environment variables either through .env or managed hosting:

```bash
ALLOWEDORIGINS__0=https://your-frontend-domain.com
ALLOWEDORIGINS__1=https://another-allowed-origin.com
```

When the backend starts in production, it will read the allowed origins and enforce CORS accordingly.
