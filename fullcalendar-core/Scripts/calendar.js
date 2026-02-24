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

// Initialize calendar when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    const calendarEl = document.getElementById('calendar');

    // Initialize Bootstrap modal
    const eventModalEl = document.getElementById('eventModal');
    eventModal = new Modal(eventModalEl);

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
            day: 'Dag'
        },
        editable: true,
        selectable: true,
        selectMirror: true,
        dayMaxEvents: true,
        events: '/Home/GetCalendarEvents',
        eventDidMount: function(info) {
            // Add Bootstrap 5 tooltip to event
            const eventEl = info.el;
            const description = info.event.extendedProps.description || 'Geen beschrijving';

            // Set tooltip attributes
            eventEl.setAttribute('data-bs-toggle', 'tooltip');
            eventEl.setAttribute('data-bs-placement', 'top');
            eventEl.setAttribute('data-bs-html', 'true');
            eventEl.setAttribute('data-bs-title', `<strong>${info.event.title}</strong><br>${description}`);

            // Initialize Bootstrap tooltip
            new Tooltip(eventEl);
        },
        eventClick: function(info) {
            updateEvent(info.event);
        },
        select: function(info) {
            addEvent(info.start, info.end, info.allDay);
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

    const start = formatDate(event.start);
    const end = formatDate(event.end);

    fpStartTime.setDate(event.start);
    fpEndTime.setDate(event.end);

    document.getElementById('StartTime').value = start;
    document.getElementById('EndTime').value = end;

    const allDayCheckbox = document.getElementById('AllDay');
    const endTimeInput = document.getElementById('EndTime');

    allDayCheckbox.checked = event.allDay;

    // Disable end time if it's an all-day event
    if (event.allDay) {
        endTimeInput.disabled = true;
        endTimeInput.classList.add('bg-light');
    } else {
        endTimeInput.disabled = false;
        endTimeInput.classList.remove('bg-light');
    }

    eventModal.show();
}

function addEvent(start, end, allDay) {
    document.getElementById('eventForm').reset();

    document.getElementById('eventModalLabel').textContent = 'Nieuw evenement';
    document.getElementById('eventModalSave').textContent = 'Aanmaken';
    document.getElementById('isNewEvent').value = true;

    const startFormatted = formatDate(start);
    const endFormatted = formatDate(end);

    fpStartTime.setDate(start);
    fpEndTime.setDate(end);

    document.getElementById('StartTime').value = startFormatted;
    document.getElementById('EndTime').value = endFormatted;

    // Ensure EndTime is enabled for new events
    const endTimeInput = document.getElementById('EndTime');
    endTimeInput.disabled = false;
    endTimeInput.classList.remove('bg-light');

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
        endTime: isAllDay ? '' : endTime
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
        AllDay: event.isAllDay
    })
    .then(res => {
        const { message, eventId } = res.data;

        if (message === '') {
            // Parse the dates using our custom parser for Dutch format
            const startDate = parseDate(event.startTime);
            const endDate = event.endTime ? parseDate(event.endTime) : null;

            if (!startDate) {
                alert('Ongeldige starttijd');
                return;
            }

            const newEvent = {
                id: eventId,
                title: event.title,
                start: startDate.toISOString(),  // ✅ ISO format for FullCalendar
                end: endDate ? endDate.toISOString() : null,
                allDay: event.isAllDay,
                extendedProps: {
                    description: event.description,
                    eventId: eventId
                }
            };

            calendar.addEvent(newEvent);
            calendar.unselect();

            eventModal.hide();
        } else {
            alert(`Er ging iets mis: ${message}`);
        }
    })
    .catch(err => alert(`Er ging iets mis: ${err}`));
}

function sendUpdateEvent(event) {
    axios.post('/Home/UpdateEvent', {
        EventId: currentEvent.id,
        Title: event.title,
        Description: event.description,
        Start: event.startTime,
        End: event.endTime,
        AllDay: event.isAllDay
    })
    .then(res => {
        const { message } = res.data;

        if (message === '') {
            // Parse dates using our custom parser for Dutch format
            const startDate = parseDate(event.startTime);
            const endDate = event.endTime ? parseDate(event.endTime) : null;

            if (!startDate) {
                alert('Ongeldige starttijd');
                return;
            }

            currentEvent.setProp('title', event.title);
            currentEvent.setStart(startDate.toISOString());  // ✅ ISO format
            currentEvent.setEnd(endDate ? endDate.toISOString() : null);
            currentEvent.setAllDay(event.isAllDay);
            currentEvent.setExtendedProp('description', event.description);

            eventModal.hide();
        } else {
            alert(`Er ging iets mis: ${message}`);
        }
    })
    .catch(err => alert(`Er ging iets mis: ${err}`));
}

document.getElementById('deleteEvent').addEventListener('click', () => {
    if (confirm(`Weet je zeker dat je "${currentEvent.title}" wilt verwijderen?`)) {
        axios.post('/Home/DeleteEvent', {
            EventId: currentEvent.id
        })
        .then(res => {
            const { message } = res.data;

            if (message === '') {
                currentEvent.remove();
                eventModal.hide();
            } else {
                alert(`Er ging iets mis: ${message}`);
            }
        })
        .catch(err => alert(`Er ging iets mis: ${err}`));
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

// Verwijderd: de automatische uncheck van AllDay bij EndTime change
// Dit was verwarrend gedrag
