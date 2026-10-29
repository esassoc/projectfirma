# PF-2844: GeoServer 3.0.1 setup steps

> Never put secret values here. Reference Secret Server entries by name.

Target: GeoServer **3.0.1**, Temurin **JDK 21.0.12.101**, Jetty 12 (bundled), Windows service `GeoServerProjectFirma` (Jetty 127.0.0.1:8258), behind IIS. Public URLs are unchanged:
- local: `https://localhost-mapserver.projectfirma.com/geoserver/`
- QA: `https://qa-mapserver.projectfirma.com/geoserver/`
- prod: `https://mapserver.projectfirma.com/geoserver/`

Installers: `\\sitka\shares\Installs\GeoServer\` and `\\sitka\shares\Installs\Java\AdoptOpenJDK\Java21\`.

---

## Part A: Build the install zip (one time, by the PF-2844 developer)

Done once. The result is committed to SVN as `ProjectFirma/Geoserver/GeoServer.zip` so nobody else has to repeat these steps.

### A1. Install JDK 21.0.12 ✅ (2026-09-30)
1. Run `\\sitka\shares\Installs\Java\AdoptOpenJDK\Java21\OpenJDK21U-jdk_x64_windows_hotspot_21.0.12.1_1.msi`.
2. On **Custom Setup**, click the ❌ next to **Set JAVA_HOME variable** and choose **Will be installed on local hard drive**. It's off by default and easy to miss.
3. Check: the JDK installs to `C:\Program Files\Eclipse Adoptium\jdk-21.0.12.101-hotspot`, `JAVA_HOME` (machine) points there, and `java -version` reports `21.0.12.1`.

The JDK installs next to any older JDK, so nothing that uses the old one breaks.

### A2. Install GeoServer 3.0.1 ✅ (2026-09-30)
Run `\\sitka\shares\Installs\GeoServer\3.0.1\GeoServer-3.0.1-winsetup.exe`:

| Screen | Value |
|---|---|
| Install location | `C:\Program Files\GeoServer` (default) |
| JRE/JDK | `C:\Program Files\Eclipse Adoptium\jdk-21.0.12.101-hotspot` |
| Data directory | default (repointed later) |
| Admin user/password | admin / geoserver (replaced later by `geomaster`) |
| Port | 8080 |
| Execution | Install as a service |

**Known hang: "Writing environment variables..." stalls.** The installer broadcasts the environment change to every window and waits for each one to answer. Leftover background **Visual Studio (`devenv.exe`)** processes don't answer, even after VS looks closed. Fix: Task Manager → Details → end every `devenv.exe` (save your work first), and the installer carries on within seconds. On 2026-09-30 six leftover devenv processes caused it. Other apps (VS Code, Slack, Chrome, Phone Link) can occasionally do the same.

Result: service **`GeoServer`** (display name "GeoServer 3.0.1"), Automatic, Running. It answers on `http://localhost:8080/geoserver` but listens on **all interfaces**; a later step binds it to 127.0.0.1. The installer also sets the machine variable `GEOSERVER_DATA_DIR=C:\ProgramData\GeoServer`; a later step overrides it with `-DGEOSERVER_DATA_DIR` in jsl64.ini.

### A3. SQL Server support ✅ (2026-09-30)
Copy into `C:\Program Files\GeoServer\webapps\geoserver\WEB-INF\lib`:
- `\\sitka\shares\Installs\GeoServer\geoserver-3.0.1-sqlserver-plugin\gs-sqlserver-3.0.1.jar`
- `\\sitka\shares\Installs\GeoServer\geoserver-3.0.1-sqlserver-plugin\gt-jdbc-sqlserver-35.1.jar`
- `\\sitka\shares\Installs\GeoServer\sqljdbc_13.4\enu\jars\mssql-jdbc-13.4.0.jre11.jar`. Use this jar, **not** the plugin zip's `mssql-jdbc-13.2.1`, because the auth DLL must match the jar version exactly. Keep only one `mssql-jdbc*.jar` in `lib`.

Create `C:\Program Files\GeoServer\native\` and copy `\\sitka\shares\Installs\GeoServer\sqljdbc_13.4\enu\auth\x64\mssql-jdbc_auth-13.4.0.x64.dll` into it.
- This differs from CBF/MR, which put the DLL in the JDK's `bin`. Keeping it in the GeoServer folder means the zip carries it and a JDK upgrade doesn't break integrated security. jsl64.ini points Java at it with `-Djava.library.path=native`.

**Control-flow extension** (request throttling; it was active under Docker, and `data_dir\controlflow.properties` holds the limits). It is not on the installs share. Download the official `geoserver-3.0.1-control-flow-plugin.zip` from https://sourceforge.net/projects/geoserver/files/GeoServer/3.0.1/extensions/ and copy `gs-control-flow-3.0.1.jar` into `WEB-INF\lib`.
- **Verify the download:** the zip's jar SHA-1 does **not** equal the Maven checksum, because the release and Maven builds stamp different MANIFEST.MF times. Instead, compare it entry by entry with `https://repo.osgeo.org/repository/release/org/geoserver/extension/gs-control-flow/3.0.1/gs-control-flow-3.0.1.jar` (whose SHA-1 matches its published `.sha1`). On 2026-10-01 all 38 classes were identical and only MANIFEST.MF differed.
- Check: Server Status → Modules lists `gs-control-flow` with green checks.
- Not installed (PF never used them): WPS, CSW, printing. Their data_dir config was deleted.

### A4. Service wrapper `wrapper\jsl64.ini` ✅ (2026-09-30)
Back up the original as `jsl64.ini.orig`, then:
- Replace every `%GEOSERVER_HOME%` with the literal `C:\Program Files\GeoServer`. Boxes set up from the zip won't have the `GEOSERVER_HOME` variable the installer creates.
- Keep `jrepath=%JAVA_HOME%`, which is set when JDK 21.0.12 is installed.
- Keep `account=NT Authority\NetworkService`. The real logon account is set per box in services.msc (Parts B/C).
- In `cmdline=`:
  - Add `-Djetty.http.host=127.0.0.1` before `-Djetty.http.port=8080`. Jetty then listens on loopback only, and only IIS can reach it.
  - Add `-DALLOW_ENV_PARAMETRIZATION=true -Djava.library.path=native` after `-DSTOP.KEY=geoserver`.
  - *(added later: `-DGEOSERVER_DATA_DIR=...`)*

Restart the service (`Restart-Service GeoServer`). Check: `http://127.0.0.1:8080/geoserver/web/` returns 200, and port 8080 listens on `127.0.0.1` only (`Get-NetTCPConnection -LocalPort 8080 -State Listen`).

### A5. Jetty `start.ini` ✅ (2026-09-30)
Back up as `start.ini.orig`, then change `jetty.httpConfig.requestHeaderSize=8192` to `65536`. This matches the Docker setup and prevents 400s on WMS requests with many layers or long CQL filters. Restart the service.

No other Jetty edits are needed in stock 3.0.1:
- No SSL modules are active (`--module=http` only).
- `etc\webdefault-ee11.xml` has no `transport-guarantee`, so the CONFIDENTIAL redirect loop CBF hit behind IIS doesn't apply.

### A6. CORS ✅ (2026-10-01): no install-folder edit
Stock 3.0.1 sends no `Access-Control-Allow-Origin`. Its `web.xml` only has a commented Tomcat sample; Jetty 12 dropped web.xml CORS.

Use GeoServer's built-in **Settings → Global → CORS Settings**: Enabled, Allowed Origin Patterns `*`, and the pre-filled default methods and headers. It is saved in the **data_dir `global.xml`** (in SVN, so every deploy carries it), as this metadata entry:
```xml
<entry>
  <string>corsConfigurationSettings</string>
  <org.geoserver.security.cors.CORSConfiguration>
    <enabled>true</enabled>
    <allowedOriginPatterns>*</allowedOriginPatterns>
    <maxAge>3600</maxAge>
    <supportsCredentials>false</supportsCredentials>
  </org.geoserver.security.cors.CORSConfiguration>
</entry>
```
Default methods and headers are implicit (not written). **A restart is required** after changing it.

Verified: GET with an `Origin` header returns `Access-Control-Allow-Origin: *`; OPTIONS preflight returns `Allow-Methods: HEAD, POST, GET, OPTIONS`.

**Don't** also enable a CORS filter in `web.xml` (CBF/MR used Jetty's `CrossOriginFilter`), or browsers get duplicate headers. This differs from CBF/MR on purpose: upgrades replace `web.xml`, which is how CBF's CORS broke after 2.28.5.

### A7. Retire the Docker GeoServer and move the data_dir ✅ (2026-10-01)
Confirmed first that these are the live files: git has the app and DB (Shannon, through 2026-09-16); SVN `trunk/ProjectFirma` has the data_dir and build (Shannon, 2026-07/08). No other copy exists.
1. Stop and remove the container: in `C:\svn\sitkatech\trunk\ProjectFirma\GeoServerDocker`, run `docker-compose --project-name projectfirma_geoserver_project --file docker-compose.yml down`. Local maps stop until IIS fronts the new GeoServer.
2. Delete the runtime `data_dir\geoserver-environment.properties` (svn-ignored; the Docker entry point generated it and it holds the SQL password; **don't open it**).
3. In `C:\svn\sitkatech\trunk\ProjectFirma`, run `svn mv GeoServerDocker Geoserver`. The ignored runtime files move with it.
4. Delete the Docker runtime leftovers `data_dir\security\` and `data_dir\.updatepassword.lock`, so 3.0.1 creates a fresh `security\` on first start. The leftover keystores in `sitka-geoserver-config\ssl\` go away with that folder in A8.

### A8. Remove Docker-only files from SVN ✅ (2026-10-01)
In `C:\svn\sitkatech\trunk\ProjectFirma\Geoserver`:
- `svn rm` `docker-compose.yml`, `docker-compose.qa.yml`, `docker-compose.prod.yml`, `data_dir\geoserver-environment.properties.template` (the Docker template), and `data_dir\s3.properties`.
- `svn rm --force data_dir\sitka-geoserver-config`. That removes the entry-point scripts, the Tomcat CORS and redirect patches, the local cert/key, and the leftover keystores. CORS now lives in `global.xml` and TLS in IIS.
- data_dir `svn:ignore` = `security temp tmp .updatepassword.lock legendsamples`. `geoserver-environment.properties` comes off the list because the local one is now committed and holds no secret.
- README.txt: rewritten as a pointer to these steps (done in A16).

### A9. data_dir config for native 3.0.1 ✅ (2026-10-01)
- **All 12 `workspaces\*\GeoServerSqlDataSource\datastore.xml`:**
  - `Integrated Security`=true; `user`=`unused`; **no `passwd` entry at all**. On start GeoServer encrypts any `passwd` into a `crypt1:` value keyed to that box's `security\` folder, which other boxes can't decrypt (CBF r217763). It did this to `passwd=unused` here on first start; with no entry it leaves the files alone (verified by file hash across a restart). Delete any `passwd`/`dateModified` that shows up in `svn diff` before committing.
  - `validate connections`=true and `Test while idle`=true; add `Evictor run periodicity`=300, `Max connection idle time`=300 and `disableOnConnFailure`=false (CBF-8195).
  - Keep `${datastore-host}` and `${datastore-database}`.
- **`global.xml` `<settings>`:** add `<proxyBaseUrl>${proxy-base-url}</proxyBaseUrl>` after `onlineResource`, the CORS `<metadata>` block (A6) after `verboseExceptions`, and `<useHeadersProxyURL>false</useHeadersProxyURL>` last (ARR doesn't send X-Forwarded-Proto).
- **`global.xml` `<contact>`:** replaced the out-of-date Sitka / Ray Lee block with ESA (Environmental Science Associates, San Francisco, CA, https://www.esassoc.com), matching MR and CBF. It shows in GetCapabilities.
- **`logging.xml`:** `<level>PRODUCTION_LOGGING</level>` (3.0 profile names have no `.properties`); `<stdOutLogging>false</stdOutLogging>`.
- **Env files (`svn add`), keys `datastore-host`, `datastore-database`, `proxy-base-url`:**

| File | host | proxy-base-url |
|---|---|---|
| `geoserver-environment.properties` (local, committed) | localhost | https://localhost-mapserver.projectfirma.com/geoserver |
| `.template.qa` | kettle.sitka.local | https://qa-mapserver.projectfirma.com/geoserver |
| `.template.prod` | deschutes.sitka.local | https://mapserver.projectfirma.com/geoserver |

The database is `ProjectFirma` everywhere. There are no secrets in these files.

### A10. Point the service at the data_dir and run as the domain account ✅ (2026-10-01)
1. `icacls "C:\Program Files\GeoServer" /grant "SITKA\FirmaGeoLocal:(OI)(CI)M" /T`, and the same on the data_dir. GeoServer writes logs, temp files, the tile cache and `security\`.
2. jsl64.ini `cmdline=`: add `-DGEOSERVER_DATA_DIR=C:/svn/sitkatech/trunk/ProjectFirma/Geoserver/data_dir` after `-Djava.library.path=native`. QA/prod use `C:/sitka/ProjectFirma/GeoServer/data_dir` instead.
3. services.msc → GeoServer → Log On → `SITKA\FirmaGeoLocal` + the Secret Server password → Restart. If you get a logon failure, ask to have the account unlocked.
4. Verify, as admin/geoserver (a fresh `security\` uses the public default until `geomaster` is set up):
   - `http://127.0.0.1:8080/geoserver/rest/workspaces` → 12 workspaces; `/rest/layers` → 66.
   - WMS GetCapabilities: 65 queryable layers; `OnlineResource` = `https://localhost-mapserver.projectfirma.com/geoserver/...`, which proves `${proxy-base-url}` resolved.
   - WFS GetFeature `TCSProjectTracker:ProjectSimpleLocations` with `count=1` returns GeoJSON in lon/lat order, which proves integrated security works.
   - Note: the `ProjectFirma` workspace only publishes `Watershed`; the project layers live in the tenant workspaces.

### A11. IIS reverse proxy (local) ✅ (2026-10-01)
Prerequisites:
- **IIS URL Rewrite 2** (was already installed here).
- **ARR 3.0** from https://www.microsoft.com/en-us/download/details.aspx?id=47333 (not on the installs share).
- **Cert:** import `C:\svn\sitkatech\trunk\ProjectFirma\Build\SslCertificates\localhost-mapserver.projectfirma.com.KeyPassword=….pfx` into Local Machine → Personal (the key password is in the filename). The `*.localhost.projectfirma.com` wildcard does **not** cover `localhost-mapserver.projectfirma.com`.

Steps (elevated; `appcmd` = `%windir%\System32\inetsrv\appcmd.exe`). The file below is committed at SVN `ProjectFirma/Geoserver/iis/web.config`. It has an HTTP→HTTPS redirect, a reverse proxy to `http://127.0.0.1:8080/{R:1}` (8258 after the A17 rename), and `maxUrl`/`maxQueryString` 65536.
1. Enable the ARR proxy server-wide: `appcmd set config -section:system.webServer/proxy /enabled:"True" /timeout:"00:02:00" /commit:apphost`.
2. Create `C:\inetpub\localhost-mapserver.projectfirma.com` and copy `Geoserver\iis\web.config` into it.
3. `appcmd add apppool /name:localhost-mapserver.projectfirma.com /managedRuntimeVersion:""` (No Managed Code).
4. `appcmd add site /name:localhost-mapserver.projectfirma.com /physicalPath:C:\inetpub\localhost-mapserver.projectfirma.com /bindings:"http/127.0.0.180:80:localhost-mapserver.projectfirma.com"`
5. `appcmd set site /site.name:localhost-mapserver.projectfirma.com "/+bindings.[protocol='https',bindingInformation='127.0.0.180:443:localhost-mapserver.projectfirma.com',sslFlags='1']"` (SNI).
6. `appcmd set app "localhost-mapserver.projectfirma.com/" /applicationPool:localhost-mapserver.projectfirma.com`
7. Attach the cert: `netsh http add sslcert hostnameport=localhost-mapserver.projectfirma.com:443 certhash=<thumbprint of CN=localhost-mapserver.projectfirma.com> appid={4dc3e181-e14b-4a21-b022-59fc669b0914} certstorename=MY`.
8. `netsh http add iplisten ipaddress=127.0.0.180`, then `iisreset`. **No reboot needed** (verified 2026-10-01; CBF's notes say reboot). Only if `.180` still doesn't listen afterwards, reboot; don't use `net stop http`, which hangs on SSRS. Devrigs with an **empty** iplisten list skip this step.

Verified through IIS:
- `http://` → 301 to `https://`.
- `https://localhost-mapserver.projectfirma.com/geoserver/web/` → 200.
- Capabilities advertise `https://localhost-mapserver.projectfirma.com/geoserver/wms` with 65 queryable layers.
- CORS GET and preflight return `Access-Control-Allow-Origin: *`.
- Git Bash `curl` doesn't trust the Sitka local root CA; test with PowerShell or a browser.

`localhost-mapserver.projectfirma.com` resolves to 127.0.0.180 through public DNS, so no hosts entry is needed.

**Browser check (passed 2026-10-01):** on a tenant's Project Map (e.g. `https://tcsprojecttracker.localhost.projectfirma.com`), with F12 → Console open:
- Project and area layers draw.
- **Left-click** on the map shows the info popup. That click sends a cross-origin WMS GetFeatureInfo per visible layer, so it exercises CORS.
- No CORS errors in the console. An `aria-hidden` warning about `ProjectLocationsMap` is pre-existing and unrelated.

### A12. Replace the default admin login ✅ (2026-10-01)
A fresh `security\` accepts the public `admin / geoserver` until this is done.
1. Log in at `https://<mapserver host>/geoserver/web/` as admin / geoserver.
2. Security → Users, Groups, Roles → Users/Groups → Add new user: `geomaster`, role **ADMIN**. Password:
   - **Devrigs:** your choice. Local `security\` is never committed or shared, and there's no Secret Server entry for local. Don't reuse the QA/prod password.
   - **QA / Prod:** Secret Server "GeoServer Admin ProjectFirma QA" / the Prod equivalent.
3. Log out, log back in as geomaster, then remove the `admin` user.
4. If the home page warns about digest password encoding: User Group Services → default → Password encryption = Digest → Save.

Check: REST with admin/geoserver returns 401.

**Forgot the local geomaster password?** (devrigs only; QA/prod use Secret Server.) Stop `GeoServerProjectFirma`, delete `C:\svn\sitkatech\trunk\ProjectFirma\Geoserver\data_dir\security` (svn-ignored, per box), and start the service. GeoServer recreates `admin / geoserver`; then redo A12. Nothing else depends on `security\`, because the datastores hold no password.
### A13. Build / deploy scripts (SVN) ✅ (2026-10-01)
- New `ProjectFirma\Build\geoserver.build`: MR's version with a fixed header, PF paths and service `GeoServer` (renamed in A17: `GeoServerProjectFirma`). Robocopy `/mir` excludes `gwc\tmp temp tmp security logs legendsamples` and `geoserver-environment.properties`, each listed for both source **and** destination (CBF r217803). `svn rm` `geoserverDocker.build`. `make.cmd geoserver` resolves to the project-local file before the old shared `Build\geoserver.build`.
- `local.build` (next to each env's `geoserver-url`):
  - local: `geoservice-name=GeoServer` (renamed in A17: `GeoServerProjectFirma`, all envs).
  - qa/prod: `geoservice-name=GeoServer`, `geoserver-webserver=${qa-web-server}` / `${prod-web-server}` (wallowa / salmonberry), `geoserver-unc-deploy-dir=c$\sitka\ProjectFirma\GeoServer`.
- `release.cmd` and `Commands\UpdateRestoreBuildAll*.cmd` (3 files): `make.cmd geoserverDocker|geoserverdocker` → `make.cmd geoserver`.
- Verified locally: `make.cmd geoserver local release` (from the Build folder, admin shell) restarts the service → **Success**, and the site stays up.

### A14. App / DB changes (git) ✅ (2026-10-01)
- `Database\Baseline\config_roles.sql` (runs on DB restore):
  - The GeoServer Windows account (`${db-geoserver-user}` = `SITKA\FirmaGeo{Local,Qa,Prod}`) gets **db_datareader**; everyone else keeps db_owner. MR made the same change.
  - The SQL login (`${db-geoserver-docker-user}`, type S) is **removed from the create list**. The existing cleanup block now drops it on restore, and the day-one `create-db` failure ("FirmaGeoLocal is not a valid Windows NT name") is gone.
  - Syntax-checked with `SET PARSEONLY ON`. Takes effect on the next restore.
  - **db_datareader verified (2026-10-01):** locally ran `alter role db_owner drop member [SITKA\FirmaGeoLocal]; alter role db_datareader add member [SITKA\FirmaGeoLocal];` and restarted GeoServer. WFS GeoJSON, WMS GetMap, GetFeatureInfo, GetLegendGraphic and WFS 1.0 sortBy all return 200 with data, and the log shows no permission errors. Other devs and QA get this on their next restore, or can run the two `alter role` lines by hand.
- `Source\ProjectFirmaModels\UnitTestCommon\TestGeoserverConfig.cs`: data_dir path `GeoServerDocker` → `Geoserver`.

### A15. Retire the local SQL login ✅ (2026-10-01)
The password leaked to a research agent on 2026-09-30, so this step is required.
1. Check nothing else uses it: only the `ProjectFirma` DB had the user, and no sessions were open.
2. `use ProjectFirma; drop user [FirmaGeoLocal]; drop login [FirmaGeoLocal];` (SQL login only; keep the Windows login `SITKA\FirmaGeoLocal`).
3. Delete `C:\Sitka\ProjectFirma\Geoserver\GeoserverSqlServerPassword.txt` and `GeoserverAdminPassword.txt` (don't open them).
4. Check: WFS GetFeature still returns data through integrated security.

### A16. Record GeoServer's first-start changes, README, zip ✅ (2026-10-01)
- On first start, 3.0.1 migrated the logging profiles: `svn rm` the seven `data_dir\logs\*_LOGGING.properties`, `svn add` the seven `*_LOGGING.xml`, and delete the `*.properties.bak` files. It also created `data_dir\wmts.xml` (`svn add`). No other unplanned rewrites. `logs\` is excluded from deploys, so QA/prod generate their own profiles on first start.
- `Geoserver\README.txt` rewritten for the native setup. It points to this runbook and gives the datastore rules.
- **`Geoserver\GeoServer.zip`** (117 MB), the configured `C:\Program Files\GeoServer`, built from a copy:
  - robocopy to staging, excluding `work\` and `logs\` contents, Jetty's demo `etc\keystore` and `etc\realm.properties`, `GeoServer-uninstall.exe` and `*.orig`; recreate empty `work\` and `logs\`.
  - `[IO.Compression.ZipFile]::CreateFromDirectory(<stage>\GeoServer, GeoServer.zip, Optimal, includeBaseDirectory=$true)`, so the zip's top folder is `GeoServer` (renamed in A17: `GeoServerProjectFirma`; the install script expects that name).
  - Never run a test GeoServer out of the staging folder. It writes session files with paths over 260 characters, which break the zip and normal deletes; clear them with `robocopy <empty folder> <stage> /MIR`.
  - Rebuilt 2026-10-01 with the control-flow jar: 117 MB, 394 entries.
  - Contains A3–A5 and the local `-DGEOSERVER_DATA_DIR`. `svn add` (binary).

---

### A17. Rename to the Sitka convention (2026-10-01)
PF's QA/prod servers (wallowa/salmonberry) run no other GeoServer, but **developer machines can also run CBFish (`GeoServerGemini`, Jetty 8254, stop port 8079) and MR (`GeoServerMonitoringResources`, 8256/8255)**. The installer defaults (service `GeoServer`, port 8080, stop port **8079**) collide with CBFish's stop port, and the generic name collides with anyone using the installer's default. So, the same everywhere so one zip works:
- Service and install folder: **`GeoServerProjectFirma`** / `C:\Program Files\GeoServerProjectFirma`.
- jsl64.ini: `appname`/`servicename`/`displayname` = `GeoServerProjectFirma`; `systemout`/`systemerr`/`wrkdir` paths updated; `-Djetty.http.port=8258` (the stop port/key originally set to 8257/`projectfirma` were later removed; see A18). `start.ini`: `jetty.http.port=8258` (change it in both files).
- Register with the **absolute** ini path: `GeoServer.exe -install "C:\Program Files\GeoServerProjectFirma\wrapper\jsl64.ini"`. Services start in `C:\Windows\System32`, so a relative `-install jsl64.ini` (as in CBF's runbook) registers `-ini "jsl64.ini"`, which may not be found.
- `Geoserver\iis\web.config` proxies to `http://127.0.0.1:8258`. `local.build` `geoservice-name` = `GeoServerProjectFirma` (all envs).
- Port 8258 (and 8257) aren't used by any other Sitka project (searched SVN trunk).
- Steps on this devrig: stop and `GeoServer.exe -remove` the old service; move the folder; edit the two files; register; services.msc Log On; copy web.config to the IIS site; rebuild the zip.

### A18. Security hardening (2026-10-01, from the PF-2844 security review)
Each item makes things more secure without changing behavior. CBF/MR don't do these; they're deliberate improvements.
- `start.ini`: `jetty.httpConfig.sendServerVersion=false` (no Jetty version banner) and `jetty.http.host=127.0.0.1`, so Jetty stays on loopback even when started by hand (`bin\startup.bat`), not only through the service.
- `jsl64.ini`: **removed** `-DSTOP.PORT=8257 -DSTOP.KEY=projectfirma`. The JSL wrapper stops Java with `System.exit`, so no Jetty stop port is needed. Verified: the service stops in about 1 second, and only `127.0.0.1:8258` listens. This also ends any stop-port collision with CBF (8079) or MR (8255).
- `Geoserver\iis\web.config`: `Strict-Transport-Security: max-age=31536000`; removed `X-Powered-By`. ARR adds `X-Powered-By: ARR/3.0` *after* `customHeaders` run, so an outbound rewrite rule blanks it; `<remove>` alone isn't enough. `<requestFiltering removeServerHeader="true">` also drops IIS's own `Server: Microsoft-IIS/10.0` from responses IIS generates itself, such as the http→https 301.
- **Permissions:** the service account gets Modify only on `C:\Program Files\GeoServerProjectFirma\logs` and `\work` plus the data_dir. The binaries and `jsl64.ini` stay read-only to it (read access comes from the standard Program Files permissions). `Install-GeoServer.ps1` does this with `icacls <install> /reset /T`, then grants Modify on those three folders, and stops if any `icacls` call fails.
  - Why `/reset`, not just removing our own account: installer-built boxes carry **stray write grants**. On the PF-2844 devrig, CBFish's service account `SITKA\GeminiGeoLocal` had Modify on the whole install folder (likely left over from an earlier CBFish setup), and the installer gave `NETWORK SERVICE` (its default service account) Full Control of `logs\` and `work\`. Either could change our `jsl64.ini` or jars and have the code run as `SITKA\FirmaGeo*` with its DB access. `/reset` makes the folder inherit only the standard Program Files permissions (Administrators/SYSTEM full, Users read). MR keeps the installer defaults; this is a deliberate improvement.
  - Verified 2026-10-01: no Gemini or NETWORK SERVICE entries remain; the root and binaries have inherited permissions only; the service restarts, writes `logs\` and `work\`, serves maps, and logs no access-denied errors.
  - On QA/prod, check after install: `icacls "C:\Program Files\GeoServerProjectFirma"` should show only inherited `(I)` entries, and `logs\`/`work\` should add only the env's `SITKA\FirmaGeo*` account.
- `svn rm data_dir\logs\geoserver.log.1` and `.2`: Docker-era logs (about 10 MB each) that had been committed.

---

## Part B: Developer devrig setup (scripted)
Admin PowerShell. The script never asks for or handles a password; secrets go only into the official dialogs (services.msc, GeoServer's admin page, the cert import wizard). The script `C:\svn\sitkatech\trunk\ProjectFirma\Geoserver\Install-GeoServer.ps1` does every step it can and is safe to re-run; it finishes with a pass/fail check list.
1. **Before `svn update`:** in `C:\svn\sitkatech\trunk\ProjectFirma\GeoServerDocker`, run `docker-compose --project-name projectfirma_geoserver_project --file docker-compose.yml down`.
2. `svn update` trunk; delete the leftover `...\ProjectFirma\GeoServerDocker` folder and `C:\Sitka\ProjectFirma\Geoserver\*.txt` (old Docker runtime and password files). `git pull` the PF repo.
3. Install **ARR 3.0** (https://www.microsoft.com/en-us/download/details.aspx?id=47333) if you don't have it. Import `Build\SslCertificates\localhost-mapserver.projectfirma.com.KeyPassword=….pfx` into Local Machine → Personal.
4. DB access (the script's WFS check needs it): `make.cmd database local create-db download restore` gives `SITKA\FirmaGeoLocal` db_datareader and drops the old `FirmaGeoLocal` SQL login. To skip the restore, run the two `alter role` lines in A14.
5. Run `.\Install-GeoServer.ps1 -Env local` from `...\ProjectFirma\Geoserver`. It:
   - installs JDK 21.0.12 with JAVA_HOME if needed
   - unzips `GeoServer.zip` into `C:\Program Files`
   - registers the `GeoServerProjectFirma` service with automatic start, and grants `SITKA\FirmaGeoLocal` Modify on `logs\`, `work\` and the data_dir only
   - **pauses** for you to set the service Log On in services.msc (`SITKA\FirmaGeoLocal` + its Secret Server password)
   - starts GeoServer
   - sets up IIS (ARR proxy, app pool, site, web.config, cert binding, iplisten `.180`)
   - **pauses** for you to create `geomaster` (password of your choice, role ADMIN) and remove `admin` in the GeoServer admin page at `https://localhost-mapserver.projectfirma.com/geoserver/web/` (not 127.0.0.1: the login form posts to the proxy URL), then restarts and confirms `admin/geoserver` is rejected
   - runs the checks
6. Browser check: Project Map layers draw; left-click shows a popup; no CORS errors in the console.

**Day to day:** after editing data_dir in SVN, run `make.cmd geoserver local release`. Admin-UI saves write straight into the working copy, so review the `svn diff`.

## Part C: QA / Prod server setup (scripted)
| | QA | Prod |
|---|---|---|
| Web server (GeoServer host) | wallowa.sitka.local | salmonberry.sitka.local |
| DB server | kettle.sitka.local | deschutes.sitka.local |
| Service account | `SITKA\FirmaGeoQa` | `SITKA\FirmaGeoProd` |
| Host name | qa-mapserver.projectfirma.com | mapserver.projectfirma.com |
| Admin password | Secret Server "GeoServer Admin ProjectFirma QA" | the Prod equivalent |

1. Manual prerequisites:
   - IIS with URL Rewrite 2 + ARR 3.0 on the web server.
   - The host name's cert in the Centralized Certificate Store.
   - DBA: the env's account has **db_datareader** (and not db_owner) on `ProjectFirma` on the env's DB server (kettle / deschutes):
     `use ProjectFirma; select r.name from sys.database_role_members m join sys.database_principals r on r.principal_id=m.role_principal_id join sys.database_principals u on u.principal_id=m.member_principal_id where u.name='SITKA\FirmaGeoQa';` (FirmaGeoProd for prod) should return only `db_datareader`. If it doesn't: create the database user for the Windows login if it's missing, then `alter role db_datareader add member [SITKA\FirmaGeoQa];` and `alter role db_owner drop member [SITKA\FirmaGeoQa];`.
   - `netsh http show iplisten` is empty; if it isn't, coordinate with Ops first.
2. Copy `...\ProjectFirma\Geoserver\` (the script, `GeoServer.zip`, `iis\`) to the web server. Run `.\Install-GeoServer.ps1 -Env qa` (or `prod`). It installs the JDK, unzips, points jsl64.ini at `C:\sitka\ProjectFirma\GeoServer\data_dir`, registers the service, grants Modify on `logs\`, `work\` and the data_dir only, and **pauses** for the Log On. It then stops because the data_dir is empty.
3. From the release machine: `make.cmd geoserver qa release` (or `prod`). It copies the data_dir and env properties, then starts the service.
4. Run the script again on the web server. It sets up the IIS site on **127.0.0.1 only** (SNI + CCS), adds a temporary hosts entry `127.0.0.1 <host>`, **pauses** for you to create `geomaster` (Secret Server password, role ADMIN) and remove `admin` in the GeoServer admin page in a browser **on the server** (and, still as geomaster, check that **Security → URL Checks** shows "Enable URL checks" turned on: the 3.0.1 default for a fresh `security\`, confirmed on the PF-2844 devrig 2026-10-01; it stops requests from making GeoServer fetch arbitrary URLs), removes the hosts entry, confirms `admin/geoserver` is rejected, and only then moves the site to `*:80`/`*:443`. The public default login is never reachable from the network. If `https://<host>/geoserver/web/` won't load during the pause, stop there: don't move the site to `*` by hand while `admin/geoserver` still works. It then runs the checks against the local box with `--resolve`, so it works **before** the DNS cutover.

## Release sequencing (read before the PF-2844 commit reaches QA/prod)
Once PF-2844 is committed, `release.cmd` runs `make.cmd geoserver <env> release`, which **fails** if the `GeoServerProjectFirma` service isn't installed on that env's web server, and QA DB restores drop the Docker GeoServer's SQL login. So:
1. **Before the first QA release after the commit:** finish Part C on wallowa (service installed with logon, IIS site, `geomaster`). Then run the QA release. It deploys the data_dir and starts the service. Then do the QA DNS cutover (Part D) **right away**, because the Docker GeoServer stops getting data_dir updates from that release on.
2. **Avoid a QA DB restore between the commit and the QA cutover.** The new `config_roles.sql` drops the SQL login `FirmaGeoQa` on restore, and the Docker GeoServer still uses it until DNS moves.
3. Prod: same as step 1 on salmonberry, then the prod DNS cutover.
4. If a GeoServer deploy fails partway (e.g. the robocopy can't reach `\\<server>\c$`), the service is left **stopped** on purpose rather than started on a half-copied data_dir. Fix the cause and re-run the release, or `make.cmd geoserver <env> release` alone.
5. Developers: until a devrig has the new service, the GeoServer step of `UpdateRestoreBuildAll*.cmd` prints "not installed; skipping" instead of failing.
## Part D: Cutover and rollback
1. SVN `trunk/config/server/ns00.sitkatech.net/etc/bind/sitka/db.projectfirma.com.zone` (Ops/DNS owner):
   - QA: `qa-mapserver IN CNAME sycan.sitkatech.com.` → `qa-mapserver IN CNAME wallowa.sitkatech.com.`
   - Prod: `mapserver IN CNAME yachats.sitkatech.com.` → `mapserver IN CNAME salmonberry.sitkatech.com.`
   - Bump the SOA serial. Keep the trailing dots. Same pattern as MR r219630/r219634.
   - Leave `localhost-mapserver IN A 127.0.0.180` as is.
   - Not part of PF-2844: `bor-qa-mapserver` (→ sycan) and `bor-localhost-mapserver` point at a separate Reclamation GeoServer. Check with its owner before sycan is decommissioned.
2. Do QA first. After DNS propagates: PF QA maps draw, left-click popups work, no CORS errors. Then prod.
3. **Rollback:** point the CNAME back to sycan/yachats. Leave the Swarm GeoServer stacks running until prod has been stable for an agreed period. Note: once `config_roles.sql` runs on a QA restore, the Docker GeoServer's SQL login `FirmaGeoQa` is dropped, so QA rollback after that means recreating the login.

## Part E: Decommission (after prod is stable)
- Portainer: remove the ProjectFirma GeoServer + MinIO stacks on sycan (QA) and yachats (prod), their Swarm secrets/configs (`ProjectFirma_QA_Geoserver_SqlServer_Password_1`, `ProjectFirma-QA_Geoserver-Admin-Password_1`, `ProjectFirma-QA_Geoserver_Environment_Properties_Template`, and the Prod equivalents), the Traefik routes, the MinIO buckets/gluster volumes (`ProjectFirma-{QA,Prod}`), and the Portainer webhooks. **Deleting the webhooks is required:** their URLs are in SVN history (`geoserverDocker.build`), and anyone holding one can restart the container.
- Release machines: remove the `mc` aliases `ProjectFirmaQA` / `ProjectFirmaProd`.
- SQL: drop the SQL logins `FirmaGeoQa` (kettle) and `FirmaGeoProd` (deschutes) and their DB users. Retire the Secret Server entries for the SQL login passwords.
- Later cleanup (not required): `local.build` `*-db-geoserver-docker-user` properties and the matching cleanup lines in `config_roles.sql`.

## Part F: Future upgrades
1. On a devrig: install the new GeoServer version over a scratch folder (or uninstall first). Reapply A3 (sqlserver **and control-flow** plugins for the matching version, one mssql-jdbc jar + matching auth DLL in `native\`), A4 (jsl64.ini: copy the `cmdline` additions over), and A5 (start.ini header size). Check `etc\webdefault-*.xml` for a new `transport-guarantee`.
2. CORS needs no reapplying; it lives in `global.xml`.
3. Start it against the SVN data_dir, record any migrated files (as in A16), run the A10/A11 checks and the browser check.
4. Rebuild the zip (A16) and update this runbook's version numbers.
5. Roll out (every devrig and QA/prod, after the new zip is committed): stop `GeoServerProjectFirma`, delete `C:\Program Files\GeoServerProjectFirma`, then run `.\Install-GeoServer.ps1 -Env <env>`. The script only unzips when that folder is missing. It reuses the existing service registration, so the logon account is kept, and it reapplies the QA/prod data-dir line and the folder permissions. The data_dir and per-box `security\` live outside the install folder and aren't touched. *(Upgrade path not yet tested; first used for the next GeoServer version.)*
