# FullCalendar .NET 10 with Authentication

> Implementation of [FullCalendar](https://fullcalendar.io/) in ASP.NET Core with ASP.NET Core Identity

This project is a fork of [esausilva/fullcalendar-aspnet-core](https://github.com/esausilva/fullcalendar-aspnet-core) with added user authentication, role-based access control, and multi-user support. The original implementation by [Esau Silva](https://github.com/esausilva) has been extended with ASP.NET Core Identity integration.

The project includes the implementation of FullCalendar in JavaScript ES6+ and comes wired with the necessary database access layer to interact with SQLite database.

## Technology Stack

- **Framework**: ASP.NET Core (.NET 10)
- **Authentication**: ASP.NET Core Identity
- **Database**: SQLite with Entity Framework Core
- **Frontend**: FullCalendar JavaScript (ES6+), Webpack, Sass
- **Bundling**: Webpack with NPM Task Runner

## Features

### Authentication & Authorization
- User login and logout
- Role-based access control (Admin and User roles)
- User registration (Admin only)
- User management (Admin only)
- Change password functionality
- Default admin account created on first run

### Calendar Features
- Create single day events
- Create all day events
- Update existing events
- Delete events
- User-specific events (each user sees only their own events)

## Setting Up

### Frontend

You will need to have [Node](https://nodejs.org) and npm installed as I use [Webpack](https://webpack.js.org) to compile my Sass files and modern ES6+ JavaScript files to JavaScript browsers can understand.

Clone this repository then in **Command Prompt** navigate to the project's directory and install NPM packages

```
cd [project's path]\fullcalendar-core\fullcalendar-core
npm i
```

You will also need [NPM Task Runner](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.NPMTaskRunner) as I have set up to run Webpack in watch mode when the project opens. Webpack will also run before each build _building_ the JavaScript for production. You can see the bindings if you open **Task Runner** within Visual Studio

### Backend

The application uses **SQLite** database which will be automatically created on first run.

1. Open `appsettings.json` and modify the connection string if needed (default is `Data Source=FullCalendar.db`).

2. The database tables will be created automatically when you first run the application:
   - **Events** table - for calendar events
   - **AspNetUsers** table - for user accounts
   - **AspNetRoles** table - for user roles
   - Other ASP.NET Identity tables

3. A default admin account will be created automatically:
   - **Username**: `admin`
   - **Password**: `Admin123!`
   - **Email**: `admin@wardenaar.org`

4. After first login, you should:
   - Change the default admin password
   - Create additional user accounts as needed

## Database Schema

### Events Table
| Column Name  | Data Type    | Description                    |
| ------------ | ------------ | ------------------------------ |
| event_id     | INTEGER PK   | Auto-increment, not null       |
| title        | TEXT         | Event title, not null          |
| description  | TEXT         | Event description              |
| event_start  | TEXT         | Start date/time (ISO 8601)     |
| event_end    | TEXT         | End date/time (ISO 8601)       |
| all_day      | INTEGER      | Boolean (0/1), not null        |
| user_id      | TEXT         | FK to AspNetUsers.Id           |
| user_name    | TEXT         | Display name of event owner    |

### Identity Tables
The application uses standard ASP.NET Core Identity tables for user management.

## Things to Know

The source Sass and JavaScript files are found under `.\Styles` and `.\Scripts` directories. Webpack will compile them every time upon save and place the output files under `.\wwwroot\css` and `.\wwwroot\js`, so **DO NOT** modify these output files directly.

For production, Webpack will minify and optimize the output files appending `*.min.*` to the file name. The CSS will also get auto-prefixed.

## User Roles

- **Admin**: Can manage users, create/edit/delete all events, and access all administrative features
- **User**: Can create, edit, and delete their own events

## Key Changes from Original Repository

This fork adds the following features to the [original repository](https://github.com/esausilva/fullcalendar-aspnet-core) by Esau Silva:

1. **ASP.NET Core Identity Integration**
   - Complete authentication system with login/logout
   - Role-based authorization (Admin and User roles)
   - User management interface

2. **Database Migration**
   - Changed from SQL Server to SQLite
   - Automatic database creation and migration
   - Added user_id and user_name fields to Events table

3. **Multi-User Support**
   - Each user can only see their own events
   - User-specific event management

4. **Framework Update**
   - Updated to .NET 10

5. **Security Improvements**
   - Password policies
   - Anti-forgery tokens
   - Secure cookie configuration

## Implemented Features

**Authentication & User Management:**
- User login and logout
- User registration (Admin only)
- Password change
- User management interface (Admin only)
- Role-based access control

**Calendar Features:**
- Create single day events
- Create all day events
- Update existing events
- Delete events
- User-specific events (each user sees only their own events)

## Upcoming Features

- Drag & Drop events
- Resize events
- Event sharing between users
- Export calendar to iCal format

_In no particular order_

## Credits

**Original Repository**: [fullcalendar-aspnet-core](https://github.com/esausilva/fullcalendar-aspnet-core) by [Esau Silva](https://github.com/esausilva)

This fork extends the original implementation with authentication, user management, and multi-user support.

## Giving Back

If you would like to support the original author's work, consider getting Esau Silva a coffee:

[![Buy Me A Coffee](https://www.buymeacoffee.com/assets/img/custom_images/black_img.png)](https://www.buymeacoffee.com/esausilva)

## Preview

![Imgur](https://i.imgur.com/p6BjJ2Vm.jpg)

![Imgur](https://i.imgur.com/3378pXYm.jpg)

![Imgur](https://i.imgur.com/nlDoTsQm.jpg)

---

## License

This project is based on [fullcalendar-aspnet-core](https://github.com/esausilva/fullcalendar-aspnet-core) by Esau Silva.

-Roland Wardenaar
