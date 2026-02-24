# FullCalendar ASP.NET Core - Publicatie Instructies

## 🚀 Publiceren voor Productie (Kestrel op poort 8086)

### Optie 1: Publiceren met Visual Studio

1. **Right-click** op het project in Solution Explorer
2. Klik op **"Publish..."**
3. Kies **"Folder"** als target
4. Kies een output folder (bijv. `bin\Release\net10.0\publish`)
5. Klik op **"Publish"**

### Optie 2: Publiceren via Command Line

```powershell
# Zorg dat npm build is gedaan
cd fullcalendar-core
npm run build

# Publiceer de applicatie
dotnet publish -c Release -o ./publish
```

---

## 🏃 Applicatie Starten

### Direct vanuit publish folder:

```powershell
cd fullcalendar-core/publish

# Start op poort 8086
$env:ASPNETCORE_URLS="http://localhost:8086"
$env:ASPNETCORE_ENVIRONMENT="Production"
dotnet fullcalendarcore.dll
```

### Of vanuit project directory:

```powershell
cd fullcalendar-core

# Run in productie mode op poort 8086
dotnet run --launch-profile Production
```

### Of met environment variables:

```powershell
cd fullcalendar-core

$env:ASPNETCORE_URLS="http://localhost:8086"
$env:ASPNETCORE_ENVIRONMENT="Production"
dotnet run
```

---

## 🌐 Toegang tot de applicatie

Open je browser en ga naar:
```
http://localhost:8086
```

---

## 🔧 Configuratie

### Poort wijzigen

**Optie 1: In launchSettings.json**
```json
"Production": {
  "applicationUrl": "http://localhost:8086"  // Wijzig hier de poort
}
```

**Optie 2: Environment variable**
```powershell
$env:ASPNETCORE_URLS="http://localhost:JOUW_POORT"
```

**Optie 3: In Program.cs**
```csharp
builder.WebHost.UseUrls("http://localhost:8086");
```

---

## 📦 Checklist voor Productie

Voordat je publiceert:

- ✅ **Build frontend**: `npm run build`
- ✅ **Test de applicatie**: Start in Development mode
- ✅ **Database check**: Zorg dat `fullcalendar.db` bestaat of wordt aangemaakt
- ✅ **Logging**: Check `appsettings.Production.json` logging levels
- ✅ **Publish**: `dotnet publish -c Release`

---

## 🐳 Als Windows Service (optioneel)

Als je de app als Windows Service wilt draaien:

1. **Installeer NuGet package:**
```powershell
dotnet add package Microsoft.Extensions.Hosting.WindowsServices
```

2. **Update Program.cs:**
```csharp
builder.Services.AddWindowsService();
```

3. **Publish:**
```powershell
dotnet publish -c Release -r win-x64 --self-contained
```

4. **Installeer service:**
```powershell
sc create "FullCalendarService" binPath="C:\path\to\fullcalendarcore.exe"
sc start "FullCalendarService"
```

---

## 🔒 Firewall (optioneel)

Als je de applicatie toegankelijk wilt maken voor andere computers:

```powershell
# Voeg firewall regel toe
netsh advfirewall firewall add rule name="Kestrel Port 8086" dir=in action=allow protocol=TCP localport=8086

# Of wijzig URL binding naar:
$env:ASPNETCORE_URLS="http://0.0.0.0:8086"
```

---

## 📊 Productie Monitoring

**Check logs:**
```powershell
# Console output toont logs
# Of configureer file logging in appsettings.Production.json
```

**Health check:**
```
http://localhost:8086
```

---

## ⚠️ Belangrijk

- De SQLite database (`fullcalendar.db`) moet **schrijfbaar** zijn
- Bij eerste start wordt de database automatisch geïnitialiseerd
- Datums worden automatisch genormaliseerd naar ISO formaat
- Zorg dat de `wwwroot` folder **mee wordt gepubliceerd** (bevat CSS/JS)

---

## 🆘 Troubleshooting

**Poort al in gebruik?**
```powershell
# Check welk proces poort 8086 gebruikt
netstat -ano | findstr :8086

# Stop het proces (vervang PID)
taskkill /PID <PID> /F
```

**Database permission errors?**
```powershell
# Zorg dat de publish folder schrijfbaar is
icacls publish /grant Users:F /T
```

**CSS/JS niet geladen?**
```powershell
# Zorg dat npm build is gedaan VOOR dotnet publish
npm run build
dotnet publish -c Release
```

---

## 📝 Git Deployment

Als je naar een server wilt deployen:

```bash
# Op de server:
git clone https://github.com/rolandwardenaar/fullcalendar-aspnet-core
cd fullcalendar-aspnet-core/fullcalendar-core

# Install dependencies
npm install
npm run build

# Publish
dotnet publish -c Release -o /var/www/fullcalendar

# Run
cd /var/www/fullcalendar
ASPNETCORE_URLS="http://localhost:8086" ASPNETCORE_ENVIRONMENT="Production" ./fullcalendarcore
```

---

Succes met je deployment! 🚀
