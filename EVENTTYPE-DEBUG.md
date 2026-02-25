# EventType Opslag - ✅ OPGELOST

## Status: ✅ **PROBLEEM OPGELOST**

EventType wordt nu correct opgeslagen bij het toevoegen en aanpassen van events!

## Oplossing
Het probleem is opgelost door het toevoegen van een `JsonNumberEnumConverter` die JavaScript integers correct omzet naar C# enum waarden.

**Geïmplementeerde fix:**
- `JsonNumberEnumConverter<T>` in `Library/JsonNumberEnumConverter.cs`
- `[JsonConverter]` attribute op `EventType` property in `Events.cs`

## Verificatie

✅ **Browser console** - EventType waarde wordt correct verstuurd (bijv. 1 voor Birthday)  
✅ **Backend** - EventType wordt correct ontvangen en verwerkt  
✅ **Database** - event_type kolom bevat correcte waarde (0-6)  
✅ **Calendar** - Events tonen correcte kleur en icoon per type  

## EventType Mapping

### 1. Browser Console Checken
Open de browser Developer Tools (F12) en ga naar het Console tab.

**Bij het aanmaken van een event:**
1. Klik op "Nieuw evenement"
2. Selecteer een EventType (bijv. Verjaardag 🎂)
3. Vul de overige velden in
4. Klik op "Aanmaken"
5. Kijk in de console naar:
   ```
   Sending add event: {...}
   EventType value: <nummer>
   ```
   Het nummer moet overeenkomen met de geselecteerde type (0=Meeting, 1=Birthday, etc.)

**Bij het bewerken van een event:**
1. Klik op een bestaand event
2. Wijzig het EventType
3. Klik op "Bijwerken"
4. Kijk in de console naar:
   ```
   Sending update event: {...}
   EventType value: <nummer>
   ```

### 2. Visual Studio Output Checken
Open in Visual Studio de Output window (View -> Output) en selecteer "Debug" in de dropdown.

**Bij het aanmaken/bewerken:**
Je zou moeten zien:
```
AddEvent received - EventType: Birthday (1)
Before AddEvent - EventType: Birthday (1)
DA.AddEvent - EventType: Birthday, Value: 1
```

Of voor update:
```
UpdateEvent received - EventType: Birthday (1)
Before UpdateEvent - EventType: Birthday (1)
DA.UpdateEvent - EventType: Birthday, Value: 1
```

### 3. Database Checken
Open de SQLite database (`FullCalendar.db`) met een SQLite browser tool.

**Query om te checken:**
```sql
SELECT event_id, title, event_type FROM Events ORDER BY event_id DESC LIMIT 10;
```

De `event_type` kolom moet een waarde hebben (0-6), niet NULL.

## Mogelijke Oorzaken & Oplossingen

### Oorzaak 1: JavaScript stuurt geen waarde
**Symptoom:** Console toont `EventType value: undefined` of `EventType value: null`

**Oplossing:** Check of de EventType select field een waarde heeft.

In `calendar.js` bij `eventModalSave` event listener:
```javascript
const eventType = parseInt(document.getElementById('EventType').value);
console.log('Selected EventType:', eventType); // Moet 0-6 zijn
```

### Oorzaak 2: Backend ontvangt geen waarde of default waarde
**Symptoom:** Visual Studio Output toont `EventType: Meeting (0)` terwijl een ander type is geselecteerd, OF browser console toont correct nummer maar database heeft 0.

**Oplossing:** ✅ **OPGELOST** - JSON converter toegevoegd voor correcte enum binding.

De `JsonNumberEnumConverter` in `Library/JsonNumberEnumConverter.cs` zorgt ervoor dat JavaScript integers correct worden omgezet naar C# enums. Deze converter is al toegepast op de `EventType` en `RecurrencePattern` properties in het `Event` model.

Als het probleem zich nog steeds voordoet, check of:
1. De `[JsonConverter(typeof(JsonNumberEnumConverter<EventType>))]` attribute aanwezig is
2. De `JsonNumberEnumConverter.cs` file bestaat in de Library folder
3. De using statement `using System.Text.Json.Serialization;` aanwezig is in `Events.cs`

### Oorzaak 3: Database kolom accepteert geen waarde
**Symptoom:** Database toont NULL in event_type kolom

**Oplossing:** Check of de kolom bestaat en de juiste type heeft.

SQL om kolom te checken:
```sql
PRAGMA table_info(Events);
```

Zoek naar: `event_type | INTEGER | 0 | 0 | 0`

Als de kolom niet bestaat, voer uit:
```sql
ALTER TABLE Events ADD COLUMN event_type INTEGER NOT NULL DEFAULT 0;
```

### Oorzaak 4: Bestaande events hebben NULL waarde
**Symptoom:** Nieuwe events werken, maar oude events tonen nog Meeting

**Oplossing:** Update bestaande events:
```sql
UPDATE Events SET event_type = 0 WHERE event_type IS NULL;
```

## Verificatie

Na het oplossen, verifieer dat:

1. ✅ Browser console toont correct EventType nummer
2. ✅ Visual Studio Output toont correct EventType naam en nummer
3. ✅ Database bevat correct EventType nummer (0-6)
4. ✅ Calendar toont event met juiste kleur en icoon

## EventType Mapping

Voor referentie:
- 0 = Meeting (📅 Blauw)
- 1 = Birthday (🎂 Roze)
- 2 = Anniversary (💐 Paars)
- 3 = Reminder (⏰ Oranje)
- 4 = Task (✓ Groen)
- 5 = Holiday (🎉 Rood)
- 6 = Other (📌 Blauw Grijs)

## Debug Logging Verwijderen

Als het probleem is opgelost, kun je de debug logging verwijderen:

**In calendar.js:**
- Verwijder `console.log()` statements in `sendAddEvent` en `sendUpdateEvent`

**In HomeController.cs:**
- Verwijder `System.Diagnostics.Debug.WriteLine()` statements in `AddEvent` en `UpdateEvent`

**In DA.cs:**
- Verwijder `System.Diagnostics.Debug.WriteLine()` statements in `AddEvent` en `UpdateEvent`

---

**Auteur:** Roland Wardenaar  
**Datum:** 2025
