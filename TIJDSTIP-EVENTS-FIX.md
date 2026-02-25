# Tijdstip Events Styling Fix

## Probleem
Events met een specifiek tijdstip (timed events) werden weergegeven als kleine gekleurde bolletjes in de kalender, terwijl hele-dag events (all-day events) als volle gekleurde balken verschijnen. Dit maakte tijdstip events moeilijk zichtbaar en inconsistent met de rest van de kalender.

## Voor & Na

### VOOR
```
┌─────────────────────────────────┐
│ 25 November                     │
├─────────────────────────────────┤
│ ████████████████ Hele dag event │  ← Volle balk, goed zichtbaar
│                                 │
│ • 10:00 Meeting                 │  ← Klein bolletje, slecht zichtbaar
│ • 14:30 Verjaardag              │  ← Klein bolletje, slecht zichtbaar
└─────────────────────────────────┘
```

### NA
```
┌─────────────────────────────────┐
│ 25 November                     │
├─────────────────────────────────┤
│ ████████████████ Hele dag event │  ← Volle balk
│                                 │
│ ████████████ 10:00 Meeting      │  ← Volle balk, goed zichtbaar!
│ ████████████ 14:30 Verjaardag   │  ← Volle balk, goed zichtbaar!
└─────────────────────────────────┘
```

## Oplossing

### CSS Aanpassingen (calendar.scss)

Drie belangrijke aanpassingen:

#### 1. Verberg de kleine bolletjes (dots)
```scss
.fc-daygrid-dot-event {
    .fc-daygrid-event-dot {
        display: none !important; // Verberg het kleine bolletje
    }
}
```

#### 2. Maak tijdstip events full-width
```scss
.fc-daygrid-dot-event {
    padding: 3px 6px !important;
    margin: 1px 2px !important;
    
    .fc-event-time {
        display: inline !important; // Toon tijd
        margin-right: 4px;
    }

    .fc-event-title {
        display: inline !important; // Toon titel naast tijd
    }
}
```

#### 3. Consistente styling voor alle daygrid events
```scss
.fc-daygrid-event {
    border: 2px solid rgba(255, 255, 255, 0.9) !important;
    border-radius: 5px;
    padding: 3px 6px;
    font-weight: 600;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
    margin: 1px 2px;
    
    &:hover {
        transform: translateY(-1px);
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
        border-color: rgba(255, 255, 255, 1) !important;
    }
}
```

## Resultaat

Nu hebben ALLE events in de kalender dezelfde professionele uitstraling:
- ✅ Volle gekleurde balken (geen bolletjes meer)
- ✅ Duidelijke witte borders
- ✅ Box-shadow voor diepte
- ✅ Tijd wordt getoond naast de titel
- ✅ Consistente hover effecten
- ✅ Alle eventtype kleuren worden correct toegepast

## Weergave per Event Type

Tijdstip events (timed events) krijgen nu EXACT dezelfde styling als hele-dag events:

| Type | Weergave | Kleur |
|------|----------|-------|
| 📅 10:00 Afspraak | Volle balk | #1565c0 (Donkerblauw) |
| 🎂 14:30 Verjaardag | Volle balk | #c2185b (Donker Roze) |
| 💐 09:00 Jubileum | Volle balk | #7b1fa2 (Donker Paars) |
| ⏰16:00 Herinnering | Volle balk | #ef6c00 (Donker Oranje) |
| ✓ 11:00 Taak | Volle balk | #2e7d32 (Donker Groen) |
| 🎉 Hele dag Vakantie | Volle balk | #c62828 (Donker Rood) |

## Technische Details

### FullCalendar CSS Klassen

FullCalendar gebruikt verschillende klassen voor verschillende event types:

1. **`.fc-event`** - Basis klasse voor alle events
2. **`.fc-daygrid-event`** - Events in de dag grid (maand/week weergave)
3. **`.fc-daygrid-dot-event`** - Tijdstip events (voorheen met bolletjes)
4. **`.fc-daygrid-event-dot`** - Het kleine bolletje zelf (nu verborgen)

### Styling Prioriteit

De CSS styling werkt als volgt:
1. `.fc-event` - Algemene event styling
2. `.fc-daygrid-event` - Specifiek voor daygrid events (overschrijft algemene styling)
3. `.fc-daygrid-dot-event` - Specifiek voor tijdstip events (verbergt bolletje, toont volle balk)

## Testen

Om te verifiëren dat de wijziging werkt:

1. ✅ Maak een event met een specifiek tijdstip (bijv. 10:00 - 11:00)
2. ✅ Maak een hele-dag event
3. ✅ Beide events moeten als volle gekleurde balken verschijnen
4. ✅ Tijdstip moet zichtbaar zijn (bijv. "10:00 Meeting")
5. ✅ Alle events moeten dezelfde borders, shadows en hover effecten hebben

## Gewijzigde Bestanden

- `fullcalendar-core\Styles\calendar.scss` - CSS styling toegevoegd voor tijdstip events
- `fullcalendar-core\wwwroot\css\calendar.css` - Gecompileerde CSS

## Voordelen

1. **Consistentie**: Alle events zien er hetzelfde uit, ongeacht type
2. **Zichtbaarheid**: Tijdstip events zijn nu even goed zichtbaar als hele-dag events
3. **Professionaliteit**: Geen verwarrende bolletjes meer
4. **Toegankelijkheid**: Betere zichtbaarheid voor alle gebruikers
5. **Gebruikerservaring**: Duidelijker overzicht van de agenda

---

**Datum:** 2025-01-29  
**Update:** Tijdstip Events Styling Fix  
**Auteur:** GitHub Copilot
