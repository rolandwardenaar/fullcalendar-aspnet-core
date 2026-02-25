# Alle Events Tonen - "+X more" Link Verwijderd

## Probleem
Wanneer er meerdere events op één dag waren, toonde FullCalendar slechts een paar events en verborg de rest achter een "+3 extra" of "+X more" link. Dit maakte het moeilijk om snel een overzicht te krijgen van alle afspraken op een dag.

## Voor & Na

### VOOR ❌
```
┌─────────────────────────┐
│ 27 November            │
├─────────────────────────┤
│ ████ 09:00 Afspraak    │
│ ████ 14:00 Vergadering │
│ 🔗 +3 extra            │  ← Verborgen events!
└─────────────────────────┘
     ↑
Moet klikken om rest te zien
```

### NA ✅
```
┌─────────────────────────┐
│ 27 November            │
├─────────────────────────┤
│ ████ 09:00 Afspraak    │
│ ████ 14:00 Vergadering │
│ ████ 10:30 Meeting     │  ← Allemaal zichtbaar!
│ ████ 16:00 Verjaardag  │
│ ████ 18:00 Reminder    │
└─────────────────────────┘
     ↑
Alle events direct zichtbaar
```

## Oplossing

### 1. JavaScript Configuratie (calendar.js)

De `dayMaxEvents` optie is aangepast van `true` naar `false`:

**VOOR:**
```javascript
dayMaxEvents: true,  // Beperkt aantal zichtbare events
```

**NA:**
```javascript
dayMaxEvents: false,  // Toon alle events, geen "+X more" link
```

### 2. CSS Optimalisatie (calendar.scss)

Styling toegevoegd om dagen met veel events goed weer te geven:

```scss
// Day events container - allow all events to show
.fc-daygrid-day-events {
    min-height: 0 !important;
    margin-bottom: 0 !important;
}

// Day frame - make it flexible height
.fc-daygrid-day-frame {
    min-height: 100px;
    position: relative;
}

.fc-daygrid-day {
    background-color: white;
    position: relative;  // Voor betere event positionering
}
```

## Voordelen

### ✅ Direct Overzicht
Alle events op een dag zijn direct zichtbaar zonder extra klikken.

### ✅ Betere Planning
Je ziet meteen of een dag vol is of nog ruimte heeft.

### ✅ Minder Interactie
Geen "+X more" popups meer - alles in één oogopslag.

### ✅ Consistente Weergave
Alle events worden als volle balkjes getoond, onder elkaar.

## Alternatieve Configuraties

Als je toch een limiet wilt instellen, kun je `dayMaxEvents` op een getal zetten:

### Optie 1: Maximaal 5 events
```javascript
dayMaxEvents: 5,  // Toon max 5 events, rest achter "+X more"
```

### Optie 2: Maximaal 10 events
```javascript
dayMaxEvents: 10,  // Toon max 10 events, rest achter "+X more"
```

### Optie 3: Geen limiet (huidige instelling)
```javascript
dayMaxEvents: false,  // Toon ALLE events
```

## Weergave in Verschillende Views

### Maand Weergave (Month View)
- Alle events worden als balkjes onder elkaar getoond
- Dagen met veel events worden automatisch hoger
- Scroll door de maand om alle dagen te zien

### Week Weergave (Week View)
- Alle events op een dag zijn zichtbaar
- Tijdslots tonen events op hun werkelijke tijd
- Overlappende events worden naast elkaar getoond

### Dag Weergave (Day View)
- Alle events van de geselecteerde dag in tijdslijn
- Geen limiet nodig - altijd alles zichtbaar

## Responsive Gedrag

### Desktop
- Dagen kunnen meerdere events tonen
- Kalender past automatisch de hoogte aan

### Tablet/Mobile
- Events blijven allemaal zichtbaar
- Mogelijk scrollen nodig bij veel events op één dag

## Technische Details

### FullCalendar Optie
`dayMaxEvents: false` zorgt ervoor dat FullCalendar:
1. Alle events render in de dag cel
2. Geen "+more" link genereert
3. De dag cel automatisch vergroot indien nodig

### CSS Aanpassingen
- `min-height: 100px` - Minimum hoogte voor dag cellen
- `position: relative` - Voor correcte event positionering
- Flexible height - Dagen groeien mee met aantal events

## Testen

Verificatiestappen:
1. ✅ Maak 5+ events op dezelfde dag
2. ✅ Bekijk de maandweergave
3. ✅ Alle events moeten zichtbaar zijn als balkjes
4. ✅ Geen "+X more" link aanwezig
5. ✅ Dag cel is automatisch hoger geworden

## Mogelijke Issues

### Probleem: Dag wordt te hoog
**Oplossing:** Stel een maximum in met `dayMaxEvents: 8`

### Probleem: Events zijn te klein
**Oplossing:** Past de padding aan in `calendar.scss`:
```scss
.fc-daygrid-event {
    padding: 4px 8px;  // Grotere padding
}
```

### Probleem: Tekst is niet leesbaar
**Oplossing:** Al opgelost met text-shadow en goede kleuren

## Gewijzigde Bestanden

1. **fullcalendar-core\Scripts\calendar.js**
   - `dayMaxEvents: true` → `dayMaxEvents: false`

2. **fullcalendar-core\Styles\calendar.scss**
   - CSS voor flexible dag hoogte
   - Styling voor event containers

3. **fullcalendar-core\wwwroot\js\calendar.js** (gecompileerd)
4. **fullcalendar-core\wwwroot\css\calendar.css** (gecompileerd)

## Samenvatting

| Aspect | Voor | Na |
|--------|------|-----|
| Event limiet | Max ~3 events | Onbeperkt |
| "+X more" link | Ja | Nee ❌ |
| Zichtbaarheid | Gedeeltelijk | Volledig ✅ |
| Klikken nodig | Ja | Nee ✅ |
| Overzicht | Beperkt | Compleet ✅ |

---

**Datum:** 2025-01-29  
**Update:** Alle Events Tonen  
**Auteur:** GitHub Copilot

**Tip:** Als je toch een limiet wilt, verander `dayMaxEvents: false` naar `dayMaxEvents: 10` (of ander getal) in `calendar.js`.
