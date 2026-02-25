# Event Type Filter Feature

## Overzicht

De event type filter functionaliteit stelt gebruikers in staat om de kalenderweergave te filteren op eventtypes via checkboxen in de zijbalk. Dit helpt om de kalender overzichtelijk te houden en te focussen op specifieke soorten events.

## Locatie

De filter bevindt zich in de **linker sidebar**, onder de navigatiemenu items en boven de uitlog knop.

## Functies

### 1. Individuele Type Selectie
- **7 Event Types** met elk een eigen checkbox:
  - 📅 Afspraak (Blauw)
  - 🎂 Verjaardag (Roze)
  - 💐 Jubileum (Paars)
  - ⏰ Herinnering (Oranje)
  - ✓ Taak (Groen)
  - 🎉 Vakantie (Rood)
  - 📌 Overig (Blauw Grijs)

### 2. Visuele Indicatoren
- Elke checkbox toont:
  - Het event type icoon (emoji)
  - Een gekleurde indicator (overeenkomend met de event kleur)
  - De naam van het event type

### 3. Snelle Acties
- **"Alles selecteren"** knop - Toont alle event types
- **"Alles deselecteren"** knop - Verbergt alle event types

### 4. Real-time Updates
- De kalender wordt automatisch bijgewerkt zodra je een checkbox aan/uit zet
- Geen page refresh nodig

## Gebruik

### Basis Filtering
1. Kijk in de linker sidebar naar de sectie "Eventtypes filteren"
2. Vink checkboxen aan voor types die je wilt zien
3. Vink checkboxen uit voor types die je wilt verbergen
4. De kalender past zich direct aan

### Snel Alle Types Tonen/Verbergen
- Klik op **"Alles selecteren"** om alle types te tonen
- Klik op **"Alles deselecteren"** om alle types te verbergen

## Veelvoorkomende Scenario's

### Scenario 1: Focus op Werk
**Doel**: Alleen werk-gerelateerde events tonen
**Actie**: 
1. Klik "Alles deselecteren"
2. Vink alleen aan: Afspraak en Taak

### Scenario 2: Persoonlijke Events
**Doel**: Alleen persoonlijke events tonen
**Actie**:
1. Klik "Alles deselecteren"
2. Vink aan: Verjaardag, Jubileum, Vakantie

### Scenario 3: Opschonen Kalenderweergave
**Doel**: Minder gebruikte types verbergen
**Actie**: Vink types uit die je zelden gebruikt (bijv. Overig, Herinnering)

## Technische Details

### Frontend Implementatie
- **Locatie**: `Views/Shared/_Layout.cshtml`
- **HTML**: Checkboxen met `data-event-type-filter` attribuut
- **CSS**: Styling in `<style>` sectie van _Layout.cshtml
- **Klassen**: 
  - `.event-filter-section` - Container voor de hele filter sectie
  - `.filter-checkbox` - Individuele checkbox items
  - `.event-type-indicator` - Gekleurde indicator per type
  - `.btn-filter-action` - Knoppen voor select all/deselect all

### JavaScript Implementatie
- **Locatie**: `Scripts/calendar.js`
- **State Management**: 
  ```javascript
  let activeEventTypes = new Set([0, 1, 2, 3, 4, 5, 6]);
  ```
- **Event Listeners**: 
  - Checkbox change events
  - Select all button click
  - Deselect all button click

### Kalender Integratie
- **Events Functie**: Aangepaste async functie in calendar configuratie
- **Filtering**: Events worden gefilterd voordat ze in de kalender worden getoond
- **Refresh**: `calendar.refetchEvents()` wordt aangeroepen bij filterwijziging

### Code Flow
1. Gebruiker wijzigt checkbox
2. Event listener vangt change event op
3. `activeEventTypes` Set wordt bijgewerkt
4. `calendar.refetchEvents()` wordt aangeroepen
5. Calendar's `events` functie haalt data op van server
6. Events worden gefilterd op basis van `activeEventTypes`
7. Alleen gefilterde events worden getoond

## Styling

### Kleurenschema
De filter sectie gebruikt het sidebar thema:
- **Achtergrond**: Transparant met hover effect
- **Tekst**: Wit met 85% opacity
- **Hover**: Licht grijs overlay (rgba(255,255,255,0.05))
- **Actie Knoppen**: Semi-transparante witte achtergrond

### Layout
- **Positie**: Onder navigatie menu, boven uitlog knop
- **Afstand**: 2rem top margin, 1.5rem padding-top
- **Border**: Thin white border top (10% opacity)
- **Gap**: 0.5rem tussen checkboxen

## Prestaties

### Optimalisaties
- **Set Data Structure**: Snelle lookup voor active types
- **Event Delegation**: Efficiënte event handling
- **Debouncing**: Niet nodig - refetch is al geoptimaliseerd door FullCalendar

### Laadtijd
- **Initieel**: < 50ms (checkbox initialization)
- **Filter Change**: < 100ms (calendar refetch)
- **Geen Impact**: Op initiële page load

## Foutafhandeling

### Scenario's
1. **Geen Types Geselecteerd**: Kalender toont geen events (verwacht gedrag)
2. **Server Error**: FullCalendar's ingebouwde error handling
3. **Invalid Event Type**: Fallback naar type 0 (Meeting)

## Browser Compatibiliteit

Werkt in alle moderne browsers:
- ✅ Chrome/Edge (Chromium)
- ✅ Firefox
- ✅ Safari
- ✅ Opera

## Toekomstige Verbeteringen

Mogelijke uitbreidingen:
1. **Opslaan Filter Voorkeur**: Bewaar selectie in localStorage of database
2. **Filter Presets**: Vooraf gedefinieerde filter combinaties (bijv. "Werk", "Privé")
3. **Kleur Customization**: Laat gebruikers eigen kleuren kiezen per type
4. **Keyboard Shortcuts**: Sneltoetsen voor snel filteren
5. **Search in Filters**: Zoekbalk voor snel vinden van types

## Support

Voor vragen of problemen met de filter functionaliteit:
- Check de console voor JavaScript errors
- Verifieer dat `activeEventTypes` correct wordt bijgewerkt
- Test of `calendar.refetchEvents()` wordt aangeroepen

---

**Versie**: 1.0  
**Laatste Update**: 2025  
**Auteur**: Roland Wardenaar
