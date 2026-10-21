# AgOpenGPS – tracksForTC közösségi build / community build

> [!WARNING]
> **NEM HIVATALOS közösségi build – UNOFFICIAL community build.** Nem az AgOpenGPS csapat adta ki, a hibákat itt jelezd. · Not released by the AgOpenGPS team, report problems on this fork.

**Letöltés / Download:** lent az *Assets* alatt / below under *Assets* – `AgOpenGPS_<verzió>_Setup.exe` (telepítő / installer) vagy / or `AgOpenGPS_<verzió>.zip`.

<details>
<summary><b>🇭🇺 Magyar leírás – kattints a kinyitáshoz</b></summary>

**Ez egy NEM HIVATALOS, közösségi build.** Nem az AgOpenGPS csapat adta ki. A hivatalos 6.8.6 kiadásra, a `develop` ág még ki nem adott javításaira és a `tracksForTC` ág saját fejlesztéseire épül. Használat előtt olvasd el az alábbi listát, és először szimulátorban vagy biztonságos körülmények között próbáld ki. A hibákat a fork oldalán jelezd, **ne** a hivatalos AgOpenGPS csapatnak.

## Újdonságok

### U-kanyar (forduló)
- **Spirál forduló** – az U / K stílus mellett új harmadik stílus. Mindig a jelölő nyila szerinti irányba fordul, és minden fordulónál változtatja a kihagyást: balra fordulva eggyel többet hagy ki, jobbra fordulva eggyel kevesebbet. A számláló a kihagyás-mezőben beállított értékről indul.
<img width="312" height="117" alt="image" src="https://github.com/user-attachments/assets/588261ff-86d1-45f1-8198-3b4c6a9814cd" />

- **Kihagyás gomb (bal alsó sarok)** – most mutatja a következő kihagyást és a fordulás irányát (nyíl). Spirálnál a számlálót, a váltakozó kihagyásnál az „N / N-1" párost (a következő kiemelve), a munkált nyomok módnál a kiválasztott kihagyást. A számok ugyanazok, mint a kihagyás-mezőben (0 = a szomszédos nyom). Spirálnál a gombra koppintva újraindul a spirál.
<img width="195" height="85" alt="image" src="https://github.com/user-attachments/assets/18d52f74-6136-42db-ad5c-cff986f65d8f" /><img width="222" height="91" alt="image" src="https://github.com/user-attachments/assets/3ea21da5-1e98-4732-aa5b-ce1c8f74460d" /><img width="195" height="87" alt="image" src="https://github.com/user-attachments/assets/064533d7-45dc-47dc-88f2-d39efddc9777" />

- **Automatikus kihagyás a már megművelt nyomokra** – a program most a festett (lefedett) terület alapján dönt: egy nyom „megművelt", ha a lefedési cél (legalább 50%, legfeljebb 95%) szerint le van fedve. Így akkor is működik, ha a tábla felét kézzel vezetted végig. Minden forduló megjelöli a nyomot, amit elhagy, a kézi fordulás gomb is. A lefedettségi térkép háttérszálon készül, az első ilyen fordulóvonal a táblán kicsit később jelenhet meg.
<img width="195" height="80" alt="image" src="https://github.com/user-attachments/assets/fd5e609b-8734-4800-a4f6-201fd4200732" />

- **Munkaeszköz-eltolás a fordulóvonalon** – az eltolt munkaeszköz kilóg a jármű egyik oldalán. A fordulóvonal mostantól a forduló irányától függően eltolódik (a jármű szélességét a „nyomtáv" beállításból veszi), így a kilógó oldal nem éri el a kerítést.
<img width="400" height="300" alt="image" src="https://github.com/user-attachments/assets/c4b94115-f7cf-473a-b15f-c1ed9ae057c0" />
<img width="437" height="322" alt="image" src="https://github.com/user-attachments/assets/f9823595-d97d-4f2e-adc2-cce13de4e008" />


### Egyéb
- **Irány szűrő (VTG)** – új csúszka az Irány beállításoknál (egyantennás csoport), 0–98%. Azt adja meg, hogy az előző irányból mennyi marad meg: 0% = az új értéket használja változtatás nélkül, 30% = 30% régi + 70% új. Körkörösen számol (359° és 1° átlaga ≈ 0°), és megállás után újraindul. **Sorvezetős** (lightbar) traktoroknál a hepehupás talajon ugráló irányt nyugtatja. Alapértéke 0, tehát semmi sem változik, amíg nem használod. Csak a VTG-alapú irányra hat.
<img width="1232" height="692" alt="image" src="https://github.com/user-attachments/assets/0381f00a-f12b-4088-9dea-6ec93666d2be" />

- **Régi profilok automatikus átalakítása** – ha nincs új típusú Jármű/Eszköz profil, de vannak régi (`Vehicles` mappa) profilok, az első indításkor mindet átalakítja, az utoljára használtat kiválasztja, a környezeti beállításokat egyszer átveszi, és értesítést mutat.
- **ISOBUS nyomok PGN** – új PGN a nyomok ISOBUS felé küldéséhez (lásd `docs/pgn-protocol.md`).
- **Windows telepítő** – első ízben van telepítő (`AgOpenGPS_<verzió>_Setup.exe`), magyar és angol nyelvvel. Felhasználónként telepít, választható az asztali ikon és a `Dokumentumok\AgOpenGPS` mappa (táblák, beállítások) telepítés előtti mentése.
- **Telepítő – automatikus indítás és tűzfal** – új opció: az AgOpenGPS a Windows-zal együtt indul (az AgIO-t ő maga indítja). A régebbi AgOpenGPS / AgIO / GPS_Out verziók automatikus indítását kikapcsolja, hogy ne induljon el egy régi példány is. Egy másik opció az AgIO-t és a GPS_Out-ot engedélyezi a Windows tűzfalon magán- és nyilvános hálózatokon is.
- **Kormányzás varázsló** – az „Alapértékek betöltése” most gombos (Button) kormányzás-engedélyezést, bekapcsolt áramérzékelős (Current Turn Sensor) automatikus megszakítást 40%-on és 15-ös proporcionális erősítést állít be. A varázsló összes felirata, gombja és üzenete lefordítható, magyarul már elérhető.
- **Magyar fordítások** – az Easy Drive szövegei, az új beállítások, a kormányzás varázsló és a telepítő magyarul is elérhető.

## Javítások
- **Webkamera ablak** – a tábla váltását már nem akadályozza, nyitva maradhat. Bezáráskor leállítja a kamerát; korábban a kamera és az AgOpenGPS folyamat is életben maradt a program bezárása után.
- **AgIO – Task Controller napló** – a TC kimenete már nem blokkolhatja az AgIO felületét (bővítve a hibakimenet olvasásával is; tesztekkel). *Köszönet: Marek, #1221.*
- **NTRIP kapcsolat** – az ügyfél már nem fogadja el csendben az elutasított kapcsolatot: ellenőrzi a caster válaszát, felismeri azt a casztert, amely „200 OK"-t küld de RTCM adat nélkül lezár, újraoldja a caster nevét (dinamikus DNS), és látható hibaüzenetet ad. *Köszönet: aortner, #1219.*
- **NTRIP – késleltetett adatfolyamú mountpointok (pl. Centipede NEAR / legközelebbi bázis)** – a figyelő (watchdog) most már akkor is nullázódik, amikor a caster elfogadja a kapcsolatot, így az a mountpoint, amely csak a GGA után kezd RTCM adatot küldeni, nem esik végtelen újracsatlakozási körbe. *Köszönet: aortner.*
- **NTRIP internet nélkül** – a caster nevének feloldása (DNS) a háttérben fut. Internet nélkül eddig az újracsatlakozási kísérletek néhány másodpercre megakaszthatták az AgIO-t, és vele az összes modulkommunikációt – erre az időre az automata kormányzás is leállt.
- **Hamis „Field Origin" távolság-figyelmeztetés** a tábla bezárása után. *Köszönet: Richard Klasens.*
- **ISOBUS nyomok PGN U-kanyarnál** – bekapcsolt U-kanyarnál a PGN már a forduló indításakor a következő nyom számát küldte, miközben a munkaeszköz még az aktuális nyomon dolgozott. Most a forduló végéig az aktuális nyomot küldi, és csak utána vált a következőre. Megszakított fordulónál az eredeti nyom marad.
- **Régi (v5) táblák AB- és görbe vonalai** – a v5 a vonalakat az `ABLines.txt` és `CurveLines.txt` fájlban tárolta. A 6.3/6.4 még átalakította ezeket, a későbbi verziók azonban nem, így egy v5-ös tábla vonalak nélkül nyílt meg. Indításkor (a profilok automatikus átalakítása után) a program most minden ilyen táblát átalakít `TrackLines.txt`-re, és a tábla megnyitásakor is. A régi fájlok megmaradnak.
- **Forduló beállításai munkaeszközönként** – a 6.9-ben a forduló beállításai minden profilban közösek lettek, így ha az egyik profilban átírtad például a táblaszéltől való távolságot, az összes többiben is megváltozott. Most újra profilonként tárolódnak, a munkaeszköz-profilban: táblaszéltől való távolság, kihagyás, fordulási sugár, hosszabbítás, stílus, simítás és kompenzáció. Munkaeszköz váltásakor az új értékek azonnal érvényesek, a fordulóvonal újraépül.
- **U-forduló be- és kivezető egyenese** – az U alakú (széles) fordulónál a forduló eleji és végi egyenes szakasz a fordulási sugár kb. 1%-ával (5 m-es sugárnál 5 cm, 7 m-esnél 7 cm) a vezetővonal mellett, a forduló belső oldala felé futott. Ezért a forduló indításakor az XTE hirtelen ennyit ugrott, és a traktor belerántott a kormányba; a forduló végén, a vonalra visszaálláskor ugyanez történt. Most mindkét szakasz pontosan a vonalon fut. Az omega fordulót nem érintette.
- **Telepítő – biztonsági mentés** – a Dokumentumok\AgOpenGPS mappa mentése sok táblánál sokáig tartott, és közben a telepítő lefagyottnak tűnt. Most sokszor gyorsabb (egy 300 fájlos mappán 13 másodperc helyett 1), és két külön választható része van: a **beállítások** (profilok, AgIO – gyors) és a **táblák**. Mentés közben a telepítő kiírja, mit ment. A két mentés külön zip fájlba kerül a Dokumentumok mappába (`AgOpenGPS_backup_settings_<dátum>.zip`, `AgOpenGPS_backup_fields_<dátum>.zip`). Egy futó AgOpenGPS / AgIO által nyitva tartott fájl már nem akasztja meg a mentést; ha a mentés mégsem sikerül, a telepítő megkérdezi, folytassa-e.

## Frissítés előtt olvasd el
- **Munkaeszköz-eltolás:** ha eddig kézzel adtad hozzá az eltolást a fordulótávolsághoz, hogy a kasza ne vigye ki a kerítést most ez kétszer számít. Állítsd újra a fordulótávolságot.
- **Régi profilok:** az automatikus konvertálás automatikusan lefut, ha még egyetlen új típusú Jármű/Eszköz profil sincs. A régi fájlokat nem törli.
- **Irány szűrő:** magas érték késleltetést okoz (kb. 1/(1−szűrő) mérésnyit). Autosteerhez nem javasolt. Sorvezetőhöz igen.
- **Kormányzás varázsló:** az „Alapértékek betöltése” új értékeket ír (Button, áramérzékelő 40%, P erősítés 15). Ha a saját beállításaidat használod, ne nyomd meg.
- **Telepítő:** a „rögzítés a tálcán" opció csak Windows 7/8-on működik; Windows 10/11-en jobb gombbal rögzítsd kézzel.
- **Telepítő – tűzfal:** a tűzfalszabályokhoz rendszergazdai jóváhagyás kell, ezért a telepítő egyszer rákérdez (UAC). Ha elutasítod, a telepítés folytatódik, és leírja, hogyan engedélyezd kézzel.
- **Telepítő – automatikus indítás:** a régi verziók indítási bejegyzéseit csak kikapcsolja, nem törli; a Feladatkezelő > Indítási alkalmazások alatt visszakapcsolhatók.
- **Forduló beállításai:** frissítés után minden munkaeszköz a mostani (közös) forduló-értékekkel indul, semmi sem változik. Utána munkaeszközönként külön állíthatók.

## Ismert korlátok
- A spirál forduló, a lefedettség-alapú automatikus kihagyás és az eltolás-alapú fordulóvonal **még nem járt éles terepen**, csak szimulátorban lett kipróbálva. Kérjük, jelezd a tapasztalatokat.
- A „megművelt nyomok" jelölései csak a memóriában élnek; újraindítás után a festett terület alapján épülnek újra.
- Az Irány beállításoknál nincs külön VTG választógomb, ehhez a profilban kell VTG-re állítani a forrást.

## Köszönet
Marek (#1221), aortner (#1219), Richard Klasens, valamint a Weblate fordítók és az AgOpenGPS közösség.

</details>

<details>
<summary><b>🇬🇧 English release notes – click to expand</b></summary>

**This is an UNOFFICIAL community build.** It is not released by the AgOpenGPS team. It is based on the official 6.8.6 release, the not-yet-released fixes on the `develop` branch, and the own work on the `tracksForTC` branch. Read the lists below before using it, and try it in the simulator or under safe conditions first. Report problems on this fork, **not** to the official AgOpenGPS team.

## What's new

### U-turn
- **Spiral turn** – a third style next to U and K. It always turns the way the marker arrow points and changes the skip on every turn: a left turn skips one more, a right turn one less. The counter starts at the value set in the skip box.
- **Skip button (bottom left)** – now shows the skip of the next turn and the turn direction (arrow). For the spiral it shows the counter, for alternate skip the "N / N-1" pair (the upcoming one highlighted), and for worked tracks the skip it chose. The numbers are the same as in the skip box (0 = the next track). Tapping the icon in spiral mode starts the spiral over.
- **Auto-skip of worked tracks** – it now decides by the painted (covered) area: a track counts as worked when it is covered up to the coverage target (at least 50%, at most 95%). This also works when you drove half the field by hand. Every turn marks the track it leaves, including the manual turn button. The coverage grid is built on a background thread, so the first turn line in a field can appear a little later.
- **Implement offset in the turn line** – an offset implement sticks out on one side of the vehicle. The turn line now moves depending on the turn direction (vehicle width is taken from the track width setting), so the side that sticks out stays away from the fence.

### Other
- **Heading filter (VTG)** – a new slider in the Heading settings (single antenna group), 0–98%. It sets how much of the previous heading is kept: 0% uses the new value as it is, 30% is 30% old + 70% new. It works around the circle (the average of 359° and 1° is about 0°) and starts fresh after standing still. It calms the jumping heading of lightbar tractors on bumpy fields. Default is 0, so nothing changes until you use it. It only affects the VTG based heading.
- **Automatic conversion of old profiles** – if there is no new style Vehicle/Tool profile but there are old ones (`Vehicles` folder), all of them are converted on first start, the last used one is selected, the environment settings are taken over once, and a notice is shown.
- **ISOBUS Tracks PGN** – a new PGN for sending tracks to ISOBUS (see `docs/pgn-protocol.md`).
- **Windows installer** – first time there is an installer (`AgOpenGPS_<version>_Setup.exe`), in English and Hungarian. It installs per user, with optional desktop icon and an optional backup of `Documents\AgOpenGPS` (fields, settings) before installing.
- **Installer – auto-start and firewall** – a new option starts AgOpenGPS with Windows (it starts AgIO itself). Startup entries of older AgOpenGPS / AgIO / GPS_Out versions are disabled, so an old copy does not start as well. Another option allows AgIO and GPS_Out through Windows Firewall on both private and public networks.
- **Steer Wizard** – "Load Defaults" now selects the Button steer enable mode, turns on the Current Turn Sensor automatic cancelling at 40% and starts the proportional gain at 15. Every label, button and message of the wizard can now be translated; Hungarian is included.
- **Hungarian translations** – the Easy Drive texts, the new settings, the Steer Wizard and the installer.

## Fixes
- **Webcam window** – it no longer blocks changing fields and can stay open. It stops the camera when it closes; before, the camera and the AgOpenGPS process stayed alive after closing the program.
- **AgIO – Task Controller log** – TC output can no longer block the AgIO UI (also reads the error output now; with tests). *Thanks: Marek, #1221.*
- **NTRIP connection** – the client no longer silently accepts a rejected connection: it checks the caster's response, detects casters that answer "200 OK" but close without RTCM data, re-resolves the caster name (dynamic DNS) and shows a visible error message. *Thanks: aortner, #1219.*
- **NTRIP – delayed-stream mount points (e.g. Centipede NEAR / nearest base)** – the watchdog is now reset when the caster accepts the connection, so a mount point that only starts sending RTCM after it gets a GGA no longer falls into an endless reconnect loop. *Thanks: aortner.*
- **NTRIP without internet** – the caster name (DNS) is now looked up in the background. Without internet, reconnect attempts could freeze AgIO for a few seconds, and with it all module communication – autosteer stopped for that time.
- **False "Field Origin" distance warning** after closing a field. *Thanks: Richard Klasens.*
- **ISOBUS Tracks PGN during U-turns** – with U-turn enabled, the PGN switched to the next track number as soon as the turn was triggered, while the implement was still working the current pass. It now keeps sending the current pass until the turn completes, then switches to the next one. A cancelled turn keeps the original pass.
- **AB and curve lines of old (v5) fields** – v5 stored lines in `ABLines.txt` and `CurveLines.txt`. 6.3/6.4 still converted them, but later versions did not, so a v5 field opened without any lines. On startup (after the automatic profile conversion) every such field is now converted to `TrackLines.txt`, and also when the field is opened. The old files are kept.
- **U-turn settings per implement** – 6.9 made the U-turn settings shared by all profiles, so changing e.g. the distance from the boundary in one profile changed it in every other one. They are stored per profile again, in the tool profile: distance from boundary, skip width, turn radius, extension length, style, smoothing and compensation. Switching the tool applies its values right away and rebuilds the turn line.
- **U-turn lead-in and lead-out lines** – with the U-shaped (wide) turn, the straight parts at the start and end of the turn ran about 1% of the turn radius beside the guidance lines (5 cm with a 5 m radius, 7 cm with 7 m), toward the inside of the turn. When the turn started, the XTE jumped by that much and the tractor jerked the steering to correct; the same happened when it went back onto the line at the end. Both parts now lie exactly on the lines. The omega turn was not affected.
- **Installer – backup** – backing up the Documents\AgOpenGPS folder took a long time with many fields, and the installer looked frozen meanwhile. It is now many times faster (1 second instead of 13 on a 300-file folder) and split into two options: **settings** (profiles, AgIO – quick) and **fields**. The installer shows what it is backing up. The two backups are separate zips in Documents (`AgOpenGPS_backup_settings_<date>.zip`, `AgOpenGPS_backup_fields_<date>.zip`). A file held open by a running AgOpenGPS / AgIO no longer stops the backup; if the backup still fails, the installer asks whether to continue.

## Read before upgrading
- **Implement offset:** if you used to add the offset to the U-turn distance by hand, it now counts twice. Reduce the turn distance.
- **Old profiles:** the automatic conversion only runs when there is not a single new style Vehicle/Tool profile yet. It does not delete the old files.
- **Heading filter:** a high value adds lag (about 1/(1−filter) fixes). Not recommended for autosteer.
- **Steer Wizard:** "Load Defaults" writes new values (Button, current sensor 40%, P gain 15). If you use your own settings, do not press it.
- **Installer:** the "pin to taskbar" option only works on Windows 7/8; on Windows 10/11 pin it by hand with a right click.
- **Installer – firewall:** the firewall rules need administrator approval, so the installer asks once (UAC). If you decline, the install continues and explains how to allow them by hand.
- **Installer – auto-start:** startup entries of old versions are only disabled, not deleted; they can be switched back on in Task Manager > Startup apps.
- **U-turn settings:** after upgrading every tool starts with the current (shared) U-turn values, so nothing changes. After that they can be set per tool.

## Known limitations
- The spiral turn, the coverage based auto-skip and the offset based turn line have **not been through real field trials**, only the simulator. Please report your experience.
- The "worked tracks" marks live in memory only; after a restart they are rebuilt from the painted area.
- There is no VTG radio button in the Heading settings; to use VTG the source has to be set in the profile.

## Credits
Marek (#1221), aortner (#1219), Richard Klasens, plus the Weblate translators and the AgOpenGPS community.

</details>
