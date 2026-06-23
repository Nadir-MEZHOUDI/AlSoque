# AGENTS.md — AlSoque

موقع مستقل لتراجم وتاريخ وتراث علماء آل سوق. Blazor Server + PostgreSQL. يعمل `D:\Projects\AGENTS.md` (معايير الشركة) فوق هذا الملف — لا تكرّره.

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

## Sources — مصادر معلومات عن آل سوق وعلمائها

مرجع أساسي لتعبئة بيانات العلماء والكتب والمخطوطات والتراجم. ارجع إلى هذه المصادر عند بناء بيانات Seed أو مراجعة مساهمات المستخدمين.

### مصادر محلية (ضمن المستودع)

- `docs/الجوهر الثمين في اخبار الملثمين.pdf` — كتاب «الجوهر الثمين في أخبار الملثمين» (PDF، ~36MB). مصدر أولي للتراجم والأخبار المتعلقة بعلماء المنطقة وعلم الملثمين. استخرج منه أسماء العلماء، أسماء الكتب، التواريخ، الأنساب، والمشايخ. (ملاحظة: اسم الملف عربي — قد تحتاج أدوات القراءة إلى التعامل مع ترميز المسار.)

### مواقع ويب

- **https://alsoque.com/vb/** — منتدى «موقع السوق» (vBulletin 3.8.7). المنتدى الأصلي للأعضاء والمهتمين بعلماء آل سوق. يحتوي أقسامًا رئيسية:
  - `forumdisplay.php?f=4` — منتدى التعارف والترحيب (36 موضوعًا).
  - `forumdisplay.php?f=6` — منتدى المنطقة العامة (29 موضوعًا) + `f=48` منتدى الأنساب والعائلات + `f=49` منتدى الصور التراثية.
  - `forumdisplay.php?f=8` — منتدى تراجم العلماء وتراثهم (55 موضوعًا) — **المصدر الأغنى لبيانات العلماء**؛ يضم تراجم مفصّلة بأسماء العلماء، تواريخ الولادة/الوفاة، شيوخهم، تلامذتهم، وكتبهم. أقسامه الفرعية: `f=40` علماء النواحي، `f=42` علماء القرى المجاورة، `f=43` علماء خارج المنطقة لهم صلة.
  - `forumdisplay.php?f=2` — المنتدى العام (35 موضوعًا) + `f=12` المنتدى الأدبي والكتابات + `f=47` الكتاب والسنة (21 موضوعًا).
  - `forumdisplay.php?f=11` — منتدى العلماء (14 موضوعًا) + `f=10` تراجم العلماء (58 موضوعًا، 250+ صفحة) + `f=30` تراجم العلماء من خارج المنطقة.
  - `f=14` الكتب والمخطوطات (11 موضوعًا) + `f=16` الخطب والمحاضرات + `f=18` الأبحاث والدراسات.
  - نسخة الأرشيف للقراءة الآلية: `https://alsoque.com/vb/archive/index.php/` (نصي، بدون JS، مناسب للاستخراج).
- **https://alsoque.net/** — الموقع التعريفي الرسمي (Joomla). يضم قسم «about-alsoque» بمقالات عن المنطقة وتاريخها: «نبذة تعريفية بالمنطقة»، «تاريخ نشأة المنطقة»، «أعلام المنطقة عبر العصور»، «قرى المنطقة»، «أسماء قرى المنطقة وأهميتها». صفحات المقالات على نمط `/index.php/about-alsoque/<id>-<slug>.html` و`/index.php/hestorical/<id>-<slug>.html`. (ملاحظة: المقالات قد تعيد توجيهًا 404 لبعض الروابط القديمة — استخدم البحث داخل الموقع أو الأرشيف.)
- **https://www.alsoque.net/** — صفحة الموقع الرئيسية (تظهر كواجهة تعريفية بالمنطقة وعلمائها).

### مصادر خارجية ذات صلة (للمراجعة والتقاطع)

عند البحث عن تراجم مفصّلة أو توثيق تاريخي إضافي، راجع أيضًا (غير مملوكة للمشروع — استخدمها للمراجعة فقط):
- **موقع الألوكة** (alukah.net) — له قسم «تراجم وأعلام» غني بعلماء الجزائر والمغرب الإسلامي.
- **موقع دار الإسلام** (dar-alislam.com) — تراجم علماء المغرب الإسلامي.
- **موقع طريق الإسلام** (islamweb.net) — قسم التراجم.
- **ويكيبيديا العربية** (ar.wikipedia.org) — مقالات علماء الجزائر (للتحقق من التواريخ والتلقيح فقط، ليست مصدرًا أوليًا).
- **مكتبة الكونجرس / الأرشيف الوطني الجزائري** — للمخطوطات والوثائق الأصلية غير المنشورة.
- **شذرات الذهب** و**الأعلام للزركلي** و**معجم المؤلفين لكحالة** — مراجع مطبوعة للتراجم، متوفرة رقميًا عبر المكتبات الرقمية (Internet Archive، Hindawi).

### كيفية الاستخدام

- عند إضافة عالم جديد في `Seed/DemoDataSeeder` أو مراجعة `Contribution`، اذكر المصدر في حقل ملاحظات المساهم إن أمكن (رابط المنتدى أو صفحة المقال).
- بيانات المنتدى (`alsoque.com/vb`) هي الأغنى لكنها تحتاج تنظيفًا (نصوص طويلة، ردود متعددة) — استخرج الترجمة الأصلية فقط.
- التواريخ الهجرية في المصادر تُحوّل عبر `AlSoque.Data/Extensions/DateHelper.cs`.
- لا ترفع محتوى المصادر التجارية/المحمية بحقوق نشر إلى المستودع — اكتب ملخصاتك الخاصة واستشهد بالرابط.
