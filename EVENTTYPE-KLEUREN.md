# EventType Kleuren - Implementatie

## Overzicht
Elk eventtype (Afspraak, Verjaardag, Jubileum, etc.) heeft nu zijn eigen unieke kleur in de kalender.

## Implementatie Details

### 1. Kleurenconfiguratie (Backend)
**Bestand:** `fullcalendar-core\Library\EventTypeConfig.cs`

De kleuren zijn gedefinieerd in de `GetEventColor()` methode:

```csharp
public static string GetEventColor(EventType eventType)
{
    return eventType switch
    {
        EventType.Meeting => "#3788d8",      // 📅 Blauw
        EventType.Birthday => "#e91e63",     // 🎂 Roze
        EventType.Anniversary => "#9c27b0",  // 💐 Paars
        EventType.Reminder => "#ff9800",     // ⏰ Oranje
        EventType.Task => "#4caf50",         // ✓ Groen
        EventType.Holiday => "#f44336",      // 🎉 Rood
        EventType.Other => "#607d8b",        // 📌 Blauw Grijs
        _ => "#3788d8"
    };
}
```

### 2. Kleuren Toepassen in Controller
**Bestand:** `fullcalendar-core\Controllers\HomeController.cs` (regel 76-77)

Bij het ophalen van events wordt de kleur per event ingesteld:

```csharp
var calendarEvents = allEvents.Select(e => new {
    id = e.EventId,
    title = e.Title,
    start = e.Start,
    end = e.End,
    allDay = e.AllDay,
    editable = e.UserId == currentUserId,
    backgroundColor = EventTypeConfig.GetEventColor(e.EventType),  // Achtergrondkleur
    borderColor = EventTypeConfig.GetEventColor(e.EventType),      // Randkleur
    extendedProps = new {
        // ...
    }
});
```

### 3. CSS Aanpassingen
**Bestand:** `fullcalendar-core\Styles\calendar.scss`

De vaste gradient achtergrond is verwijderd, zodat de kleuren uit de backend worden gebruikt:

**VOOR (problematisch):**
```scss
.fc-event {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%) !important;
    color: white !important;
    // ...
}
```

**NA (opgelost):**
```scss
.fc-event {
    border: none !important;
    border-radius: 4px;
    padding: 2px 4px;
    font-weight: 500;
    cursor: pointer;
    transition: transform 0.2s, box-shadow 0.2s;
    color: white !important;
    
    // Event colors are set via backgroundColor from backend (EventTypeConfig)
    // No default background color here to allow per-type colors
    // ...
}
```

## Kleuren Overzicht

| EventType     | Kleur           | Hex Code  | Icoon | Contrast |
|--------------|-----------------|-----------|-------|----------|
| Afspraak     | 📅 Donkerblauw   | #1565c0   | 📅    | ⚪ Wit    |
| Verjaardag   | 🎂 Donker Roze   | #c2185b   | 🎂    | ⚪ Wit    |
| Jubileum     | 💐 Donker Paars  | #7b1fa2   | 💐    | ⚪ Wit    |
| Herinnering  | ⏰ Donker Oranje | #ef6c00   | ⏰    | ⚪ Wit    |
| Taak         | ✓ Donker Groen   | #2e7d32   | ✓     | ⚪ Wit    |
| Vakantie     | 🎉 Donker Rood   | #c62828   | 🎉    | ⚪ Wit    |
| Overig       | 📌 Donker Grijs  | #37474f   | 📌    | ⚪ Wit    |

## Visuele Verbeteringen

### Event Styling
- **Witte border**: 2px solid rgba(255, 255, 255, 0.9) voor duidelijk contrast
- **Box-shadow**: 0 2px 4px rgba(0, 0, 0, 0.2) voor diepte
- **Border-radius**: 5px voor afgeronde hoeken
- **Text-shadow**: 0 1px 2px rgba(0, 0, 0, 0.3) voor betere leesbaarheid

### Hover Effect
- **Transform**: translateY(-2px) - event beweegt omhoog
- **Box-shadow**: 0 4px 12px rgba(0, 0, 0, 0.3) - sterkere schaduw
- **Border**: Volledig wit (opacity 1.0)

### Events van Andere Gebruikers
- **Opacity**: 80% transparantie
- **Special border**: Dikke witte border aan linkerkant (4px)
- **Hover opacity**: 95%

## Kleuren Aanpassen

Om de kleuren aan te passen, wijzig je de hex-codes in `EventTypeConfig.cs`:

1. Open `fullcalendar-core\Library\EventTypeConfig.cs`
2. Pas de hex-code aan in de `GetEventColor()` methode
3. Sla op en herstart de applicatie

**Let op:** De kleuren in de sidebar filters (in `_Layout.cshtml`) moeten handmatig worden bijgewerkt om overeen te komen met de nieuwe kleuren.

## Gerelateerde Bestanden

- `fullcalendar-core\Library\EventTypeConfig.cs` - Kleurenconfiguratie
- `fullcalendar-core\Controllers\HomeController.cs` - Kleuren toepassen
- `fullcalendar-core\Styles\calendar.scss` - CSS styling
- `fullcalendar-core\Views\Shared\_Layout.cshtml` - Sidebar filters (kleuren handmatig bijwerken)

## Testen

1. Start de applicatie
2. Log in
3. Maak events aan met verschillende EventTypes
4. Verifieer dat elk event de juiste kleur heeft:
   - Afspraak = Blauw (#3788d8)
   - Verjaardag = Roze (#e91e63)
   - Jubileum = Paars (#9c27b0)
   - Etc.

## Speciale Styling

### Events van andere gebruikers
Events van andere gebruikers krijgen:
- 75% opacity (transparantie)
- Een donkere rand aan de linkerkant
- Bij hover: 90% opacity

### Gedeelde events
Gedeelde events krijgen:
- Een gouden rand aan de linkerkant (`border-left: 4px solid gold`)
- Een 🔗 symbool in de rechterbovenhoek

---

**Datum:** 2025-01-29  
**Auteur:** GitHub Copilot
