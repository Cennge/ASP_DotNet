# MASTER PROMPT 001 — CenngeShop (бэкенд + фронт)

> Вставь этот промт в новый чат, чтобы продолжить работу с того же места.
> Он описывает контекст, текущее состояние, доступы и список открытых задач.

---

## РОЛЬ И ЦЕЛЬ
Ты — инженер, продолжаешь работу над учебным e-commerce проектом «CenngeShop»:
ASP.NET Core бэкенд + React/Vite фронт, база в self-hosted Supabase (PostgreSQL).
Аккаунт владельца на GitHub — **Cennge** (`ilyagliba02@gmail.com`).
Общение веди на русском. Платформа: Windows 11, PowerShell + Git Bash.

## ДВА РЕПОЗИТОРИЯ
1. **Бэкенд:** `C:\Users\ilya\source\repos\asp-all`
   - Проект: `CenngeShop/` (namespace `CenngeShop`), решение `CenngeShop.slnx`
   - Git remote: `https://github.com/Cennge/ASP_DotNet.git` (ветка `master`)
   - Подробное описание архитектуры: см. `PROJECT_OVERVIEW.md` в корне.
2. **Фронт:** `C:\Users\ilya\commerceKNP231`
   - React 19 + Vite 7 + TypeScript + react-router-dom 7
   - Отдельный git-репозиторий (изменения НЕ закоммичены).

## СТЕК
- Бэкенд: .NET 10, ASP.NET Core MVC + Web API, EF Core 10.
- БД: PostgreSQL (Supabase в Docker на VPS), провайдер `Npgsql.EntityFrameworkCore.PostgreSQL`.
- Фронт: React/Vite/TS, слой DAO, JWT-аутентификация (Bearer-токен в заголовке, без кук).

## КАК ЗАПУСКАТЬ
Бэкенд (из `C:\Users\ilya\source\repos\asp-all\CenngeShop`):
```
dotnet run --launch-profile http      # http://localhost:5151  (https-профиль: 7024)
```
Фронт (из `C:\Users\ilya\commerceKNP231`):
```
npm install      # один раз
npm run dev      # http://localhost:5173
```
Связка: фронт смотрит на бэкенд через `src/entities/config/Config.ts`
(`backendUrl: "http://localhost:5151"`). Все запросы → `Config.backendUrl + "/api/..."`.

## ДОСТУПЫ
- Учётка для входа (сид-админ, роль Root Administrator):
  **логин `DefaultAdministrator`, пароль `A946A088`**.
- БД (Supabase) — строка подключения хранится в **.NET user-secrets** проекта
  `CenngeShop` (НЕ в git), ключ `ConnectionStrings:MainDb`:
  ```
  Host=13.140.151.91;Port=5432;Database=postgres;Username=postgres.your-tenant-id;Password=<см. .env>;SSL Mode=Disable
  ```
- Секреты VPS/Supabase лежат в `C:\Users\ilya\source\repos\asp-all\.env`
  (в `.gitignore`, в репозиторий не коммитить!).
- SSH на сервер по ключу: `ssh -i $env:USERPROFILE\.ssh\id_ed25519_supabase root@13.140.151.91`.

## ВАЖНЫЕ НЮАНСЫ (GOTCHAS)
- **Доступ к БД по IP:** порт Postgres 5432 на VPS открыт в фаерволе (ufw) ТОЛЬКО для
  текущего IP разработчика (на момент настройки — `212.8.39.14`). Если IP сменился —
  прямое подключение не работает. Решения: обновить правило ufw
  (`ufw allow from <НОВЫЙ_IP> to any port 5432 proto tcp`) или поднять SSH-туннель
  (`ssh -N -L 5432:127.0.0.1:5432 root@13.140.151.91`) и в строке подключения
  использовать `Host=127.0.0.1`.
- **Username для Supabase ОБЯЗАТЕЛЬНО `postgres.your-tenant-id`** — пуллер Supavisor
  требует tenant в имени пользователя, иначе ошибка «no tenant identifier provided».
- **Даты:** в `Program.cs` стоит `AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true)` —
  не убирать, иначе seed/`DateTime` падают на timestamptz.
- **Razor не перекомпилируется на лету:** правки в `.cshtml` видны только после
  пересборки/перезапуска бэкенда.
- **CORS:** сейчас `app.UseCors("AllowAll")` (любой origin). Для прода сузить до
  `LocalTesting`/конкретных доменов.

## ЧТО УЖЕ СДЕЛАНО
- Переписаны все git-коммиты бэкенда на аккаунт Cennge, запушено в `Cennge/ASP_DotNet`.
- Проект переименован `asp_all` → `CenngeShop` (namespace, папка, .csproj, БД, бренд).
- БД переведена с SQL Server (LocalDB) на PostgreSQL/Supabase; миграции пересозданы и применены.
- Секреты вынесены в user-secrets; `.env` в `.gitignore`.
- Порт БД открыт напрямую (фаервол по IP), вход на VPS по SSH-ключу.
- Убраны эмодзи с главной страницы; добавлен `PROJECT_OVERVIEW.md`.
- Фронт связан с бэком: `Config.backendUrl` → `http://localhost:5151`, убраны хардкоды
  URL в `SectionDao.ts`, исправлен баг с незакрытой скобкой в `BaseDao.ts`.
- Подтверждено: логин (`/User/SignIn/jwt`, Basic→JWT) и CORS-preflight работают.

## ОТКРЫТЫЕ ЗАДАЧИ (TODO)
1. **Регистрация на фронте.** Формы/кнопки регистрации нет. На бэке `SignUp` —
   только MVC-форма (multipart + redirect), не SPA-friendly. Нужно:
   а) добавить JSON-эндпоинт регистрации на бэке (создаёт UserData + UserAccess
      с ролью «Self Registered», хеш пароля через Kdf: `Dk(salt, password)`);
   б) на фронте — кнопку «Реєстрація» и форму, шлющую JSON на этот эндпоинт.
2. **`npm run build` падает** из-за TS6133 (неиспользуемые `data`/`reject`) в
   `src/entities/search/api/SearchDao.ts` — почистить.
3. **Закоммитить изменения фронта** в репозитории `commerceKNP231` (сейчас не закоммичены).
4. **Наполнить контентом:** в БД только сид-админ. Разделы/товары добавить через
   `/Shop/Admin` (MVC-админка) или API.
5. (опц.) Сузить CORS, сменить дефолтный пароль админа.

## ПЕРВЫЙ ШАГ В НОВОЙ СЕССИИ
Уточни у пользователя, с какой из открытых задач продолжаем (вероятно — #1, регистрация).
Перед работой с БД проверь доступ: `Test-NetConnection 13.140.151.91 -Port 5432`.
