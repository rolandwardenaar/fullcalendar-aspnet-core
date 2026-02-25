import { Calendar } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';
import nlLocale from '@fullcalendar/core/locales/nl';
import axios from 'axios';
import flatpickr from 'flatpickr';
import { Dutch } from 'flatpickr/dist/l10n/nl.js';
import 'flatpickr/dist/flatpickr.min.css';
import { Modal, Tooltip } from 'bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';

let currentEvent;
let calendar;
let eventModal; // Bootstrap 5 Modal instance
let shareModal; // Bootstrap modal for sharing

// Event Type Filter - All types active by default
let activeEventTypes = new Set([0, 1, 2, 3, 4, 5, 6]);

// Helper function to parse Dutch date format (dd-mm-yyyy HH:mm)
function parseDate(dateStr) {
    if (!dateStr) return null;

    // Try ISO format first (from FullCalendar)
    const isoDate = new Date(dateStr);
    if (!isNaN(isoDate.getTime())) {
        return isoDate;
    }

    // Parse Dutch format: "24-02-2026 14:30" or "24-02-2026, 14:30"
    const cleanStr = dateStr.replace(',', '').trim();
    const parts = cleanStr.split(' ');

    if (parts.length >= 2) {
        const dateParts = parts[0].split('-');
        const timeParts = parts[1].split(':');

        if (dateParts.length === 3 && timeParts.length === 2) {
            const day = parseInt(dateParts[0], 10);
            const month = parseInt(dateParts[1], 10) - 1; // Month is 0-indexed
            const year = parseInt(dateParts[2], 10);
            const hour = parseInt(timeParts[0], 10);
            const minute = parseInt(timeParts[1], 10);

            return new Date(year, month, day, hour, minute);
        }
    }

    return null;
}

const formatDate = date => {
    if (!date) return '';
    const d = new Date(date);
    return d.toLocaleString('nl-NL', { 
        year: 'numeric', 
        month: '2-digit', 
        day: '2-digit', 
        hour: '2-digit', 
        minute: '2-digit'
    });
};

const fpStartTime = flatpickr("#StartTime", {
    enableTime: true,
    dateFormat: "d-m-Y H:i",
    time_24hr: true,
    locale: Dutch
});

const fpEndTime = flatpickr("#EndTime", {
    enableTime: true,
    dateFormat: "d-m-Y H:i",
    time_24hr: true,
    locale: Dutch
});

const fpRecurrenceEndDate = flatpickr("#RecurrenceEndDate", {
    enableTime: true,
    dateFormat: "d-m-Y H:i",
    time_24hr: true,
    locale: Dutch
});

// Initialize calendar when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    const calendarEl = document.getElementById('calendar');

    // Initialize Bootstrap modals
    const eventModalEl = document.getElementById('eventModal');
    eventModal = new Modal(eventModalEl);

    const shareModalEl = document.getElementById('shareModal');
    shareModal = new Modal(shareModalEl);

    calendar = new Calendar(calendarEl, {
        plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
        locale: nlLocale,
        initialView: 'dayGridMonth',
        height: 'parent',
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay'
        },
        buttonText: {
            today: 'Vandaag',
            month: 'Maand',
            week: 'Week',
            dag: 'Dag'
        },
        editable: true, // Global editable, but controlled per event
        eventStartEditable: true,
        eventDurationEditable: true,
        selectable: true,
        selectMirror: true,
        dayMaxEvents: false, // Show all events, no "+X more" link
        events: async function(fetchInfo, successCallback, failureCallback) {
            try {
                const response = await axios.get('/Home/GetCalendarEvents', {
                    params: {
                        start: fetchInfo.startStr,
                        end: fetchInfo.endStr
                    }
                });

                // Filter events based on active event types
                const filteredEvents = response.data.filter(event => {
                    const eventType = event.extendedProps?.eventType ?? 0;
                    return activeEventTypes.has(eventType);
                });

                successCallback(filteredEvents);
            } catch (error) {
                console.error('Error fetching events:', error);
                failureCallback(error);
            }
        },
        // Control editability per event
        eventAllow: function(dropInfo, draggedEvent) {
            // Only allow dragging/dropping own events
            return draggedEvent.extendedProps.isOwn === true;
        },
        eventDidMount: function(info) {
            // Add Bootstrap 5 tooltip to event
            const eventEl = info.el;
            const description = info.event.extendedProps.description || 'Geen beschrijving';
            const userName = info.event.extendedProps.userName || 'Onbekend';
            const isOwn = info.event.extendedProps.isOwn || false;
            const eventTypeName = info.event.extendedProps.eventTypeName || 'Afspraak';
            const eventIcon = info.event.extendedProps.eventIcon || '📅';
            const isShared = info.event.extendedProps.isShared || false;

            // Add visual indicator for other users' events
            if (!isOwn) {
                eventEl.style.opacity = '0.7';
                eventEl.style.cursor = 'pointer';
                eventEl.classList.add('other-user-event');
            }

            // Add shared indicator
            if (isShared) {
                eventEl.classList.add('shared-event');
                eventEl.style.borderLeft = '4px solid gold';
            }

            // Prepend icon to event title
            const titleEl = eventEl.querySelector('.fc-event-title');
            if (titleEl) {
                titleEl.innerHTML = `${eventIcon} ${info.event.title}`;
            }

            // Set tooltip attributes
            eventEl.setAttribute('data-bs-toggle', 'tooltip');
            eventEl.setAttribute('data-bs-placement', 'top');
            eventEl.setAttribute('data-bs-html', 'true');
            eventEl.setAttribute('data-bs-title', `<strong>${info.event.title}</strong><br>${description}<br><em>Type: ${eventTypeName}</em><br><em>Door: ${userName}</em>${isShared ? '<br><span class="badge bg-warning">Gedeeld</span>' : ''}`);

            // Initialize Bootstrap tooltip
            new Tooltip(eventEl);
        },
        eventClick: function(info) {
            updateEvent(info.event);
        },
        select: function(info) {
            addEvent(info.start, info.end, info.allDay);
        },
        // Handle drag and drop
        eventDrop: function(info) {
            handleEventMove(info);
        },
        // Handle resize
        eventResize: function(info) {
            handleEventResize(info);
        }
    });

    calendar.render();
});

/**
 * Calendar Methods
 **/

function updateEvent(event) {
    currentEvent = event;

    document.getElementById('eventModalLabel').textContent = 'Evenement bewerken';
    document.getElementById('eventModalSave').textContent = 'Bijwerken';
    document.getElementById('EventTitle').value = event.title;
    document.getElementById('Description').value = event.extendedProps.description || '';
    document.getElementById('isNewEvent').value = false;
    document.getElementById('EventType').value = event.extendedProps.eventType || 0;

    // Show owner/creator name
    const userName = event.extendedProps.userName || 'Onbekend';
    const isOwnEvent = event.extendedProps.isOwn || false;
    document.getElementById('UserName').value = userName;
    document.getElementById('isOwnEvent').value = isOwnEvent;

    // Recurrence fields
    const isRecurring = event.extendedProps.isRecurring || false;
    document.getElementById('IsRecurring').checked = isRecurring;
    if (isRecurring) {
        document.getElementById('recurrenceOptions').style.display = 'block';
        document.getElementById('RecurrencePattern').value = event.extendedProps.recurrencePattern || 0;
        document.getElementById('RecurrenceInterval').value = event.extendedProps.recurrenceInterval || 1;
    } else {
        document.getElementById('recurrenceOptions').style.display = 'none';
    }

    // Disable form fields and buttons if not own event
    const deleteBtn = document.getElementById('deleteEvent');
    const saveBtn = document.getElementById('eventModalSave');
    const shareBtn = document.getElementById('shareEvent');
    const exportBtn = document.getElementById('exportEvent');
    const titleInput = document.getElementById('EventTitle');
    const descInput = document.getElementById('Description');
    const startInput = document.getElementById('StartTime');
    const endInput = document.getElementById('EndTime');
    const allDayCheckbox = document.getElementById('AllDay');
    const eventTypeSelect = document.getElementById('EventType');
    const isRecurringCheckbox = document.getElementById('IsRecurring');

    if (!isOwnEvent) {
        // Read-only mode for other users' events
        titleInput.readOnly = true;
        descInput.readOnly = true;
        startInput.disabled = true;
        endInput.disabled = true;
        allDayCheckbox.disabled = true;
        eventTypeSelect.disabled = true;
        isRecurringCheckbox.disabled = true;
        deleteBtn.style.display = 'none';
        saveBtn.style.display = 'none';
        shareBtn.style.display = 'none';
        exportBtn.style.display = 'none';

        titleInput.classList.add('bg-light');
        descInput.classList.add('bg-light');
        startInput.classList.add('bg-light');
        endInput.classList.add('bg-light');
    } else {
        // Editable mode for own events
        titleInput.readOnly = false;
        descInput.readOnly = false;
        startInput.disabled = false;
        allDayCheckbox.disabled = false;
        eventTypeSelect.disabled = false;
        isRecurringCheckbox.disabled = false;
        deleteBtn.style.display = 'inline-block';
        saveBtn.style.display = 'inline-block';
        shareBtn.style.display = 'inline-block';
        exportBtn.style.display = 'inline-block';

        titleInput.classList.remove('bg-light');
        descInput.classList.remove('bg-light');
        startInput.classList.remove('bg-light');
    }

    const start = formatDate(event.start);
    const end = formatDate(event.end);

    fpStartTime.setDate(event.start);
    fpEndTime.setDate(event.end);

    document.getElementById('StartTime').value = start;
    document.getElementById('EndTime').value = end;

    allDayCheckbox.checked = event.allDay;

    // Disable end time if it's an all-day event or not own event
    if (event.allDay || !isOwnEvent) {
        endInput.disabled = true;
        endInput.classList.add('bg-light');
    } else {
        endInput.disabled = false;
        endInput.classList.remove('bg-light');
    }

    eventModal.show();
}

function addEvent(start, end, allDay) {
    document.getElementById('eventForm').reset();

    document.getElementById('eventModalLabel').textContent = 'Nieuw evenement';
    document.getElementById('eventModalSave').textContent = 'Aanmaken';
    document.getElementById('isNewEvent').value = true;
    document.getElementById('isOwnEvent').value = true;
    document.getElementById('UserName').value = ''; // Will be set by server
    document.getElementById('EventType').value = 0; // Default to Meeting
    document.getElementById('IsRecurring').checked = false;
    document.getElementById('recurrenceOptions').style.display = 'none';

    // Enable all form fields for new event
    const deleteBtn = document.getElementById('deleteEvent');
    const saveBtn = document.getElementById('eventModalSave');
    const shareBtn = document.getElementById('shareEvent');
    const exportBtn = document.getElementById('exportEvent');
    const titleInput = document.getElementById('EventTitle');
    const descInput = document.getElementById('Description');
    const startInput = document.getElementById('StartTime');
    const endInput = document.getElementById('EndTime');
    const allDayCheckbox = document.getElementById('AllDay');
    const eventTypeSelect = document.getElementById('EventType');
    const isRecurringCheckbox = document.getElementById('IsRecurring');

    titleInput.readOnly = false;
    descInput.readOnly = false;
    startInput.disabled = false;
    allDayCheckbox.disabled = false;
    eventTypeSelect.disabled = false;
    isRecurringCheckbox.disabled = false;
    deleteBtn.style.display = 'none'; // No delete for new events
    saveBtn.style.display = 'inline-block';
    shareBtn.style.display = 'none'; // Can't share new events yet
    exportBtn.style.display = 'none'; // Can't export new events yet

    titleInput.classList.remove('bg-light');
    descInput.classList.remove('bg-light');
    startInput.classList.remove('bg-light');

    const startFormatted = formatDate(start);
    const endFormatted = formatDate(end);

    fpStartTime.setDate(start);
    fpEndTime.setDate(end);

    document.getElementById('StartTime').value = startFormatted;
    document.getElementById('EndTime').value = endFormatted;

    // Ensure EndTime is enabled for new events
    endInput.disabled = false;
    endInput.classList.remove('bg-light');

    eventModal.show();
}

/**
 * Modal
 * */

document.getElementById('eventModalSave').addEventListener('click', () => {
    const title = document.getElementById('EventTitle').value;
    const description = document.getElementById('Description').value;
    const startTime = document.getElementById('StartTime').value;
    const endTime = document.getElementById('EndTime').value;
    const isAllDay = document.getElementById('AllDay').checked;
    const isNewEvent = document.getElementById('isNewEvent').value === 'true';
    const eventType = parseInt(document.getElementById('EventType').value);
    const isRecurring = document.getElementById('IsRecurring').checked;
    const recurrencePattern = parseInt(document.getElementById('RecurrencePattern').value);
    const recurrenceInterval = parseInt(document.getElementById('RecurrenceInterval').value);
    const recurrenceEndDate = document.getElementById('RecurrenceEndDate').value;

    if (!startTime) {
        alert('Vul een starttijd in');
        return;
    }

    if (!isAllDay && !endTime) {
        alert('Vul een eindtijd in of selecteer "Hele dag"');
        return;
    }

    const event = {
        title,
        description,
        isAllDay,
        startTime,
        endTime: isAllDay ? '' : endTime,
        eventType,
        isRecurring,
        recurrencePattern: isRecurring ? recurrencePattern : 0,
        recurrenceInterval: isRecurring ? recurrenceInterval : 1,
        recurrenceEndDate: isRecurring ? recurrenceEndDate : ''
    };

    if (isNewEvent) {
        sendAddEvent(event);
    } else {
        sendUpdateEvent(event);
    }
});

function sendAddEvent(event) {
    axios.post('/Home/AddEvent', {
        Title: event.title,
        Description: event.description,
        Start: event.startTime,
        End: event.endTime,
        AllDay: event.isAllDay,
        EventType: event.eventType,
        IsRecurring: event.isRecurring,
        RecurrencePattern: event.recurrencePattern,
        RecurrenceInterval: event.recurrenceInterval,
        RecurrenceEndDate: event.recurrenceEndDate
    })
    .then(res => {
        // Refresh calendar to show new event(s)
        calendar.refetchEvents();
        eventModal.hide();
    })
    .catch(err => {
        console.error('Add error:', err);
        const errorMessage = err.response?.data?.message || err.message || 'Er ging iets mis bij het toevoegen';
        alert(errorMessage);
    });
}

function sendUpdateEvent(event) {
    axios.post('/Home/UpdateEvent', {
        EventId: currentEvent.id,
        Title: event.title,
        Description: event.description,
        Start: event.startTime,
        End: event.endTime,
        AllDay: event.isAllDay,
        EventType: event.eventType,
        IsRecurring: event.isRecurring,
        RecurrencePattern: event.recurrencePattern,
        RecurrenceInterval: event.recurrenceInterval,
        RecurrenceEndDate: event.recurrenceEndDate
    })
    .then(res => {
        // Refresh calendar to show updated event(s)
        calendar.refetchEvents();
        eventModal.hide();
    })
    .catch(err => {
        console.error('Update error:', err);
        const errorMessage = err.response?.data?.message || err.message || 'Er ging iets mis bij het bijwerken';
        alert(errorMessage);
    });
}

document.getElementById('deleteEvent').addEventListener('click', () => {
    if (confirm(`Weet je zeker dat je "${currentEvent.title}" wilt verwijderen?`)) {
        axios.post('/Home/DeleteEvent', {
            EventId: currentEvent.id
        })
        .then(res => {
            currentEvent.remove();
            eventModal.hide();
        })
        .catch(err => {
            console.error('Delete error:', err);
            const errorMessage = err.response?.data?.message || err.message || 'Er ging iets mis bij het verwijderen';
            alert(errorMessage);
        });
    }
});

document.getElementById('AllDay').addEventListener('change', function (e) {
    const endTimeInput = document.getElementById('EndTime');

    if (e.target.checked) {
        // Als "Hele dag" is aangevinkt, verberg/disable eindtijd
        endTimeInput.value = '';
        endTimeInput.disabled = true;
        endTimeInput.classList.add('bg-light');
        fpEndTime.clear();
    } else {
        // Als "Hele dag" is uitgev inkt, enable eindtijd
        endTimeInput.disabled = false;
        endTimeInput.classList.remove('bg-light');
    }
});

// Recurrence checkbox handler
document.getElementById('IsRecurring').addEventListener('change', function (e) {
    const recurrenceOptions = document.getElementById('recurrenceOptions');
    if (e.target.checked) {
        recurrenceOptions.style.display = 'block';
    } else {
        recurrenceOptions.style.display = 'none';
    }
});

// Export calendar button
document.getElementById('exportCalendar').addEventListener('click', function() {
    // Get current calendar view dates
    const view = calendar.view;
    const start = view.activeStart.toISOString();
    const end = view.activeEnd.toISOString();

    window.location.href = `/Home/ExportICalendar?start=${encodeURIComponent(start)}&end=${encodeURIComponent(end)}`;
});

// Export single event button
document.getElementById('exportEvent').addEventListener('click', function() {
    if (currentEvent) {
        window.location.href = `/Home/ExportEventICalendar?eventId=${currentEvent.id}`;
    }
});

// Share event button
document.getElementById('shareEvent').addEventListener('click', async function() {
    if (!currentEvent) return;

    // Load users list
    try {
        const response = await axios.get('/Home/GetAllUsers');
        const users = response.data;

        const selectEl = document.getElementById('shareWithUser');
        selectEl.innerHTML = '<option value="">Selecteer een gebruiker...</option>';
        users.forEach(user => {
            const option = document.createElement('option');
            option.value = user.id;
            option.textContent = `${user.name} (${user.email})`;
            selectEl.appendChild(option);
        });

        // Load existing shares
        loadEventShares(currentEvent.id);

        shareModal.show();
    } catch (err) {
        console.error('Error loading users:', err);
        alert('Fout bij laden van gebruikers');
    }
});

// Share modal save button
document.getElementById('shareModalSave').addEventListener('click', async function() {
    const sharedWithUserId = document.getElementById('shareWithUser').value;
    const canEdit = document.getElementById('shareCanEdit').checked;

    if (!sharedWithUserId) {
        alert('Selecteer een gebruiker');
        return;
    }

    try {
        await axios.post('/Home/ShareEvent', {
            EventId: currentEvent.id,
            SharedWithUserId: sharedWithUserId,
            CanEdit: canEdit
        });

        // Reload shares list
        loadEventShares(currentEvent.id);

        // Clear selection
        document.getElementById('shareWithUser').value = '';
        document.getElementById('shareCanEdit').checked = false;
    } catch (err) {
        console.error('Error sharing event:', err);
        const errorMessage = err.response?.data?.message || 'Fout bij delen van evenement';
        alert(errorMessage);
    }
});

async function loadEventShares(eventId) {
    try {
        const response = await axios.get(`/Home/GetEventShares?eventId=${eventId}`);
        const shares = response.data;

        const listEl = document.getElementById('sharedUsersList');

        if (shares.length === 0) {
            listEl.innerHTML = '<p class="text-muted">Nog niet gedeeld</p>';
        } else {
            listEl.innerHTML = '';
            shares.forEach(share => {
                const div = document.createElement('div');
                div.className = 'd-flex justify-content-between align-items-center mb-2';
                div.innerHTML = `
                    <span>${share.sharedWithUserName} ${share.canEdit ? '<span class="badge bg-warning">Kan bewerken</span>' : ''}</span>
                    <button class="btn btn-sm btn-danger" onclick="unshareEvent(${share.eventId}, '${share.sharedWithUserId}')">Intrekken</button>
                `;
                listEl.appendChild(div);
            });
        }
    } catch (err) {
        console.error('Error loading shares:', err);
    }
}

async function unshareEvent(eventId, sharedWithUserId) {
    try {
        await axios.post('/Home/UnshareEvent', {
            EventId: eventId,
            SharedWithUserId: sharedWithUserId
        });

        loadEventShares(eventId);
    } catch (err) {
        console.error('Error unsharing event:', err);
        alert('Fout bij intrekken van gedeeld evenement');
    }
}

// Make unshareEvent globally available
window.unshareEvent = unshareEvent;

/**
 * Event Type Filter Functionality
 */
// Initialize filter checkboxes
const initializeEventTypeFilters = () => {
    const filterCheckboxes = document.querySelectorAll('[data-event-type-filter]');

    filterCheckboxes.forEach(checkbox => {
        checkbox.addEventListener('change', function() {
            const eventType = parseInt(this.value);

            if (this.checked) {
                activeEventTypes.add(eventType);
            } else {
                activeEventTypes.delete(eventType);
            }

            // Refresh calendar to apply filter
            if (calendar) {
                calendar.refetchEvents();
            }
        });
    });

    // Select all button
    const selectAllBtn = document.getElementById('selectAllTypes');
    if (selectAllBtn) {
        selectAllBtn.addEventListener('click', function() {
            filterCheckboxes.forEach(checkbox => {
                checkbox.checked = true;
                activeEventTypes.add(parseInt(checkbox.value));
            });
            if (calendar) {
                calendar.refetchEvents();
            }
        });
    }

    // Deselect all button
    const deselectAllBtn = document.getElementById('deselectAllTypes');
    if (deselectAllBtn) {
        deselectAllBtn.addEventListener('click', function() {
            filterCheckboxes.forEach(checkbox => {
                checkbox.checked = false;
                activeEventTypes.delete(parseInt(checkbox.value));
            });
            if (calendar) {
                calendar.refetchEvents();
            }
        });
    }
};

// Call initialization after DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeEventTypeFilters);
} else {
    initializeEventTypeFilters();
}

// Verwijderd: de automatische uncheck van AllDay bij EndTime change
// Dit was verwarrend gedrag

/**
 * Handle Event Drag and Drop
 */
function handleEventMove(info) {
    const event = info.event;

    // Check if user can edit this event
    if (!event.extendedProps.isOwn) {
        alert('Je mag alleen je eigen events verplaatsen.');
        info.revert();
        return;
    }

    // Send update to server
    axios.post('/Home/MoveEvent', {
        EventId: event.id,
        Start: event.start.toISOString(),
        End: event.end ? event.end.toISOString() : null,
        AllDay: event.allDay
    })
    .then(res => {
        // Success - event already moved by FullCalendar
        console.log('Event moved successfully');
    })
    .catch(err => {
        console.error('Move error:', err);
        const errorMessage = err.response?.data?.message || err.message || 'Er ging iets mis bij het verplaatsen';
        alert(errorMessage);
        // Revert the change
        info.revert();
    });
}

/**
 * Handle Event Resize
 */
function handleEventResize(info) {
    const event = info.event;

    // Check if user can edit this event
    if (!event.extendedProps.isOwn) {
        alert('Je mag alleen je eigen events wijzigen.');
        info.revert();
        return;
    }

    // Send update to server
    axios.post('/Home/ResizeEvent', {
        EventId: event.id,
        Start: event.start.toISOString(),
        End: event.end ? event.end.toISOString() : null,
        AllDay: event.allDay
    })
    .then(res => {
        // Success - event already resized by FullCalendar
        console.log('Event resized successfully');
    })
    .catch(err => {
        console.error('Resize error:', err);
        const errorMessage = err.response?.data?.message || err.message || 'Er ging iets mis bij het aanpassen van de grootte';
        alert(errorMessage);
        // Revert the change
        info.revert();
    });
}
