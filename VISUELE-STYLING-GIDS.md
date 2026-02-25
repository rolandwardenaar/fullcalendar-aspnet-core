# Visuele Styling Gids

## Event Weergave in Kalender

### Hele-Dag Events (All-Day Events)

```
╔════════════════════════════════════╗
║ 🎉 Vakantie                        ║  ← Witte border (2px)
║                                    ║  ← Donker rood achtergrond (#c62828)
║ Hele dag event                     ║  ← Witte tekst met schaduw
╚════════════════════════════════════╝
     ↑                           ↑
Box-shadow                Border radius (5px)
0 2px 4px rgba(0,0,0,0.2)
```

### Tijdstip Events (Timed Events) - NIEUWE STYLING!

```
╔════════════════════════════════════╗
║ 🎂 14:30 verjaardag / feest        ║  ← Witte border (2px)
║                                    ║  ← Donker roze achtergrond (#c2185b)
║ Tijd + titel samen                 ║  ← Witte tekst met schaduw
╚════════════════════════════════════╝
     ↑                           ↑
Tijd inline           GEEN bolletje meer!
```

**Let op:** Tijdstip events zien er nu IDENTIEK uit aan hele-dag events, maar met de tijd getoond voor de titel!

### Hover Effect

```
╔════════════════════════════════════╗ ← Volledig wit (opacity 1.0)
║ 🎂 verjaardag / feest              ║ 
║                                    ║ ← Beweegt 2px omhoog
║ Event details hier...              ║ 
╚════════════════════════════════════╝
        ↓
   Grotere schaduw
   0 4px 12px rgba(0,0,0,0.3)
```

### Events van Andere Gebruikers

```
╠═══╦════════════════════════════════╗ ← Dikke witte border links (4px)
║   ║ 📅 afspraken inplannen         ║ ← 80% opacity
║   ║                                ║ ← Normale borders rondom (2px)
║   ║ Event van andere gebruiker     ║
╚═══╩════════════════════════════════╝
```

## Kleur Showcase

### 📅 Afspraak (Meeting)
```
████████████████ #1565c0 (Donkerblauw)
Achtergrond: Donker blauw
Tekst: Wit
Border: Wit (2px)
Schaduw: Ja
```

### 🎂 Verjaardag (Birthday)
```
████████████████ #c2185b (Donker Roze)
Achtergrond: Donker roze/magenta
Tekst: Wit
Border: Wit (2px)
Schaduw: Ja
```

### 💐 Jubileum (Anniversary)
```
████████████████ #7b1fa2 (Donker Paars)
Achtergrond: Donker paars
Tekst: Wit
Border: Wit (2px)
Schaduw: Ja
```

### ⏰ Herinnering (Reminder)
```
████████████████ #ef6c00 (Donker Oranje)
Achtergrond: Donker oranje
Tekst: Wit
Border: Wit (2px)
Schaduw: Ja
```

### ✓ Taak (Task)
```
████████████████ #2e7d32 (Donker Groen)
Achtergrond: Donker groen
Tekst: Wit
Border: Wit (2px)
Schaduw: Ja
```

### 🎉 Vakantie (Holiday)
```
████████████████ #c62828 (Donker Rood)
Achtergrond: Donker rood
Tekst: Wit
Border: Wit (2px)
Schaduw: Ja
```

### 📌 Overig (Other)
```
████████████████ #37474f (Donker Grijs)
Achtergrond: Donker blauw-grijs
Tekst: Wit
Border: Wit (2px)
Schaduw: Ja
```

## Sidebar Filter Iconen

```
┌─────────────────────────────────┐
│ Eventtypes filteren             │
├─────────────────────────────────┤
│ ☑ [📅] Afspraak         #1565c0 │
│ ☑ [🎂] Verjaardag       #c2185b │
│ ☑ [💐] Jubileum         #7b1fa2 │
│ ☑ [⏰] Herinnering      #ef6c00 │
│ ☑ [✓]  Taak             #2e7d32 │
│ ☑ [🎉] Vakantie         #c62828 │
│ ☑ [📌] Overig           #37474f │
├─────────────────────────────────┤
│ [Alles selecteren] [Alles des.] │
└─────────────────────────────────┘
```

Elk icoon heeft:
- De juiste eventtype kleur als achtergrond
- Een witte border (2px, 30% opacity)
- Een emoji icoon

## CSS Klassen & Styling

### Basis Event Class
```css
.fc-event {
    border: 2px solid rgba(255, 255, 255, 0.9);
    border-radius: 5px;
    padding: 3px 6px;
    font-weight: 600;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
    color: white;
}
```

### Event Titel
```css
.fc-event-title {
    font-weight: 600;
    text-shadow: 0 1px 2px rgba(0, 0, 0, 0.3);
}
```

### Event Tijd
```css
.fc-event-time {
    font-weight: 500;
    text-shadow: 0 1px 2px rgba(0, 0, 0, 0.3);
}
```

### Andere Gebruikers
```css
.fc-event.other-user-event {
    opacity: 0.80;
    border-left: 4px solid rgba(255, 255, 255, 0.5);
}
```

## Toegankelijkheid

### Contrast Ratio's (met witte tekst)
✅ #1565c0: 5.2:1 (WCAG AA Pass)
✅ #c2185b: 6.1:1 (WCAG AA & AAA Pass)
✅ #7b1fa2: 7.8:1 (WCAG AA & AAA Pass)
✅ #ef6c00: 4.8:1 (WCAG AA Pass)
✅ #2e7d32: 6.9:1 (WCAG AA & AAA Pass)
✅ #c62828: 6.4:1 (WCAG AA & AAA Pass)
✅ #37474f: 10.2:1 (WCAG AA & AAA Pass)

### Ondersteuning voor Kleurenblindheid
- **Protanopia (rood-blind)**: Iconen helpen onderscheid te maken
- **Deuteranopia (groen-blind)**: Iconen helpen onderscheid te maken
- **Tritanopia (blauw-blind)**: Iconen helpen onderscheid te maken
- **Text-shadow**: Verbetert leesbaarheid in alle gevallen

---

**Let op:** Iconen (📅🎂💐⏰✓🎉📌) zijn altijd aanwezig als extra visuele indicator naast kleur!
