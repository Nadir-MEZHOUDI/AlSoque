# AlSoque

موقع مستقل لتراجم وتاريخ وتراث علماء آل سوق. مبني على Blazor Server وPostgreSQL.

## التقنيات

- .NET 10 / C# 13
- Blazor Web App (Interactive Server)
- EF Core 10 + Npgsql (PostgreSQL)
- ASP.NET Core Identity (تسجيل عام، `RequireConfirmedAccount = false`)
- Tailwind CSS v4 (CLI عبر `npx`)
- بنية Vertical Slice — كل ميزة في `src/AlSoque.Web/Features/<Name>/`

## المتطلبات

- PostgreSQL يستمع على `localhost:5433`، قاعدة `AlSoqueDb`، مستخدم `postgres` كلمة `5512` (معرّفة في `src/AlSoque.Web/appsettings.json`). غيّرها عبر User Secrets أو بيئة — لا تعدّل الملف.
- Node + `npx` متاحان؛ يُستدعى `npx @tailwindcss/cli` تلقائياً كهدف MSBuild قبل `Build`.
- حساب مسؤول أولي يُزرع من `SeedAdmin` في `appsettings.Development.json` (`admin@alsoque.net` / `ChangeMe!2026`). غيّره قبل النشر.

## بنية المشروع

```
AlSoque.slnx
src/
├── AlSoque.Data/          # كيانات + AppDbContext + Identity stores + الهجرات + DateHelper (هجري↔ميلادي)
└── AlSoque.Web/
    ├── Components/        # App/Routes/MainLayout + Account scaffolded من قالب Identity
    ├── Features/          # Home, ScholarsBrowse, ScholarProfile, Books, Manuscripts, Contribute, Admin/*
    ├── Shared/            # NavBar, Footer, ScholarCard, SlugHelper, Contribution payloads
    ├── Identity/          # IdentitySeeder + AppRoles (Admin, Member)
    ├── Seed/              # DemoDataSeeder (يشتغل في Development فقط)
    ├── Styles/app.tailwind.css   # مصدر Tailwind + @source لكل ملفات .razor
    └── wwwroot/app.css    # ناتج بناء Tailwind — لا يُحرَّر يدويًا
```

## الأوامر

من جذر المستودع:

```bash
# بناء
dotnet build

# تخطّي بناء Tailwind (CI/تحرير سريع)
dotnet build /p:SkipTailwind=true

# تشغيل محلي
dotnet run --project src/AlSoque.Web
# افتراضياً http://localhost:5094، أو https://localhost:7200 عبر بروفايل https

# تحديث قاعدة البيانات
dotnet ef database update --project src/AlSoque.Data --startup-project src/AlSoque.Web

# إضافة هجرة جديدة
dotnet ef migrations add <Name> --project src/AlSoque.Data --startup-project src/AlSoque.Web

# تثبيت أدوات EF محلياً إن لزم
dotnet tool install --global dotnet-ef --version 10.*
```

## الميزات

- **تصفّح العلماء**: قائمة العلماء مع بطاقات تعريفية وروابط slug فريدة.
- **صفحة العالم**: ترجمة العالم، شيوخه وتلاميذه (علاقات `ScholarRelation`)، كتبه ومخطوطاته.
- **الكتب والمخطوطات**: تصنيفات منفصلة لكل من الكتب المطبوعة والمخطوطات.
- **المساهمات**: أي مستخدم مسجّل يقدّم مساهمة (`Contribution`) بحالة `Pending` حتى يوافق `Admin` في `/admin/contributions` ويطبّق `PayloadJson` على الكيان الأصلي.
- **الإدارة**: لوحات للأدوار `Admin`/`Member` فقط — تسجيل عام جديد = `Member` تلقائياً.
- **تقويم هجري↔ميلادي**: عبر `AlSoque.Data/Extensions/DateHelper.cs`.

## النشر

Docker + GHCR + Azure DevOps SSH deploy — لا systemd/Kestrel مباشر. التفاصيل الكاملة في [`docs/DEPLOY.md`](docs/DEPLOY.md).

- `src/AlSoque.Web/Dockerfile`: يبني الصورة، يجلب ثنائي Tailwind v4 لـ linux-x64 مباشرة (لا Node في الصورة).
- `docker-compose.yml`: يشغّل الحاوية على `127.0.0.1:8089` على شبكة `shared-net` المشتركة، ويتصل بحاوية `postgres` المشتركة (`Database=AlSoqueDb`). `Program.cs` يستدعي `Database.MigrateAsync()` عند الإقلاع.
- `DeployToVPS.yml`: دفعة إلى `master` → بناء ودفع إلى `ghcr.io/nadir-mezhoudi/alsoque-web` → SSH إلى `/opt/AlSoque` وإعادة `docker compose up -d`.
- مفاتيح Data Protection محفوظة في volume `dpkeys` (خارج `Development` فقط).
- `SeedAdmin__Email`/`SeedAdmin__Password` تُمرَّر كمتغيرات بيئة في `docker-compose.yml` — غيّر كلمة المرور الافتراضية عبر `/opt/AlSoque/.env` على الخادم قبل أول تشغيل.

## التحقق

بعد أي تعديل، تحقق يدوياً بسير العمل الكامل:

1. تسجيل حساب
2. دخول
3. تصفح العلماء
4. تقديم مساهمة
5. دخول كمسؤول
6. قبول المساهمة في `/admin/contributions`
7. ظهورها فوراً في صفحة العالم العامة

## ملفات للقراءة أولًا

- [`PLAN.html`](PLAN.html) — خطة المشروع المعمارية الكاملة بالعربية (مرجع للقرارات).
- [`docs/DEPLOY.md`](docs/DEPLOY.md) — تفاصيل النشر.
- [`src/AlSoque.Web/Program.cs`](src/AlSoque.Web/Program.cs) — تركيب DI، الهوية، Seeders.
- [`src/AlSoque.Web/AlSoque.Web.csproj`](src/AlSoque.Web/AlSoque.Web.csproj) — هدف بناء Tailwind وعلم `SkipTailwind`.