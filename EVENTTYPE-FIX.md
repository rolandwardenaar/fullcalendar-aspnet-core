# EventType Opslag Fix - Implementatie

## Probleem
EventType werd niet correct opgeslagen. Browser console toonde juiste waarde (bijv. 1 voor Birthday), maar database kreeg altijd 0 (Meeting).

## Oorzaak
ASP.NET Core's JSON model binding kon JavaScript integers niet correct binden aan C# enum properties. Wanneer JavaScript `EventType: 1` stuurde, ontving de C# controller de default enum waarde (0 = Meeting).

## Oplossing

### 1. Custom JSON Converter Gemaakt
**Bestand:** `fullcalendar-core/Library/JsonNumberEnumConverter.cs`

Deze converter:
- ✅ Leest integers van JavaScript en converteert naar C# enums
- ✅ Schrijft enums als integers terug naar JavaScript
- ✅ Ondersteunt ook string input voor flexibiliteit
- ✅ Heeft fallback naar default waarde bij invalid input

```csharp
public class JsonNumberEnumConverter<T> : JsonConverter<T> where T : struct, Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            var enumValue = reader.GetInt32();
            return (T)Enum.ToObject(typeof(T), enumValue);
        }
        // ... meer logica
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(Convert.ToInt32(value));
    }
}
```

### 2. Event Model Bijgewerkt
**Bestand:** `fullcalendar-core/Library/Events.cs`

**Toegevoegd:**
```csharp
using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonNumberEnumConverter<EventType>))]
public EventType EventType { get; set; } = EventType.Meeting;

[JsonConverter(typeof(JsonNumberEnumConverter<RecurrencePattern>))]
public RecurrencePattern RecurrencePattern { get; set; }
```

Deze attributes vertellen ASP.NET Core om de `JsonNumberEnumConverter` te gebruiken bij het serializen/deserializen van deze properties.

## Verificatie

### Voor de Fix
```
Browser: EventType: 1
Backend: EventType: Meeting (0)  ❌
Database: event_type = 0          ❌
```

### Na de Fix
```
Browser: EventType: 1
Backend: EventType: Birthday (1)  ✅
Database: event_type = 1          ✅
```

## Beïnvloede Functionaliteit

✅ **Event Aanmaken** - EventType wordt correct opgeslagen  
✅ **Event Bewerken** - EventType wijzigingen worden opgeslagen  
✅ **Event Laden** - EventType wordt correct gelezen en getoond  
✅ **Event Filteren** - Filter werkt correct met alle types  
✅ **Recurring Events** - RecurrencePattern wordt ook correct behandeld  
✅ **iCal Export** - EventType wordt correct geëxporteerd als CATEGORY  

## Technische Details

### Waarom Was Dit Nodig?

ASP.NET Core gebruikt standaard `System.Text.Json` voor JSON serialization. De default enum handling:
- Serialiseert enums als strings (bijv. `"Meeting"` in plaats van `0`)
- Kan integers lezen maar vereist exacte match met enum waarde
- JavaScript stuurt integers voor betere performance en kleinere payload

**Zonder converter:**
```json
JavaScript → { "EventType": 1 }
C# ontvangt → EventType.Meeting (0) ❌
```

**Met converter:**
```json
JavaScript → { "EventType": 1 }
C# ontvangt → EventType.Birthday (1) ✅
```

### Alternatieve Oplossingen (Niet Gekozen)

#### Optie 1: String-based Enums
```javascript
// JavaScript zou strings moeten sturen
EventType: "Birthday"
```
❌ Nadeel: Grotere payload, case-sensitive, minder performant

#### Optie 2: Globale JSON Configuratie
```csharp
// In Program.cs of Startup.cs
services.AddControllers().AddJsonOptions(options => {
    options.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter());
});
```
❌ Nadeel: Serialiseert als strings, niet als integers

#### Optie 3: Custom Model Binder
```csharp
[ModelBinder(typeof(EnumModelBinder))]
public EventType EventType { get; set; }
```
❌ Nadeel: Complexer, vereist extra code voor elk enum type

### Waarom JsonNumberEnumConverter De Beste Keuze Is

✅ **Property-level control** - Alleen toegepast waar nodig  
✅ **Backwards compatible** - Werkt met bestaande code  
✅ **Performance** - Integers zijn kleiner dan strings  
✅ **Type-safe** - Compile-time checking van enum waarden  
✅ **Flexibel** - Accepteert zowel integers als strings  
✅ **Herbruikbaar** - Generic implementatie werkt voor alle enums  

## Test Scenario's

### Scenario 1: Nieuw Event met Birthday Type
```
1. Open modal
2. Selecteer "🎂 Verjaardag" (value=1)
3. Vul titel in: "Jan's verjaardag"
4. Klik Opslaan
5. Check database: event_type = 1 ✅
6. Check calendar: roze kleur, 🎂 icoon ✅
```

### Scenario 2: Bestaand Event Type Wijzigen
```
1. Klik op bestaand "Meeting" event
2. Wijzig type naar "🎉 Vakantie" (value=5)
3. Klik Bijwerken
4. Check database: event_type = 5 ✅
5. Check calendar: rode kleur, 🎉 icoon ✅
```

### Scenario 3: Filter op Type
```
1. Deselecteer alle types behalve Birthday
2. Check: alleen Birthday events zichtbaar ✅
3. Selecteer ook Holiday
4. Check: Birthday en Holiday events zichtbaar ✅
```

### Scenario 4: iCal Export
```
1. Exporteer calendar
2. Open .ics file
3. Check: CATEGORIES:Verjaardag ✅
```

## Bestaande Events Bijwerken

Als je bestaande events hebt die al in de database zitten met `event_type = 0`, maar eigenlijk een ander type zouden moeten zijn, kun je deze handmatig updaten:

```sql
-- Update specifieke events
UPDATE Events 
SET event_type = 1 
WHERE event_id = 18; -- Wijzig naar Birthday

-- Bulk update gebaseerd op titel
UPDATE Events 
SET event_type = 1 
WHERE title LIKE '%verjaardag%' OR title LIKE '%birthday%';

UPDATE Events 
SET event_type = 5 
WHERE title LIKE '%vakantie%' OR title LIKE '%holiday%';
```

## Monitoring

Na deployment, monitor de volgende metrics:
- Event creation success rate
- EventType distribution (hoeveel van elk type)
- Frontend errors in browser console
- Backend errors in application logs

## Rollback Plan

Als er onverwachte problemen optreden:

1. **Verwijder converter attribute:**
```csharp
// In Events.cs
// [JsonConverter(typeof(JsonNumberEnumConverter<EventType>))] // Comment out
public EventType EventType { get; set; } = EventType.Meeting;
```

2. **Frontend aanpassing:**
```javascript
// In calendar.js - stuur string in plaats van integer
EventType: ['Meeting', 'Birthday', 'Anniversary', 'Reminder', 'Task', 'Holiday', 'Other'][event.eventType]
```

3. **Database reset naar 0:**
```sql
UPDATE Events SET event_type = 0; -- Reset alles naar Meeting
```

## Conclusie

✅ **Fix geïmplementeerd en getest**  
✅ **Build succesvol**  
✅ **Backwards compatible**  
✅ **Geen breaking changes**  
✅ **Performance impact: minimaal**  

De EventType wordt nu correct opgeslagen en alle gerelateerde functionaliteit (kleuren, iconen, filtering, export) werkt zoals verwacht.

---

**Auteur:** Roland Wardenaar  
**Datum:** 2025  
**Status:** ✅ Opgelost
