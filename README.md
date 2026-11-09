## RaceDay Event Management System

## System Description
RaceDay is a full-stack web-based event management system designed specifically for the South African road running, walking, and cycling community. The platform streamlines the event lifecycle, allowing Organisers to efficiently manage events, categories, and participant results, while enabling Participants to browse upcoming events, enter races, and track their personal performance history.

## User Roles
The system enforces strict role-based access control for two distinct user types:

## 1. Organiser
Organisers are responsible for the creation and management of events. Their permissions include:
* Creating, editing, and deleting events.
* Managing event categories (e.g., age groups, distances).
* Capturing and publishing participant results (finish times and positions).
* Viewing all participant enrolments for their events.

### 2. Participant
Participants are the end-users who engage with the events. Their permissions include:
* Creating a personal account and managing their profile.
* Browsing upcoming events and filtering by type.
* Entering an event by selecting a specific category.
* Viewing their personal enrolment status.
* Tracking their personal race history and viewing their official results.

## Setup Instructions (How to Run the Database)

To set up the RaceDay database locally, follow these steps:

1. Ensure you have **SQL Server Management Studio (SSMS)** installed and connected to your local SQL Server instance.
2. Navigate to the `/docs` folder in this repository.
3. Open the `RaceDay_Schema.sql` file.
4. Copy the entire contents of the script and paste it into a **New Query** window in SSMS.
5. Click **Execute** (or press F5). The script will automatically create the `RaceDayDB` database, all required tables, constraints, and seed the database with realistic sample data.
6. To verify the setup, expand the `RaceDayDB` database in the Object Explorer, right-click on the `dbo.Users` table, and select **Select Top 1000 Rows** to view the seeded Organisers and Participants.
## CI/CD Build Status
The repository uses GitHub Actions to validate the Part 1 documentation structure.
![CI/CD Green Build](ERD-RaceDay.png.)

## Video Presentation
[Watch the Part 1 Video Presentation on YouTube](https://youtu.be/9gdheO_wHHg)

## AI Usage Disclosure
AI tools were utilized to assist in the initial planning and structuring of the database schema, generating templates for the API endpoint table, and drafting the GitHub Actions CI/CD workflow. All Entity Relationship Diagram (ERD) design, SQL script testing in SSMS, final documentation, and the video presentation were completed independently by the student.


   ---
   ## Part 2: Backend API & Testing

### 1. API Documentation (Swagger)
The API is fully documented using Swagger. You can test endpoints directly in the browser.
![Swagger UI](docs/swagger-auth-test.png)

### 2. Unit Testing
We implemented xUnit tests to ensure security and functionality:
- **Authentication:** Tests successful registration and duplicate email rejection.
- **Role Rejection:** Proves that Participants cannot create Events (returns 403 Forbidden).
- **Enrolments:** Tests successful event enrolment for Participants.
![Test Explorer](docs/test-explorer-green.png)

### 3. CI/CD Pipeline
A GitHub Actions workflow was added to automatically build the .NET project and run all unit tests on every push.
![CI/CD Pipeline](docs/ci-cd-dotnet.png) 

PART 3 

# RaceDay - Event Management System

## Project Overview
RaceDay is a full-stack web application designed to manage running events, participant enrolments, and race results. The system features role-based access control, separating functionality for Event Organisers and Race Participants.

## Technologies Used
- **Backend API:** ASP.NET Core Web API (.NET 8)
- **Frontend:** ASP.NET Core MVC
- **Database:** SQL Server (via Entity Framework Core)
- **Language:** C#
- **Frontend Styling:** HTML5, CSS3, Bootstrap

## Features Implemented

### Authentication & Authorization
- Secure login and registration system.
- Role-based dashboards for **Organisers** and **Participants**.
- Session management and cookie-based authentication.

### Organiser Features
- **Dashboard:** View and manage upcoming events.
- **Manage Categories:** Create and manage race distance categories (e.g., 10km, 21km, 42km).
- **Enter Results:** Input finish times and positions for participants after a race.

### Participant Features
- **Dashboard:** View available upcoming events.
- **Enrolments:** Sign up for available races.
- **My Enrolments:** View a list of all races the participant has signed up for.
- **My Results:** View personal race history, including finish times and final positions.

---

## How to Run the Project

*Note: To avoid IIS Express port conflicts, please run the projects using the terminal as described below.*

### 1. Database Setup
1. Open the solution in Visual Studio.
2. Open the **Package Manager Console** (`Tools` > `NuGet Package Manager` > `Package Manager Console`).
3. Ensure the **Default project** is set to `RaceDay.API`.
4. Run the following command to ensure the database is up to date:
   ```text
   Update-Database
