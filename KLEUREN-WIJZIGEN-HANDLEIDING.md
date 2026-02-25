# EventType Kleuren Wijzigen - Handleiding

## Snel Overzicht

Elk eventtype heeft zijn eigen kleur in de kalender. Om kleuren aan te passen:

## Stap 1: Open het configuratiebestand

Open: `fullcalendar-core\Library\EventTypeConfig.cs`

## Stap 2: Vind de GetEventColor methode

```csharp
public static string GetEventColor(EventType eventType)
{
    return eventType switch
    {
        EventType.Meeting => "#3788d8",      // 📅 Afspraak - Blauw
        EventType.Birthday => "#e91e63",     // 🎂 Verjaardag - Roze
        EventType.Anniversary => "#9c27b0",  // 💐 Jubileum - Paars
        EventType.Reminder => "#ff9800",     // ⏰ Herinnering - Oranje
        EventType.Task => "#4caf50",         // ✓ Taak - Groen
        EventType.Holiday => "#f44336",      // 🎉 Vakantie - Rood
        EventType.Other => "#607d8b",        // 📌 Overig - Blauw Grijs
        _ => "#3788d8"
    };
}
```

## Stap 3: Wijzig de kleur

Vervang de hex-code (bijv. `#3788d8`) met je gewenste kleur.

**Voorbeelden:**

| Kleur        | Hex Code  |
|--------------|-----------|
| Lichtblauw   | #00bcd4   |
| Donkerblauw  | #1976d2   |
| Roze         | #e91e63   |
| Paars        | #9c27b0   |
| Diep Paars   | #673ab7   |
| Indigo       | #3f51b5   |
| Groen        | #4caf50   |
| Lichtgroen   | #8bc34a   |
| Limoen       | #cddc39   |
| Geel         | #ffeb3b   |
| Amber        | #ffc107   |
| Oranje       | #ff9800   |
| Diep Oranje  | #ff5722   |
| Rood         | #f44336   |
| Bruin        | #795548   |
| Grijs        | #9e9e9e   |
| Blauw Grijs  | #607d8b   |

## Stap 4: Update de sidebar filters (optioneel)

Als je de kleuren hebt gewijzigd, update dan ook de kleuren in de sidebar.

Open: `fullcalendar-core\Views\Shared\_Layout.cshtml`

Zoek naar de filter checkboxes (rond regel 240) en pas de `background-color` aan:

```html
<label class="filter-checkbox">
    <input type="checkbox" value="0" checked data-event-type-filter>
    <span class="event-type-indicator" style="background-color: #3788d8;">📅</span>
    <span>Afspraak</span>
</label>
```

Wijzig `background-color: #3788d8;` naar je nieuwe kleur.

## Stap 5: Herstart de applicatie

Stop de applicatie en start deze opnieuw op om de wijzigingen te zien.

## Tips voor het kiezen van kleuren

1. **Gebruik een color picker** zoals:
   - [Google Color Picker](https://g.co/kgs/color-picker)
   - [HTML Color Codes](https://htmlcolorcodes.com/)
   - [Coolors.co](https://coolors.co/)

2. **Contrast is belangrijk**:
   - Gebruik voldoende contrast met witte tekst
   - Test of de tekst goed leesbaar is op de gekozen kleur

3. **Consistentie**:
   - Gebruik soortgelijke kleuren voor gerelateerde eventtypes
   - Bijv. Birthday en Anniversary kunnen beide roze/paars tinten zijn

4. **Toegankelijkheid**:
   - Denk aan kleurenblinde gebruikers
   - Gebruik ook iconen (die zijn al aanwezig) als extra visuele indicatie

## Kleurschema's

### Optie 1: Pastel Kleuren
```csharp
EventType.Meeting => "#64b5f6",      // Licht blauw
EventType.Birthday => "#f48fb1",     // Licht roze
EventType.Anniversary => "#ce93d8",  // Licht paars
EventType.Reminder => "#ffb74d",     // Licht oranje
EventType.Task => "#81c784",         // Licht groen
EventType.Holiday => "#e57373",      // Licht rood
EventType.Other => "#90a4ae",        // Licht grijs
```

### Optie 2: Levendige Kleuren
```csharp
EventType.Meeting => "#2196f3",      // Blauw
EventType.Birthday => "#e91e63",     // Roze
EventType.Anniversary => "#9c27b0",  // Paars
EventType.Reminder => "#ff9800",     // Oranje
EventType.Task => "#4caf50",         // Groen
EventType.Holiday => "#f44336",      // Rood
EventType.Other => "#607d8b",        // Blauw grijs
```

### Optie 3: Donkere Kleuren
```csharp
EventType.Meeting => "#1976d2",      // Donker blauw
EventType.Birthday => "#c2185b",     // Donker roze
EventType.Anniversary => "#7b1fa2",  // Donker paars
EventType.Reminder => "#f57c00",     // Donker oranje
EventType.Task => "#388e3c",         // Donker groen
EventType.Holiday => "#d32f2f",      // Donker rood
EventType.Other => "#455a64",        // Donker grijs
```

---

**Let op:** De applicatie hoeft niet opnieuw gecompileerd te worden - alleen herstarten is voldoende!
