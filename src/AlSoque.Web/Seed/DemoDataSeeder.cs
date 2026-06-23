using AlSoque.Data;
using AlSoque.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlSoque.Web.Seed;

/// <summary>
/// بيانات بذرة تجريبية لتطوير الواجهة محليًا فقط — أسماء وتراجم افتراضية لا تمثّل تراجم حقيقية موثّقة.
/// تُستبدل بالمحتوى الحقيقي عند توفّره. لا تُشغَّل إلا في بيئة التطوير (انظر Program.cs).
/// </summary>
public static class DemoDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (await db.Scholars.AnyAsync())
        {
            return;
        }

        var fiqh = new Specialization { Name = "الفقه" };
        var hadith = new Specialization { Name = "الحديث" };
        var tafsir = new Specialization { Name = "التفسير" };
        var lugha = new Specialization { Name = "اللغة العربية" };
        var usul = new Specialization { Name = "أصول الفقه" };
        db.Specializations.AddRange(fiqh, hadith, tafsir, lugha, usul);

        var alFiqhi = new Family { Name = "آل الفقيه", Slug = "al-faqih", Description = "أسرة تجريبية — بيانات بذرة للتطوير فقط." };
        var alKatib = new Family { Name = "آل الكاتب", Slug = "al-katib", Description = "أسرة تجريبية — بيانات بذرة للتطوير فقط." };
        var alImam = new Family { Name = "آل الإمام", Slug = "al-imam", Description = "أسرة تجريبية — بيانات بذرة للتطوير فقط." };
        db.Families.AddRange(alFiqhi, alKatib, alImam);

        var sheikhMohamed = new Scholar
        {
            Name = "محمد الفقيه السوقي",
            AlKunyah = "أبو عبد الله",
            AlIsm = "محمد",
            AlNisbah = "السوقي",
            AlMadhab = "مالكي",
            AlAkidah = "أشعري",
            AlMawlid = 1180,
            AlWufat = 1245,
            WolidaBi = "السوق",
            TowofiyaBi = "السوق",
            Slug = "mohamed-al-faqih-al-souqi",
            Family = alFiqhi,
            Biography = "ترجمة تجريبية لغرض تطوير الواجهة. كان من أبرز فقهاء آل سوق في زمانه، طلب العلم وتصدّر للتدريس والفتوى. تُستبدل هذه السيرة بمحتوى موثّق عند توفّره.",
            Specializations = [fiqh, usul],
        };

        var sheikhAhmed = new Scholar
        {
            Name = "أحمد الكاتب السوقي",
            AlKunyah = "أبو إسحاق",
            AlIsm = "أحمد",
            AlNisbah = "السوقي",
            AlMadhab = "مالكي",
            AlMawlid = 1205,
            AlWufat = 1270,
            WolidaBi = "السوق",
            TowofiyaBi = "السوق",
            Slug = "ahmed-al-katib-al-souqi",
            Family = alKatib,
            Biography = "ترجمة تجريبية لغرض تطوير الواجهة. عُرف بحفظه للحديث وروايته، وتلقّى العلم عن جماعة من علماء عصره.",
            Specializations = [hadith, lugha],
        };

        var sheikhAbdelkader = new Scholar
        {
            Name = "عبد القادر الإمام السوقي",
            AlKunyah = "أبو محمد",
            AlIsm = "عبد القادر",
            AlNisbah = "السوقي",
            AlMadhab = "مالكي",
            AlAkidah = "أشعري",
            AlMawlid = 1230,
            AlWufat = 1298,
            WolidaBi = "السوق",
            TowofiyaBi = "السوق",
            Slug = "abdelkader-al-imam-al-souqi",
            Family = alImam,
            Biography = "ترجمة تجريبية لغرض تطوير الواجهة. تلقّى العلم عن الشيخ محمد الفقيه السوقي، وتصدّر للإمامة والتدريس بعده.",
            Specializations = [fiqh, tafsir],
        };

        var sheikhYahya = new Scholar
        {
            Name = "يحيى الفقيه السوقي",
            AlKunyah = "أبو زكريا",
            AlIsm = "يحيى",
            AlNisbah = "السوقي",
            AlMadhab = "مالكي",
            AlMawlid = 1260,
            AlWufat = 1330,
            WolidaBi = "السوق",
            Slug = "yahya-al-faqih-al-souqi",
            Family = alFiqhi,
            Biography = "ترجمة تجريبية لغرض تطوير الواجهة. من تلاميذ الشيخ عبد القادر الإمام السوقي.",
            Specializations = [fiqh],
        };

        db.Scholars.AddRange(sheikhMohamed, sheikhAhmed, sheikhAbdelkader, sheikhYahya);
        await db.SaveChangesAsync();

        db.ScholarRelations.AddRange(
            new ScholarRelation { Teacher = sheikhMohamed, Student = sheikhAbdelkader },
            new ScholarRelation { Teacher = sheikhAbdelkader, Student = sheikhYahya });

        db.Books.AddRange(
            new Book
            {
                Title = "الفتاوى السوقية في الفقه المالكي",
                Description = "ترجمة تجريبية — مجموعة فتاوى مفترضة لغرض تطوير الواجهة.",
                Status = BookStatus.Printed,
                PublicationYear = 1240,
                Slug = "al-fatawa-al-souqiya",
                Scholar = sheikhMohamed,
            },
            new Book
            {
                Title = "شرح مختصر خليل",
                Description = "ترجمة تجريبية — شرح مفترض لغرض تطوير الواجهة.",
                Status = BookStatus.Manuscript,
                PublicationYear = 1265,
                Slug = "sharh-mukhtasar-khalil",
                Scholar = sheikhAhmed,
            },
            new Book
            {
                Title = "تفسير آيات الأحكام",
                Description = "ترجمة تجريبية — تفسير مفترض لغرض تطوير الواجهة، يُعدّ من مفقودات تراث آل سوق.",
                Status = BookStatus.Lost,
                Slug = "tafsir-ayat-al-ahkam",
                Scholar = sheikhAbdelkader,
            });

        db.Manuscripts.AddRange(
            new Manuscript
            {
                Title = "إجازة الشيخ محمد الفقيه لتلميذه عبد القادر",
                Description = "ترجمة تجريبية — نموذج إجازة مفترض لغرض تطوير الواجهة.",
                Category = ManuscriptCategory.Ijaza,
                EstimatedYear = 1244,
                Slug = "ijaza-mohamed-abdelkader",
                Scholar = sheikhAbdelkader,
            },
            new Manuscript
            {
                Title = "وثيقة وقف أسرة آل الفقيه",
                Description = "ترجمة تجريبية — وثيقة وقف مفترضة لغرض تطوير الواجهة.",
                Category = ManuscriptCategory.Document,
                EstimatedYear = 1250,
                Slug = "waqf-al-faqih-family",
                Family = alFiqhi,
            });

        await db.SaveChangesAsync();
    }
}
