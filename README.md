<div align='center'>
<h1>Headers by Omegapoint</h1>
<p>A webapp which evaluates HTTP headers for security misconfigurations</p>
</div>

## ⭐ About the Project

Headers by Omegapoint is a tool to help developers and 
security researchers analyse the HTTP headers in responses 
from web servers for security misconfigurations.

## 🧰 Getting Started

### 📜 Prerequisites

- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Node JS v20+](https://nodejs.org/en/download)
- [mkcert](https://github.com/FiloSottile/mkcert) (to automate setting up valid HTTPS certificates for Vite dev server)


### 🏃 Running locally

Both the API and frontend project need to be started separately.

#### Starting the API
1. Go to the `API` directory
    ```bash
    cd headers.security.Api
    ```
2. Start the server
    ```bash
    ASPNETCORE_URLS=https://localhost:5000 DOTNET_ENVIRONMENT=Development dotnet watch
    ```

#### Starting the SPA dev server
1. Go to the `Frontend` directory
    ```bash
    cd headers.security.Api/Frontend
    ```
2. Install dependencies
    ```bash
    npm i
    ```
3. Start the server
    ```bash
    npm run dev
    ```
4. Browse to the Vite dev server at `https://localhost:5123`