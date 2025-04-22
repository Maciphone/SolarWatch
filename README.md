<h1 align="center">SolarWatch 🌞</h1>
<a id="readme-top"></a>

![alt text](images/logo.png)

<div align="center">

</div>

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Built With](#built-with)
- [Installation](#installation)
- [Usage](#usage)
- [Contribution](#contribution)
- [License](#license)

## Overview

SolarWatch is a web application designed to manage solar data for cities, offering both public data views and administrative capabilities for city-specific information.

## Features

- **View Sunrise and Sunset Data**: Display solar data for selected cities.
- **Manage City Information**: Create, update, and delete city records (name, latitude, longitude).
- **Protected Endpoints**: Restrict certain operations to authorized users based on roles (Admin/User).
- **Frontend/Backend Communication**: Secured via JWT tokens stored in HTTPOnly cookies.
- **User Roles**: Different functionality available depending on user permissions.
- **Interactive Frontend**: Built with React and Vite for fast development.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Built With

- [![.NET Core][dotnet-core-shield]][dotnet-core-url]
- [![ASP.NET Core][aspnet-core-shield]][aspnet-core-url]
- [![Entity Framework Core][ef-core-shield]][ef-core-url]
- [![SQL Server][sql-shield]][sql-url]
- [![ASP.NET Core Identity][identity-shield]][identity-url]
- [![JWT Bearer][jwt-shield]][jwt-url]
- [![Swashbuckle Swagger][swagger-shield]][swagger-url]
- [![React][react-shield]][react-url]
- [![Vite][vite-shield]][vite-url]
- [![React Router][router-shield]][router-url]
- [![Docker][docker-shield]][docker-url]

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Installation

### Prerequisites

- .NET 8.0 SDK
- Node.js
- Docker & Docker Compose (optional)

### Backend Setup

```bash
cd Backend/SolarWatch
dotnet restore
dotnet build
dotnet run
```

### Frontend Setup

```bash
cd Frontend
npm install
npm run dev
```

### Using Docker

```bash
docker-compose up --build
```

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Usage

1. Register a new account or log in.
2. View solar data for cities.
3. If authorized, manage city information via the admin interface.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Contribution

Contributions are welcome! Please fork the repository, create a feature branch, and submit a pull request.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## License

This project is licensed under the MIT License.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- MARKDOWN LINKS & IMAGES -->

[dotnet-core-shield]: https://img.shields.io/badge/.NET%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white
[dotnet-core-url]: https://dotnet.microsoft.com/
[aspnet-core-shield]: https://img.shields.io/badge/ASP.NET%20Core-339933?style=for-the-badge&logo=aspdotnet&logoColor=white
[aspnet-core-url]: https://docs.microsoft.com/aspnet/core/
[ef-core-shield]: https://img.shields.io/badge/EF%20Core-512BD4?style=for-the-badge&logo=entity-framework&logoColor=white
[ef-core-url]: https://docs.microsoft.com/ef/core/
[sql-shield]: https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft%20sql%20server&logoColor=white
[sql-url]: https://www.microsoft.com/sql-server
[identity-shield]: https://img.shields.io/badge/Identity-000000?style=for-the-badge&logo=aspdotnetcore&logoColor=white
[identity-url]: https://docs.microsoft.com/aspnet/core/security/authentication/identity
[jwt-shield]: https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=json-web-tokens&logoColor=white
[jwt-url]: https://jwt.io/
[swagger-shield]: https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black
[swagger-url]: https://swagger.io/
[react-shield]: https://img.shields.io/badge/React-20232A?style=for-the-badge&logo=react&logoColor=61DAFB
[react-url]: https://reactjs.org/
[vite-shield]: https://img.shields.io/badge/Vite-646CFF?style=for-the-badge&logo=vite&logoColor=white
[vite-url]: https://vitejs.dev/
[router-shield]: https://img.shields.io/badge/React%20Router-CA4245?style=for-the-badge&logo=react-router&logoColor=white
[router-url]: https://reactrouter.com/
[docker-shield]: https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white
[docker-url]: https://www.docker.com/
