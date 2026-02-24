# FullCalendar ASP.NET Core - Moderne Build Setup

## 🚀 Gemoderniseerd naar Vite

Deze applicatie gebruikt nu **Vite** in plaats van Webpack voor een snellere en modernere development ervaring.

### Wat is er veranderd?

- ✅ **Vite 6** in plaats van Webpack 4
- ✅ **FullCalendar 6** (moderne API, geen jQuery dependency)
- ✅ **ES Modules** in plaats van CommonJS
- ✅ **Moderne Sass compiler** in plaats van node-sass (deprecated)
- ✅ **Bootstrap 5** in plaats van Bootstrap 4
- ✅ **Nederlandse localisatie** (FullCalendar, Flatpickr, UI teksten)
- ✅ **24-uurs tijdnotatie** (dd-mm-yyyy uu:mm)
- ✅ Geen Babel meer nodig (Vite gebruikt esbuild)

### 📦 Installatie

```bash
npm install
```

### 🔧 Development

Start de development server met hot module replacement:

```bash
npm run dev
```

De Vite dev server draait op `http://localhost:5173` en werkt samen met je ASP.NET Core applicatie.

### 🏗️ Production Build

Build voor productie (geminificeerd en geoptimaliseerd):

```bash
npm run build
```

Output wordt gegenereerd in `wwwroot/`:
- `wwwroot/js/calendar.js` - JavaScript bundle
- `wwwroot/css/calendar.css` - CSS bundle

### 📁 Project Structuur

```
fullcalendar-core/
├── Scripts/
│   ├── main.js          # Entry point (importeert styles + calendar)
│   └── calendar.js      # Calendar logica met FullCalendar v6 API
├── Styles/
│   └── calendar.scss    # Custom styles
├── wwwroot/             # Build output (gegenereerd)
│   ├── js/
│   └── css/
├── vite.config.js       # Vite configuratie
└── package.json
```

### 🔄 Visual Studio Integration

De npm scripts worden automatisch uitgevoerd:
- **ProjectOpened**: `npm run dev` (development mode met watch)
- **BeforeBuild**: `npm run build` (production build)

### ⚡ Voordelen van Vite

- **Instant HMR**: Wijzigingen zijn onmiddellijk zichtbaar
- **Snelle builds**: 10-100x sneller dan Webpack
- **Kleinere bundles**: Betere tree-shaking en code splitting
- **Moderne tooling**: Gebouwd voor ES modules
- **Minder configuratie**: Out-of-the-box functionaliteit

### 🛠️ Technologie Stack

| Package | Versie | Doel |
|---------|--------|------|
| Vite | 6.0 | Build tool & dev server |
| FullCalendar | 6.1 | Moderne calendar library |
| Axios | 1.7 | HTTP client |
| Flatpickr | 4.6 | Date/time picker |
| Bootstrap | 5.3 | UI framework |
| Sass | 1.83 | CSS preprocessor |

### 🇳🇱 Nederlandse Localisatie

De applicatie is volledig gelokaliseerd naar het Nederlands:

**FullCalendar:**
- Maandnamen, dagnamen in het Nederlands
- Knoppen: "Vandaag", "Maand", "Week", "Dag"

**Flatpickr (Datum/tijd picker):**
- 24-uurs tijdnotatie (HH:mm)
- Nederlandse datumnotatie (dd-mm-yyyy)
- Nederlandse maand- en dagnamen

**UI Teksten:**
- Modal: "Nieuw evenement", "Evenement bewerken"
- Labels: "Titel", "Starttijd", "Eindtijd", "Beschrijving", "Hele dag"
- Knoppen: "Aanmaken", "Bijwerken", "Verwijderen", "Sluiten", "Opslaan"
- Meldingen in het Nederlands

**Datum/tijd formaat:**
- Input: `dd-mm-yyyy HH:mm` (bijv. 24-02-2026 14:30)
- Display: Nederlandse locale (bijv. 24-02-2026, 14:30)

### 📝 Migratie Notes

Als je van de oude setup komt:
- ✅ Webpack verwijderd
- ✅ Babel verwijderd (esbuild doet dit)
- ✅ node-sass vervangen door moderne Sass compiler
- ✅ FullCalendar v3 → v6 (breaking changes in API)
- ✅ Moment.js dependency verwijderd (native Date gebruikt)
- ✅ Bootstrap 4 → Bootstrap 5 (nieuwe modal API)
- ✅ Volledige Nederlandse localisatie
- ✅ 12-uurs AM/PM → 24-uurs tijdnotatie
- ✅ m/d/Y formaat → d-m-Y formaat

### 🐛 Troubleshooting

**Build werkt niet?**
```bash
# Verwijder node_modules en package-lock.json
rm -rf node_modules package-lock.json
npm install
```

**Dev server niet bereikbaar?**
Zorg dat poort 5173 vrij is of pas de poort aan in `vite.config.js`.
