# FullCalendar .NET 10 with Authentication

> Modern, feature-rich calendar application with ASP.NET Core Identity, multi-user support, event types, recurring events, and event sharing.

This project is a fork of [esausilva/fullcalendar-aspnet-core](https://github.com/esausilva/fullcalendar-aspnet-core) with extensive enhancements including authentication, role-based access control, event type system, recurring events, and advanced UI/UX improvements.

## ✨ Key Features

- 🔐 **Authentication & Authorization** - ASP.NET Core Identity with role-based access
- 📅 **7 Event Types** - Color-coded categories with emoji icons
- 🔄 **Recurring Events** - Daily, weekly, monthly, yearly patterns
- 🤝 **Event Sharing** - Share events with view or edit permissions
- 📥 **iCal Export** - Standard .ics format for external calendars
- 🎨 **High-Contrast Design** - WCAG AA compliant colors
- 🖱️ **Drag & Drop** - Reschedule events instantly
- 📏 **Resize Events** - Adjust duration by dragging
- 🔍 **Event Filtering** - Filter by type via sidebar checkboxes
- 📱 **Responsive Layout** - Works on desktop, tablet, and mobile

## Quick Start

1. Clone the repository
2. Install dependencies: `cd fullcalendar-core && npm i`
3. Run the application (F5 in Visual Studio)
4. Login with default admin credentials:
   - Username: `admin`
   - Password: `Admin123!`
5. Change the admin password and start creating events!

The database will be created automatically on first run.

## Technology Stack

- **Framework**: ASP.NET Core (.NET 10)
- **Authentication**: ASP.NET Core Identity
- **Database**: SQLite with Entity Framework Core
- **Frontend**: FullCalendar JavaScript (ES6+), Vite, Sass
- **Bundling**: Vite build system
- **UI Framework**: Bootstrap 5
- **Icons**: Unicode emoji icons for event types

## Features

### Authentication & Authorization
- User login and logout
- Role-based access control (Admin and User roles)
- User registration (Admin only)
- User management (Admin only)
- Change password functionality
- Default admin account created on first run

### Calendar Features
- Create single day events and timed events
- Create all day events
- Update existing events (drag & drop, resize)
- Delete events
- **Event Type System** with 7 distinct categories:
  - 📅 **Afspraak** (Meeting) - Dark Blue (#1565c0)
  - 🎂 **Verjaardag** (Birthday) - Dark Pink (#c2185b)
  - 💐 **Jubileum** (Anniversary) - Dark Purple (#7b1fa2)
  - ⏰ **Herinnering** (Reminder) - Dark Orange (#ef6c00)
  - ✓ **Taak** (Task) - Dark Green (#2e7d32)
  - 🎉 **Vakantie** (Holiday) - Dark Red (#c62828)
  - 📌 **Overig** (Other) - Dark Grey (#37474f)
- **Visual Enhancements**:
  - High-contrast colors for optimal visibility
  - White borders (2px) on all events
  - Box-shadow for depth effect
  - Text-shadow for improved readability
  - All events displayed as full colored bars (no dots)
  - Smooth hover effects with transform and shadow
- **Filtering**: Filter calendar view by event type via sidebar checkboxes
- **Recurring Events**: Daily, weekly, monthly, or yearly recurrence patterns
- **Event Sharing**: Share events with other users (view or edit permissions)
- **iCal Export**: Export calendar or individual events to .ics format
- **Multi-User Support**: Each user sees their own events plus shared events
- **Responsive Layout**: Sidebar navigation with optimized calendar display
- **All Events Visible**: No "+X more" links - all events shown as stacked bars

## Setting Up

### Frontend

You will need to have [Node](https://nodejs.org) and npm installed as the project uses [Vite](https://vitejs.dev) to compile Sass files and modern ES6+ JavaScript.

Clone this repository then in **Command Prompt** navigate to the project's directory and install NPM packages

```
cd [project's path]\fullcalendar-core
npm i
```

The project uses Vite which will automatically:
- Compile Sass to CSS on every save
- Bundle JavaScript modules
- Build optimized production assets before each build
- Run in watch mode when you start development

Build commands:
- **Development**: `npm run dev` - Starts Vite dev server with hot reload
- **Production**: `npm run build` - Creates optimized production build
- **Preview**: `npm run preview` - Preview production build locally

The build is automatically triggered before compilation through Visual Studio project bindings.

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
| Column Name         | Data Type    | Description                          |
| ------------------- | ------------ | ------------------------------------ |
| event_id            | INTEGER PK   | Auto-increment, not null             |
| title               | TEXT         | Event title, not null                |
| description         | TEXT         | Event description                    |
| event_start         | TEXT         | Start date/time (ISO 8601)           |
| event_end           | TEXT         | End date/time (ISO 8601)             |
| all_day             | INTEGER      | Boolean (0/1), not null              |
| user_id             | TEXT         | FK to AspNetUsers.Id                 |
| user_name           | TEXT         | Display name of event owner          |
| event_type          | INTEGER      | Event type (0-6), default 0          |
| is_recurring        | INTEGER      | Boolean (0/1), default 0             |
| recurrence_pattern  | INTEGER      | Recurrence pattern (0-4), default 0  |
| recurrence_interval | INTEGER      | Interval for recurrence, default 1   |
| recurrence_end_date | TEXT         | End date for recurrence              |
| parent_event_id     | INTEGER      | FK to parent event for instances     |

### EventShares Table
| Column Name           | Data Type    | Description                          |
| --------------------- | ------------ | ------------------------------------ |
| share_id              | INTEGER PK   | Auto-increment, not null             |
| event_id              | INTEGER      | FK to Events.event_id                |
| owner_user_id         | TEXT         | FK to AspNetUsers.Id (owner)         |
| shared_with_user_id   | TEXT         | FK to AspNetUsers.Id (shared with)   |
| shared_with_user_name | TEXT         | Display name of shared user          |
| can_edit              | INTEGER      | Boolean (0/1), default 0             |
| shared_date           | TEXT         | When event was shared (ISO 8601)     |

### Identity Tables
The application uses standard ASP.NET Core Identity tables for user management.

## Things to Know

The source Sass and JavaScript files are found under `.\Styles` and `.\Scripts` directories. Vite will compile them and place the output files under `.\wwwroot\css` and `.\wwwroot\js`, so **DO NOT** modify these output files directly.

For production, Vite will minify and optimize the output files. The CSS will also get auto-prefixed for browser compatibility.

### Event Styling

All events are displayed with:
- **Full colored bars** - No small dots, all events are clearly visible
- **High-contrast colors** - Dark, saturated colors (WCAG AA compliant)
- **White borders** (2px) - Clear visual separation
- **Box-shadow** - Depth and 3D effect
- **Consistent display** - Timed events and all-day events look identical
- **All events visible** - No "+X more" links, events stack vertically

### Event Colors

Each event type has a distinct color that can be customized in `Library/EventTypeConfig.cs`:

| Event Type | Color | Hex Code |
|------------|-------|----------|
| 📅 Afspraak | Dark Blue | #1565c0 |
| 🎂 Verjaardag | Dark Pink | #c2185b |
| 💐 Jubileum | Dark Purple | #7b1fa2 |
| ⏰ Herinnering | Dark Orange | #ef6c00 |
| ✓ Taak | Dark Green | #2e7d32 |
| 🎉 Vakantie | Dark Red | #c62828 |
| 📌 Overig | Dark Grey | #37474f |

See `KLEUREN-WIJZIGEN-HANDLEIDING.md` for instructions on changing colors.

## User Roles

- **Admin**: Can manage users, create/edit/delete all events, access administrative features, and manage event sharing
- **User**: Can create, edit, and delete their own events, share events with other users, and view shared events

## Visual Features & Accessibility

### High-Contrast Design
All event colors have been optimized for maximum visibility and accessibility:
- **WCAG AA Compliant**: All colors meet or exceed contrast ratio requirements (4.5:1 minimum)
- **Dark Color Palette**: Saturated, dark colors for better visibility on light backgrounds
- **White Borders**: 2px white borders on all events for clear visual separation
- **Shadow Effects**: Box-shadow and text-shadow for depth and improved readability

### Event Display
- **Full Colored Bars**: All events (timed and all-day) displayed as complete colored bars
- **No Dots**: Timed events show as full bars instead of small dots
- **Stacked Layout**: Multiple events on same day stack vertically (no "+X more" links)
- **Consistent Sizing**: Uniform padding and margins across all event types
- **Hover Effects**: Smooth transform and shadow animations on hover

### Responsive Layout
- **Fixed Sidebar**: Navigation sidebar (250px width) with event type filters
- **Flexible Calendar**: Calendar content area adjusts to available space
- **Scrollable Days**: Days with many events automatically adjust height
- **Mobile-Friendly**: Responsive design works on all screen sizes

## Key Changes from Original Repository

This fork adds the following features to the [original repository](https://github.com/esausilva/fullcalendar-aspnet-core) by Esau Silva:

1. **ASP.NET Core Identity Integration**
   - Complete authentication system with login/logout
   - Role-based authorization (Admin and User roles)
   - User management interface
   - Secure password policies

2. **Database Enhancements**
   - Changed from SQL Server to SQLite
   - Automatic database creation and migration
   - Added user_id and user_name fields to Events table
   - Event sharing system with EventShares table
   - Recurring events support with pattern and interval fields

3. **Multi-User Support**
   - Each user can only see their own events
   - Event sharing between users
   - View-only and edit permissions for shared events
   - User-specific event management

4. **Advanced Calendar Features**
   - **Event Types**: 7 distinct event categories with custom colors and emoji icons
   - **Event Filtering**: Sidebar checkboxes to filter by event type
   - **Recurring Events**: Support for daily, weekly, monthly, and yearly patterns
   - **Event Sharing**: Share events with other users
   - **iCal Export**: Export to standard .ics format
   - **Drag & Drop**: Move events by dragging to new dates/times
   - **Resize Events**: Adjust event duration by dragging edges
   - **Full Event Display**: All events shown as colored bars (no "+X more" links)

5. **UI/UX Improvements**
   - **Responsive Sidebar Layout**: Fixed sidebar navigation with calendar content area
   - **High-Contrast Colors**: WCAG AA compliant dark colors for better visibility
   - **Visual Enhancements**: White borders, box-shadows, and text-shadows
   - **Consistent Event Display**: Timed and all-day events look identical
   - **Smooth Animations**: Hover effects with transform and shadow changes
   - **Bootstrap 5**: Modern UI framework integration

6. **Framework & Tooling Updates**
   - Updated to .NET 10
   - Migrated from Webpack to Vite for faster builds
   - Modern JavaScript ES6+ modules
   - Sass compilation with auto-prefixing

7. **Security Improvements**
   - Password policies
   - Anti-forgery tokens
   - Secure cookie configuration
   - Role-based access control

## Implemented Features

**Authentication & User Management:**
- ✅ User login and logout
- ✅ User registration (Admin only)
- ✅ Password change functionality
- ✅ User management interface (Admin only)
- ✅ Role-based access control (Admin/User roles)
- ✅ Default admin account created on first run

**Calendar Core Features:**
- ✅ Create single day events and timed events
- ✅ Create all day events
- ✅ Update existing events
- ✅ Delete events
- ✅ User-specific events (each user sees only their own events)

**Advanced Calendar Features:**
- ✅ **Drag & Drop Events** - Drag events to new dates/times directly on the calendar
- ✅ **Resize Events** - Adjust event duration by dragging the event edges
- ✅ **Event Types** - 7 distinct categories with custom colors and emoji icons:
  - 📅 Afspraak (Meeting) - Dark Blue
  - 🎂 Verjaardag (Birthday) - Dark Pink
  - 💐 Jubileum (Anniversary) - Dark Purple
  - ⏰ Herinnering (Reminder) - Dark Orange
  - ✓ Taak (Task) - Dark Green
  - 🎉 Vakantie (Holiday) - Dark Red
  - 📌 Overig (Other) - Dark Grey
- ✅ **Event Type Filtering** - Filter calendar view by event type using checkboxes in the sidebar
- ✅ **Recurring Events** - Create events that repeat daily, weekly, monthly, or yearly
- ✅ **Event Sharing** - Share events with other users (view-only or edit permissions)
- ✅ **iCal Export** - Export your calendar or individual events to iCal format (.ics files)

**UI/UX Enhancements:**
- ✅ **Responsive Sidebar Layout** - Fixed navigation sidebar with calendar content area
- ✅ **High-Contrast Event Colors** - WCAG AA compliant colors for accessibility
- ✅ **Full Event Display** - All events shown as colored bars, no "+X more" links
- ✅ **Visual Consistency** - Timed events and all-day events have identical styling
- ✅ **Professional Styling** - White borders, box-shadows, and smooth hover effects
- ✅ **Event Icons** - Emoji icons for quick event type identification

## Documentation

The project includes comprehensive documentation:

- **KALENDER-LAYOUT-FIX.md** - Calendar layout and sidebar configuration
- **EVENTTYPE-KLEUREN.md** - Event type colors technical documentation
- **KLEUREN-WIJZIGEN-HANDLEIDING.md** - Guide to customizing event colors
- **KLEUR-CONTRAST-UPDATE.md** - High-contrast color implementation details
- **VISUELE-STYLING-GIDS.md** - Visual styling reference guide
- **TIJDSTIP-EVENTS-FIX.md** - Timed events display improvements
- **ALLE-EVENTS-TONEN.md** - Full event display configuration
- **EVENTTYPE-DEBUG.md** - Event type troubleshooting guide (historical)

## Customization

### Changing Event Colors
Event colors can be easily customized in `Library/EventTypeConfig.cs`:

```csharp
public static string GetEventColor(EventType eventType)
{
    return eventType switch
    {
        EventType.Meeting => "#1565c0",      // Change this hex code
        EventType.Birthday => "#c2185b",     // for any event type
        // ... etc
    };
}
```

After changing colors, rebuild the application. See `KLEUREN-WIJZIGEN-HANDLEIDING.md` for detailed instructions and color palette suggestions.

### Adjusting Event Display Limit
To limit the number of events shown per day (currently unlimited), edit `Scripts/calendar.js`:

```javascript
dayMaxEvents: false,  // Change to a number (e.g., 10) to limit events shown
```

Then run `npm run build` to recompile.

### Modifying Event Icons
Event icons can be changed in `Library/EventTypeConfig.cs`:

```csharp
public static string GetEventIcon(EventType eventType)
{
    return eventType switch
    {
        EventType.Meeting => "📅",     // Change to any emoji
        EventType.Birthday => "🎂",    // or unicode character
        // ... etc
    };
}
```

## Tips & Best Practices

### Event Organization
- **Use Event Types**: Assign appropriate event types for better visual organization
- **Enable Filtering**: Use sidebar checkboxes to focus on specific event types
- **Recurring Events**: Use for birthdays, anniversaries, and regular meetings
- **Event Sharing**: Share important events with team members

### Calendar Navigation
- **Month View**: Best for overview and long-term planning
- **Week View**: Detailed view of week schedule with time slots
- **Day View**: Focus on single day with hourly breakdown
- **Drag & Drop**: Quickly reschedule by dragging events
- **Resize**: Adjust event duration by dragging event edges

### Performance
- Days with many events (10+) will automatically stack
- Use event filtering to reduce visual clutter
- Export to iCal for backup or use in external calendar apps

### Accessibility
- All colors meet WCAG AA standards for contrast
- Emoji icons provide additional visual cues beyond color
- Keyboard navigation supported in calendar views
- Screen reader friendly with proper ARIA labels

## Credits & Acknowledgments

**Original Repository**: [fullcalendar-aspnet-core](https://github.com/esausilva/fullcalendar-aspnet-core) by [Esau Silva](https://github.com/esausilva)

This fork extends the original implementation with:
- ASP.NET Core Identity authentication and authorization
- Multi-user support with event sharing
- Advanced calendar features (recurring events, event types, filtering)
- Enhanced UI/UX with high-contrast colors and responsive design
- SQLite database integration
- .NET 10 framework update

Special thanks to Esau Silva for the excellent foundation and FullCalendar implementation.

## Giving Back

If you would like to support the original author's work, consider getting Esau Silva a coffee:

[![Buy Me A Coffee](https://www.buymeacoffee.com/assets/img/custom_images/black_img.png)](https://www.buymeacoffee.com/esausilva)

## Preview

### Calendar View with Event Types
![Calendar with multiple event types showing different colors and icons](https://i.imgur.com/p6BjJ2Vm.jpg)

### Event Modal with Type Selection
![Event creation modal with event type dropdown](https://i.imgur.com/3378pXYm.jpg)

### Sidebar with Event Type Filters
![Sidebar navigation with event type filter checkboxes](https://i.imgur.com/nlDoTsQm.jpg)

**Features shown in screenshots:**
- 📅 Multiple event types with distinct colors (blue, pink, purple, orange, green, red, grey)
- 🎨 High-contrast design with white borders and shadows
- 📊 Full event bars (no dots) for both timed and all-day events
- 🔍 Event type filtering via sidebar checkboxes
- 📱 Responsive sidebar layout with calendar content area
- 🖱️ Drag & drop and resize functionality
- 📝 Comprehensive event modal with all options

---

## License

This project is based on [fullcalendar-aspnet-core](https://github.com/esausilva/fullcalendar-aspnet-core) by Esau Silva.

## Author

**Roland Wardenaar**
- GitHub: [@rolandwardenaar](https://github.com/rolandwardenaar)
- Repository: [fullcalendar-aspnet-core](https://github.com/rolandwardenaar/fullcalendar-aspnet-core)

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Version

**Current Version**: 2.0.0 (January 2025)
- .NET 10
- FullCalendar 6.1.15
- Bootstrap 5.3.3
- Vite 6.0.7

---

**Built with ❤️ by Roland Wardenaar, based on the excellent foundation by Esau Silva**
