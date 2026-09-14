# aql.KeyFlip v3.1.3 — حزمة الإصدار والتثبيت

تم تجهيز المشروع ليخرج نسخة Windows x64 احترافية مع مثبت Inno Setup حديث.

## ما تم إضافته

- 4 صفحات تعريفية داخل المثبت قبل اختيار مكان التثبيت.
- تصميم بصري قريب من أسلوب Windows 11 باستخدام `WizardStyle=modern`.
- صور تعريفية للبرنامج وطريقة العمل والمميزات والمطور.
- شعار `aql.KeyFlip` المستخدم كأيقونة للمثبت والبرنامج.
- صفحة مطور تشمل:
  - Eng. Ahmed Salah Aql
  - AQL — Building Ideas Into Software
  - WhatsApp: 01098486663
  - Email: info@ahmedaql.online
- زراّن قابلان للنقر لفتح WhatsApp والبريد.
- اختصار سطح المكتب واختيار التشغيل مع Windows.
- ملف GitHub Actions جاهز لبناء EXE والمثبت تلقائيًا على Windows.

## بناء النسخة على Visual Studio 2026

من Windows افتح:

`native\\aql.KeyFlip.sln`

ثم شغّل:

`native\\build\\build-release.cmd`

السكريبت يقوم بـ:

1. تشغيل الاختبارات.
2. نشر `aql.KeyFlip.exe` كملف self-contained x64.
3. التحقق من ملف EXE.
4. إذا كان Inno Setup مثبتًا، بناء `aql.KeyFlip Setup.exe` تلقائيًا.

## GitHub Actions

يوجد Workflow في:

`.github/workflows/build-windows.yml`

يمكن تشغيله يدويًا من GitHub Actions أو بمجرد إنشاء Tag يبدأ بحرف `v`.

> ملاحظة: بيئة تجهيز الملفات هنا ليست Windows ولا تحتوي Visual Studio/.NET SDK/Inno Setup، لذلك تم تجهيز كل ملفات البناء والمثبت والـassets، بينما عملية إخراج EXE وSetup النهائية تُنفّذ على Windows أو GitHub Actions.
