# MVC + Performance Refactor

تم تطبيق refactor على الباك إند بأسلوب MVC/API من غير تغيير البزنس:

- **Models**: داخل `Models/`
- **Controllers**: داخل `Controllers/` وأصبحت رفيعة جدًا (Thin Controllers)
- **ViewModels / DTOs**: داخل `DTOs/` وتعمل كطبقة العرض الخاصة بالـ API
- **Business Logic**: نُقلت إلى `Application/Services/`
- **Abstractions**: داخل `Application/Interfaces/`
- **Infrastructure**: مثل `Infrastructure/Identity/CurrentUserService.cs`

## تحسينات الأداء المضافة

- تفعيل `SplitQuery` لتقليل Cartesian explosion في الاستعلامات التي فيها Includes كثيرة.
- تفعيل `EnableRetryOnFailure` للاتصال بقاعدة البيانات.
- إضافة `AsNoTracking()` لعمليات القراءة.
- إضافة فهارس Indexes على الجداول الأكثر استخدامًا.
- استخدام `ExecuteUpdateAsync` في الإشعارات لتقليل تحميل الكيانات في الذاكرة.
- تقليل منطق الكنترولرز وجعل المعالجة داخل الخدمات لتسهيل الصيانة والاختبار.

## ملاحظة مهمة

هذه النسخة تحافظ على Angular كواجهة أمامية حالية، لأن استبدالها بـ Razor MVC سيغيّر بنية النظام والـ UI بشكل كبير. لذلك تم تطبيق MVC عمليًا على الباك إند الحالي بالشكل الأنسب للمشروع من غير كسر العقود الحالية للـ API.
