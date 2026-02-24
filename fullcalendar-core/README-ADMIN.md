# Admin Handleiding - FullCalendar Gedeelde Agenda

## Eerste Keer Opstarten

Bij de eerste keer opstarten van de applicatie wordt automatisch een admin account aangemaakt:

- **Gebruikersnaam**: `admin`
- **Wachtwoord**: `Admin123!`
- **Rol**: Administrator

⚠️ **Belangrijk**: Wijzig dit wachtwoord direct na eerste login via "Wachtwoord wijzigen" in het menu.

## Gebruikersbeheer

### Nieuwe Gebruiker Toevoegen

1. Log in als administrator
2. Klik op "Gebruikersbeheer" in het menu
3. Klik op "Nieuwe gebruiker aanmaken"
4. Vul de gegevens in:
   - **Gebruikersnaam**: De login naam (bijv. `jandejong`)
   - **Weergavenaam**: De naam die bij events getoond wordt (bijv. `Jan de Jong`)
   - **Email**: Email adres van de gebruiker
   - **Wachtwoord**: Stel een tijdelijk wachtwoord in
5. Klik op "Gebruiker aanmaken"

**Let op**: Deel het tijdelijke wachtwoord met de nieuwe gebruiker op een veilige manier (niet via email!). De gebruiker kan daarna zelf het wachtwoord wijzigen.

### Gebruiker Verwijderen

1. Ga naar "Gebruikersbeheer"
2. Klik op "Verwijderen" bij de betreffende gebruiker
3. Bevestig de verwijdering

⚠️ **Let op**: Je kunt je eigen admin account niet verwijderen.

## Hoe Werkt de Agenda?

### Voor Alle Gebruikers

- **Zien**: Alle gebruikers kunnen elkaars events zien
- **Eigenaarschap**: Bij elk event wordt getoond wie de eigenaar/maker is
- **Kleuren**:
  - 🟣 **Paars/Blauw**: Eigen events
  - ⚫ **Grijs**: Events van andere gebruikers

### Bewerken & Verwijderen

- Gebruikers kunnen **alleen hun eigen events** bewerken en verwijderen
- Events van anderen zijn read-only (niet te wijzigen)
- Als een gebruiker probeert een event van iemand anders te wijzigen, krijgen ze een foutmelding

### Events Aanmaken

1. Klik en sleep op de kalender om een tijdslot te selecteren
2. Vul de event details in:
   - Titel (verplicht)
   - Starttijd (verplicht)
   - Eindtijd (optioneel, tenzij "Hele dag" aangevinkt)
   - Beschrijving (optioneel)
   - Hele dag checkbox (optioneel)
3. Klik op "Aanmaken"

Het systeem koppelt automatisch de ingelogde gebruiker aan het nieuwe event.

## Beveiliging

### Toegangscontrole

- **Login vereist**: Alle agenda functionaliteit vereist inloggen
- **Gebruikersbeheer**: Alleen administrators kunnen gebruikers toevoegen/verwijderen
- **Event management**: Gebruikers kunnen alleen hun eigen events wijzigen

### Wachtwoord Eisen

Minimale eisen voor wachtwoorden:
- Minimaal 6 karakters lang
- Geen andere restricties (hoofdletters, cijfers, speciale tekens zijn optioneel)

💡 **Tip**: Moedig gebruikers aan om sterke wachtwoorden te gebruiken met een mix van:
- Hoofdletters en kleine letters
- Cijfers
- Speciale tekens
- Minimaal 8-12 karakters

## Technische Details

### Database

- **Type**: SQLite
- **Locatie**: `Data Source=calendar.db` (zie appsettings.json)
- **Tabellen**:
  - Identity tabellen (AspNetUsers, AspNetRoles, etc.)
  - Events tabel (event_id, title, description, event_start, event_end, all_day, user_id, user_name)

### Backup

Het is aan te raden om regelmatig een backup te maken van het `calendar.db` bestand.

### Troubleshooting

**Probleem**: "SQLite Error 1: 'no such table: AspNetRoles'"
**Oplossing**: De applicatie voert automatisch database migrations uit bij opstarten. Als dit probleem blijft bestaan, verwijder het `calendar.db` bestand en start de applicatie opnieuw.

**Probleem**: "Kan niet inloggen met admin account"
**Oplossing**: 
1. Controleer of je de juiste credentials gebruikt: `admin` / `Admin123!`
2. Als het probleem blijft, verwijder `calendar.db` en start opnieuw

## Contact & Support

Voor vragen of problemen, neem contact op met de systeembeheerder.
