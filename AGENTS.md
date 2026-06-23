# AGENTS.md — AlSoque

موقع مستقل لتراجم وتاريخ وتراث علماء منطقة «السوق». Blazor Server + PostgreSQL. يعمل `D:\Projects\AGENTS.md` (معايير الشركة) فوق هذا الملف — لا تكرّره.

## Stack

- .NET 10 / C# 13، Blazor Web App InteractiveServer، EF Core 10 + Npgsql، ASP.NET Core Identity (تسجيل عام، `RequireConfirmedAccount = false`)، Tailwind v4 (CLI عبر `npx`).
- البنية: Vertical Slice — كل ميزة في `src/AlSoque.Web/Features/<Name>/` وتحتوي صفحاتها وخدماتها.

## Solution Layout

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

## Commands

من جذر المستودع:

- بناء: `dotnet build`
- تشغيل محلي: `dotnet run --project src/AlSoque.Web` (افتراضياً `http://localhost:5094`، أو `https://localhost:7200` عبر بروفايل `https`).
- هجرات EF:
  `dotnet ef database update --project src/AlSoque.Data --startup-project src/AlSoque.Web`
  إضافة هجرة جديدة من نفس نقطة `--startup-project` مع `--project src/AlSoque.Data`.
- تخطّي بناء Tailwind (CI/تحرير سريع): `dotnet build /p:SkipTailwind=true`.
- تثبيت أدوات EF محلياً إن لزم: `dotnet tool install --global dotnet-ef --version 10.*`.

## Prerequisites

- PostgreSQL محلي يستمع على `localhost:5433`، قاعدة `AlSoqueDb`، مستخدم `postgres` كلمة `5512` (معرّفة في `src/AlSoque.Web/appsettings.json`). غيّرها عبر User Secrets أو بيئة — لا تعدّل الملف.
- Node + npx متاحان؛ يُستدعى `npx @tailwindcss/cli` تلقائياً كهدف MSBuild قبل `Build` (لا حاجة لـ `npm install` يدوياً إن كانت `node_modules` موجودة).
- حساب مسؤول أولي يُزرع من `SeedAdmin` في `appsettings.Development.json` (`admin@alsoque.net` / `ChangeMe!2026`). غيّره قبل النشر.

## Key Conventions

- RTL دائمًا: `html { direction: rtl; }` في `app.tailwind.css` base layer. لا تضف `dir="ltr"` لمحتوى عربي.
- الأنماط في `wwwroot/app.css` فقط عبر `@layer components` بأصناف دلالية (`navbar`, `card`, `btn-gold`...). لا `*.razor.css` ولا `style="..."` على عناصر (معمول به فعلاً، حافظ عليه).
- لا CQRS ولا Repository: الخدمات (`*Service.cs`) تحمل منطق الوصول إلى البيانات مباشرة عبر `IDbContextFactory<AppDbContext>` المُحقَنة — لا تحقن `AppDbContext` كـ Scoped في صفحات Blazor.
- الأدوار: `Admin` و`Member` فقط. تسجيل عام جديد = `Member` تلقائياً (راجع `IdentitySeeder`).
- لا نشر فوري لمحتوى المستخدمين: `Contribution` بحالة `Pending` حتى يوافق `Admin` في `/admin/contributions` ويطبّق `PayloadJson` على الكيان الأصلي.
- إعادة كتابة منطق هجري↔ميلادي: استعمل `AlSoque.Data/Extensions/DateHelper.cs` (نُسخ يدوياً من مشروع A3lam السابق — لا تستورد A3lam).
- لا تلمس `AlMahmoud` أو `A3lam` من هذا المستودع — مشروع مستقل تماماً.

## DB & EF Gotchas

- `ScholarRelation` يستخدم `DeleteBehavior.Restrict` على طرفي `TeacherId`/`StudentId` — احذر عند حذف عالم له علاقات.
- `Scholar` و`Family` و`Book` و`Manuscript` لها `Slug` فهرس فريد — لا تترك المولّد (`Shared/SlugHelper.cs`) يولد تضارباً.
- `Contribution.PayloadJson` مخزَّن كـ `jsonb` على PostgreSQL.
- الهجرات تولدها دائماً مع `--project src/AlSoque.Data --startup-project src/AlSoque.Web` (DbContext في Data، الخدمات في Web).
- `Seed/DemoDataSeeder` يعمل تلقائياً في `Development` فقط — لا تعتمد عليه لبيئة الإنتاج.

## Verification (مقتبس من الخطة — PLAN.html)

بعد أي تعديل، تحقق يدوياً بسير العمل الكامل: تسجيل حساب → دخول → تصفح العلماء → تقديم مساهمة → دخول كمسؤول → قبول المساهمة في `/admin/contributions` → ظهورها فوراً في صفحة العالم العامة.

## Deployment

Docker + GHCR + Azure DevOps SSH deploy — **لا** systemd/Kestrel مباشر (يطابق نمط AlMahmoud الفعلي، لا الخطة الأصلية في القسم 8). التفاصيل الكاملة في `docs/DEPLOY.md`.

- `src/AlSoque.Web/Dockerfile`: يبني الصورة، يجلب ثنائي Tailwind v4 لـ linux-x64 مباشرة (لا Node في الصورة).
- `docker-compose.yml` (الجذر): يشغّل الحاوية على `127.0.0.1:8089` على شبكة `shared-net` المشتركة مع باقي مشاريع الـ VPS، ويتصل بحاوية `postgres` المشتركة نفسها (`Database=AlSoqueDb`). `Program.cs` يستدعي `Database.MigrateAsync()` عند الإقلاع — لا حاجة لإنشاء القاعدة يدويًا.
- `DeployToVPS.yml` (الجذر): دفعة إلى `master` → بناء ودفع إلى `ghcr.io/nadir-mezhoudi/alsoque-web` → SSH إلى `/opt/AlSoque` وإعادة `docker compose up -d`. يعتمد على Service Connections موجودة فعلاً في المشروع (`ghcr-login`, `vps-ssh`) — لا حاجة لإنشاء جديدة.
- مفاتيح Data Protection محفوظة في volume `dpkeys` (`Program.cs`، خارج `Development` فقط) — بدونها تُفقَد كل الجلسات عند كل إعادة تشغيل للحاوية.
- `SeedAdmin__Email`/`SeedAdmin__Password` تُمرَّر كمتغيرات بيئة في `docker-compose.yml` — غيّر كلمة المرور الافتراضية عبر `/opt/AlSoque/.env` على الخادم قبل أول تشغيل.

## Files to Read First

- `PLAN.html` — خطة المشروع المعمارية الكاملة بالعربية (مرجع للقرارات).
- `D:\Projects\AGENTS.md` — معايير الشركة العامة.
- `src/AlSoque.Web/Program.cs` — تركيب DI، الهوية، Seeders.
- `src/AlSoque.Web/AlSoque.Web.csproj` — هدف بناء Tailwind وعلم `SkipTailwind`.
