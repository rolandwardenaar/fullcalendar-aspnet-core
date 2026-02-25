# Kalender Layout Fix

## Probleem
De kalender was niet volledig zichtbaar:
1. De kalender viel achter het linker menu (sidebar) weg
2. Navigatieknoppen om naar andere jaren en maanden te bladeren waren niet zichtbaar

## Oorzaak
In `fullcalendar-core\Styles\calendar.scss` had het `#calendar` element een `position: fixed` met `left: 0`. Dit zorgde ervoor dat de kalender over de volledige breedte van het venster werd geplaatst, waardoor het achter de sidebar (die ook `position: fixed` heeft met `width: 250px` en `left: 0`) verdween.

```scss
// VOOR (problematisch):
#calendar {
    position: fixed;
    top: 60px;
    left: 0;
    right: 0;
    bottom: 0;
    padding: 1rem;
}
```

## Oplossing
De `position: fixed` is verwijderd, zodat het `#calendar` element normaal binnen het `main-content` element (dat al `margin-left: 250px` heeft) wordt geplaatst.

```scss
// NA (opgelost):
#calendar {
    // No longer fixed position - let it flow in the main-content container
    padding: 1rem;
    min-height: calc(100vh - 120px);
}
```

## Gewijzigde bestanden
1. **fullcalendar-core\Styles\calendar.scss** - Bron SCSS bestand aangepast
2. **fullcalendar-core\wwwroot\css\calendar.css** - Gecompileerde CSS bijgewerkt via `npm run build`

## Resultaat
✅ Kalender is nu volledig zichtbaar naast de sidebar  
✅ Navigatieknoppen (vorige/volgende maand, jaar, etc.) zijn zichtbaar  
✅ De kalender neemt de juiste ruimte in binnen het main-content gebied  
✅ Project compileert zonder fouten

## Verificatie
De wijziging kan geverifieerd worden door:
1. De applicatie te starten
2. In te loggen
3. Te controleren of de kalender volledig zichtbaar is
4. Te controleren of de navigatieknoppen (prev/next, today, month/week/day view) zichtbaar zijn in de header van de kalender

---

**Datum:** 2025-01-29  
**Auteur:** GitHub Copilot
