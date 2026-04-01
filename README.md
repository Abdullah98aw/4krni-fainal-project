# نظام ذُكِّرني - Thakkirni System

## نظرة عامة

**ذُكِّرني** هو نظام متكامل لإدارة المهام والمنظمات (اللجان)، مبني بتقنيات حديثة:

| المكوّن | التقنية |
|---------|---------|
| **Backend** | ASP.NET Core 8 Web API (C#) |
| **Frontend** | Angular 21 (TypeScript) |
| **قاعدة البيانات** | SQL Server 2022 (Docker) |
| **المصادقة** | JWT Bearer Token |
| **ORM** | Entity Framework Core 8 |

---

## هيكل المشروع

```
/home/ubuntu/
├── Thakkirni.API/          # Backend - ASP.NET Core Web API
│   ├── Controllers/        # وحدات التحكم (API Endpoints)
│   │   ├── AuthController.cs
│   │   ├── ItemsController.cs
│   │   ├── UsersController.cs
│   │   └── NotificationsController.cs
│   ├── Models/             # نماذج قاعدة البيانات
│   │   ├── User.cs
│   │   ├── Item.cs
│   │   ├── ItemMember.cs
│   │   ├── ItemAssignee.cs
│   │   ├── ChatMessage.cs
│   │   ├── AuditEvent.cs
│   │   ├── Notification.cs
│   │   └── MessageReadStatus.cs
│   ├── DTOs/               # Data Transfer Objects
│   │   ├── AuthDto.cs
│   │   ├── ItemDto.cs
│   │   └── UserDto.cs
│   ├── Data/               # قاعدة البيانات
│   │   ├── AppDbContext.cs
│   │   └── DbSeeder.cs
│   ├── Migrations/         # ترحيلات قاعدة البيانات
│   ├── Program.cs          # نقطة الدخول الرئيسية
│   └── appsettings.json    # الإعدادات
│
└── thakkirni-angular/      # Frontend - Angular 21
    └── src/
        └── app/
            ├── models/     # نماذج TypeScript
            │   ├── user.model.ts
            │   └── item.model.ts
            ├── services/   # الخدمات
            │   ├── auth.service.ts
            │   ├── items.service.ts
            │   ├── users.service.ts
            │   ├── notifications.service.ts
            │   └── auth.interceptor.ts
            ├── guards/     # حماية المسارات
            │   └── auth.guard.ts
            ├── components/ # المكونات المشتركة
            │   └── layout/
            └── pages/      # الصفحات
                ├── login/
                ├── dashboard/
                ├── items-list/
                ├── item-detail/
                ├── users/
                └── managers/
```

---

## متطلبات التشغيل

### المتطلبات الأساسية
- **Docker** (لتشغيل SQL Server)
- **.NET 8 SDK**
- **Node.js 22+** و **npm**
- **Angular CLI** (`npm install -g @angular/cli`)

---

## تعليمات التشغيل

### 1. تشغيل SQL Server (Docker)

```bash
sudo docker run -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=YourStrong!Passw0rd" \
  -p 1433:1433 \
  --name sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

### 2. تشغيل Backend

```bash
cd /home/ubuntu/Thakkirni.API
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$PATH:$HOME/.dotnet:$HOME/.dotnet/tools

# تطبيق ترحيلات قاعدة البيانات (أول مرة فقط)
dotnet ef database update

# تشغيل الـ Backend
dotnet run
```

الـ Backend سيعمل على: `http://localhost:5059`

### 3. تشغيل Frontend

```bash
cd /home/ubuntu/thakkirni-angular
ng serve --host 0.0.0.0 --port 4200
```

التطبيق سيعمل على: `http://localhost:4200`

---

## بيانات الدخول التجريبية

| الاسم | البريد الإلكتروني | كلمة المرور | الدور |
|-------|-----------------|-------------|-------|
| أحمد محمد العمري | ahmed@example.com | Admin@123! | مدير (ADMIN) |
| سارة خالد الزهراني | sara@example.com | Admin@123! | موظف (USER) |
| محمد عبدالله القحطاني | mohammed@example.com | Admin@123! | موظف (USER) |

---

## API Endpoints

### المصادقة
| الطريقة | المسار | الوصف |
|---------|--------|-------|
| POST | `/api/auth/login` | تسجيل الدخول |
| POST | `/api/auth/register` | تسجيل مستخدم جديد |
| GET | `/api/auth/me` | معلومات المستخدم الحالي |

### المهام والمنظمات
| الطريقة | المسار | الوصف |
|---------|--------|-------|
| GET | `/api/items` | جلب جميع المهام |
| GET | `/api/items/{id}` | جلب مهمة محددة |
| POST | `/api/items` | إنشاء مهمة جديدة |
| PUT | `/api/items/{id}` | تحديث مهمة |
| DELETE | `/api/items/{id}` | حذف مهمة |
| POST | `/api/items/{id}/complete` | إتمام مهمة |
| POST | `/api/items/{id}/messages` | إرسال رسالة في المحادثة |
| GET | `/api/items/{id}/messages` | جلب رسائل المحادثة |

### المستخدمون
| الطريقة | المسار | الوصف |
|---------|--------|-------|
| GET | `/api/users` | جلب جميع المستخدمين |
| GET | `/api/users/{id}` | جلب مستخدم محدد |
| POST | `/api/users` | إنشاء مستخدم جديد |
| PUT | `/api/users/{id}` | تحديث مستخدم |
| DELETE | `/api/users/{id}` | حذف مستخدم |
| GET | `/api/users/{id}/stats` | إحصائيات المستخدم |

### الإشعارات
| الطريقة | المسار | الوصف |
|---------|--------|-------|
| GET | `/api/notifications` | جلب إشعارات المستخدم |
| PUT | `/api/notifications/{id}/read` | تعليم إشعار كمقروء |
| PUT | `/api/notifications/read-all` | تعليم جميع الإشعارات كمقروءة |

---

## ميزات النظام

### إدارة المهام والمنظمات
- إنشاء مهام وتعيينها لأعضاء متعددين
- إنشاء لجان (منظمات) مع أعضاء ومكلفين
- تصنيف المهام: عادية / سرية
- تتبع الحالة: قيد التنفيذ / متأخرة / مكتملة
- تاريخ الاستحقاق والتنبيهات

### المحادثة والتواصل
- محادثة داخلية لكل مهمة/منظمة
- عداد الرسائل غير المقروءة
- تاريخ الرسائل

### لوحة التحكم
- إحصائيات شاملة (إجمالي / قيد التنفيذ / متأخرة / مكتملة)
- إحصائيات أداء المستخدمين
- آخر المهام والمنظمات

### إدارة المستخدمين
- إضافة وتعديل وحذف المستخدمين
- أدوار: مدير (ADMIN) / موظف (USER)
- عرض إحصائيات كل مستخدم

### تقارير المديرين
- عرض أداء كل موظف
- إحصائيات: قيد التنفيذ / متأخرة / مكتملة
- نسبة الإنجاز

### الإشعارات
- إشعارات فورية عند إضافة مهام جديدة
- عداد الإشعارات غير المقروءة

---

## نموذج قاعدة البيانات

```
Users
├── Id (int, PK)
├── Name (nvarchar)
├── Email (nvarchar, unique)
├── PasswordHash (nvarchar)
├── Role (nvarchar: ADMIN/USER)
├── NationalId (nvarchar)
├── Phone (nvarchar)
└── Avatar (nvarchar)

Items
├── Id (int, PK)
├── ItemNumber (nvarchar, unique)
├── Type (nvarchar: TASK/COMMITTEE)
├── Title (nvarchar)
├── Description (nvarchar)
├── Status (nvarchar: TODO/OVERDUE/COMPLETED)
├── Importance (nvarchar: NORMAL/HIGH)
├── IsSecret (bit)
├── DueDate (datetime2)
├── CreatedAt (datetime2)
├── CompletedAt (datetime2, nullable)
└── CreatedById (int, FK -> Users)

ItemMembers (أعضاء المهمة)
├── ItemId (int, FK -> Items)
└── UserId (int, FK -> Users)

ItemAssignees (المكلفون بالمهمة)
├── ItemId (int, FK -> Items)
└── UserId (int, FK -> Users)

ChatMessages (رسائل المحادثة)
├── Id (int, PK)
├── ItemId (int, FK -> Items)
├── SenderId (int, FK -> Users)
├── Content (nvarchar)
└── SentAt (datetime2)

MessageReadStatus (حالة قراءة الرسائل)
├── Id (int, PK)
├── MessageId (int, FK -> ChatMessages)
├── UserId (int, FK -> Users)
└── ReadAt (datetime2)

AuditEvents (سجل الأحداث)
├── Id (int, PK)
├── ItemId (int, FK -> Items)
├── UserId (int, FK -> Users)
├── EventType (nvarchar)
├── Description (nvarchar)
└── CreatedAt (datetime2)

Notifications (الإشعارات)
├── Id (int, PK)
├── UserId (int, FK -> Users)
├── Title (nvarchar)
├── Message (nvarchar)
├── IsRead (bit)
├── ItemId (int, FK -> Items, nullable)
└── CreatedAt (datetime2)
```

---

## الأمان

- **JWT Authentication**: توكن صالح لمدة 7 أيام
- **Password Hashing**: BCrypt
- **CORS**: مُهيأ للسماح بطلبات Angular
- **Authorization**: حماية جميع endpoints بـ `[Authorize]`
- **Role-based Access**: بعض العمليات مقيدة بدور ADMIN

---

## ملاحظات التطوير

- الـ Frontend يستخدم **Proxy** لتوجيه طلبات `/api/*` إلى Backend على المنفذ 5059
- الـ Backend يستخدم **Kestrel** ويستمع على جميع الواجهات (`0.0.0.0:5059`)
- قاعدة البيانات تُملأ تلقائياً بـ **Seed Data** عند أول تشغيل
- الـ Frontend يستخدم **ChangeDetectorRef** لضمان تحديث الواجهة بعد استقبال البيانات
