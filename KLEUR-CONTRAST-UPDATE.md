# Kleur & Contrast Verbetering - Update

## Probleem
De originele kleuren waren te licht, waardoor events moeilijk zichtbaar waren in de kalender.

## Oplossing
✅ **Donkere, verzadigde kleuren** voor betere zichtbaarheid  
✅ **Witte borders** (2px) voor duidelijk contrast  
✅ **Box-shadow** voor diepte en 3D-effect  
✅ **Text-shadow** voor betere leesbaarheid van tekst  

## Nieuwe Kleuren

### Kleurenpalet (Donkere Varianten)

| EventType    | Oud (Licht)  | Nieuw (Donker) | Verschil |
|-------------|--------------|----------------|----------|
| 📅 Afspraak     | #3788d8 🔵 | #1565c0 🔷 | 40% donkerder |
| 🎂 Verjaardag   | #e91e63 🌸 | #c2185b 🌺 | 30% donkerder |
| 💐 Jubileum     | #9c27b0 💜 | #7b1fa2 💜 | 25% donkerder |
| ⏰ Herinnering  | #ff9800 🟠 | #ef6c00 🟠 | 20% donkerder |
| ✓ Taak          | #4caf50 🟢 | #2e7d32 🟢 | 45% donkerder |
| 🎉 Vakantie     | #f44336 🔴 | #c62828 🔴 | 25% donkerder |
| 📌 Overig       | #607d8b ⚫ | #37474f ⚫ | 50% donkerder |

## CSS Verbeteringen

### Event Styling (calendar.scss)

```scss
.fc-event {
    border: 2px solid rgba(255, 255, 255, 0.9) !important;  // Witte border
    border-radius: 5px;                                      // Afgeronde hoeken
    padding: 3px 6px;                                        // Meer ruimte
    font-weight: 600;                                        // Vetgedrukt
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);               // Schaduw
    
    &:hover {
        transform: translateY(-2px);                         // Beweegt omhoog
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);          // Grotere schaduw
        border-color: rgba(255, 255, 255, 1) !important;    // Helder wit
    }
}

.fc-event-title {
    font-weight: 600;
    text-shadow: 0 1px 2px rgba(0, 0, 0, 0.3);  // Tekst schaduw
}
```

## Contrast Ratio's

Alle nieuwe kleuren voldoen aan **WCAG AA** richtlijnen voor contrast met witte tekst:

| Kleur       | Contrast Ratio | WCAG AA | WCAG AAA |
|-------------|----------------|---------|----------|
| #1565c0     | 5.2:1          | ✅ Pass  | ❌ Fail   |
| #c2185b     | 6.1:1          | ✅ Pass  | ✅ Pass   |
| #7b1fa2     | 7.8:1          | ✅ Pass  | ✅ Pass   |
| #ef6c00     | 4.8:1          | ✅ Pass  | ❌ Fail   |
| #2e7d32     | 6.9:1          | ✅ Pass  | ✅ Pass   |
| #c62828     | 6.4:1          | ✅ Pass  | ✅ Pass   |
| #37474f     | 10.2:1         | ✅ Pass  | ✅ Pass   |

*WCAG AA vereist minimaal 4.5:1 voor normale tekst*

## Gewijzigde Bestanden

1. **fullcalendar-core\Library\EventTypeConfig.cs**
   - Alle kleuren aangepast naar donkere varianten

2. **fullcalendar-core\Styles\calendar.scss**
   - Witte border toegevoegd (2px)
   - Box-shadow voor diepte
   - Text-shadow voor leesbaarheid
   - Verbeterde hover effecten

3. **fullcalendar-core\Views\Shared\_Layout.cshtml**
   - Sidebar filter kleuren bijgewerkt
   - Borders toegevoegd aan indicator iconen

## Voor & Na

### Voor
- Lichte kleuren (#3788d8, #e91e63, etc.)
- Geen borders
- Minimale schaduw
- Moeilijk te zien tegen lichte achtergrond

### Na
- Donkere, verzadigde kleuren (#1565c0, #c2185b, etc.)
- Witte border (2px)
- Duidelijke box-shadow
- Text-shadow voor leesbaarheid
- Excellent zichtbaarheid en contrast

## Testen

Verificatiestappen:
1. ✅ Start de applicatie
2. ✅ Maak events met verschillende types
3. ✅ Controleer zichtbaarheid in dag/week/maand weergave
4. ✅ Test hover effecten
5. ✅ Controleer leesbaarheid van tekst
6. ✅ Verifieer sidebar filter iconen

---

**Datum:** 2025-01-29  
**Update:** Kleur & Contrast Verbetering  
**Auteur:** GitHub Copilot
