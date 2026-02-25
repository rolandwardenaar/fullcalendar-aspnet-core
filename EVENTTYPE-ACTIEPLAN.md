# EventType Debug - Actie Plan

## Huidige Situatie
- ❌ EventType blijft 0 in database
- ❌ Geen console logging zichtbaar
- ✅ Build succesvol
- ✅ JsonNumberEnumConverter toegevoegd

## Debug Logging Toegevoegd

### 1. Script Load Check
Aan het begin van `calendar.js`:
```javascript
console.log('Calendar.js loaded!');
```

### 2. Button Click Check
In de `eventModalSave` event listener:
```javascript
console.log('eventModalSave clicked!');
console.log('EventType element:', eventTypeElement);
console.log('EventType value (raw):', eventTypeElement?.value);
console.log('EventType (parsed):', eventType);
console.log('Event object being sent:', event);
```

### 3. Backend Logging
In `HomeController.cs`:
```csharp
System.Diagnostics.Debug.WriteLine($"AddEvent received - EventType: {evt.EventType} ({(int)evt.EventType})");
```

## Actie Stappen

### Stap 1: Hard Refresh Browser
**Zeer belangrijk!** De browser cache bevat mogelijk oude JavaScript.

**Chrome/Edge:**
1. Druk `Ctrl + Shift + Delete`
2. Selecteer "Cached images and files"
3. Klik "Clear data"

**Of sneller:**
1. Druk `Ctrl + F5` (hard refresh)
2. Of `Ctrl + Shift + R`

### Stap 2: Applicatie Herstarten
1. Stop de applicatie in Visual Studio (Shift + F5)
2. Start opnieuw (F5)

### Stap 3: Browser Developer Tools Openen
1. Druk F12
2. Ga naar Console tab
3. Check of je ziet: `Calendar.js loaded!`

### Stap 4: Test Event Aanmaken
1. Klik "Nieuw evenement"
2. Selecteer "🎂 Verjaardag" (value=1)
3. Vul titel in
4. Klik "Aanmaken"

### Stap 5: Check Console Output
Je zou moeten zien:
```
Calendar.js loaded!
eventModalSave clicked!
EventType element: <select id="EventType" class="form-select">...</select>
EventType value (raw): "1"
EventType (parsed): 1
Event object being sent: {eventType: 1, ...}
Sending add event: {eventType: 1, ...}
EventType value: 1
Add event response: {...}
```

### Stap 6: Check Visual Studio Output
Open Output window (View → Output), selecteer "Debug":
```
AddEvent received - EventType: Birthday (1)
Before AddEvent - EventType: Birthday (1)
DA.AddEvent - EventType: Birthday, Value: 1
```

## Mogelijke Problemen & Oplossingen

### Probleem 1: "Calendar.js loaded!" verschijnt niet
**Oorzaak:** JavaScript wordt niet geladen of oude versie wordt gebruikt

**Oplossing:**
1. Hard refresh browser (Ctrl + F5)
2. Check of `wwwroot/js/calendar.js` bestaat en recent is aangepast
3. Check browser Network tab voor 404 errors op calendar.js

### Probleem 2: "eventModalSave clicked!" verschijnt niet
**Oorzaak:** Event listener wordt niet geregistreerd

**Oplossing:**
1. Check of er JavaScript errors zijn in Console tab
2. Check of `eventModalSave` button bestaat in HTML
3. Verifieer dat modal correct wordt geïnitialiseerd

### Probleem 3: EventType element is null
**Oorzaak:** HTML select element bestaat niet of heeft verkeerde ID

**Oplossing:**
```javascript
// Test in browser console:
document.getElementById('EventType')
// Moet <select> element retourneren, niet null
```

### Probleem 4: EventType value is undefined
**Oorzaak:** Select heeft geen value attribute

**Oplossing:**
Check HTML in Index.cshtml - elke option moet value hebben:
```html
<option value="0">📅 Afspraak</option>
<option value="1">🎂 Verjaardag</option>
```

### Probleem 5: Backend ontvangt nog steeds 0
**Oorzaak:** JsonNumberEnumConverter wordt niet gebruikt

**Oplossing:**
Check in `Events.cs`:
```csharp
using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonNumberEnumConverter<EventType>))]
public EventType EventType { get; set; } = EventType.Meeting;
```

## Webpack Build (Indien Nodig)

Als changes niet worden doorgevoerd:

### Optie 1: Visual Studio Build
1. Build → Rebuild Solution
2. Webpack wordt automatisch uitgevoerd als NPM Task Runner is geconfigureerd

### Optie 2: Handmatig Webpack
1. Open terminal in project root
2. Navigeer naar `fullcalendar-core` folder
3. Run: `npm run build`

### Optie 3: Watch Mode (Voor Development)
1. Open Task Runner Explorer (View → Other Windows → Task Runner Explorer)
2. Dubbelklik op "watch" onder package.json
3. Webpack rebuildt automatisch bij wijzigingen

## Verificatie Checklist

- [ ] Browser cache geleegd (Ctrl + F5)
- [ ] Applicatie herstart
- [ ] Console toont "Calendar.js loaded!"
- [ ] Bij klikken op Opslaan: "eventModalSave clicked!" verschijnt
- [ ] EventType waarde wordt gelogd (niet undefined)
- [ ] Backend logging verschijnt in Visual Studio Output
- [ ] Database toont correcte event_type waarde (1 voor Birthday)

## Als Alles Werkt

Verwijder de debug logging:

### In calendar.js:
```javascript
// Verwijder deze regels:
console.log('Calendar.js loaded!');
console.log('eventModalSave clicked!');
console.log('EventType element:', eventTypeElement);
console.log('EventType value (raw):', eventTypeElement?.value);
console.log('EventType (parsed):', eventType);
console.log('Event object being sent:', event);
console.log('Sending add event:', event);
console.log('EventType value:', event.eventType);
```

### In HomeController.cs:
```csharp
// Verwijder deze regels:
System.Diagnostics.Debug.WriteLine($"AddEvent received - EventType: {evt.EventType} ({(int)evt.EventType})");
System.Diagnostics.Debug.WriteLine($"Before AddEvent - EventType: {evt.EventType} ({(int)evt.EventType})");
```

### In DA.cs:
```csharp
// Verwijder deze regels:
System.Diagnostics.Debug.WriteLine($"DA.AddEvent - EventType: {evt.EventType}, Value: {eventTypeValue}");
System.Diagnostics.Debug.WriteLine($"DA.UpdateEvent - EventType: {evt.EventType}, Value: {eventTypeValue}");
```

Rebuild en test nogmaals!

---

**Auteur:** Roland Wardenaar  
**Datum:** 2025  
**Status:** 🔍 Debug Mode Actief
