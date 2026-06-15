# CenngeShop — описание проекта

Учебный веб-проект на **ASP.NET Core (.NET 10, C#)**. Совмещает классическое
MVC-приложение (серверный рендеринг страниц на Razor) и **REST Web API** для
будущего отдельного фронтенда (SPA). Данные хранятся в **PostgreSQL (Supabase)**
через **Entity Framework Core**.

> Исторически это «сборник тем» курса по ASP: главная страница — оглавление демо
> (Razor, модели, DI, middleware, формы, CORS, криптография), а поверх построен
> рабочий интернет-магазин (разделы, товары, скидки, корзина, пользователи).

---

## 1. Стек и ключевые версии

| Компонент | Значение |
|---|---|
| Платформа | .NET 10 (`net10.0`) |
| Язык | C# (nullable enabled, implicit usings) |
| Веб | ASP.NET Core MVC + Web API |
| ORM | Entity Framework Core 10 |
| СУБД | PostgreSQL (self-hosted Supabase), провайдер `Npgsql.EntityFrameworkCore.PostgreSQL` |
| Фронт (vendored) | Bootstrap 5, jQuery, jquery-validation, Bootstrap Icons |
| Namespace / сборка | `CenngeShop` |

Запуск (профиль http): `http://localhost:5151`, https: `https://localhost:7024`
(см. `Properties/launchSettings.json`).

---

## 2. Структура решения

```
asp-all/                     # корень репозитория
├─ CenngeShop.slnx           # solution
├─ .env                      # секреты VPS/Supabase (НЕ в git, в .gitignore)
└─ CenngeShop/               # проект
   ├─ Program.cs             # точка входа, регистрация сервисов и middleware
   ├─ appsettings*.json      # конфиг (строка подключения берётся из user-secrets)
   ├─ Controllers/
   │  ├─ HomeController.cs        # демо-страницы (Razor/MVC)
   │  ├─ ShopController.cs        # магазин (серверный рендеринг + админка)
   │  ├─ UserController.cs        # вход/регистрация, выдача JWT
   │  ├─ StorageController.cs     # отдача загруженных файлов
   │  └─ Api/                     # REST API (JSON) — для фронта
   │     ├─ SectionsController.cs
   │     ├─ ProductController.cs
   │     ├─ CartController.cs
   │     └─ RegisteredUsersController.cs
   ├─ Data/
   │  ├─ DataContext.cs           # DbContext, связи сущностей, seed-данные
   │  ├─ DataAccessor.cs          # вспомогательные запросы к БД
   │  └─ Entities/                # сущности (таблицы)
   ├─ Models/                     # ViewModel / FormModel / API-модели
   ├─ Migrations/                 # миграции EF Core (под PostgreSQL)
   ├─ Middleware/                 # кастомные middleware (см. ниже)
   ├─ Services/                   # сервисы приложения (DI)
   ├─ Views/                      # Razor-вьюхи (.cshtml)
   ├─ wwwroot/                    # статика: css, js, lib (bootstrap/jquery)
   └─ LocalStorage/               # загруженные пользователями файлы (картинки товаров/аватары)
```

---

## 3. Модель данных (Data/Entities)

| Сущность | Назначение |
|---|---|
| `UserData` | профиль пользователя (имя, email, дата рождения) |
| `UserAccess` | учётка для входа: `Login`, `Salt`, `Dk` (производный ключ пароля), связь с ролью и профилем |
| `UserRole` | роль и уровни доступа (Create/Read/Update/Delete level) |
| `ShopSection` | раздел магазина (`Slug` уникальный) |
| `ShopProduct` | товар (`Slug` уникальный, привязан к разделу) |
| `Discount` / `DiscountDetail` | скидки и их детали по товарам |
| `Cart` / `CartItem` | корзина и позиции в ней |

**Связи** настраиваются в `DataContext.OnModelCreating` (FK, уникальные индексы по
`Login`/`Slug`). Там же **seed-данные**: роли `Self Registered` и `Root Administrator`,
а также админ `DefaultAdministrator` (`admin@change.me`).

### Важно про даты (PostgreSQL)
В `Program.cs` включён режим:
```csharp
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
```
Он заставляет `DateTime` мапиться на `timestamp without time zone` (как было в
SQL Server). Без него Npgsql требует UTC-даты и падает на seed-данных. Не убирать,
пока сущности используют `DateTime` без указания UTC.

---

## 4. Сервисы (Services/, регистрируются в Program.cs)

Каждый сервис оформлен как интерфейс + реализация + extension-метод `Add...()`:

| Сервис | Что делает |
|---|---|
| `Hash` (`Md5HashService`, `ShaHashService`) | хеширование |
| `Kdf` (`PbKdfService`) | производный ключ пароля (PBKDF) — соль + хеш для `UserAccess.Dk` |
| `Storage` (`LocalStorageService`) | сохранение/отдача файлов в папку `LocalStorage/` |
| `DateTime` (`NationalDateTimeService`, `SqlDateTimeService`) | работа с датами/форматами |
| `ScopedService`, `TransientService` | демонстрация жизненных циклов DI |

`DataAccessor` (scoped) — обёртка над `DataContext` для частых выборок.

---

## 5. Middleware (Middleware/, подключаются в Program.cs)

Порядок в конвейере: `Session → Demo → Ticks → AuthSession → AuthToken → Cart`.

| Middleware | Назначение |
|---|---|
| `AuthSession` | аутентификация по серверной сессии (для MVC-страниц) |
| `AuthToken` | аутентификация по **JWT Bearer-токену** (для API) |
| `Cart` | подгружает активную корзину пользователя в `HttpContext.Items["ActiveCart"]` |
| `Demo`, `Ticks` | учебные/вспомогательные |

---

## 6. Аутентификация и авторизация

Два механизма работают параллельно:

1. **Сессионная** (`AuthSessionMiddleware`) — для серверных страниц. Логин через
   модалку на `_Layout`, POST на `UserController`.
2. **JWT** — для API. Поток:
   - Клиент шлёт `GET /User/SignIn/jwt` с заголовком
     `Authorization: Basic base64(login:password)`.
   - Сервер проверяет пароль через `Kdf` (`Dk(salt, password) == UserAccess.Dk`).
   - Возвращает `JwtModel` с payload (`Name`, `Email`, `Aud`=Admin/Guest, `Sub`,
     `Exp` ~100 минут и т.д.).
   - Дальше клиент шлёт токен в `Authorization`, его разбирает `AuthTokenMiddleware`.

Роль `Admin` определяется по `UserRoleId == Root Administrator`.

---

## 7. REST API (Controllers/Api) — для фронтенда

Базовый префикс — `/api`. Ответы обёрнуты в `Models/Api/RestResponse.cs`
(`{ status, data, ... }`).

| Метод и путь | Описание |
|---|---|
| `GET /api/sections` | список разделов |
| `GET /api/sections/{id}` | раздел по id |
| `GET /api/product/{id}` | товар по id (без id → 404) |
| `GET /api/cart` · `GET /api/cart/{id}` | корзина |
| `POST /api/cart` · `POST /api/cart/{id}` | добавить/создать |
| `PUT /api/cart/{id}` | изменить позицию |
| `DELETE /api/cart/{id}` | удалить позицию |
| `GET /api/registeredusers` · `/{idOrLogin}` | пользователи |

**CORS** (`Program.cs`): две политики —
- `AllowAll` — сейчас применяется глобально (`app.UseCors("AllowAll")`), разрешает любой origin.
- `LocalTesting` — заточена под Vite-SPA: `http://localhost:5173/5174/5175`,
  с `AllowCredentials()`.

> Проект изначально спроектирован под **отдельный фронт (React/Vite на :5173)**,
> который дёргает эти `/api`-эндпоинты. Для продакшена стоит сузить CORS с
> `AllowAll` до конкретных доменов.

---

## 8. Конфигурация и секреты

- **Строка подключения** хранится в **.NET user-secrets** (вне репозитория), ключ
  `ConnectionStrings:MainDb`. В `appsettings.json` она пустая — чтобы пароль не
  попал в git.
- Формат строки (Supabase через пуллер Supavisor):
  ```
  Host=<хост>;Port=5432;Database=postgres;Username=postgres.<TENANT_ID>;Password=<пароль>;SSL Mode=Disable
  ```
  Имя пользователя обязательно `postgres.<TENANT_ID>` — Supavisor требует tenant.
- `.env` (в корне) содержит креды VPS/Supabase и **добавлен в `.gitignore`** —
  в репозиторий не коммитится.

### Доступ к БД
PostgreSQL живёт на VPS (Supabase в Docker). Порт `5432` открыт в фаерволе
**только для конкретного IP** разработчика. Если IP сменится — прямое подключение
перестанет работать (нужно обновить правило ufw на сервере или использовать
SSH-туннель `ssh -N -L 5432:127.0.0.1:5432 root@<server>`).

---

## 9. Как запустить локально

1. Прописать строку подключения в user-secrets (один раз на машине):
   ```
   cd CenngeShop
   dotnet user-secrets set "ConnectionStrings:MainDb" "Host=...;Port=5432;Database=postgres;Username=postgres.<tenant>;Password=...;SSL Mode=Disable"
   ```
2. Применить миграции (если БД пустая): `dotnet ef database update`
3. Запуск: `dotnet run --launch-profile http`  → `http://localhost:5151`
   (или F5 в Visual Studio).

> Изменения в `.cshtml` подхватываются только после пересборки/перезапуска
> (runtime-компиляция Razor не включена).

---

## 10. Что дальше (планы)

- **Добавить фронтенд (SPA).** Бэкенд уже готов: есть `/api`, CORS под Vite, JWT.
  Рекомендуемый путь — отдельное React+Vite приложение на `:5173`, которое логинится
  через JWT и работает с `/api/...`. Бэкенд при этом почти не меняется.
- При деплое: сузить CORS, перенести секреты в защищённое хранилище, не открывать
  порт БД в интернет (туннель или приватная сеть).
