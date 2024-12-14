# SolarWatch

# SolarWatch 🌞

SolarWatch is a web application designed to manage solar data for cities, including sunrise and sunset times, and provides administrative capabilities for managing city-specific information.

## Features

- **View Sunrise and Sunset Data**: Display solar data for cities.
- **Manage City Information**: Update city details like latitude, longitude, name, etc.
- **Protected Endpoints**: Restrict certain operations (e.g., updates, deletions) to authorized users.
- **Frontend/Backend Communication**: Secured using JWT tokens stored in httpOnly cookies.
- **User Roles**: Different functionality available based on user roles (Admin/User).
- **Interactive Frontend**: Built with React, styled for ease of use.

---

## Installation Guide

### Prerequisites

To run this project locally, you will need:

- **Node.js**: [Download here](https://nodejs.org/)
- **.NET SDK**: [Download here](https://dotnet.microsoft.com/)
- **Database**: SQL Server or a compatible database.

---

### Backend Setup

1. Navigate to the backend directory:
   ```bash
   cd Backend/SolarWatch
   ```

2. Restore required NuGet packages:
   ```bash
   dotnet restore
   ```

3. Run the backend application:
   ```bash
   dotnet run
   ```

---

### Frontend Setup

1. Navigate to the frontend directory:
   ```bash
   cd Frontend
   ```

2. Install npm dependencies:
   ```bash
   npm install
   ```

3. Start the development server:
   ```bash
   npm run dev
   ```

---

### Access the Application

- Once both the backend and frontend are running, open your browser and go to: [http://localhost:3000](http://localhost:3000)
- Log in with the following test credentials:
  - **Username**: `test`
  - **Password**: `password`

---

## Technologies Used

- **Frontend**: React, Vite
- **Backend**: ASP.NET Core
- **Database**: Entity Framework Core with SQL Server
- **Authentication**: JWT (JSON Web Tokens)
- **State Management**: React Context and Cookies

---

## Usage and Functionality

### Public Endpoints

1. **Welcome Page**: `/`
   - Displays a simple welcome page.
   
2. **Login**: `/login`
   - Allows users to log in and obtain access to restricted features.

3. **Registration**: `/registration`
   - Enables new users to sign up.

---

### Protected Endpoints

These endpoints require authentication and/or specific roles (e.g., Admin).

1. **View All Data**: `/all`
   - Available for authenticated users.
   - Displays a list of all cities and their solar data.

2. **Authenticated View**: `/allAuthenticated`
   - Only accessible to logged-in users with valid JWT tokens.

3. **Admin View**: `/allAdmin`
   - Restricted to users with the Admin role.
   - Displays data specifically for administrative purposes.

4. **Update City Information**: `/update/:cityId`
   - Allows authorized users to update details of a specific city.
   - Restricted by role and authentication.

5. **Manage Solar Data**: `/solarwatch`
   - Provides options to manage sunrise and sunset data.

---

## Authentication and Authorization

- **JWT Tokens**: Used for secure authentication.
- **Role-Based Access Control**: Certain routes and actions are restricted based on the user's role (Admin/User).
- **Protected Components**: Implemented using `RouteWithRole` for dynamic role-based rendering.

---

## Example Protected Routes

### Backend API Endpoints

- **GET /api/SolarWatch/GetAll**
  - Fetches all solar watch data.
  - Requires a valid JWT token in the `Authorization` header.

- **POST /api/SolarWatch/UpdateCity**
  - Updates city information.
  - Restricted to Admin users.
  - Requires a valid JWT token.

- **DELETE /api/SolarWatch/DeleteCity**
  - Deletes a city and cascades related data.
  - Restricted to Admin users.
  - Requires a valid JWT token.

- **POST /api/SolarWatch/UpdateSunsetSunrise**
  - Updates sunrise and sunset data for a city.
  - Requires authentication and Admin privileges.

---

## Running Tests

The project includes integration tests for validating backend functionality.

### Run Tests
1. Navigate to the `IntegrationTest` or relevant test project directory:
   ```bash
   cd Backend/IntegrationTest
   ```

2. Execute the tests:
   ```bash
   dotnet test
   ```

---

## Contributing

1. Fork the repository.
2. Create a new branch for your feature:
   ```bash
   git checkout -b feature-name
   ```
3. Commit your changes:
   ```bash
   git commit -m "Add your message here"
   ```
4. Push to your branch:
   ```bash
   git push origin feature-name
   ```
5. Create a Pull Request.

---

## License

This project is licensed under the MIT License.

---

Feel free to suggest modifications or include additional sections if needed!

