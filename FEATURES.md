# New Features Implementation Guide

This document describes all the newly implemented features in the FullCalendar .NET application.

## Table of Contents
1. [Event Types](#event-types)
2. [Event Type Filtering](#event-type-filtering)
3. [Drag & Drop](#drag--drop)
4. [Event Resizing](#event-resizing)
5. [Recurring Events](#recurring-events)
6. [Event Sharing](#event-sharing)
7. [iCal Export](#ical-export)

---

## Event Types

### Overview
Events can now be categorized into different types, each with its own color and icon for easy visual identification.

### Available Event Types
| Type        | Icon | Color      | Use Case                |
|-------------|------|------------|-------------------------|
| Meeting     | 📅   | Blue       | Default, work meetings  |
| Birthday    | 🎂   | Pink       | Birthday celebrations   |
| Anniversary | 💐   | Purple     | Anniversaries, special dates |
| Reminder    | ⏰   | Orange     | Reminders, alerts       |
| Task        | ✓    | Green      | To-do items, tasks      |
| Holiday     | 🎉   | Red        | Holidays, vacations     |
| Other       | 📌   | Blue Grey  | Miscellaneous events    |

### How to Use
1. When creating or editing an event, select the type from the "Type" dropdown
2. The calendar will automatically apply the corresponding color and icon
3. Icons appear in the calendar view and in tooltips

### Technical Details
- **Backend**: `EventType` enum in `Library/EventType.cs`
- **Configuration**: `EventTypeConfig` class provides color/icon mappings
- **Database**: `event_type` column (INTEGER) in Events table

---

## Event Type Filtering

### Overview
Filter which event types are displayed in the calendar using checkboxes in the sidebar menu.

### Features
- **Individual Type Selection**: Check/uncheck any event type to show/hide it
- **Quick Actions**: 
  - "Alles selecteren" - Show all event types
  - "Alles deselecteren" - Hide all event types
- **Real-time Updates**: Calendar refreshes immediately when filters change
- **Visual Indicators**: Each checkbox shows the event type's color and icon
- **Persistent During Session**: Filter selections remain active while navigating the calendar

### How to Use

#### Filter Specific Types
1. Look at the sidebar menu under "Eventtypes filteren"
2. Check the boxes for event types you want to see
3. Uncheck boxes for types you want to hide
4. The calendar updates automatically

#### Quick Select/Deselect All
- Click "Alles selecteren" to show all event types
- Click "Alles deselecteren" to hide all event types

### Use Cases
- **Focus on Work**: Only show Meeting and Task types
- **Personal Events**: Show only Birthday, Anniversary, and Holiday types
- **Declutter View**: Hide types you don't use frequently
- **Quick Overview**: Toggle between different event type groups

### Technical Details
- **Frontend**: Filter checkboxes in `_Layout.cshtml`
- **JavaScript**: Filter logic in `calendar.js` using `activeEventTypes` Set
- **Calendar Integration**: Custom `events` function filters data before rendering
- **State Management**: `activeEventTypes` Set stores currently visible types

---

## Drag & Drop

### Overview
Events can be moved to different dates/times by dragging them on the calendar.

### How to Use
1. Click and hold on an event
2. Drag it to a new date/time
3. Release to save the new position
4. The system automatically updates the event in the database

### Restrictions
- Only your own events can be dragged
- Other users' events are read-only
- If an error occurs, the event reverts to its original position

### Technical Details
- **Frontend**: `eventDrop` handler in `calendar.js`
- **Backend**: `MoveEvent` endpoint in `HomeController.cs`
- **Database**: Updates `event_start`, `event_end`, and `all_day` fields

---

## Event Resizing

### Overview
Event duration can be adjusted by dragging the event's start or end edge.

### How to Use
1. Hover over the bottom edge of an event
2. Click and drag to extend or shorten the duration
3. Release to save the new duration
4. The system automatically updates the event in the database

### Restrictions
- Only your own events can be resized
- Other users' events are read-only
- If an error occurs, the event reverts to its original size

### Technical Details
- **Frontend**: `eventResize` handler in `calendar.js`
- **Backend**: `ResizeEvent` endpoint in `HomeController.cs`
- **Database**: Updates `event_start` and `event_end` fields

---

## Recurring Events

### Overview
Events can repeat at regular intervals (daily, weekly, monthly, or yearly).

### Recurrence Patterns
- **Daily**: Repeats every N days
- **Weekly**: Repeats every N weeks
- **Monthly**: Repeats every N months
- **Yearly**: Repeats every N years

### How to Use
1. When creating/editing an event, check "Herhalend evenement"
2. Select the recurrence pattern (Daily, Weekly, Monthly, Yearly)
3. Set the interval (e.g., "2" for every 2 weeks)
4. Optionally set an end date for the recurrence
5. Save the event

### Features
- Recurring events show all instances within the visible calendar range
- Each instance is generated on-the-fly based on the recurrence pattern
- Editing the master event updates all future instances
- Export to iCal includes RRULE for compatibility with other calendar apps

### Technical Details
- **Backend Service**: `RecurrenceService.cs` generates instances
- **Database Fields**:
  - `is_recurring` (BOOLEAN)
  - `recurrence_pattern` (INTEGER: 0=None, 1=Daily, 2=Weekly, 3=Monthly, 4=Yearly)
  - `recurrence_interval` (INTEGER: how many units between occurrences)
  - `recurrence_end_date` (TEXT: optional end date)
- **Frontend**: Recurrence options panel in event modal

---

## Event Sharing

### Overview
Share your events with other users, giving them view-only or edit permissions.

### How to Use

#### Sharing an Event
1. Open an event you own
2. Click the "Delen" (Share) button
3. Select a user from the dropdown
4. Optionally check "Kan bewerken" to allow editing
5. Click "Delen" to share

#### Viewing Shared Events
- Shared events appear in your calendar with a gold border
- A 🔗 icon appears on shared events
- Hover over the event to see sharing details in the tooltip

#### Unsharing an Event
1. Open the event
2. Click "Delen"
3. In the "Gedeeld met" list, click "Intrekken" next to the user
4. The event is immediately unshared

### Restrictions
- Only event owners can share/unshare events
- You can only share with users in the system
- Shared users see the event in read-only mode (unless given edit permission)

### Technical Details
- **Database Table**: `EventShares` stores sharing relationships
- **Endpoints**:
  - `ShareEvent`: Create a share
  - `UnshareEvent`: Remove a share
  - `GetEventShares`: List users an event is shared with
  - `GetAllUsers`: Get list of users to share with
- **Frontend**: Share modal with user selection

---

## iCal Export

### Overview
Export your calendar or individual events to iCal format (.ics files) for use in other calendar applications.

### How to Use

#### Export Entire Calendar
1. Click the "Exporteer naar iCal" button at the top of the calendar
2. A file named `agenda_YYYYMMDD.ics` will download
3. Import this file into any iCal-compatible application (Apple Calendar, Google Calendar, Outlook, etc.)

#### Export Single Event
1. Open an event you own
2. Click the "Exporteer" button
3. A file named `event_X.ics` will download (where X is the event ID)
4. Import this file into any calendar application

### Features
- **RFC 5545 Compliant**: Standard iCal format
- **Timezone Support**: Uses Europe/Amsterdam timezone
- **Recurring Events**: Exports with RRULE for proper recurrence
- **Event Types**: Included as CATEGORIES
- **All-Day Events**: Properly formatted as DATE values
- **Timed Events**: Formatted as DATETIME with timezone

### Technical Details
- **Service**: `ICalService.cs` generates RFC 5545 compliant iCal
- **Endpoints**:
  - `ExportICalendar`: Export calendar for date range
  - `ExportEventICalendar`: Export single event
- **Content Type**: `text/calendar`
- **File Extension**: `.ics`

---

## Database Schema Changes

### New Fields in Events Table
```sql
event_type          INTEGER DEFAULT 0    -- Event type (0-6)
is_recurring        INTEGER DEFAULT 0    -- Is event recurring (0/1)
recurrence_pattern  INTEGER DEFAULT 0    -- Recurrence pattern (0-4)
recurrence_interval INTEGER DEFAULT 1    -- Interval between occurrences
recurrence_end_date TEXT                 -- Optional end date for recurrence
parent_event_id     INTEGER              -- Link to parent event for instances
```

### New EventShares Table
```sql
CREATE TABLE EventShares (
    share_id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_id INTEGER NOT NULL,
    owner_user_id TEXT NOT NULL,
    shared_with_user_id TEXT NOT NULL,
    shared_with_user_name TEXT,
    can_edit INTEGER NOT NULL DEFAULT 0,
    shared_date TEXT NOT NULL,
    FOREIGN KEY (event_id) REFERENCES Events(event_id) ON DELETE CASCADE
)
```

---

## API Endpoints Summary

### New Endpoints
| Endpoint | Method | Description |
|----------|--------|-------------|
| `/Home/MoveEvent` | POST | Move event to new date/time |
| `/Home/ResizeEvent` | POST | Resize event duration |
| `/Home/ShareEvent` | POST | Share event with user |
| `/Home/UnshareEvent` | POST | Unshare event |
| `/Home/GetEventShares` | GET | Get list of users event is shared with |
| `/Home/GetAllUsers` | GET | Get list of users for sharing |
| `/Home/ExportICalendar` | GET | Export calendar to iCal |
| `/Home/ExportEventICalendar` | GET | Export single event to iCal |

### Updated Endpoints
| Endpoint | Changes |
|----------|---------|
| `/Home/GetCalendarEvents` | Now includes event type, recurrence, and sharing info |
| `/Home/AddEvent` | Now accepts event type and recurrence parameters |
| `/Home/UpdateEvent` | Now accepts event type and recurrence parameters |

---

## Testing Checklist

### Event Types
- [ ] Create event with each type
- [ ] Verify correct color displays
- [ ] Verify correct icon displays
- [ ] Update event type and verify changes

### Event Type Filtering
- [ ] Check/uncheck individual event types
- [ ] Verify calendar updates immediately
- [ ] Use "Alles selecteren" button
- [ ] Use "Alles deselecteren" button
- [ ] Create event while filter is active
- [ ] Verify filtered events still save correctly
- [ ] Navigate between calendar views with filters active

### Drag & Drop
- [ ] Drag own event to new date
- [ ] Verify database update
- [ ] Try to drag other user's event (should fail)
- [ ] Drag event with error and verify revert

### Resize
- [ ] Resize own event
- [ ] Verify database update
- [ ] Try to resize other user's event (should fail)
- [ ] Resize with error and verify revert

### Recurring Events
- [ ] Create daily recurring event
- [ ] Create weekly recurring event
- [ ] Create monthly recurring event
- [ ] Create yearly recurring event
- [ ] Set end date for recurrence
- [ ] Verify instances appear in calendar
- [ ] Edit recurring event and verify all instances update

### Event Sharing
- [ ] Share event with another user
- [ ] Verify shared user can see event
- [ ] Share with edit permission
- [ ] Verify shared user can edit (if permission granted)
- [ ] Unshare event
- [ ] Verify shared user can no longer see event

### iCal Export
- [ ] Export calendar
- [ ] Import into external calendar app
- [ ] Export single event
- [ ] Import single event into external app
- [ ] Verify recurring events import correctly
- [ ] Verify event types appear as categories

---

## Known Limitations

1. **Recurring Event Editing**: Currently, editing a recurring event updates the master event. Individual instance editing is not yet supported.

2. **Sharing Permissions**: Edit permissions for shared events are stored but not fully enforced in the UI.

3. **Export Date Range**: Calendar export uses the currently visible date range in the calendar view.

4. **Timezone**: All events use Europe/Amsterdam timezone. Multi-timezone support is not yet implemented.

---

## Future Enhancements

Possible future improvements:
- Edit single instances of recurring events
- Exception dates for recurring events
- More recurrence patterns (e.g., "every last Friday of the month")
- Email notifications for shared events
- Event comments/notes
- Attachment support
- Calendar subscriptions (read-only iCal URL)
- Multi-timezone support

---

## Support

For issues or questions about these features, please refer to:
- Project README.md
- GitHub Issues: https://github.com/rolandwardenaar/fullcalendar-aspnet-core/issues
- Original repository: https://github.com/esausilva/fullcalendar-aspnet-core

---

**Version**: 1.0  
**Last Updated**: 2025  
**Author**: Roland Wardenaar
